using System;
using System.Drawing;
using System.Windows.Forms;

namespace BartectorQuery
{
    public partial class AutoSizeWindows : Form
    {
        #region 視窗ReSize
        int X = new int();  //窗口寬度
        int Y = new int(); //窗口高度
        float fgX = new float(); //寬度縮放比例
        float fgY = new float(); //高度縮放比例
        bool isLoaded;  // 是否已設定各控制的尺寸資料到Tag屬性
        #endregion
        public AutoSizeWindows()
        {
            InitializeComponent();
        }

        private void AutoSizeWindows_Load(object sender, EventArgs e)
        {
            #region 獲取窗體info
            X = this.Width;//獲取窗體的寬度
            Y = this.Height;//獲取窗體的高度
            isLoaded = true;// 已設定各控制項的尺寸到Tag屬性中
            SetTag(this);//調用方法
            #endregion
        }

        private void AutoSizeWindows_Resize(object sender, EventArgs e)
        {
            if (X == 0 || Y == 0) return;
            fgX = (float)this.Width / (float)X;
            fgY = (float)this.Height / (float)Y;

            SetControls(fgX, fgY, this);
        }
        private void SetControls(float newx, float newy, Control cons)
        {
            if (isLoaded)
            {
                //遍歷窗體中的控制項，重新設置控制項的值
                foreach (Control con in cons.Controls)
                {
                    string[] mytag = con.Tag.ToString().Split(new char[] { ':' });//獲取控制項的Tag屬性值，並分割後存儲字元串數組
                    float a = System.Convert.ToSingle(mytag[0]) * newx;//根據窗體縮放比例確定控制項的值，寬度
                    con.Width = (int)a;//寬度
                    a = System.Convert.ToSingle(mytag[1]) * newy;//高度
                    con.Height = (int)(a);
                    a = System.Convert.ToSingle(mytag[2]) * newx;//左邊距離
                    con.Left = (int)(a);
                    a = System.Convert.ToSingle(mytag[3]) * newy;//上邊緣距離
                    con.Top = (int)(a);
                    Single currentSize = System.Convert.ToSingle(mytag[4]) * newy;//字體大小
                    con.Font = new Font(con.Font.Name, currentSize, con.Font.Style, con.Font.Unit);
                    if (con.Controls.Count > 0)
                    {
                        SetControls(newx, newy, con);
                    }
                }
            }
        }
        /// <summary>
        /// 將控制項的寬，高，左邊距，頂邊距和字體大小暫存到tag屬性中
        /// </summary>
        /// <param name="cons">遞歸控制項中的控制項</param>
        private void SetTag(Control cons)
        {
            foreach (Control con in cons.Controls)
            {
                con.Tag = con.Width + ":" + con.Height + ":" + con.Left + ":" + con.Top + ":" + con.Font.Size;
                if (con.Controls.Count > 0)
                    SetTag(con);
            }
        }
    }
}
