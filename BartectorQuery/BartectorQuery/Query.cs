using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BartectorQuery
{
    public class Query
    {
        #region 參數
        public string Date1 { get; set; } //日期start
        public string Date2 { get; set; } //日期end
        public string EngSr { get; set; } //工程編號
        public string Wono { get; set; } //工單號碼
        public bool Side_A { get; set; }//A面
        public bool Side_B { get; set; } //B面        
        public string PN { get; set; } //零件料號
        public string LotCode { get; set; }//零件LOT
        public string DateCode { get; set; }//零件DC
        public string Supplier { get; set; }//零件供應商
        public string PCBID { get; set; }//PCB序號
        public string ReelID { get; set; }//Reel ID
        public string Station { get; set; }//SMT站別
        public string Program { get; set; }//製件程式
        public string Slot { get; set; }//機台料站
        public string FeederID { get; set; }//料槍編號

        public List<string> columns = new List<string>();

        private string baseSql;
        private string basemerge;
        #endregion

        DataSet ds = new DataSet();        
        public void merge()
        {
            if (columns.Count ==0 )
            {
                columns = new List<string>() { "TRACE_LOG.TIMESTAMP", "TRACE_LOG.KITID", "PN.PN", "REEL.LOT", "REEL.DATECODE", "REEL.SUPPLIER", "TRACE_LOG.PCBID", "REEL.RID", "TRACE_LOG.TRACE_STATION", "TRACE_LOG.FCODE", "TRACE_LOG.LOC", "TRACE_LOG.MPROG" };
            }
            baseSql =
            @" from TRACE_LOG inner join REEL on TRACE_LOG.RID=REEL.RID inner join PN on REEL.SPN= PN.SPN where PN.PN is not null ";

            basemerge = $"select {string.Join(", ", columns)} {baseSql}";
        }
                        
        public Tuple<string, List<SqlParameter>> QDate()
        {
            merge();
            var parameters = new List<SqlParameter>();
            string str = string.Empty;

            #region 日期條件
            if (Date1.IndexOf("eg")>=0 || Date2.IndexOf("eg")>=0)
            {
                Date1 = "";
                Date2 = "";
            }
            else
            {
                if (!string.IsNullOrEmpty(Date1) && string.IsNullOrEmpty(Date2))
                {
                    str = " and cast(TRACE_LOG.TIMESTAMP as date) = @startDate ";
                    parameters.Add(new SqlParameter("@startDate", Date1));
                }
                else if (string.IsNullOrEmpty(Date1) && !string.IsNullOrEmpty(Date2))
                {
                    str += " and cast(TRACE_LOG.TIMESTAMP as date) <= @endDate ";
                    parameters.Add(new SqlParameter("@endDate", Date2));
                }
                else if (!string.IsNullOrEmpty(Date1) && !string.IsNullOrEmpty(Date2))
                {
                    str += " and cast(TRACE_LOG.TIMESTAMP as date) >= @startDate AND cast(TRACE_LOG.TIMESTAMP as date) <= @endDate ";
                    parameters.Add(new SqlParameter("@startDate", Date1));
                    parameters.Add(new SqlParameter("@endDate", Date2));
                }
            }
            
            #endregion

            #region 工程編號
            if (!string.IsNullOrEmpty(EngSr))
            {
                str += " AND CHARINDEX(@engsr, TRACE_LOG.MPROG) > 0 ";
                parameters.Add(new SqlParameter("@engsr", EngSr));
            }
            #endregion

            #region 正反面
            if (Side_A)
            {
                str += " AND CHARINDEX('_A_', TRACE_LOG.MPROG) > 0 ";                
            }
            else if (Side_B)
            {
                str += " AND CHARINDEX('_B_', TRACE_LOG.MPROG) > 0 ";                
            }
            else
            {
                str += "";
            }

            #endregion

            #region 工單號碼
            if (!string.IsNullOrEmpty(Wono))
            {
                
                string[] wip = Wono.Split(',');
                str += " and ( ";
                for (int i = 0; i < wip.Length; i++)
                {
                    str += " TRACE_LOG.KITID= @wip" + i;
                    str += " or ";
                    if (i+1==wip.Length)
                    {
                        str = str.Substring(0, str.Length - 3) + " )"; 
                    }
                    parameters.Add(new SqlParameter("@wip" + i, wip[i]));
                }                                                
            }
            #endregion

            #region 零件料號PN
            if (!string.IsNullOrEmpty(PN))
            {
                str += " AND  PN.PN = @pn ";
                parameters.Add(new SqlParameter("@pn", PN));
            }
            #endregion

            #region 零件LOT
            if (!string.IsNullOrEmpty(LotCode))
            {
                str += " AND  REEL.LOT = @lotcode ";
                parameters.Add(new SqlParameter("@lotcode", LotCode));
            }
            #endregion

            #region 零件DC
            if (!string.IsNullOrEmpty(DateCode))
            {
                str += " AND  REEL.DATECODE = @dc ";
                parameters.Add(new SqlParameter("@dc", DateCode));
            }
            #endregion

            #region 供應商
            if (!string.IsNullOrEmpty(Supplier))
            {
                str += " AND REEL.SUPPLIER = @supplier ";
                parameters.Add(new SqlParameter("@supplier", Supplier));
            }
            #endregion

            #region PCBA序號
            if (!string.IsNullOrEmpty(PCBID))
            {
                str += " AND TRACE_LOG.PCBID = @pcba ";
                parameters.Add(new SqlParameter("@pcba", PCBID));
            }
            #endregion

            #region ReelID
            if (!string.IsNullOrEmpty(ReelID))
            {
                str += " AND REEL.RID like @rid +'%'";
                parameters.Add(new SqlParameter("@rid", ReelID));
            }
            #endregion

            #region SMT站別
            if (!string.IsNullOrEmpty(Station))
            {
                str += " AND TRACE_LOG.TRACE_STATION = @station ";
                parameters.Add(new SqlParameter("@station", Station));
            }
            #endregion

            #region 料槍
            if (!string.IsNullOrEmpty(FeederID))
            {
                str += " AND TRACE_LOG.FCODE = @feeder ";
                parameters.Add(new SqlParameter("@feeder", FeederID));
            }
            #endregion

            #region 製件程式
            if (!string.IsNullOrEmpty(Program))
            {
                str += " AND TRACE_LOG.MPROG = @program ";
                parameters.Add(new SqlParameter("@program", Program));
            }
            #endregion

            #region 機台料站
            if (!string.IsNullOrEmpty(Slot))
            {
                str += " AND TRACE_LOG.LOC = @slot ";
                parameters.Add(new SqlParameter("@slot", Slot));
            }
            #endregion


            str += " order by cast(TRACE_LOG.TIMESTAMP as date) ";
            return new Tuple<string, List<SqlParameter>>(basemerge + str, parameters); 
        }
    }
}
