using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace BartectorQuery
{
    /// <summary>
    /// 簡易檔案日誌器，用於記錄 SQL 與執行時間，以協助找出慢查詢。
    /// </summary>
    /// <remarks>
    /// 非同步安全：以內部鎖物件序列化檔案寫入。所有 I/O 錯誤皆在內部捕捉以避免影響呼叫端。
    /// 日誌會寫入應用程式執行目錄下的 logs 子目錄，檔名格式為 bartector-YYYYMMDD.log。
    /// </remarks>
    internal static class Logger
    {
        /// <summary>
        /// 用於序列化檔案寫入的鎖物件。
        /// </summary>
        private static readonly object _lock = new object();

        /// <summary>
        /// 日誌目錄路徑，預設為執行目錄下的 "logs" 子目錄。
        /// </summary>
        private static readonly string LogDirectory = Path.Combine(Environment.CurrentDirectory, "logs");

        /// <summary>
        /// 取得當日的日誌檔案完整路徑。
        /// </summary>
        /// <returns>
        /// 回傳組合後的檔案路徑，格式為 {LogDirectory}\bartector-yyyyMMdd.log。
        /// </returns>
        /// <remarks>
        /// 方法會嘗試建立 <see cref="LogDirectory"/>（Directory.CreateDirectory），
        /// 但任何建立或 I/O 的例外均會被捕捉並忽略，方法仍會回傳預期格式的路徑字串。
        /// </remarks>
        private static string GetLogFilePath()
        {
            try
            {
                Directory.CreateDirectory(LogDirectory);
            }
            catch { }
            return Path.Combine(LogDirectory, $"bartector-{DateTime.Now:yyyyMMdd}.log");
        }

        /// <summary>
        /// 將一則 SQL 查詢與其參數、執行耗時記錄到日誌檔案。
        /// </summary>
        /// <param name="sql">要記錄的 SQL 指令文字。</param>
        /// <param name="parameters">與 SQL 相關的參數集合，若為 null 則不列出參數。</param>
        /// <param name="elapsed">SQL 執行所耗費的時間。</param>
        /// <param name="note">可選的備註文字，會一併寫入日誌，用以標記上下文。</param>
        /// <remarks>
        /// 寫入操作以 <see cref="_lock"/> 互斥，確保多執行緒情境下的序列化寫入。
        /// 所有內部例外會被捕捉並忽略，以避免影響呼叫端邏輯。
        /// 參數的每一項在寫入時亦會以 try/catch 包裹，避免單一參數序列化錯誤導致整體失敗。
        /// </remarks>
        public static void LogQuery(string sql, IEnumerable<SqlParameter> parameters, TimeSpan elapsed, string note = null)
        {
            try
            {
                var path = GetLogFilePath();
                lock (_lock)
                {
                    using (var sw = new StreamWriter(path, true, Encoding.UTF8))
                    {
                        sw.WriteLine($"{DateTime.Now:O} | QUERY | {elapsed.TotalMilliseconds} ms | {note}");
                        sw.WriteLine(sql);
                        if (parameters != null)
                        {
                            foreach (var p in parameters)
                            {
                                try
                                {
                                    sw.WriteLine($"  {p.ParameterName} = {p.Value} ({p.SqlDbType})");
                                }
                                catch { }
                            }
                        }
                        sw.WriteLine();
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// 將例外資訊記錄到日誌檔案，並可提供額外的上下文文字。
        /// </summary>
        /// <param name="ex">要記錄的例外物件。</param>
        /// <param name="context">可選的上下文說明文字，會顯示於日誌中以協助診斷。</param>
        /// <remarks>
        /// 與 <see cref="LogQuery(string, IEnumerable{SqlParameter}, TimeSpan, string)"/> 相同，
        /// 寫入也會以 <see cref="_lock"/> 序列化，並在內部捕捉所有例外以避免影響呼叫端。
        /// </remarks>
        public static void LogError(Exception ex, string context = null)
        {
            try
            {
                var path = GetLogFilePath();
                lock (_lock)
                {
                    using (var sw = new StreamWriter(path, true, Encoding.UTF8))
                    {
                        sw.WriteLine($"{DateTime.Now:O} | ERROR | {context}");
                        sw.WriteLine(ex.ToString());
                        sw.WriteLine();
                    }
                }
            }
            catch { }
        }
    }
}
