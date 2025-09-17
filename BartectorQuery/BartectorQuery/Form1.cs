using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BartectorQuery
{
    /// <summary>
    /// 主視窗類別，負責查詢、分頁、以及匯出 Excel 的 UI 與行為。
    /// 此類別繼承自 AutoSizeWindows 並包含多個事件處理器與非同步作業。
    /// </summary>
    public partial class Form1 : AutoSizeWindows
    {
        #region 參數宣告
        /// <summary>
        /// 目前顯示的頁碼（1-based）。當無資料時為 0。
        /// </summary>
        private int currentPage = 1;

        /// <summary>
        /// 每頁顯示的資料數量（固定值）。
        /// </summary>
        private const int pageSize = 50;

        /// <summary>
        /// 總共的分頁數量（依查詢結果計算）。
        /// </summary>
        private int _totalPage = 0;// 總共多少分頁

        /// <summary>
        /// 最後一次查詢所使用的 SQL 字串（不含分頁子句）。
        /// </summary>
        private string lastSql = "";

        /// <summary>
        /// 最後一次查詢所使用的 SQL 參數清單。
        /// </summary>
        private List<SqlParameter> lastParams = new List<SqlParameter>();

        /// <summary>
        /// 查詢參數與欄位定義的 Query 物件實例。
        /// </summary>
        public Query GetQuery = new Query();

        /// <summary>
        /// 用於 DataGridView 的當前頁面的資料表來源。
        /// </summary>
        DataTable dt = new DataTable();

        /// <summary>
        /// 用於匯出 Excel 的完整資料表（未分頁）。
        /// </summary>
        DataTable Exceldt = new DataTable();

        /// <summary>
        /// 匯出檔案預設資料夾路徑 (應用程式目前工作目錄下的 Export)。
        /// </summary>
        static string exportPath = Environment.CurrentDirectory + "\\Export\\";

        /// <summary>
        /// 匯出檔案路徑（不含副檔名），預設以目前時間為檔名。
        /// </summary>
        string strFilePath = Path.Combine(exportPath, DateTime.Now.ToString("yyyyMMddHHmmss"));

        /// <summary>
        /// 共用欄位顯示對照表，避免在多處重複建立
        /// </summary>
        private static readonly Dictionary<string, string> ColumnDisplayMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "TIMESTAMP", "生產日期" },
                { "KITID", "工單號碼" },
                { "PN", "零件料號" },
                { "LOT", "零件LOT" },
                { "DATECODE", "零件DC" },
                { "SUPPLIER", "供應商" },
                { "PCBID", "PCBA序號" },
                { "RID", "ReelID" },
                { "TRACE_STATION", "SMT站別" },
                { "FCODE", "料槍" },
                { "MPROG", "製件程式" },
                { "LOC", "機台料站" },
            };
        #endregion

        /// <summary>
        /// 建構子：初始化元件並註冊頁碼輸入的 Enter 鍵事件處理器。
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            // 註冊頁碼輸入的 Enter 鍵處理器，讓使用者可以直接輸入頁碼並跳轉
            this.txtPageNumber.KeyPress += txtPageNumber_KeyPress;
        }

        #region Click 事件處理器
        /// <summary>
        /// 查詢按鈕事件處理器：蒐集 UI 輸入、設定 Query 屬性，執行 QDate 並載入分頁資料。
        /// 會在查詢期間顯示 progressBar2。
        /// </summary>
        /// <param name="sender">事件來源</param>
        /// <param name="e">事件參數</param>
        private async void btn_Serach_Click(object sender, EventArgs e)
        {
            // 每次新查詢時將頁碼重置為 1
            currentPage = 1;
            // 查詢之前顯示進度條
            progressBar2.Visible = true;
            CheckedChanged();

            #region 生產日期欄位處理
            // 只接受 yyyyMMdd 格式的輸入，使用 TryParseExact 以避免文化或分隔符造成解析錯誤
            // 保留原本在無效或空輸入時將 DateTime? 設為 null 的行為
            string d1 = txt_date1.Text.Trim();
            if (!string.IsNullOrEmpty(d1) && DateTime.TryParseExact(d1, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt1))
            {
                GetQuery.Date1_dt = dt1.Date;
            }
            else
            {
                GetQuery.Date1_dt = null;
            }

            string d2 = txt_date2.Text.Trim();
            if (!string.IsNullOrEmpty(d2) && DateTime.TryParseExact(d2, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt2))
            {
                GetQuery.Date2_dt = dt2.Date;
            }
            else
            {
                GetQuery.Date2_dt = null;
            }
            #endregion

            GetQuery.EngSr = txt_engsr.Text.Trim();
            GetQuery.Side_A = rdb_A.Checked;
            GetQuery.Side_B = rdb_B.Checked;
            GetQuery.PN = txt_PN.Text.Trim();
            GetQuery.LotCode = txt_lotcode.Text.Trim();
            GetQuery.DateCode = txt_datecode.Text.Trim();
            GetQuery.Supplier = txt_supplier.Text.Trim();
            GetQuery.PCBID = txt_pcbid.Text.Trim();
            GetQuery.ReelID = txt_reelID.Text.Trim();
            GetQuery.Station = txt_station.Text.Trim();
            GetQuery.Program = txt_program.Text.Trim();
            GetQuery.Slot = txt_slot.Text.Trim();
            GetQuery.FeederID = txt_feederid.Text.Trim();
            GetQuery.Wono = txt_wono.Text.Trim();

            try
            {
                // QDate 是計算 SQL 與參數的同步方法，直接呼叫即可
                var queryresult = GetQuery.QDate();
                lastSql = queryresult.Item1;
                // 克隆參數清單，避免在 LoadPageData 中被修改
                lastParams = CloneParameters(queryresult.Item2);

                await LoadPageData(lastSql, lastParams, currentPage);
            }
            finally
            {
                // 確保在任何情況下都會隱藏 progressBar
                progressBar2.Visible = false;
            }
        }

        /// <summary>
        /// 頁碼輸入欄位按下 Enter 的事件處理器：嘗試跳轉至指定頁碼（自動夾住在有效範圍）。
        /// 若輸入無效會顯示警告並還原顯示頁碼。
        /// </summary>
        /// <param name="sender">事件來源</param>
        /// <param name="e">鍵盤事件參數</param>
        private async void txtPageNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                e.Handled = true; // 防止系統的 "ding" 聲

                if (_totalPage <= 0)
                {
                    // 沒有資料或總頁數為 0，不作任何動作
                    return;
                }

                string raw = txtPageNumber.Text.Trim();
                if (!int.TryParse(raw, out int targetPage))
                {
                    MessageBox.Show("請輸入有效的頁碼數字。", "無效輸入", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPageNumber.Text = currentPage.ToString();
                    return;
                }

                // 夾住範圍
                if (targetPage < 1) targetPage = 1;
                if (targetPage > _totalPage) targetPage = _totalPage;

                if (targetPage == currentPage)
                {
                    // 仍要將顯示頁碼同步（格式化）
                    txtPageNumber.Text = currentPage.ToString();
                    return;
                }

                currentPage = targetPage;
                await LoadPageData(lastSql, lastParams, currentPage);
            }
        }

        /// <summary>
        /// 下一頁按鈕事件：若未到最後一頁則遞增 currentPage 並載入資料。
        /// </summary>
        /// <param name="sender">事件來源</param>
        /// <param name="e">事件參數</param>
        private async void btnNextPage_Click(object sender, EventArgs e)
        {
            // 在嘗試進入下一頁前先確認是否尚未到最後一頁
            if (_totalPage <= 0) return;
            if (currentPage >= _totalPage) return;

            currentPage++;
            await LoadPageData(lastSql, lastParams, currentPage);
        }

        /// <summary>
        /// 上一頁按鈕事件：若目前頁碼大於 1 則遞減 currentPage 並載入資料。
        /// </summary>
        /// <param name="sender">事件來源</param>
        /// <param name="e">事件參數</param>
        private async void btnPreviousPage_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                await LoadPageData(lastSql, lastParams, currentPage);
            }
        }

        /// <summary>
        /// 檢查並整理 panel1 中被勾選的 CheckBox，將對應的欄位名稱填入 GetQuery.columns。
        /// 同時清除 DataGridView 的資料來源與欄位顯示。
        /// </summary>
        public void CheckedChanged()
        {
            GetQuery.columns.Clear();
            // 清除数据源
            dataGridView1.DataSource = null;
            // 清除所有行和列
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            string[] parts;
            string temp;
            foreach (Control ctr in panel1.Controls)
            {
                if (ctr is CheckBox && ((CheckBox)ctr).Checked == true)
                {
                    string condition = ctr.Name.Substring(0, 1);
                    switch (condition)
                    {
                        case "P":
                        case "R":
                            GetQuery.columns.Add(ctr.Name.Replace('_', '.'));
                            break;
                        case "T":
                            parts = ctr.Name.Split('_');
                            if (parts.Length > 3)
                            {
                                temp = string.Join("_", parts[0], parts[1]) + "." + string.Join("_", parts[2], parts[3]);
                            }
                            else
                            {
                                temp = string.Join("_", parts[0], parts[1]) + "." + string.Join("_", parts[2]);
                            }

                            GetQuery.columns.Add(temp);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 清除查詢條件與 DataGridView 的顯示，並重置分頁按鈕狀態。
        /// </summary>
        /// <param name="sender">事件來源</param>
        /// <param name="e">事件參數</param>
        private void btn_clear_Click(object sender, EventArgs e)
        {
            foreach (Control ctl in panel1.Controls)
            {
                if (ctl is TextBox)
                {
                    ctl.Text = "";
                }
            }
            rdb_A.Checked = false;
            rdb_B.Checked = false;
            rdb_N.Checked = true;
            // 清除数据源
            dataGridView1.DataSource = null;

            // 清除所有行和列
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            btnPreviousPage.Enabled = false;
            btnNextPage.Enabled = false;
        }

        /// <summary>
        /// 匯出按鈕事件：顯示 progressBar1，執行匯出作業，並在完成後顯示訊息。
        /// </summary>
        /// <param name="sender">事件來源</param>
        /// <param name="e">事件參數</param>
        private async void btn_export_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = true;  // 顯示進度條
            try
            {
                // 準備克隆參數（供分批或計數使用）
                var clonedParams = CloneParameters(lastParams).ToArray();

                bool shouldUseBatchExport = false;

                // 如果沒有 lastSql，代表目前只有 Exceldt 的內容可用（可能是 UI 已載入的全部或分頁）
                if (string.IsNullOrWhiteSpace(lastSql))
                {
                    // 若沒有任何資料則直接提示
                    if (Exceldt == null || Exceldt.Rows.Count == 0)
                    {
                        MessageBox.Show("沒有可匯出的資料。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    // 否則使用現有的 Exceldt 匯出（ExpExcelSheet 會處理）
                    shouldUseBatchExport = false;
                }
                else
                {
                    // 當有 lastSql 時，我們要判斷 Exceldt 是否已包含完整結果
                    if (Exceldt == null || Exceldt.Rows.Count == 0)
                    {
                        // 沒有本機資料，直接使用分批匯出
                        shouldUseBatchExport = true;
                    }
                    else
                    {
                        // 嘗試取得總筆數（優先使用 TryFastCount，失敗時使用同步的 ExecuteScalarInt）
                        int totalRows = 0;
                        try
                        {
                            var clonedParamList = CloneParameters(clonedParams?.ToList());
                            if (!TryFastCount(lastSql, clonedParamList, out totalRows))
                            {
                                // 若快速計數失敗，退回到執行計數（同步呼叫放到背景執行以免阻塞 UI）
                                string countSql = BuildCountSql(lastSql);
                                totalRows = await Task.Run(() => Db.ExecuteScalarInt(countSql, CommandType.Text, clonedParams));
                            }
                        }
                        catch
                        {
                            // 若計數失敗，保守採用分批匯出以確保資料完整性與記憶體安全
                            shouldUseBatchExport = true;
                        }

                        // 若我們成功取得總筆數，且 Exceldt 的列數小於總筆數，代表目前只載入部分資料，應使用分批匯出
                        if (!shouldUseBatchExport && totalRows > 0 && Exceldt.Rows.Count < totalRows)
                        {
                            shouldUseBatchExport = true;
                        }
                    }
                }

                if (shouldUseBatchExport)
                {
                    // 移除 ORDER BY 以供串流查詢使用
                    string fullSql = RemoveOrderBy(lastSql);
                    int batchSize = 10000; // 每批抓取筆數，可視需求調整
                    await ExportByBatchesAsync(fullSql, clonedParams, batchSize);
                    MessageBox.Show("Excel 匯出完成（分批匯出）！");
                    return;
                }

                // 使用現有的 Exceldt 或 reader 路徑由 ExpExcelSheet 處理
                await ExpExcelSheet();
                MessageBox.Show("Excel 匯出完成！");
            }
            catch (Exception ex)
            {
                MessageBox.Show("匯出時發生錯誤: " + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                progressBar1.Visible = false; // 匯出完成或失敗後隱藏進度條
            }
        }


        #endregion

        #region Enter 事件
        /// <summary>
        /// 日期欄位 txt_date1 的 Enter 事件：若為範例文字則清除。
        /// </summary>
        /// <param name="sender">事件來源</param>
        /// <param name="e">事件參數</param>
        private void txt_date1_Enter(object sender, EventArgs e)
        {
            if (txt_date1.Text == "eg. 2 0 2 4 0 1 0 1")
            {
                txt_date1.Text = "";
            }
        }

        /// <summary>
        /// 日期欄位 txt_date2 的 Enter 事件：若為範例文字則清除。
        /// </summary>
        /// <param name="sender">事件來源</param>
        /// <param name="e">事件參數</param>
        private void txt_date2_Enter(object sender, EventArgs e)
        {
            if (txt_date2.Text == "eg. 2 0 2 4 0 4 0 1")
            {
                txt_date2.Text = "";
            }
        }

        /*
       Pseudocode / 詳細計畫（中文）:
       1. 取得使用者在 txt_kyworld 的輸入並做 Trim 與 ToLower 處理。
       2. 若輸入為空，跳出提示並結束。
       3. 遍歷 dataGridView1 的每一列：
          a. 對於每一列，遍歷其所有 Cell，檢查 cell.Value 是否包含搜尋字串（以小寫比較）。
          b. 若找到匹配，將該列設為 Selected，並將 DataGridView 的 FirstDisplayedScrollingRowIndex 設為該列索引，記錄已找到。
          c. 若當前列沒有任何 cell 匹配，則取消該列的選中狀態。
       4. 如果遍歷結束後沒有找到任何匹配，顯示「未找到匹配的資料」對話方塊。
       5. 保持現有行為與效能考量：避免因大量列操作造成 UI 卡頓（必要時可改為分批搜尋或背景執行，但此方法維持同步行為以符合原設計）。
       */

        /***
         <summary>
          當使用者在關鍵字搜尋輸入框按下 Enter 鍵時觸發。
          會以不區分大小寫的方式在目前 DataGridView 已載入的資料列中搜尋關鍵字，
          找到時選中包含關鍵字的列並捲動到第一個匹配列；若未找到則顯示提示。
         </summary>
         <param name="sender">事件來源，通常為 txt_kyworld 控制項。</param>
         <param name="e">鍵盤事件參數，用於判斷是否按下 Enter 鍵。</param>
         <remarks>
          - 搜尋以字串包含 (Contains) 方式進行，會將欄位值與搜尋字串皆轉為小寫比較以達到不區分大小寫。
          - 若搜尋字串為空白或僅有空白字元，會顯示提示並終止搜尋。
          - 當找到多個匹配列時，會同時選中所有匹配列，並將畫面捲動到第一個發現的匹配列。
          - 此方法為同步操作；在資料列非常多（例如數萬筆）時，可能導致 UI 短暫停頓。
          - 若需改善大量資料的搜尋效能，可考慮改為在背景執行搜尋或以索引/資料表過濾方式搜尋。
         </remarks>
         <example>
          // 使用範例：在 txt_kyworld 輸入 "ABC" 並按下 Enter，
          // 函式會尋找 DataGridView 中任何含有 "abc"（不分大小寫）的儲存格，並選中該列。
         </example>
        ***/
        private void txt_kyworld_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                string searchTerm = txt_kyworld.Text.Trim().ToLower(); // 從搜索框獲取關鍵字並轉為小寫
                if (string.IsNullOrEmpty(searchTerm))
                {
                    MessageBox.Show("請輸入搜索關鍵字。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                bool matchFound = false; // 用來檢查是否找到匹配的結果

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    bool rowMatched = false;
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.Value != null && cell.Value.ToString().ToLower().Contains(searchTerm))
                        {
                            row.Selected = true;
                            dataGridView1.FirstDisplayedScrollingRowIndex = row.Index;
                            matchFound = true;
                            rowMatched = true;
                        }
                    }

                    if (!rowMatched)
                    {
                        row.Selected = false; // 若行沒有匹配，取消選中
                    }
                }


                if (!matchFound)
                {
                    MessageBox.Show("未找到匹配的資料。", "結果", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        #endregion

        /// <summary>
        /// 非同步載入指定頁面的資料並更新 UI。
        /// 會先載入完整查詢結果 (Exceldt) 以計算總筆數與總頁數，然後再載入分頁資料到 dt。
        /// </summary>
        /// <param name="baseSql">不含 OFFSET/FETCH 的基礎 SQL 查詢字串</param>
        /// <param name="parameters">查詢所需的 SqlParameter 列表（此方法會新增/移除分頁參數）</param>
        /// <param name="pageNumber">要顯示的頁碼（1-based）</param>
        /// <returns>Task 代表非同步作業</returns>
        private async Task LoadPageData(string baseSql, List<SqlParameter> parameters, int pageNumber)
        {

            progressBar2.Visible = true;

            int originalTimeout = Db.CommandTimeoutSeconds;
            try
            {
                // 在長時間查詢操作前，暫時增加命令逾時以避免預設逾時
                Db.CommandTimeoutSeconds = 300; // 5 分鐘

                // 克隆參數，避免直接修改呼叫端的參數集合
                var paramCopy = CloneParameters(parameters);

                // 先嘗試快速計數（使用統計或 partition metadata），若不適用則回退到 COUNT(*)
                int rowCount = 0;
                // 嘗試快速計數的方法
                bool fastOk = TryFastCount(baseSql, paramCopy, out rowCount);
                if (!fastOk)
                {
                    string countSql = BuildCountSql(baseSql);
                    // 使用 ExecuteScalarInt 以避免建立 DataTable 的開銷
                    rowCount = await Task.Run(() => Db.ExecuteScalarInt(countSql, CommandType.Text, paramCopy.ToArray()));
                }

                // 計算分頁
                _totalPage = rowCount / pageSize;
                if (rowCount % pageSize > 0) _totalPage += 1;

                if (_totalPage == 0)
                {
                    currentPage = 0;
                    pageNumber = 0;
                }
                else
                {
                    if (pageNumber < 1) pageNumber = 1;
                    if (pageNumber > _totalPage) pageNumber = _totalPage;
                    currentPage = pageNumber;
                }

                lb_totalPage.Text = _totalPage.ToString();
                txtPageNumber.Text = $"{currentPage}";

                // 設定分頁按鈕狀態
                btnPreviousPage.Enabled = currentPage > 1;
                btnNextPage.Enabled = currentPage > 0 && currentPage < _totalPage;

                // 若沒有資料，清空 DataGridView 並結束
                if (rowCount == 0)
                {
                    dataGridView1.DataSource = null;
                    dataGridView1.Rows.Clear();
                    dataGridView1.Columns.Clear();
                    // 不立即載入整份資料以節省記憶體，匯出時再做完整查詢
                    Exceldt = new DataTable();
                    return;
                }

                // 只載入分頁資料
                paramCopy.RemoveAll(p => p.ParameterName == "@Offset" || p.ParameterName == "@PageSize");
                int offset = (pageNumber - 1) * pageSize;
                // 分頁 SQL 必須包含 ORDER BY 子句，OFFSET/FETCH 在沒有 ORDER BY 時會拋錯
                // 若 baseSql 中沒有 ORDER BY，則自動補一個保守的 ORDER BY 子句以避免語法錯誤。
                // 注意：此處使用 (SELECT NULL) 作為欄位以避免猜測實際排序欄位；若需要穩定排序，建議 caller 提供明確的 ORDER BY。
                string paginatedSql;
                if (baseSql.IndexOf(" order by ", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    // 若呼叫端未提供 ORDER BY，使用 TRACE_LOG.TIMESTAMP 的日期部分做為保守且有意義的預設排序。
                    // 避免在欄位上使用函式（例如 cast），以利使用索引；改為直接以完整時間欄位排序
                    paginatedSql = baseSql + " order by TRACE_LOG.TIMESTAMP " + $" OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY ";
                }
                else
                {
                    paginatedSql = baseSql + $" OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY ";
                }

                paramCopy.Add(new SqlParameter("@Offset", SqlDbType.Int) { Value = offset });
                paramCopy.Add(new SqlParameter("@PageSize", SqlDbType.Int) { Value = pageSize });

                dt = await Task.Run(() => Db.ExecuteDataTable(paginatedSql, CommandType.Text, paramCopy.ToArray()));
                dataGridView1.DataSource = dt;
                // 不在每次分頁時載入完整資料，匯出功能會在需要時再執行完整查詢
                Exceldt = dt;
            }
            finally
            {
                // 恢復原始逾時設定
                try { Db.CommandTimeoutSeconds = originalTimeout; } catch { }
                progressBar2.Visible = false;
            }
        }

        /*
        Pseudocode（詳細計畫）:
        1. 檢查傳入的 parameters 是否為 null，若為 null 回傳空的 List<SqlParameter>。
        2. 建立一個新的 List<SqlParameter> 作為回傳用。
        3. 對於每一個來源的 SqlParameter p：
           a. 嘗試建立一個新的 SqlParameter np，保留 ParameterName 與 SqlDbType。
           b. 將 Value 設為 p.Value（若為 null 則設為 DBNull.Value）。
           c. 複製 Size 與 Direction 等屬性（若有）。
           d. 將 np 加入回傳清單。
           e. 如果在建立或複製參數屬性時發生例外，使用簡單的 SqlParameter 建構子回退，僅保留 ParameterName 與 Value。
        4. 回傳新建立的參數清單。
        */

        /**
         <summary>
          複製傳入的 SqlParameter 集合，回傳一份深層複本的清單以避免原集合被後續呼叫端修改的副作用。
          此方法會嘗試保留常見的屬性（SqlDbType、Value、Size、Direction），
          若在複製過程中遇到無法複製的屬性或例外，會使用較保守的回退策略建立只包含參數名稱與值的 SqlParameter。
         </summary>
         <param name="parameters">
          要複製的參數集合。允許為 null；若為 null，方法會回傳一個空的 List&lt;SqlParameter&gt;。
         </param>
         <returns>
          回傳一個新的 List&lt;SqlParameter&gt;，每個元素為對應來源參數的複本。
          備註：回傳的參數物件與輸入參數物件為不同實例（參考不同），因此修改回傳集合不會影響原始集合。
         </returns>
         <remarks>
          - 此方法嘗試保留 SqlDbType/Size/Direction 等屬性以維持參數行為一致性。
          - 若來源參數的某些屬性在複製時導致例外（例如某些非典型的自訂屬性），會以較簡單的建構子回退以確保輸出仍然可用。
          - 若來源參數的 Value 為 null，會將複本的 Value 設為 DBNull.Value，以便直接加入 SqlCommand.Parameters 使用。
          - 此方法僅複製常用屬性；若有其他特殊屬性（例如精度/尺度、TypeName、UDT 等）需額外複製，可依需求擴充。
         </remarks>
        */
        private static List<SqlParameter> CloneParameters(IEnumerable<SqlParameter> parameters)
        {
            var list = new List<SqlParameter>();
            if (parameters == null) return list;

            foreach (var p in parameters)
            {
                try
                {
                    var np = new SqlParameter(p.ParameterName, p.SqlDbType)
                    {
                        Value = p.Value ?? DBNull.Value,
                        Size = p.Size,
                        Direction = p.Direction
                    };
                    // 若需要，可在此擴充複製 Precision / Scale / TypeName / UdtTypeName 等屬性
                    list.Add(np);
                }
                catch
                {
                    // 若無法完整複製（例如某些屬性在建構時拋例外），以最小資訊回退
                    list.Add(new SqlParameter(p.ParameterName, p.Value ?? DBNull.Value));
                }
            }
            return list;
        }

        /*
        Pseudocode / 詳細計畫（中文）:
        1. 驗證輸入參數 sql：
           - 若為 null 或僅包含空白字元，直接回傳原始輸入（保持原狀）。
        2. 在 SQL 字串中以不區分大小寫的方式尋找最後一次出現的 " order by " 子字串：
           - 使用 LastIndexOf(" order by ", StringComparison.OrdinalIgnoreCase) 取得位置。
        3. 若找到 " order by "：
           - 回傳從字串開頭到該位置之前的子字串（不包含 " order by " 及其後面內容）。
        4. 若找不到 " order by "：
           - 回傳原始 SQL 字串（表示無需移除）。
        5. 注意事項：
           - 使用 LastIndexOf 可減少誤判子查詢中的 ORDER BY，但對於非常複雜的 SQL（包含動態字串或非標準格式）仍可能不完全精確。
           - 此函式不嘗試解析 SQL 結構或處理引號內文字，僅以字串方式移除尾端的 ORDER BY。
        */

        /// <summary>
        /// 從傳入的 SQL 查詢字串中移除最末端的 ORDER BY 子句（若存在）。
        /// </summary>
        /// <param name="sql">輸入的 SELECT 查詢字串；可能包含 ORDER BY、WHERE、GROUP BY 等子句。</param>
        /// <returns>
        /// 回傳已移除最末端 ORDER BY 子句的 SQL 字串。
        /// 若輸入為 null、空白或沒有 ORDER BY，則回傳原始輸入字串（不做修改）。
        /// </returns>
        /// <remarks>
        /// - 該方法採用簡單的字串比對（不解析 SQL AST），以 " order by "（含前後空格）進行不區分大小寫的尋找並移除最後一個出現位置之後的內容。
        /// - 此設計可處理大多數情況下欲用於 COUNT 或重新組合分頁子句時移除排序的需求，但對於在子查詢或引號內含有 ORDER BY 的情境可能會誤判。
        /// - 若需要完全正確地處理任意複雜 SQL，建議改用 SQL 解析器來解析語法樹後移除尾端 ORDER BY。
        /// </remarks>
        /// <example>
        /// 範例：
        /// Input: "SELECT * FROM TRACE_LOG WHERE ... ORDER BY TRACE_LOG.TIMESTAMP DESC"
        /// Output: "SELECT * FROM TRACE_LOG WHERE ..."
        /// </example>
        private string RemoveOrderBy(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql)) return sql;
            int idx = sql.LastIndexOf(" order by ", StringComparison.OrdinalIgnoreCase);
            if (idx >= 0)
            {
                return sql.Substring(0, idx);
            }
            return sql;
        }

        /*
         * Pseudocode (詳細計畫):
         * 1. 如果輸入 SQL 為 null 或空白，回傳預設的 count 子查詢 "select count(*) from (select 1) as T"。
         * 2. 以不區分大小寫的方式尋找 " from " 子字串的第一個出現位置 (包含前後空格以避免匹配到內文)。
         * 3. 若找到 from:
         *    a. 取得從該位置開始到字串結尾的子字串 (from 及之後的部分)。
         *    b. 呼叫 RemoveOrderBy() 去除可能存在的 ORDER BY 子句，避免影響 COUNT 的語法或效能。
         *    c. 回傳組合為 "select count(*) " + 處理後的 from-and-after。
         * 4. 若找不到 from:
         *    a. 把原 SQL 去除 ORDER BY，包在子查詢中，回傳 "select count(*) from (" + cleanedSql + ") as T" 作為後備方案。
         * 5. 注意事項:
         *    - 此實作假定傳入的 SQL 為一般的 SELECT 查詢；若 SQL 非標準或包含多個 SELECT，fallback 會以子查詢方式處理。
         *    - 使用 RemoveOrderBy 可避免 ORDER BY 導致 COUNT 時的語法問題或額外排序成本。
         */
        /// <summary>
        /// 由傳入的 SELECT...FROM...WHERE...ORDER BY... SQL 建構對應的 COUNT 查詢字串。
        /// </summary>
        /// <param name="sql">輸入的完整 SQL 查詢字串（可包含 SELECT、FROM、WHERE、order by 等子句）。</param>
        /// <returns>
        /// 回傳用於計算資料總筆數的 SQL 字串。
        /// 若能解析出 FROM 子句，回傳形如 "select count(*) &lt;fromAndAfterWithoutOrderBy&gt;" 的字串；</returns>
        /// <remarks>
        /// - 此方法只嘗試以簡單的字串搜尋找到第一個 " from " (不區分大小寫) 來切分 SQL。
        /// - 若 SQL 包含多個 SELECT 或複雜語法，可能會無法精準切分，方法會回退到將整個 SQL 包成子查詢的策略以保證語法正確性。
        /// - 會呼叫 RemoveOrderBy(string) 以移除 order by 子句，因為 COUNT(*) 不需要排序且 order by 可能造成語法錯誤或額外的計算成本。
        /// - 保持原有行為：對於空或 null 的輸入，回傳一個安全且輕量的預設 COUNT 查詢。
        private string BuildCountSql(string sql)
        {
            // 如果輸入為 null 或僅有空白，直接回傳安全的預設 COUNT 查詢 // 驗證輸入
            if (string.IsNullOrWhiteSpace(sql)) // 判斷輸入是否為空或僅空白
                return "select count(*) from (select 1) as T"; // 回傳預設的輕量級 COUNT 查詢

            // 嘗試尋找第一個 " from " 子字串（不區分大小寫，並含前後空格以降低誤判） // 找 FROM 位置
            int idx = sql.IndexOf(" from ", StringComparison.OrdinalIgnoreCase); // 取得 first index of " from "

            // 若找到了 from 子句，使用該位置之後的字串來組建 COUNT 查詢 // 若找得到 FROM
            if (idx >= 0) // 檢查索引是否有效
            {
                // 如果找到 FROM 的分支開始
                var fromAndAfter = sql.Substring(idx); // 取得從 from 開始到結尾的子字串
                fromAndAfter = RemoveOrderBy(fromAndAfter); // 移除子字串中的 order by 子句以免影響 COUNT
                return "select count(*) " + fromAndAfter; // 回傳形如 "select count(*) <from ... without order by>"
            }
            // fallback：若無法解析出 FROM，則將整個 SQL（去除 order by）包成子查詢以確保語法正確 // 無 FROM 時的後備方案
            return "select count(*) from (" + RemoveOrderBy(sql) + ") as T"; // 回傳包成子查詢的 COUNT SQL
        }

        /// <summary>
        /// 視窗顯示後將視窗最大化。
        /// </summary>
        /// <param name="sender">事件來源</param>
        /// <param name="e">事件參數</param>
        private void Form1_Shown(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        #region Excel 匯出相關

        /// <summary>
        /// 非同步建立並寫入 Excel 工作簿（使用 NPOI）。
        /// 會建立樣式、表頭並透過 FillGunListData 填入資料，最後寫入檔案系統。
        /// </summary>
        /// <returns>Task 代表非同步作業</returns>
        private async Task ExpExcelSheet()
        {
            // 確保輸出目錄存在
            try
            {
                Directory.CreateDirectory(exportPath);
            }
            catch { }

            // 匯出可能為長時間操作，暫時延長命令逾時
            var originalTimeout = Db.CommandTimeoutSeconds;
            Db.CommandTimeoutSeconds = 300; // 5 分鐘

            XSSFWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("查詢資料");

            // 如果 Exceldt 已有資料，保留原有行為（較簡單）
            if (Exceldt != null && Exceldt.Rows.Count > 0)
            {
                // 設定ProgressBar屬性
                progressBar1.Minimum = 0;
                progressBar1.Maximum = Exceldt.Rows.Count + 1; // 總行數加1，表頭也要算
                progressBar1.Step = 1; // 每次更新一步

                // 更新進度條
                UpdateProgressBar(1);
                // 建立表頭
                CreateHeader(sheet);

                // 塞資料
                await FillGunListData(sheet, Exceldt);
            }
            else if (!string.IsNullOrWhiteSpace(lastSql))
            {
                // 使用串流方式：先查總筆數，再用 DataReader 逐筆寫入以節省記憶體
                string countSql = BuildCountSql(lastSql);
                int rowCount = 0;
                // 克隆參數
                var clonedParamsForCountList = CloneParameters(lastParams);

                // 先嘗試快速計數
                bool fastOk = TryFastCount(lastSql, clonedParamsForCountList, out rowCount);
                if (!fastOk)
                {
                    var clonedParamsForCount = clonedParamsForCountList.ToArray();
                    rowCount = await Task.Run(() => Db.ExecuteScalarInt(countSql, CommandType.Text, clonedParamsForCount));
                }

                progressBar1.Minimum = 0;
                progressBar1.Maximum = Math.Max(1, rowCount + 1);
                progressBar1.Step = 1;
                UpdateProgressBar(1);

                // 串流寫入 workbook（在背景執行）
                await Task.Run(() =>
                {
                    // 使用 EnsureOrderByTimestamp 確保輸出排序為 TRACE_LOG.TIMESTAMP
                    using (var reader = Db.ExecuteReaderPmsList(EnsureOrderByTimestamp(lastSql), CommandType.Text, lastParams))
                    {
                        int currentRow = 0;
                        int fieldCount = reader.FieldCount;

                        // 由 reader 取得欄位名稱並使用共用 CreateHeader 產生表頭（第 0 列）
                        var colNames = Enumerable.Range(0, fieldCount).Select(i => reader.GetName(i)).ToList();
                        CreateHeader(sheet, colNames);

                        // 從第 1 列開始寫入資料（第 0 列為表頭）
                        currentRow = 1;

                        // 逐筆讀取並寫入
                        while (reader.Read())
                        {
                            IRow row = sheet.CreateRow(currentRow++);
                            for (int i = 0; i < fieldCount; i++)
                            {
                                var val = reader.IsDBNull(i) ? string.Empty : reader.GetValue(i).ToString();
                                row.CreateCell(i).SetCellValue(val);
                            }

                            // 更新進度欄（回 UI 執行緒）
                            this.Invoke((Action)(() => UpdateProgressBar(currentRow)));
                        }
                    }
                });
            }
            else
            {
                // 沒有資料來源
                MessageBox.Show("沒有可匯出的資料。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 最後把 workbook 寫入檔案
            try
            {
                using (FileStream fileStream = File.Open(strFilePath + ".xlsx", FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fileStream);
                }

                // 恢復逾時設定
                try { Db.CommandTimeoutSeconds = originalTimeout; } catch { }
            }
            catch (Exception ee)
            {
                // 顯示錯誤給使用者
                // 如果是超時相關的例外，提供更友善的建議
                if (ee is System.Data.SqlClient.SqlException || ee.InnerException is System.Data.SqlClient.SqlException)
                {
                    MessageBox.Show("資料庫存取逾時：請檢查網路連線或增加查詢逾時，或優化 SQL。錯誤: " + ee.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("寫入檔案時發生錯誤: " + ee.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                try { Db.CommandTimeoutSeconds = originalTimeout; } catch { }
                return;
            }
        }

        /// <summary>
        /// 根據 Exceldt 的欄位建立 Excel 表頭，並使用 TableMap 進行欄位名稱翻譯。
        /// </summary>
        /// <param name="sheet">要建立表頭的 NPOI 工作表</param>
        private void CreateHeader(ISheet sheet)
        {
            // 使用共用欄位顯示對照表 ColumnDisplayMap

            // 建立標題樣式：置中、微軟正黑體 20pt
            var workbook = sheet.Workbook;
            ICellStyle headerStyle = workbook.CreateCellStyle();
            IFont headerfont = workbook.CreateFont();
            headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center; // 水平置中
            headerStyle.VerticalAlignment = VerticalAlignment.Center; // 垂直置中
            headerfont.FontName = "微軟正黑體";
            headerfont.FontHeightInPoints = 14;
            headerStyle.SetFont(headerfont);

            IRow headerRow = sheet.CreateRow(0); // 建立表頭列

            for (int i = 0; i < Exceldt.Columns.Count; i++)
            {
                string columnName = Exceldt.Columns[i].ColumnName;
                string displayName = ColumnDisplayMap.TryGetValue(columnName, out var display) ? display : columnName;
                var cell = headerRow.CreateCell(i);
                cell.SetCellValue(displayName);
                // 套用樣式
                cell.CellStyle = headerStyle;
                // 可選：自動欄寬（輕量）
                try { sheet.AutoSizeColumn(i); } catch { }
            }
        }

        /// <summary>
        /// 以欄位名稱清單建立工作表的表頭（支援 streaming reader 的情境）。
        /// </summary>
        private void CreateHeader(ISheet sheet, List<string> columnNames)
        {
            // 使用共用欄位顯示對照表 ColumnDisplayMap

            var workbook = sheet.Workbook;
            ICellStyle headerStyle = workbook.CreateCellStyle();
            IFont headerfont = workbook.CreateFont();
            headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center; // 水平置中
            headerStyle.VerticalAlignment = VerticalAlignment.Center; // 垂直置中
            headerfont.FontName = "微軟正黑體";
            headerfont.FontHeightInPoints = 14;
            headerStyle.SetFont(headerfont);

            IRow headerRow = sheet.CreateRow(0);
            for (int i = 0; i < columnNames.Count; i++)
            {
                var col = columnNames[i];
                string displayName = ColumnDisplayMap.TryGetValue(col, out var display) ? display : col;
                var cell = headerRow.CreateCell(i);
                cell.SetCellValue(displayName);
                cell.CellStyle = headerStyle;
                try { sheet.AutoSizeColumn(i); } catch { }
            }
        }

        /// <summary>
        /// 非同步將資料表資料填入指定的工作表中，並在每筆寫入後更新進度條 (UI 執行緒)。
        /// </summary>
        /// <param name="sheet">要填入資料的工作表</param>
        /// <param name="dataTables">來源資料表（通常為 Exceldt）</param>
        /// <returns>Task 代表非同步作業</returns>
        private async Task FillGunListData(ISheet sheet, DataTable dataTables)
        {
            int currentRow = 1; // 从第二行开始写入数据，第一行通常是表头

            await Task.Run(() =>
            {
                foreach (DataRow dataRow in dataTables.Rows)
                {
                    IRow row = sheet.CreateRow(currentRow++); // 創建新行，並遞增 currentRow

                    for (int k = 0; k < dataTables.Columns.Count; k++)
                    {
                        row.CreateCell(k).SetCellValue(dataRow[k].ToString()); // 使用dataRow[k] 而不是dataTables.Rows[k]
                    }

                    // 更新進度條（需要進行 UI 執行緒操作）
                    this.Invoke((Action)(() => UpdateProgressBar(currentRow)));
                }
            });
        }
        #endregion

        /// <summary>
        /// 安全地在 UI 執行緒上更新 progressBar1 的值。
        /// 若呼叫端不在 UI 執行緒，會透過 Invoke 回到 UI 執行緒執行更新。
        /// </summary>
        /// <param name="value">欲設定的進度值</param>
        private void UpdateProgressBar(int value)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int>(UpdateProgressBar), new object[] { value });
            }
            else
            {
                // 範圍檢查，避免 Value 超過 Min/Max 引發例外
                try
                {
                    if (value < progressBar1.Minimum) value = progressBar1.Minimum;
                    if (value > progressBar1.Maximum) value = progressBar1.Maximum;
                    progressBar1.Value = value;
                }
                catch
                {
                    // 忽略任何設定進度時的例外，UI 不應因此中斷
                }
            }
        }

        /// <summary>
        /// 分批匯出 SQL 查詢結果到 Excel，避免一次性載入全部資料。
        /// 會建立 workbook、逐批查詢並寫入，每批使用 OFFSET/FETCH，最後寫入檔案系統。
        /// </summary>
        private async Task ExportByBatchesAsync(string baseSql, SqlParameter[] parameters, int batchSize)
        {
            // 使用 SqlDataReader 流式讀取，逐筆寫入 Excel，並在達到 Excel 列上限時分檔寫出以降低記憶體使用
            const int ExcelMaxRows = 1048575; // 留一行給表頭
            int fileIndex = 1;
            int rowsInCurrentFile = 0;
            XSSFWorkbook workbook = null;
            ISheet sheet = null;
            long processed = 0;

            // 嘗試估算總筆數
            int totalRows = 0;
            try
            {
                string countSql = BuildCountSql(baseSql);
                totalRows = await Db.ExecuteScalarIntAsync(countSql, CommandType.Text, parameters);
            }
            catch { totalRows = 0; }

            // 設定 progress bar
            if (totalRows > 0)
            {
                this.Invoke((Action)(() =>
                {
                    progressBar1.Style = ProgressBarStyle.Continuous;
                    progressBar1.Minimum = 0;
                    progressBar1.Maximum = Math.Max(1, totalRows + 1);
                    progressBar1.Step = 1;
                    progressBar1.Value = progressBar1.Minimum;
                }));
            }
            else
            {
                this.Invoke((Action)(() =>
                {
                    try { progressBar1.Style = ProgressBarStyle.Marquee; progressBar1.MarqueeAnimationSpeed = 30; } catch { }
                }));
            }

            // StartNewWorkbook: optional columnNames - if provided create header using those names
            void StartNewWorkbook(List<string> columnNames = null)
            {
                workbook = new XSSFWorkbook();
                sheet = workbook.CreateSheet(fileIndex == 1 ? "查詢資料" : $"查詢資料_{fileIndex}");
                // 建立表頭（若有欄位名稱，使用會使用 streaming/read 的欄位清單；否則嘗試使用 Exceldt）
                if (columnNames != null && columnNames.Count > 0)
                {
                    CreateHeader(sheet, columnNames);
                }
                else
                {
                    CreateHeader(sheet);
                }
                // 表頭佔用第 0 列，資料從第 1 列開始
                rowsInCurrentFile = 1;
            }

            // 決定每次 flush 到磁碟的閾值（避免每筆或每小批就寫入 SSD）
            // 以 batchSize 為基準放大數倍；至少 10k，最多為 ExcelMaxRows
            int flushThreshold = Math.Min(ExcelMaxRows, Math.Max(10000, batchSize * 5));

            // 確保有 order by
            string streamSql = baseSql;
            if (baseSql.IndexOf(" order by ", StringComparison.OrdinalIgnoreCase) < 0)
            {
                streamSql = baseSql + " order by TRACE_LOG.TIMESTAMP ";
            }

            var paramList = CloneParameters(parameters).ToList();

            // 若估算筆數非常大，改用 CSV 流式匯出以降低記憶體與 NPOI 負載
            const int csvThreshold = 200000; // 超過此筆數改用 CSV
            if (totalRows > csvThreshold)
            {
                await ExportToCsvStreamAsync(streamSql, paramList, 1000000, 5000);
                return;
            }

            try
            {
                using (var reader = await Db.ExecuteReaderPmsListAsync(EnsureOrderByTimestamp(streamSql), CommandType.Text, paramList))
                {
                    int fieldCount = reader.FieldCount;
                    // 取得欄位名稱清單，並用於建立表頭
                    var colNames = Enumerable.Range(0, fieldCount).Select(i => reader.GetName(i)).ToList();
                    // 現在才建立 workbook 與標頭，避免使用不完整的 Exceldt 欄位資訊或覆寫標頭
                    StartNewWorkbook(colNames);

                    while (await reader.ReadAsync())
                    {
                        // 若即將寫入會超過 Excel 上限，先將現有 workbook 寫出並開始新檔
                        if (rowsInCurrentFile + 1 > ExcelMaxRows)
                        {
                            try
                            {
                                Directory.CreateDirectory(exportPath);
                                var outPathOld = strFilePath + $"_{fileIndex}.xlsx";
                                using (FileStream fs = File.Open(outPathOld, FileMode.Create, FileAccess.Write))
                                {
                                    workbook.Write(fs);
                                }
                            }
                            catch { }

                            fileIndex++;
                            StartNewWorkbook(colNames);
                        }

                        // 建立列並填值
                        IRow row = sheet.CreateRow(rowsInCurrentFile++);
                        for (int c = 0; c < fieldCount; c++)
                        {
                            var val = reader.IsDBNull(c) ? string.Empty : reader.GetValue(c)?.ToString() ?? string.Empty;
                            row.CreateCell(c).SetCellValue(val);
                        }

                        processed++;

                        // 更新進度
                        if (totalRows > 0)
                        {
                            int displayed = Math.Min(progressBar1.Maximum, progressBar1.Minimum + (int)processed);
                            this.Invoke((Action)(() => UpdateProgressBar(displayed)));
                        }
                        else
                        {
                            this.Invoke((Action)(() =>
                            {
                                try { progressBar1.Value = Math.Min(progressBar1.Maximum, rowsInCurrentFile); } catch { }
                            }));
                        }

                        // 若已達到 flush 閾值且尚未超過 Excel 上限，將目前 workbook 寫出（覆寫），以降低一次性大記憶體壓力
                        if (rowsInCurrentFile >= flushThreshold && rowsInCurrentFile < ExcelMaxRows)
                        {
                            try
                            {
                                Directory.CreateDirectory(exportPath);
                                var flushPath = strFilePath + $"_{fileIndex}.xlsx";
                                using (FileStream fs = File.Open(flushPath, FileMode.Create, FileAccess.Write))
                                {
                                    workbook.Write(fs);
                                }
                            }
                            catch { }
                        }
                    }
                }

                // 寫出最後一個 workbook（若有內容）
                try
                {
                    Directory.CreateDirectory(exportPath);
                    var outPath = strFilePath + $"_{fileIndex}.xlsx";
                    using (FileStream fs = File.Open(outPath, FileMode.Create, FileAccess.Write))
                    {
                        workbook.Write(fs);
                    }
                }
                catch { }
                finally
                {
                    this.Invoke((Action)(() =>
                    {
                        try { progressBar1.Style = ProgressBarStyle.Continuous; progressBar1.MarqueeAnimationSpeed = 0; progressBar1.Value = progressBar1.Minimum; } catch { }
                    }));
                }
            }
            catch (Exception ex)
            {
                // 若有例外，嘗試恢復 progressBar 樣式並丟出給上層處理
                this.Invoke((Action)(() =>
                {
                    try { progressBar1.Style = ProgressBarStyle.Continuous; progressBar1.MarqueeAnimationSpeed = 0; } catch { }
                }));
                throw;
            }
        }

        /// <summary>
        /// 使用 StreamWriter 與 SqlDataReader 逐筆寫入 CSV（UTF-8 BOM），支援分檔與定期 flush
        /// </summary>
        private async Task ExportToCsvStreamAsync(string baseSql, List<SqlParameter> parameters, int maxRowsPerFile = 1000000, int flushInterval = 5000)
        {
            int fileIndex = 1;
            long processed = 0;
            int rowsInFile = 0;

            // 嘗試估算總筆數
            int totalRows = 0;
            try
            {
                string countSql = BuildCountSql(baseSql);
                totalRows = await Db.ExecuteScalarIntAsync(countSql, CommandType.Text, parameters?.ToArray());
            }
            catch { totalRows = 0; }

            if (totalRows > 0)
            {
                this.Invoke((Action)(() =>
                {
                    progressBar1.Style = ProgressBarStyle.Continuous;
                    progressBar1.Minimum = 0;
                    progressBar1.Maximum = Math.Max(1, totalRows + 1);
                    progressBar1.Step = 1;
                    progressBar1.Value = progressBar1.Minimum;
                }));
            }
            else
            {
                this.Invoke((Action)(() => { try { progressBar1.Style = ProgressBarStyle.Marquee; progressBar1.MarqueeAnimationSpeed = 30; } catch { } }));
            }

            void UpdateProgress(long p)
            {
                if (totalRows > 0)
                {
                    int displayed = Math.Min(progressBar1.Maximum, progressBar1.Minimum + (int)p);
                    this.Invoke((Action)(() => UpdateProgressBar(displayed)));
                }
                else
                {
                    this.Invoke((Action)(() => { try { progressBar1.Value = Math.Min(progressBar1.Maximum, rowsInFile); } catch { } }));
                }
            }

            using (var reader = await Db.ExecuteReaderPmsListAsync(EnsureOrderByTimestamp(baseSql), CommandType.Text, parameters ?? new List<SqlParameter>()))
            {
                int fieldCount = reader.FieldCount;
                StreamWriter sw = null;
                try
                {
                    while (await reader.ReadAsync())
                    {
                        if (sw == null)
                        {
                            Directory.CreateDirectory(exportPath);
                            var path = strFilePath + $"_csv_part{fileIndex}.csv";
                            sw = new StreamWriter(new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 65536, FileOptions.SequentialScan), new System.Text.UTF8Encoding(true)); // include BOM
                                                                                                                                                                                                   // header - 使用共用 ColumnDisplayMap 進行欄位名稱翻譯
                            var headers = Enumerable.Range(0, fieldCount).Select(i =>
                            {
                                var col = reader.GetName(i);
                                if (ColumnDisplayMap.TryGetValue(col, out var display))
                                {
                                    return EscapeCsv(display);
                                }
                                return EscapeCsv(col);
                            });

                            sw.WriteLine(string.Join(",", headers));
                            rowsInFile = 0;
                        }

                        var cols = new string[fieldCount];
                        for (int c = 0; c < fieldCount; c++)
                        {
                            var val = reader.IsDBNull(c) ? string.Empty : reader.GetValue(c)?.ToString() ?? string.Empty;
                            cols[c] = EscapeCsv(val);
                        }
                        sw.WriteLine(string.Join(",", cols));
                        rowsInFile++;
                        processed++;

                        if (processed % flushInterval == 0)
                        {
                            try { sw.Flush(); } catch { }
                        }

                        UpdateProgress(processed);

                        if (rowsInFile >= maxRowsPerFile)
                        {
                            try { sw.Flush(); sw.Dispose(); } catch { }
                            fileIndex++;
                            sw = null;
                        }
                    }
                }
                finally
                {
                    try { sw?.Flush(); sw?.Dispose(); } catch { }
                    this.Invoke((Action)(() => { try { progressBar1.Style = ProgressBarStyle.Continuous; progressBar1.MarqueeAnimationSpeed = 0; progressBar1.Value = progressBar1.Minimum; } catch { } }));
                }
            }
        }

        /// <summary>
        /// 確保傳入的 SQL 含有 ORDER BY TRACE_LOG.TIMESTAMP，若已包含 ORDER BY 則原樣回傳。
        /// </summary>
        private string EnsureOrderByTimestamp(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql)) return sql;
            // 移除任何既有的 ORDER BY 子句，然後強制以 TRACE_LOG.TIMESTAMP 排序
            try
            {
                var noOrder = RemoveOrderBy(sql);
                return noOrder + " order by TRACE_LOG.TIMESTAMP";
            }
            catch
            {
                // 若移除失敗，保守回傳原字串並附上排序
                return sql + " order by TRACE_LOG.TIMESTAMP";
            }
        }

        /// <summary>
        /// 將輸入字串以 CSV 規則轉義，確保可安全寫入 CSV 檔。
        /// </summary>
        /// <param name="s">原始字串，可能為 null。</param>
        /// <returns>
        /// 已轉義的字串：
        /// - 若輸入為 null 回傳空字串。
        /// - 若字串包含逗號、雙引號或換行，會用雙引號包起來，且內部的雙引號會重複成兩個雙引號。
        /// - 否則回傳原字串。
        /// </returns>
        /// <remarks>
        /// - 依照常見 CSV 規範，若欄位內含逗號、雙引號或換行符，需以雙引號包覆；欄位內的雙引號需以兩個雙引號表示。
        /// - 此方法僅處理字面轉義，不會做其它格式化或編碼轉換（例如日期、數字）。
        /// - 呼叫端應確保最終輸出使用一致的編碼（例如 UTF-8 with BOM）以利 Excel 等應用程式正確開啟。
        /// </remarks>
        private static string EscapeCsv(string s)
        {
            if (s == null) return string.Empty;
            bool mustQuote = s.Contains(',') || s.Contains('"') || s.Contains('\n') || s.Contains('\r');
            if (s.Contains('"')) s = s.Replace("\"", "\"\"");
            return mustQuote ? "\"" + s + "\"" : s;
        }

        /*
         * 詳細計畫（逐步 pseudocode，中文說明）：
         * 1. 驗證輸入 SQL 是否為空或空白；若是則無法快速計數，回傳 false。
         * 2. 將 SQL 轉為小寫形式以便做簡單文本分析，並檢查是否包含 GROUP BY、DISTINCT、UNION、HAVING 或右括號等複雜語法；若包含則放棄快速計數，回傳 false。
         * 3. 嘗試以正規表示式偵測是否對 TRACE_LOG 表做查詢（例如 "from TRACE_LOG" 或帶 schema 的形式）；若偵測不到則放棄快速計數。
         * 4. 檢查 SQL 是否包含 WHERE 子句；若沒有 WHERE，直接使用 sys.dm_db_partition_stats 取得 TRACE_LOG 的行數並回傳（視為快速且精確的計數）。
         * 5. 若有 WHERE，嘗試做進一步的解析：若 WHERE 只是 PN.PN is not null（或再加上 PN.PN <> ''）則視為沒有 WHERE。
         * 6. 若仍有 WHERE，偵測 WHERE 是否為簡單的等值或前綴 LIKE 篩選（不含 OR、NOT、CHARINDEX 等複雜語法）；若包含複雜語法則放棄快速計數。
         * 7. 若 WHERE 看似簡單，使用 SET SHOWPLAN_XML ON 模式取得查詢執行計畫的估算行數（不執行查詢），解析回傳的 XML 以擷取估算行數，並以此為快速估算值回傳。
         * 8. 在任何步驟若發生例外或解析失敗，安全回退並回傳 false（代表請使用準確但較慢的 COUNT(*)）。
         *
         * 實作細節：
         * - 輸入的 parameters 可能為 null；在呼叫 Db 執行查詢時需傳入 parameters?.ToArray()。
         * - 使用正規化後的小寫 SQL 做簡單字串匹配以避免大小寫差異造成誤判。
         * - 使用 sys.dm_db_partition_stats 查詢時直接呼叫 Db.ExecuteDataTable 並解析回傳值。
         * - 使用 SHOWPLAN_XML 時，Db.ExecuteDataTable 會回傳包含 XML 的結果，解析 XML 以取得 EstimatedRows 等節點的值。
         * - 所有例外情況都應捕捉並回傳 false，確保呼叫端會退回到穩健但較慢的計數方式。
         */

        /// <summary>
        /// 嘗試以快速策略估算或取得查詢結果數量：
        /// 此方法會在安全的前提下使用資料庫 metadata 或執行計畫估算數量，避免直接執行 COUNT(*)。
        /// 若無法安全估算或遇到複雜查詢，回傳 false 並將 count 設為 0，呼叫者應使用較保守的計數方法。
        /// </summary>
        /// <param name="sql">欲估算筆數的 SELECT SQL（不一定要完整，但需包含 FROM TRACE_LOG）</param>
        /// <param name="parameters">與 SQL 相關的 SqlParameter 清單（允許為 null）</param>
        /// <param name="count">回傳的估算或精確筆數（成功時為估算值或精確值；失敗時為 0）</param>
        /// <returns>若成功以快速方式取得估算或精確筆數則回傳 true，否則回傳 false</returns>
        private bool TryFastCount(string sql, List<SqlParameter> parameters, out int count)
        {
            // 初始化輸出參數為 0
            count = 0; // 將輸出 count 預設為 0，避免未賦值情況

            try
            {
                // 若輸入 SQL 為空或僅空白，無法快速估算，直接回傳 false
                if (string.IsNullOrWhiteSpace(sql)) return false; // 檢查輸入合法性

                // 將 SQL 轉為小寫以便做後續的文本檢查（不影響原 SQL）
                var lower = sql.ToLowerInvariant(); // 正規化大小寫以利搜尋

                // 若 SQL 包含 GROUP BY、DISTINCT、UNION、HAVING 或右括號，視為複雜查詢，不使用快速估算
                if (lower.Contains(" group by ") || lower.Contains(" distinct ") || lower.Contains(" union ") || lower.Contains(" having ") || lower.Contains(")"))
                {
                    return false; // 直接放棄快速計數
                }

                // 嘗試以正則簡單匹配是否為針對 TRACE_LOG 表的查詢（支援 schema 或方括號）
                var m = Regex.Match(lower, @"from\s+(?:\[[^\]]+\]\.|[a-z0-9_]+\.)?trace_log\b"); // 找到 FROM TRACE_LOG 的片段
                if (!m.Success)
                {
                    return false; // 若找不到 TRACE_LOG，放棄快速計數
                }

                // 如果 WHERE 中含有參數化條件但沒有危險字串（例如 CHARINDEX、LIKE "%...%"）則仍可嘗試使用 partition stats 估算
                // 若沒有 WHERE（即整表查詢），使用 sys.dm_db_partition_stats 回傳精確資料行數
                // 判斷是否包含 WHERE 子句（簡單檢查）
                bool hasWhere = lower.Contains(" where "); // 確認是否有 WHERE

                // 特殊處理：若 WHERE 只是 PN.PN is not null 或再加上 PN.PN <> ''，視為無條件過濾
                if (hasWhere)
                {
                    try
                    {
                        // 找到 WHERE 的位置，並取得其後所有內容
                        int whereIdx = lower.IndexOf(" where "); // 取得 WHERE 的索引
                        var wherePart = lower.Substring(whereIdx + 7); // 擷取 WHERE 之後的內容

                        // 若有 ORDER BY，先裁掉以免干擾後續分析
                        int ordIdx = wherePart.IndexOf(" order by "); // 嘗試找到 ORDER BY
                        if (ordIdx >= 0) wherePart = wherePart.Substring(0, ordIdx); // 移除 ORDER BY 之後的內容

                        // 正規化空白字元，方便做正則比對
                        var normalized = Regex.Replace(wherePart, "\\s+", " ").Trim(); // 將多重空白替換為單一空白

                        // 若 where 內容符合特定的 pattern，視為沒有實質 WHERE
                        if (Regex.IsMatch(normalized, "^pn\\.pn\\s+is\\s+not\\s+null(\\s+and\\s+pn\\.pn\\s*<>\\s*''\\s*)?$"))
                        {
                            hasWhere = false; // 將 hasWhere 設為 false，允許使用 partition stats
                        }
                    }
                    catch
                    {
                        // 若解析失敗，保持原判斷（視為有 WHERE）
                    }
                }

                // 例如: "from TRACE_LOG ... where PN.PN = @pn and REEL.RID like @rid"
                // 若確定沒有 WHERE（幾乎為整表查詢），可以使用 metadata 查詢精確行數
                if (!hasWhere)
                {
                    // 使用 sys.dm_db_partition_stats 與 sys.objects 取得 TRACE_LOG 的行數（index_id 0 或 1）
                    var q = @"SELECT SUM(p.[row_count]) FROM sys.dm_db_partition_stats p JOIN sys.objects o ON p.[object_id]=o.[object_id] WHERE o.name='TRACE_LOG' AND p.index_id IN (0,1)"; // metadata 查詢
                    var dt = Db.ExecuteDataTable(q, CommandType.Text); // 執行查詢並取得 DataTable
                    if (dt != null && dt.Rows.Count > 0 && long.TryParse(dt.Rows[0][0].ToString(), out var lv))
                    {
                        // 將 long 轉為 int，並避免超過 int.MaxValue
                        count = (int)Math.Min(lv, int.MaxValue); // 取得安全的整型值
                        return true; // 成功取得精確值
                    }
                    return false; // 無法從 metadata 取得值，放棄快速計數
                }

                // 若有 WHERE，嘗試進一步判斷 WHERE 是否為簡單的等值或前綴 LIKE（允許 AND；不允許 OR、NOT、CHARINDEX）
                if (hasWhere)
                {
                    try
                    {
                        // 若包含 OR、NOT、CHARINDEX 或雙重百分比等複雜字串，則放棄快速估算
                        if (lower.Contains(" or ") || lower.Contains(" not ") || lower.Contains("charindex") || lower.Contains("%" + "%"))
                        {
                            return false; // 含有不安全或複雜語法，回退
                        }

                        // 使用 SHOWPLAN_XML 模式取得查詢的執行計畫（只返回估算，不執行查詢）
                        var planSql = "SET SHOWPLAN_XML ON; " + sql + "; SET SHOWPLAN_XML OFF;"; // 組成 SHOWPLAN 查詢
                        var planDt = Db.ExecuteDataTable(planSql, CommandType.Text, parameters?.ToArray()); // 執行並取得計畫 XML
                        if (planDt != null && planDt.Rows.Count > 0)
                        {
                            // 第一欄通常為 XML 字串，解析之
                            var xml = planDt.Rows[0][0].ToString(); // 取得 XML 內容
                            var doc = XDocument.Parse(xml); // 解析為 XDocument
                                                            // 嘗試找到 EstimatedRows、EstimateRows 或 EstimateRowsDistribution 等節點
                            var estimated = doc.Descendants().Where(x => x.Name.LocalName == "EstimateRows" || x.Name.LocalName == "EstimatedRows" || x.Name.LocalName == "EstimateRowsDistribution").FirstOrDefault();
                            if (estimated != null)
                            {
                                // 嘗試將節點值轉為 double，並四捨五入成 int
                                if (double.TryParse(estimated.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
                                {
                                    count = (int)Math.Min((long)Math.Round(d), int.MaxValue); // 取得估算值並限制在 int 範圍
                                    return true; // 成功以執行計畫估算筆數
                                }
                            }
                        }
                    }
                    catch
                    {
                        // 若 SHOWPLAN 或解析失敗，回退到放棄快速計數
                        return false; // 安全回退
                    }
                }

                // 若有 WHERE 但無法成功估算，回傳 false 以讓呼叫端使用穩健的計數方式
                return false; // 默認回退
            }
            catch
            {
                // 若發生任何未預期的例外，回傳 false 保持安全
                return false; // 在任何錯誤情況下回退
            }
        }


        /// <summary>
        /// DataGridView 資料綁定完成後的處理：將欄位名稱根據 ColumnDisplayMap 翻譯為顯示用的中文標題。
        /// </summary>
        /// <param name="sender">事件來源</param>
        /// <param name="e">綁定完成事件參數</param>
        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (dt == null) return;
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                var colName = dt.Columns[i].ColumnName;
                if (ColumnDisplayMap.TryGetValue(colName, out var header))
                {
                    dataGridView1.Columns[i].HeaderText = header;
                }
                else
                {
                    dataGridView1.Columns[i].HeaderText = colName;
                }
            }
        }





        /// <summary>
        /// 覆寫 WndProc，以攔截標題列的雙擊事件，防止視窗最大化或最小化。
        /// </summary>
        /// <param name="m">Windows 訊息結構</param>
        protected override void WndProc(ref Message m)
        {
            const int WM_NCLBUTTONDBLCLK = 0xA3;  // 非客戶區域的左鍵雙擊訊息

            // 攔截雙擊標題欄的訊息，阻止最大化或最小化操作
            if (m.Msg == WM_NCLBUTTONDBLCLK)
            {
                return; // 阻止雙擊標題欄的預設行為
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// 視窗載入事件：在標題列追加目前執行檔的版本號。
        /// </summary>
        /// <param name="sender">事件來源</param>
        /// <param name="e">事件參數</param>
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text += "  " + FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion.ToString();
        }
    }
}
