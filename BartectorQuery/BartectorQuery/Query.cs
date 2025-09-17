using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace BartectorQuery
{
    /// <summary>
    /// 提供用於組合並產生 TRACE_LOG 查詢字串與對應參數的功能類別。
    /// 使用類別屬性作為查詢條件，呼叫 QDate() 取得完整 SQL 與 SqlParameter 列表。
    /// </summary>
    public class Query
    {
        #region 參數
        /// <summary>
        /// 日期start
        /// 可選的日期型別起點，若設定則優先使用，格式正確且可避免字串解析。
        /// </summary>
        public DateTime? Date1_dt { get; set; }

        /// <summary>
        /// 日期end
        /// 可選的日期型別終點，若設定則優先使用，格式正確且可避免字串解析。
        /// </summary>
        public DateTime? Date2_dt { get; set; }

        /// <summary>
        /// 工程編號，用來在 MPROG 欄位中做字串比對 (使用 CHARINDEX)。
        /// </summary>
        public string EngSr { get; set; } //工程編號

        /// <summary>
        /// 工單號碼 (可為逗號分隔的多筆值)，會轉成多個 @wipN 參數進行等於比對。
        /// </summary>
        public string Wono { get; set; } //工單號碼

        /// <summary>
        /// 指示是否只查詢 A 面 (會檢查 MPROG 中是否包含 "_A_")。
        /// </summary>
        public bool Side_A { get; set; }//A面

        /// <summary>
        /// 指示是否只查詢 B 面 (會檢查 MPROG 中是否包含 "_B_")。
        /// </summary>
        public bool Side_B { get; set; } //B面

        /// <summary>
        /// 零件料號 (PN)，會與 PN.PN 欄位做相等比對。
        /// </summary>
        public string PN { get; set; } //零件料號

        /// <summary>
        /// 零件 LOT 代碼，會與 REEL.LOT 欄位做相等比對。
        /// </summary>
        public string LotCode { get; set; }//零件LOT

        /// <summary>
        /// 零件日期碼 (Date Code)，會與 REEL.DATECODE 欄位做相等比對。
        /// </summary>
        public string DateCode { get; set; }//零件DC

        /// <summary>
        /// 供應商代碼，會與 REEL.SUPPLIER 欄位做相等比對。
        /// </summary>
        public string Supplier { get; set; }//零件供應商

        /// <summary>
        /// PCB / PCBA 序號，會與 TRACE_LOG.PCBID 欄位做相等比對。
        /// </summary>
        public string PCBID { get; set; }//PCB序號

        /// <summary>
        /// Reel 編號，可做前綴比對 (LIKE @rid + '%')。
        /// </summary>
        public string ReelID { get; set; }//Reel ID

        /// <summary>
        /// SMT 站別，會與 TRACE_LOG.TRACE_STATION 做相等比對。
        /// </summary>
        public string Station { get; set; }//SMT站別

        /// <summary>
        /// 製件程式 (MPROG)，可用來做完全比對。
        /// </summary>
        public string Program { get; set; }//製件程式

        /// <summary>
        /// 機台料站 (LOC)，會與 TRACE_LOG.LOC 做相等比對。
        /// </summary>
        public string Slot { get; set; }//機台料站

        /// <summary>
        /// 料槍編號 (FCODE)，會與 TRACE_LOG.FCODE 做相等比對。
        /// </summary>
        public string FeederID { get; set; }//料槍編號

        /// <summary>
        /// 要查詢的欄位集合 (可由外部設定)，若未設定則在 merge() 中使用預設欄位集合。
        /// 預設欄位包含時間、工單、料號、LOT、DATECODE、SUPPLIER、PCBID、RID、TRACE_STATION、FCODE、LOC、MPROG。
        /// </summary>
        public List<string> columns = new List<string>();

        /// <summary>
        /// 查詢時的最大筆數限制（供 UI 層設定）。若為 0 則代表不限制（使用者應小心）。
        /// </summary>
        public int MaxRows { get; set; } = 0;

        /// <summary>
        /// 為 TRACE_LOG 與 REEL、PN 的 JOIN 條件與基本 WHERE 子句 (PN.PN is not null)
        /// </summary>
        private string baseSql;
        /// <summary>
        /// 組成完整的 SELECT 子句 (select {columns} {baseSql})，供後續 QDate() 使用
        /// </summary>
        private string basemerge;
        #endregion

        DataSet ds = new DataSet();

        /// <summary>
        /// 初始化或合併基底查詢字串與預設欄位集合。
        /// </summary>
        /// <remarks>
        /// - 若 columns 集合為空，會設定預設欄位清單。
        /// - 設定 baseSql 為 TRACE_LOG 與 REEL、PN 的 JOIN 條件與基本 WHERE 子句 (PN.PN is not null)。
        /// - basemerge 會組成完整的 SELECT 子句 (select {columns} {baseSql})，供後續 QDate() 使用。
        /// </remarks>
        public void merge()
        {
            if (columns.Count == 0)
            {
                columns = new List<string>() { "TRACE_LOG.TIMESTAMP", "TRACE_LOG.KITID", "PN.PN", "REEL.LOT", "REEL.DATECODE", "REEL.SUPPLIER", "TRACE_LOG.PCBID", "REEL.RID", "TRACE_LOG.TRACE_STATION", "TRACE_LOG.FCODE", "TRACE_LOG.LOC", "TRACE_LOG.MPROG" };
            }
            baseSql =
            @" from TRACE_LOG inner join REEL on TRACE_LOG.RID=REEL.RID inner join PN on REEL.SPN= PN.SPN where PN.PN is not null AND PN.PN <>'' ";

            basemerge = $"select {string.Join(", ", columns)} {baseSql}";
        }

        /// <summary>
        /// 根據類別屬性所設定的條件組合 SQL WHERE 子句，並回傳完整 SQL 與對應的 SqlParameter 清單。
        /// </summary>
        /// <returns>
        /// 回傳一個 Tuple：
        /// Item1: 組合完成的 SQL 字串 (包含 SELECT 與 WHERE/ORDER BY)。
        /// Item2: 與 SQL 中參數對應的 List&lt;SqlParameter&gt;，可直接用於 SqlCommand.Parameters.AddRange()。
        /// </returns>
        /// <remarks>
        /// 範例使用情境：
        /// - 先透過屬性設定過濾條件 (Date1, Date2, EngSr, Wono 等)。
        /// - 呼叫 QDate() 取得 SQL 與參數，使用 SqlCommand 執行查詢以取得結果。
        ///
        /// 注意：
        /// - 若 Date1 或 Date2 包含 "eg"，會被視為未指定並被清空。
        /// - Wono 可為逗號分隔多值，會轉換為多個相等比較條件 (TRACE_LOG.KITID = @wip0 OR ... )。
        /// - Side_A / Side_B 互斥判斷：若 Side_A 為 true 則只查 A 面；若 Side_B 為 true 則只查 B 面。
        /// - 最終查詢依照 cast(TRACE_LOG.TIMESTAMP as date) 排序。
        /// </remarks>
        public Tuple<string, List<SqlParameter>> QDate()
        {
            // 確保 baseSql 與 basemerge 已正確組合
            merge();
            // 用來存放 SqlParameter 的集合
            var parameters = new List<SqlParameter>();
            // 使用 StringBuilder 來組合 WHERE 子句
            var sb = new StringBuilder();

            #region 生產日期條件
            // 使用強型別日期作為查詢來源（Form1 已改為設定 Date1_dt / Date2_dt）
            // 舊版字串屬性 Date1/Date2 已標記為 Obsolete，若未設定強型別欄位則不套用日期篩選
            DateTime? start = Date1_dt;
            DateTime? end = Date2_dt;

            // 日期範圍處理：以 yyyyMMdd（日期不含時間）為單位來比對，仍然採用半開區間 [startDate, endDateExclusive)
            // 為了確保僅比對日期部分，使用 SqlDbType.Date 並將參數設為當天的日期（時間為 00:00:00），
            // 這樣可以避免在資料庫欄位上使用函式而影響索引效能。
            if (start.HasValue && !end.HasValue)
            {
                // 等同於當日所有時間：TRACE_LOG.TIMESTAMP >= start.Date AND TRACE_LOG.TIMESTAMP < start.Date + 1 day
                sb.Append(" AND TRACE_LOG.TIMESTAMP >= @startDate AND TRACE_LOG.TIMESTAMP < @startDateNext ");
                var s = start.Value.Date;
                parameters.Add(new SqlParameter("@startDate", SqlDbType.Date) { Value = s });
                parameters.Add(new SqlParameter("@startDateNext", SqlDbType.Date) { Value = s.AddDays(1) });
            }
            else if (!start.HasValue && end.HasValue)
            {
                // 查詢小於 end 的下一日（包含整個 end 日）
                sb.Append(" AND TRACE_LOG.TIMESTAMP < @endDateExclusive ");
                var eExclusive = end.Value.Date.AddDays(1);
                parameters.Add(new SqlParameter("@endDateExclusive", SqlDbType.Date) { Value = eExclusive });
            }
            else if (start.HasValue && end.HasValue)
            {
                // 範圍：TRACE_LOG.TIMESTAMP >= start.Date AND TRACE_LOG.TIMESTAMP < (end.Date + 1)
                sb.Append(" AND TRACE_LOG.TIMESTAMP >= @startDate AND TRACE_LOG.TIMESTAMP < @endDateExclusive ");
                var s = start.Value.Date;
                var eExclusive = end.Value.Date.AddDays(1);
                parameters.Add(new SqlParameter("@startDate", SqlDbType.Date) { Value = s });
                parameters.Add(new SqlParameter("@endDateExclusive", SqlDbType.Date) { Value = eExclusive });
            }
            #endregion
            #region 工程編號
            // 工程編號改用 LIKE 並把 % 放在參數值上，避免在 SQL 中直接拼接字串
            if (!string.IsNullOrWhiteSpace(EngSr))
            {
                sb.Append(" AND TRACE_LOG.MPROG LIKE @engsr ");
                parameters.Add(new SqlParameter("@engsr", "%" + EngSr + "%"));
            }
            #endregion
            #region 正反面
            // 正反面使用 LIKE 檢查
            if (Side_A)
            {
                sb.Append(" AND TRACE_LOG.MPROG LIKE '%_A_%' ");
            }
            else if (Side_B)
            {
                sb.Append(" AND TRACE_LOG.MPROG LIKE '%_B_%' ");
            }
            #endregion
            #region 工單號碼
            // 工單號碼：支援逗號分隔、多參數化並改用 IN(...) 子句
            // 效能改善：為每個參數明確指定 SqlDbType 與 Size，可減少 SQL Server 對參數型別的推斷，幫助快取/重用查詢計劃。
            if (!string.IsNullOrWhiteSpace(Wono))
            {
                var tokens = Wono.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                 .Select(t => t.Trim()).Where(t => t.Length > 0).ToArray();
                if (tokens.Length > 0)
                {
                    var names = new List<string>();
                    for (int i = 0; i < tokens.Length; i++)
                    {
                        var pname = "@wip" + i;
                        names.Add(pname);
                        // 根據資料庫設為 varchar(30)
                        var val = tokens[i];
                        const int kitidSize = 30; // TRACE_LOG.KITID varchar(30)
                        var param = new SqlParameter(pname, SqlDbType.VarChar, kitidSize) { Value = val };
                        parameters.Add(param);
                    }
                    sb.Append(" AND TRACE_LOG.KITID IN (" + string.Join(", ", names) + ") ");
                }
            }
            #endregion
            #region 零件料號PN
            // 其餘條件維持原有語意，但改用 StringBuilder 及參數化
            if (!string.IsNullOrWhiteSpace(PN))
            {
                sb.Append(" AND PN.PN = @pn ");
                parameters.Add(new SqlParameter("@pn", PN));
            }
            #endregion
            #region 零件LOT
            if (!string.IsNullOrWhiteSpace(LotCode))
            {
                sb.Append(" AND REEL.LOT = @lotcode ");
                parameters.Add(new SqlParameter("@lotcode", LotCode));
            }
            #endregion
            #region 零件DC
            if (!string.IsNullOrWhiteSpace(DateCode))
            {
                sb.Append(" AND REEL.DATECODE = @dc ");
                parameters.Add(new SqlParameter("@dc", DateCode));
            }
            #endregion
            #region 供應商
            if (!string.IsNullOrWhiteSpace(Supplier))
            {
                sb.Append(" AND REEL.SUPPLIER = @supplier ");
                parameters.Add(new SqlParameter("@supplier", Supplier));
            }
            #endregion
            #region PCBA序號
            if (!string.IsNullOrWhiteSpace(PCBID))
            {
                sb.Append(" AND TRACE_LOG.PCBID = @pcba ");
                parameters.Add(new SqlParameter("@pcba", PCBID));
            }
            #endregion
            #region ReelID
            if (!string.IsNullOrWhiteSpace(ReelID))
            {
                // 把 % 放到參數值，SQL 更簡潔
                sb.Append(" AND REEL.RID LIKE @rid ");
                parameters.Add(new SqlParameter("@rid", ReelID + "%"));
            }
            #endregion
            #region SMT站別
            if (!string.IsNullOrWhiteSpace(Station))
            {
                sb.Append(" AND TRACE_LOG.TRACE_STATION = @station ");
                parameters.Add(new SqlParameter("@station", Station));
            }
            #endregion
            #region 料槍
            if (!string.IsNullOrWhiteSpace(FeederID))
            {
                sb.Append(" AND TRACE_LOG.FCODE = @feeder ");
                parameters.Add(new SqlParameter("@feeder", FeederID));
            }
            #endregion
            #region 製件程式
            if (!string.IsNullOrWhiteSpace(Program))
            {
                sb.Append(" AND TRACE_LOG.MPROG = @program ");
                parameters.Add(new SqlParameter("@program", Program));
            }
            #endregion
            #region 機台料站
            if (!string.IsNullOrWhiteSpace(Slot))
            {
                sb.Append(" AND TRACE_LOG.LOC = @slot ");
                parameters.Add(new SqlParameter("@slot", Slot));
            }
            #endregion
            // 避免在資料庫欄位上使用函式以利索引使用，改以完整時間排序
            sb.Append(" ORDER BY TRACE_LOG.TIMESTAMP ");
            return new Tuple<string, List<SqlParameter>>(basemerge + sb.ToString(), parameters);
        }
    }
}



