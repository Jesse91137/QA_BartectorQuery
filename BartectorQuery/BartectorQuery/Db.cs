using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using Dapper;

namespace BartectorQuery
{
    /*
        PSEUDOCODE / 計畫 (詳細步驟)
        1. 在類別與所有公開或內部重要方法、欄位前新增 XML 文件註解（zh-TW）。
           - 包含 <summary>、<param>、<returns>、<remarks>、<exception>（若適用）。
        2. 對靜態建構子與靜態欄位也加入文件註解，說明用途與行為。
        3. 保留原有程式邏輯與實作，僅插入說明註解，不更動方法內容或簽章。
        4. 對私有輔助方法也提供簡短文件，便於維護與閱讀。
        5. 確保所有參數名稱與現有程式碼一致，並針對可能拋出的例外給予註記（若適用）。
        6. 輸出完整檔案內容，讓使用者可直接替換原始檔案。
        */

    /// <summary>
    /// 提供應用程式操作 SQL Server 的共用靜態資料庫輔助方法。
    /// 包含同步/非同步的資料查詢、ExecuteScalar 與 SqlDataReader 取得等常用方法。
    /// </summary>
    public class Db
    {
        /// <summary>
        /// 全域可設定的 SQL 命令逾時（秒）。預設為 60 秒。
        /// 可於執行時調整以處理長時間查詢。
        /// </summary>
        public static int CommandTimeoutSeconds { get; set; } = 60;

        /// <summary>
        /// 靜態建構子：應用啟動時嘗試從 App.config 的 appSettings 讀取 DbCommandTimeoutSeconds 設定值。
        /// 若設定無效或讀取失敗，會保留預設值。
        /// </summary>
        static Db()
        {
            try
            {
                var s = System.Configuration.ConfigurationManager.AppSettings["DbCommandTimeoutSeconds"];
                if (!string.IsNullOrWhiteSpace(s) && int.TryParse(s, out var v) && v > 0)
                {
                    CommandTimeoutSeconds = v;
                }
            }
            catch
            {
                // 不要讓讀取設定失敗阻斷應用啟動，保留預設值
            }
        }

        #region paramater method

        /// <summary>
        /// 連線字串從 App.config 的 &lt;connectionStrings&gt; 讀取。
        /// 若找不到名為 "conString" 的設定，會丟出 ConfigurationErrorsException。
        /// </summary>
        private static readonly String connStr = ConfigurationManager.ConnectionStrings["conString"]?.ConnectionString
            ?? throw new ConfigurationErrorsException("Missing connection string 'conString' in App.config");

