using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static BartectorQuery.Query;

namespace BartectorQuery
{
    public partial class Form1 : AutoSizeWindows
    {
        private int currentPage = 1;
        private const int pageSize = 50;
        private int _totalPage = 0;// 總共多少分頁
        private string lastSql = "";
        private List<SqlParameter> lastParams = new List<SqlParameter>();
        public Query GetQuery = new Query();
        DataTable dt = new DataTable();
        DataTable Exceldt = new DataTable();
        static string exportPath = Environment.CurrentDirectory + "\\Export\\";
        //string exportPath = @"D:\排程\CMC_DC_Over\Export";
        string strFilePath = exportPath + DateTime.Now.ToString("yyyyMMddHHmmss");
        public Form1()
        {
            InitializeComponent();
        }

        private async Task LoadPageData(string baseSql, List<SqlParameter> parameters, int pageNumber)
        {
            progressBar2.Visible = true;
            
            await Task.Run(() =>
            {
                Exceldt = Db.ExecuteDataTable(baseSql, CommandType.Text, parameters.ToArray());

                // 移除旧的分页参数（如果存在）
                parameters.RemoveAll(p => p.ParameterName == "@Offset" || p.ParameterName == "@PageSize");

                int offset = (pageNumber - 1) * pageSize;
                string paginatedSql = $@"{baseSql} OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY ";

                parameters.Add(new SqlParameter("@Offset", offset));
                parameters.Add(new SqlParameter("@PageSize", pageSize));

                dt = Db.ExecuteDataTable(paginatedSql, CommandType.Text, parameters.ToArray());
            });
            dataGridView1.DataSource = dt;

            int rowCount = Exceldt.Rows.Count;
            _totalPage = rowCount / pageSize;

            //不足一個分頁行數的還是算一頁
            if (rowCount % pageSize > 0)
                _totalPage += 1;

            lb_totalPage.Text = _totalPage.ToString();
            txtPageNumber.Text = $"{pageNumber}";
            if (rowCount > 0)
            {
                btnNextPage.Enabled = true;
                btnPreviousPage.Enabled = true;
            }
            else
            {
                btnNextPage.Enabled = false;
                btnPreviousPage.Enabled = false;
            }
            // 完成後隱藏 ProgressBar
            progressBar2.Visible = false;
        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        private async void btnNextPage_Click(object sender, EventArgs e)
        {
            currentPage++;
            await LoadPageData(lastSql, lastParams, currentPage);
        }

        private async void btnPreviousPage_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                await LoadPageData(lastSql, lastParams, currentPage);
            }
        }
        private async Task ExpExcelSheet()
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            //設定            
            ICellStyle headerStyle = workbook.CreateCellStyle();
            IFont headerfont = workbook.CreateFont();
            headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;//水平置中
            headerStyle.VerticalAlignment = VerticalAlignment.Center;//垂直置中

            headerfont.FontName = "微軟正黑體";
            headerfont.FontHeightInPoints = 20;
            //headerfont.Boldweight = (short)FontBoldWeight.Bold;
            headerStyle.SetFont(headerfont);
            ISheet sheet = workbook.CreateSheet("查詢資料");
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

            try
            {
                // 寫入文件
                using (FileStream fileStream = File.Open(strFilePath + ".xlsx", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    workbook.Write(fileStream); // 寫入檔案
                }
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message);
                Console.ReadLine();
            }
        }
        //表頭↓↓
        private void CreateHeader(ISheet sheet)
        {
            Dictionary<string, string> TableMap = new Dictionary<string, string>
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
            IRow headerRow = sheet.CreateRow(0); // 建立表頭列

            for (int i = 0; i < Exceldt.Columns.Count; i++)
            {
                string columnName = Exceldt.Columns[i].ColumnName;
                headerRow.CreateCell(i).SetCellValue(TableMap[columnName]); // 使用 TableMap 進行翻譯
            }
        }
        // 塞資料
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
        private void UpdateProgressBar(int value)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int>(UpdateProgressBar), new object[] { value });
            }
            else
            {
                progressBar1.Value = value;
            }
        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void txt_engsr_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private async void btn_Serach_Click(object sender, EventArgs e)
        {
            // 每次新查詢時將頁碼重置為 1
            currentPage = 1;
            // 查詢之前顯示進度條
            progressBar2.Visible = true;            
            CheckedChanged();
            GetQuery.Date1 = txt_date1.Text.Trim();
            GetQuery.Date2 = txt_date2.Text.Trim();
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

            var queryresult = await Task.Run(() => GetQuery.QDate());
            lastSql = queryresult.Item1;
            lastParams = queryresult.Item2;
            await LoadPageData(lastSql, lastParams, currentPage);
            // 查詢完成後隱藏進度條
            progressBar2.Visible = false;
        }

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
                            if (parts.Length>3)
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

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            Dictionary<string, string> TableMap = new Dictionary<string, string>
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

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                dataGridView1.Columns[i].HeaderText = TableMap[dt.Columns[i].ToString()];
            }
        }

        private void txt_date1_Enter(object sender, EventArgs e)
        {
            if (txt_date1.Text=="eg. 2 0 2 4 0 1 0 1")
            {
                txt_date1.Text = "";
            }
        }

        private void txt_date2_Enter(object sender, EventArgs e)
        {
            if (txt_date2.Text == "eg. 2 0 2 4 0 4 0 1")
            {
                txt_date2.Text = "";
            }
        }

        private async void btn_export_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = true;  // 顯示進度條
            await ExpExcelSheet();        // 執行匯出作業
            MessageBox.Show("Excel 匯出完成！");
            progressBar1.Visible = false; // 匯出完成後隱藏進度條
        }

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

        private void txt_kyworld_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar==13)
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

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text += "  " + FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion.ToString();
        }
    }
}