        /// <summary>
        /// 以 SqlDataAdapter 執行查詢並回傳結果為 DataTable。
        /// 適用於需要整個結果集並載入記憶體的情境。
        /// </summary>
        /// <param name="sql">要執行的 SQL 或 Stored Procedure 名稱。</param>
        /// <param name="cmdType">命令型別，例如 CommandType.Text 或 CommandType.StoredProcedure。</param>
        /// <param name="pms">可選的 SqlParameter 陣列，會被複製後加入命令。</param>
        /// <returns>查詢結果的 DataTable；若無資料則回傳空的 DataTable。</returns>
        /// <exception cref="Exception">當執行查詢過程發生任何例外時會向上拋出。</exception>
        public static DataTable ExecuteDataTable(string sql, CommandType cmdType, params SqlParameter[] pms)
        {
            DataTable dt = new DataTable();
            //use SqlDataAdapter ,it will establish Sql connection.So ,it no need to create Connection by yourself.
            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(sql, connStr))
                {
                    adapter.SelectCommand.CommandType = cmdType;
                    // 設定 SelectCommand 的逾時
                    adapter.SelectCommand.CommandTimeout = CommandTimeoutSeconds;
                    if (pms != null)
                    {
                        adapter.SelectCommand.Parameters.AddRange(CloneParameters(pms));

                    }
                    adapter.Fill(dt);
                    adapter.SelectCommand.Parameters.Clear();
                }
                sw.Stop();
                Logger.LogQuery(sql, pms, sw.Elapsed, "ExecuteDataTable");
                return dt;
            }
            catch (Exception ex)
            {
                sw.Stop();
                Logger.LogError(ex, "ExecuteDataTable");
                throw;
            }
        }

        /// <summary>
        /// 執行命令並回傳 SqlDataReader，由呼叫端負責讀取與關閉。當 reader 關閉時會同時關閉連線。
        /// 使用此方法可在需要逐筆讀取或串流大欄位時降低記憶體使用。
        /// </summary>
        /// <param name="sql">要執行的 SQL 或 Stored Procedure 名稱。</param>
        /// <param name="cmdType">命令型別。</param>
        /// <param name="pms">以 List&lt;SqlParameter&gt; 提供的參數清單（可為 null）。</param>
        /// <returns>已開啟的 SqlDataReader，讀取完成後請呼叫 reader.Close()。</returns>
        /// <exception cref="Exception">若建立連線或執行命令失敗，會拋出例外並嘗試清理連線。</exception>
        public static SqlDataReader ExecuteReaderPmsList(string sql, CommandType cmdType, List<SqlParameter> pms)
        {
            // Keep the command alive for the lifetime of the reader.
            SqlConnection con = new SqlConnection(connStr);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.CommandType = cmdType;
            // 設定命令逾時
            cmd.CommandTimeout = CommandTimeoutSeconds;
            if (pms != null)
            {
                cmd.Parameters.AddRange(CloneParameters(pms));
            }
            try
            {
                con.Open();
                var sw = System.Diagnostics.Stopwatch.StartNew();
                // Use SequentialAccess for streaming large binary/text columns efficiently
                var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess);
                sw.Stop();
                Logger.LogQuery(sql, pms, sw.Elapsed, "ExecuteReaderPmsList");
                return reader;
            }
            catch
            {
                // Ensure connection is cleaned up on error
                try { con.Close(); } catch { }
                try { con.Dispose(); } catch { }
                Logger.LogError(new Exception("ExecuteReaderPmsList failed"));
                throw;
            }
        }

        /// <summary>
        /// 複製 SqlParameter 集合，避免原始參數在重複使用時被修改造成副作用。
        /// 會複製常用屬性（Name, Value, DbType, Direction, Size, Precision, Scale, IsNullable, SqlDbType）。
        /// </summary>
        /// <param name="pms">要複製的 SqlParameter 列舉（可為 null）。</param>
        /// <returns>複製後的 SqlParameter 陣列；若輸入為 null 則回傳 null。</returns>
        private static SqlParameter[] CloneParameters(IEnumerable<SqlParameter> pms)
        {
            if (pms == null) return null;
            return pms.Select(p =>
            {
                var np = new SqlParameter
                {
                    ParameterName = p.ParameterName,
                    Value = p.Value ?? DBNull.Value,
                    DbType = p.DbType,
                    Direction = p.Direction,
                    Size = p.Size,
                    Precision = p.Precision,
                    Scale = p.Scale,
                    IsNullable = p.IsNullable
                };
                // Copy SqlDbType if set (default is 0 which is fine)
                try
                {
                    np.SqlDbType = p.SqlDbType;
                }
                catch { }
                return np;
            }).ToArray();
        }

        /// <summary>
        /// 將 SqlParameter 陣列轉為 Dapper 的 DynamicParameters。
        /// 會去掉參數名稱前置的 '@' 並保留 DbType、Direction、Size 等資訊。
        /// </summary>
        /// <param name="pms">要轉換的 SqlParameter 列舉（可為 null）。</param>
        /// <returns>結果 DynamicParameters（若輸入為 null 回傳空的 DynamicParameters）。</returns>
        private static DynamicParameters ToDynamicParameters(IEnumerable<SqlParameter> pms)
        {
            var dp = new DynamicParameters();
            if (pms == null) return dp;
            foreach (var p in pms)
            {
                var name = p.ParameterName;
                if (name.StartsWith("@")) name = name.Substring(1);
                dp.Add(name, p.Value == null ? DBNull.Value : p.Value, dbType: p.DbType, direction: p.Direction, size: p.Size);
            }
            return dp;
        }

        /// <summary>
        /// 將 Dapper QueryAsync 的結果 (IEnumerable&lt;dynamic&gt;) 轉為 DataTable。
        /// 支援字典型態的列或具名屬性的 POCO。
        /// </summary>
        /// <param name="rows">Dapper 回傳的列集合（每列為 dynamic）。</param>
        /// <returns>對應的 DataTable；若 rows 為 null 回傳空的 DataTable。</returns>
        private static DataTable DataTableFromDapper(IEnumerable<dynamic> rows)
        {
            var dt = new DataTable();
            if (rows == null) return dt;
            bool columnsCreated = false;
            foreach (var row in rows)
            {
                var dict = row as IDictionary<string, object>;
                if (!columnsCreated)
                {
                    if (dict != null)
                    {
                        foreach (var k in dict.Keys)
                        {
                            dt.Columns.Add(k);
                        }
                    }
                    else
                    {
                        // fallback: use properties via reflection
                        var props = ((object)row).GetType().GetProperties();
                        foreach (var p in props) dt.Columns.Add(p.Name);
                    }
                    columnsCreated = true;
                }

                var dr = dt.NewRow();
                if (dict != null)
                {
                    foreach (var kv in dict)
                    {
                        dr[kv.Key] = kv.Value ?? DBNull.Value;
                    }
                }
                else
                {
                    var props = ((object)row).GetType().GetProperties();
                    foreach (var p in props)
                    {
                        dr[p.Name] = p.GetValue(row) ?? DBNull.Value;
                    }
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        /// <summary>
        /// 執行一個會回傳單一整數值的查詢（例如 SELECT COUNT(*) ...），避免填充 DataTable 的開銷。
        /// 使用 ExecuteScalar 並回傳整數（若為 null 或無法轉換則回傳 0）。
        /// </summary>
        /// <param name="sql">要執行的 SQL 或 Stored Procedure 名稱。</param>
        /// <param name="cmdType">命令型別。</param>
        /// <param name="pms">可選的 SqlParameter 陣列。</param>
        /// <returns>查詢結果的整數值；若為 null 或轉換失敗則回傳 0。</returns>
        /// <exception cref="Exception">當執行查詢發生例外時會向上拋出。</exception>
        public static int ExecuteScalarInt(string sql, CommandType cmdType, params SqlParameter[] pms)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                using (var conn = new SqlConnection(connStr))
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = cmdType;
                    cmd.CommandTimeout = CommandTimeoutSeconds;
                    if (pms != null)
                    {
                        cmd.Parameters.AddRange(CloneParameters(pms));
                    }
                    conn.Open();
                    var obj = cmd.ExecuteScalar();
                    sw.Stop();
                    Logger.LogQuery(sql, pms, sw.Elapsed, "ExecuteScalarInt");
                    if (obj == null || obj == DBNull.Value) return 0;
                    try { return Convert.ToInt32(obj); } catch { return 0; }
                }
            }
            catch (Exception ex)
            {
                sw.Stop();
                Logger.LogError(ex, "ExecuteScalarInt");
                throw;
            }
        }

        /// <summary>
        /// 非同步版：執行回傳單一整數值的查詢。
        /// 使用 Dapper 的 ExecuteScalarAsync 取得純量值並轉為 int。
        /// </summary>
        /// <param name="sql">要執行的 SQL 或 Stored Procedure 名稱。</param>
        /// <param name="cmdType">命令型別。</param>
        /// <param name="pms">可選的 SqlParameter 陣列。</param>
        /// <returns>非同步任務，結果為查詢的整數值；若為 null 或轉換失敗則回傳 0。</returns>
        /// <exception cref="Exception">當執行查詢發生例外時會向上拋出。</exception>
        public static async Task<int> ExecuteScalarIntAsync(string sql, CommandType cmdType, params SqlParameter[] pms)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                using (var conn = new SqlConnection(connStr))
                {
                    var dyn = ToDynamicParameters(pms);
                    // Log parameters for diagnostic when needed
                    try
                    {
                        if (pms != null && pms.Length > 0)
                        {
                            // Use existing LogQuery to record parameters for diagnostics
                            var paramPairs = pms.Select(pp => new { Name = pp.ParameterName, Value = pp.Value });
                            var paramStr = string.Join(", ", paramPairs.Select(x => x.Name + "=" + (x.Value ?? "NULL")));
                            Logger.LogQuery(sql + " -- PARAMS: " + paramStr, pms, TimeSpan.Zero, "ParamDump");
                        }
                    }
                    catch { }

                    await conn.OpenAsync();
                    // 使用 Dapper 的 ExecuteScalarAsync 直接取得純量值，對 COUNT(*) 會更直接且可靠
                    var result = await conn.ExecuteScalarAsync(sql, dyn, commandType: cmdType, commandTimeout: CommandTimeoutSeconds);
                    sw.Stop();
                    Logger.LogQuery(sql, pms, sw.Elapsed, "ExecuteScalarIntAsync");
                    if (result == null || result == DBNull.Value) return 0;
                    try { return Convert.ToInt32(result); } catch { return 0; }
                }
            }
            catch (Exception ex)
            {
                sw.Stop();
                Logger.LogError(ex, "ExecuteScalarIntAsync");
                throw;
            }
        }

        /// <summary>
        /// 非同步版：回傳 SqlDataReader（開啟連線並由呼叫端負責讀取），reader 在關閉時會同時關閉連線。
        /// 使用 SequentialAccess 以降低大欄位讀取的記憶體壓力。
        /// </summary>
        /// <param name="sql">要執行的 SQL 或 Stored Procedure 名稱。</param>
        /// <param name="cmdType">命令型別。</param>
        /// <param name="pms">以 List&lt;SqlParameter&gt; 提供的參數清單（可為 null）。</param>
        /// <returns>非同步任務，結果為已開啟的 SqlDataReader，讀取完成後請呼叫 reader.Close()。</returns>
        /// <exception cref="Exception">若建立連線或執行命令失敗，會拋出例外並嘗試清理連線。</exception>
        public static async Task<SqlDataReader> ExecuteReaderPmsListAsync(string sql, CommandType cmdType, List<SqlParameter> pms)
        {
            SqlConnection con = new SqlConnection(connStr);
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.CommandType = cmdType;
            cmd.CommandTimeout = CommandTimeoutSeconds;
            if (pms != null)
            {
                cmd.Parameters.AddRange(CloneParameters(pms));
            }
            try
            {
                await con.OpenAsync();
                var sw = System.Diagnostics.Stopwatch.StartNew();
                // Use SequentialAccess for streaming large columns to reduce memory footprint
                var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection | CommandBehavior.SequentialAccess);
                sw.Stop();
                Logger.LogQuery(sql, pms, sw.Elapsed, "ExecuteReaderPmsListAsync");
                return reader;
            }
            catch
            {
                try { con.Close(); } catch { }
                try { con.Dispose(); } catch { }
                Logger.LogError(new Exception("ExecuteReaderPmsListAsync failed"));
                throw;
            }
        }
        #endregion

    }
}
