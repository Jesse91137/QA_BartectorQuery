
namespace BartectorQuery
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnPreviousPage = new System.Windows.Forms.Button();
            this.btnNextPage = new System.Windows.Forms.Button();
            this.btn_Serach = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rdb_N = new System.Windows.Forms.RadioButton();
            this.txt_supplier = new System.Windows.Forms.TextBox();
            this.btn_export = new System.Windows.Forms.Button();
            this.btn_clear = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.txt_pcbid = new System.Windows.Forms.TextBox();
            this.txt_engsr = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txt_date2 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.c_engsr = new System.Windows.Forms.CheckBox();
            this.txt_position = new System.Windows.Forms.TextBox();
            this.txt_date1 = new System.Windows.Forms.TextBox();
            this.txt_reelID = new System.Windows.Forms.TextBox();
            this.checkBox9 = new System.Windows.Forms.CheckBox();
            this.label14 = new System.Windows.Forms.Label();
            this.TRACE_LOG_TIMESTAMP = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.TRACE_LOG_PCBID = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txt_datecode = new System.Windows.Forms.TextBox();
            this.REEL_RID = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.TRACE_LOG_TRACE_STATION = new System.Windows.Forms.CheckBox();
            this.txt_station = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.TRACE_LOG_FCODE = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.TRACE_LOG_MPROG = new System.Windows.Forms.CheckBox();
            this.label15 = new System.Windows.Forms.Label();
            this.txt_slot = new System.Windows.Forms.TextBox();
            this.rdb_B = new System.Windows.Forms.RadioButton();
            this.TRACE_LOG_LOC = new System.Windows.Forms.CheckBox();
            this.txt_PN = new System.Windows.Forms.TextBox();
            this.txt_lotcode = new System.Windows.Forms.TextBox();
            this.REEL_DATECODE = new System.Windows.Forms.CheckBox();
            this.REEL_SUPPLIER = new System.Windows.Forms.CheckBox();
            this.txt_wono = new System.Windows.Forms.TextBox();
            this.rdb_A = new System.Windows.Forms.RadioButton();
            this.REEL_LOT = new System.Windows.Forms.CheckBox();
            this.txt_program = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.TRACE_LOG_KITID = new System.Windows.Forms.CheckBox();
            this.PN_PN = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txt_feederid = new System.Windows.Forms.TextBox();
            this.c_side = new System.Windows.Forms.CheckBox();
            this.label16 = new System.Windows.Forms.Label();
            this.lb_totalPage = new System.Windows.Forms.Label();
            this.txtPageNumber = new System.Windows.Forms.TextBox();
            this.progressBar2 = new System.Windows.Forms.ProgressBar();
            this.txt_kyworld = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(310, 2);
            this.dataGridView1.Name = "dataGridView1";
            dataGridViewCellStyle2.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(633, 384);
            this.dataGridView1.TabIndex = 33;
            this.dataGridView1.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dataGridView1_DataBindingComplete);
            // 
            // btnPreviousPage
            // 
            this.btnPreviousPage.Enabled = false;
            this.btnPreviousPage.Location = new System.Drawing.Point(310, 394);
            this.btnPreviousPage.Name = "btnPreviousPage";
            this.btnPreviousPage.Size = new System.Drawing.Size(62, 23);
            this.btnPreviousPage.TabIndex = 34;
            this.btnPreviousPage.Text = "上一頁";
            this.btnPreviousPage.UseVisualStyleBackColor = true;
            this.btnPreviousPage.Click += new System.EventHandler(this.btnPreviousPage_Click);
            // 
            // btnNextPage
            // 
            this.btnNextPage.Enabled = false;
            this.btnNextPage.Location = new System.Drawing.Point(882, 394);
            this.btnNextPage.Name = "btnNextPage";
            this.btnNextPage.Size = new System.Drawing.Size(61, 23);
            this.btnNextPage.TabIndex = 35;
            this.btnNextPage.Text = "下一頁";
            this.btnNextPage.UseVisualStyleBackColor = true;
            this.btnNextPage.Click += new System.EventHandler(this.btnNextPage_Click);
            // 
            // btn_Serach
            // 
            this.btn_Serach.Location = new System.Drawing.Point(247, 393);
            this.btn_Serach.Name = "btn_Serach";
            this.btn_Serach.Size = new System.Drawing.Size(43, 23);
            this.btn_Serach.TabIndex = 38;
            this.btn_Serach.Text = "查詢";
            this.btn_Serach.UseVisualStyleBackColor = true;
            this.btn_Serach.Click += new System.EventHandler(this.btn_Serach_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rdb_N);
            this.panel1.Controls.Add(this.txt_supplier);
            this.panel1.Controls.Add(this.btn_export);
            this.panel1.Controls.Add(this.btn_clear);
            this.panel1.Controls.Add(this.btn_Serach);
            this.panel1.Controls.Add(this.progressBar1);
            this.panel1.Controls.Add(this.txt_pcbid);
            this.panel1.Controls.Add(this.txt_engsr);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.txt_date2);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.c_engsr);
            this.panel1.Controls.Add(this.txt_position);
            this.panel1.Controls.Add(this.txt_date1);
            this.panel1.Controls.Add(this.txt_reelID);
            this.panel1.Controls.Add(this.checkBox9);
            this.panel1.Controls.Add(this.label14);
            this.panel1.Controls.Add(this.TRACE_LOG_TIMESTAMP);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.TRACE_LOG_PCBID);
            this.panel1.Controls.Add(this.label13);
            this.panel1.Controls.Add(this.txt_datecode);
            this.panel1.Controls.Add(this.REEL_RID);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.TRACE_LOG_TRACE_STATION);
            this.panel1.Controls.Add(this.txt_station);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.TRACE_LOG_FCODE);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.TRACE_LOG_MPROG);
            this.panel1.Controls.Add(this.label15);
            this.panel1.Controls.Add(this.txt_slot);
            this.panel1.Controls.Add(this.rdb_B);
            this.panel1.Controls.Add(this.TRACE_LOG_LOC);
            this.panel1.Controls.Add(this.txt_PN);
            this.panel1.Controls.Add(this.txt_lotcode);
            this.panel1.Controls.Add(this.REEL_DATECODE);
            this.panel1.Controls.Add(this.REEL_SUPPLIER);
            this.panel1.Controls.Add(this.txt_wono);
            this.panel1.Controls.Add(this.rdb_A);
            this.panel1.Controls.Add(this.REEL_LOT);
            this.panel1.Controls.Add(this.txt_program);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.TRACE_LOG_KITID);
            this.panel1.Controls.Add(this.PN_PN);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.txt_feederid);
            this.panel1.Controls.Add(this.c_side);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(304, 421);
            this.panel1.TabIndex = 46;
            // 
            // rdb_N
            // 
            this.rdb_N.AutoSize = true;
            this.rdb_N.Checked = true;
            this.rdb_N.Location = new System.Drawing.Point(84, 226);
            this.rdb_N.Name = "rdb_N";
            this.rdb_N.Size = new System.Drawing.Size(48, 16);
            this.rdb_N.TabIndex = 93;
            this.rdb_N.TabStop = true;
            this.rdb_N.Text = "None";
            this.rdb_N.UseVisualStyleBackColor = true;
            // 
            // txt_supplier
            // 
            this.txt_supplier.Location = new System.Drawing.Point(180, 20);
            this.txt_supplier.Name = "txt_supplier";
            this.txt_supplier.Size = new System.Drawing.Size(110, 22);
            this.txt_supplier.TabIndex = 55;
            // 
            // btn_export
            // 
            this.btn_export.Location = new System.Drawing.Point(181, 394);
            this.btn_export.Name = "btn_export";
            this.btn_export.Size = new System.Drawing.Size(43, 23);
            this.btn_export.TabIndex = 38;
            this.btn_export.Text = "匯出";
            this.btn_export.UseVisualStyleBackColor = true;
            this.btn_export.Click += new System.EventHandler(this.btn_export_Click);
            // 
            // btn_clear
            // 
            this.btn_clear.Location = new System.Drawing.Point(113, 394);
            this.btn_clear.Name = "btn_clear";
            this.btn_clear.Size = new System.Drawing.Size(43, 23);
            this.btn_clear.TabIndex = 38;
            this.btn_clear.Text = "清除";
            this.btn_clear.UseVisualStyleBackColor = true;
            this.btn_clear.Click += new System.EventHandler(this.btn_clear_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(181, 384);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(44, 10);
            this.progressBar1.TabIndex = 47;
            this.progressBar1.Visible = false;
            // 
            // txt_pcbid
            // 
            this.txt_pcbid.Location = new System.Drawing.Point(180, 114);
            this.txt_pcbid.Name = "txt_pcbid";
            this.txt_pcbid.Size = new System.Drawing.Size(110, 22);
            this.txt_pcbid.TabIndex = 57;
            // 
            // txt_engsr
            // 
            this.txt_engsr.Location = new System.Drawing.Point(32, 109);
            this.txt_engsr.Name = "txt_engsr";
            this.txt_engsr.Size = new System.Drawing.Size(114, 22);
            this.txt_engsr.TabIndex = 48;
            
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label7.Location = new System.Drawing.Point(30, 341);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(49, 16);
            this.label7.TabIndex = 69;
            this.label7.Text = "零件DC";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(33, 94);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 16);
            this.label2.TabIndex = 63;
            this.label2.Text = "工程編號";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label10.Location = new System.Drawing.Point(178, 99);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(62, 16);
            this.label10.TabIndex = 72;
            this.label10.Text = "PCBA序號";
            // 
            // txt_date2
            // 
            this.txt_date2.Location = new System.Drawing.Point(32, 62);
            this.txt_date2.Name = "txt_date2";
            this.txt_date2.Size = new System.Drawing.Size(106, 22);
            this.txt_date2.TabIndex = 47;
            this.txt_date2.Text = "eg. 2 0 2 4 0 4 0 1";
            this.txt_date2.Enter += new System.EventHandler(this.txt_date2_Enter);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label8.Location = new System.Drawing.Point(178, 5);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(68, 16);
            this.label8.TabIndex = 70;
            this.label8.Text = "零件供應商";
            // 
            // c_engsr
            // 
            this.c_engsr.AutoSize = true;
            this.c_engsr.Location = new System.Drawing.Point(15, 112);
            this.c_engsr.Name = "c_engsr";
            this.c_engsr.Size = new System.Drawing.Size(15, 14);
            this.c_engsr.TabIndex = 81;
            this.c_engsr.UseVisualStyleBackColor = true;
            // 
            // txt_position
            // 
            this.txt_position.Enabled = false;
            this.txt_position.Location = new System.Drawing.Point(180, 67);
            this.txt_position.Name = "txt_position";
            this.txt_position.Size = new System.Drawing.Size(110, 22);
            this.txt_position.TabIndex = 56;
            // 
            // txt_date1
            // 
            this.txt_date1.Font = new System.Drawing.Font("新細明體", 9F);
            this.txt_date1.Location = new System.Drawing.Point(32, 34);
            this.txt_date1.Name = "txt_date1";
            this.txt_date1.Size = new System.Drawing.Size(106, 22);
            this.txt_date1.TabIndex = 46;
            this.txt_date1.Text = "eg. 2 0 2 4 0 1 0 1";
            this.txt_date1.Enter += new System.EventHandler(this.txt_date1_Enter);
            
            // 
            // txt_reelID
            // 
            this.txt_reelID.Location = new System.Drawing.Point(180, 159);
            this.txt_reelID.Name = "txt_reelID";
            this.txt_reelID.Size = new System.Drawing.Size(110, 22);
            this.txt_reelID.TabIndex = 58;
            // 
            // checkBox9
            // 
            this.checkBox9.AutoSize = true;
            this.checkBox9.Enabled = false;
            this.checkBox9.Location = new System.Drawing.Point(164, 70);
            this.checkBox9.Name = "checkBox9";
            this.checkBox9.Size = new System.Drawing.Size(15, 14);
            this.checkBox9.TabIndex = 80;
            this.checkBox9.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label14.Location = new System.Drawing.Point(178, 296);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(56, 16);
            this.label14.TabIndex = 76;
            this.label14.Text = "置件程式";
            // 
            // TRACE_LOG_TIMESTAMP
            // 
            this.TRACE_LOG_TIMESTAMP.AutoSize = true;
            this.TRACE_LOG_TIMESTAMP.Location = new System.Drawing.Point(15, 52);
            this.TRACE_LOG_TIMESTAMP.Name = "TRACE_LOG_TIMESTAMP";
            this.TRACE_LOG_TIMESTAMP.Size = new System.Drawing.Size(15, 14);
            this.TRACE_LOG_TIMESTAMP.TabIndex = 79;
            this.TRACE_LOG_TIMESTAMP.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(30, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 16);
            this.label1.TabIndex = 61;
            this.label1.Text = "生產日期";
            // 
            // TRACE_LOG_PCBID
            // 
            this.TRACE_LOG_PCBID.AutoSize = true;
            this.TRACE_LOG_PCBID.Location = new System.Drawing.Point(164, 117);
            this.TRACE_LOG_PCBID.Name = "TRACE_LOG_PCBID";
            this.TRACE_LOG_PCBID.Size = new System.Drawing.Size(15, 14);
            this.TRACE_LOG_PCBID.TabIndex = 82;
            this.TRACE_LOG_PCBID.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label13.Location = new System.Drawing.Point(178, 247);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(67, 16);
            this.label13.TabIndex = 75;
            this.label13.Text = "FEEDER ID";
            // 
            // txt_datecode
            // 
            this.txt_datecode.Location = new System.Drawing.Point(32, 358);
            this.txt_datecode.Name = "txt_datecode";
            this.txt_datecode.Size = new System.Drawing.Size(114, 22);
            this.txt_datecode.TabIndex = 54;
            // 
            // REEL_RID
            // 
            this.REEL_RID.AutoSize = true;
            this.REEL_RID.Location = new System.Drawing.Point(164, 161);
            this.REEL_RID.Name = "REEL_RID";
            this.REEL_RID.Size = new System.Drawing.Size(15, 14);
            this.REEL_RID.TabIndex = 83;
            this.REEL_RID.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label5.Location = new System.Drawing.Point(30, 247);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 16);
            this.label5.TabIndex = 67;
            this.label5.Text = "零件料號";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label4.Location = new System.Drawing.Point(30, 195);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 16);
            this.label4.TabIndex = 66;
            this.label4.Text = "A/B 面";
            // 
            // TRACE_LOG_TRACE_STATION
            // 
            this.TRACE_LOG_TRACE_STATION.AutoSize = true;
            this.TRACE_LOG_TRACE_STATION.Location = new System.Drawing.Point(164, 216);
            this.TRACE_LOG_TRACE_STATION.Name = "TRACE_LOG_TRACE_STATION";
            this.TRACE_LOG_TRACE_STATION.Size = new System.Drawing.Size(15, 14);
            this.TRACE_LOG_TRACE_STATION.TabIndex = 85;
            this.TRACE_LOG_TRACE_STATION.UseVisualStyleBackColor = true;
            // 
            // txt_station
            // 
            this.txt_station.Location = new System.Drawing.Point(180, 209);
            this.txt_station.Name = "txt_station";
            this.txt_station.Size = new System.Drawing.Size(110, 22);
            this.txt_station.TabIndex = 59;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label3.Location = new System.Drawing.Point(30, 144);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 16);
            this.label3.TabIndex = 65;
            this.label3.Text = "工單號碼";
            // 
            // TRACE_LOG_FCODE
            // 
            this.TRACE_LOG_FCODE.AutoSize = true;
            this.TRACE_LOG_FCODE.Location = new System.Drawing.Point(164, 266);
            this.TRACE_LOG_FCODE.Name = "TRACE_LOG_FCODE";
            this.TRACE_LOG_FCODE.Size = new System.Drawing.Size(15, 14);
            this.TRACE_LOG_FCODE.TabIndex = 87;
            this.TRACE_LOG_FCODE.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label12.Location = new System.Drawing.Point(178, 194);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(61, 16);
            this.label12.TabIndex = 74;
            this.label12.Text = "SMT 站別";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label9.Location = new System.Drawing.Point(178, 52);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 16);
            this.label9.TabIndex = 71;
            this.label9.Text = "零件位置";
            // 
            // TRACE_LOG_MPROG
            // 
            this.TRACE_LOG_MPROG.AutoSize = true;
            this.TRACE_LOG_MPROG.Location = new System.Drawing.Point(164, 315);
            this.TRACE_LOG_MPROG.Name = "TRACE_LOG_MPROG";
            this.TRACE_LOG_MPROG.Size = new System.Drawing.Size(15, 14);
            this.TRACE_LOG_MPROG.TabIndex = 89;
            this.TRACE_LOG_MPROG.UseVisualStyleBackColor = true;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label15.Location = new System.Drawing.Point(178, 341);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(56, 16);
            this.label15.TabIndex = 77;
            this.label15.Text = "機台料站";
            // 
            // txt_slot
            // 
            this.txt_slot.Location = new System.Drawing.Point(180, 358);
            this.txt_slot.Name = "txt_slot";
            this.txt_slot.Size = new System.Drawing.Size(110, 22);
            this.txt_slot.TabIndex = 64;
            // 
            // rdb_B
            // 
            this.rdb_B.AutoSize = true;
            this.rdb_B.Location = new System.Drawing.Point(84, 206);
            this.rdb_B.Name = "rdb_B";
            this.rdb_B.Size = new System.Drawing.Size(54, 16);
            this.rdb_B.TabIndex = 51;
            this.rdb_B.Text = "B Side";
            this.rdb_B.UseVisualStyleBackColor = true;
            // 
            // TRACE_LOG_LOC
            // 
            this.TRACE_LOG_LOC.AutoSize = true;
            this.TRACE_LOG_LOC.Location = new System.Drawing.Point(164, 364);
            this.TRACE_LOG_LOC.Name = "TRACE_LOG_LOC";
            this.TRACE_LOG_LOC.Size = new System.Drawing.Size(15, 14);
            this.TRACE_LOG_LOC.TabIndex = 92;
            this.TRACE_LOG_LOC.UseVisualStyleBackColor = true;
            // 
            // txt_PN
            // 
            this.txt_PN.Location = new System.Drawing.Point(32, 264);
            this.txt_PN.Name = "txt_PN";
            this.txt_PN.Size = new System.Drawing.Size(114, 22);
            this.txt_PN.TabIndex = 52;
            // 
            // txt_lotcode
            // 
            this.txt_lotcode.Location = new System.Drawing.Point(32, 313);
            this.txt_lotcode.Name = "txt_lotcode";
            this.txt_lotcode.Size = new System.Drawing.Size(114, 22);
            this.txt_lotcode.TabIndex = 53;
            // 
            // REEL_DATECODE
            // 
            this.REEL_DATECODE.AutoSize = true;
            this.REEL_DATECODE.Location = new System.Drawing.Point(15, 364);
            this.REEL_DATECODE.Name = "REEL_DATECODE";
            this.REEL_DATECODE.Size = new System.Drawing.Size(15, 14);
            this.REEL_DATECODE.TabIndex = 91;
            this.REEL_DATECODE.UseVisualStyleBackColor = true;
            // 
            // REEL_SUPPLIER
            // 
            this.REEL_SUPPLIER.AutoSize = true;
            this.REEL_SUPPLIER.Location = new System.Drawing.Point(164, 24);
            this.REEL_SUPPLIER.Name = "REEL_SUPPLIER";
            this.REEL_SUPPLIER.Size = new System.Drawing.Size(15, 14);
            this.REEL_SUPPLIER.TabIndex = 78;
            this.REEL_SUPPLIER.UseVisualStyleBackColor = true;
            // 
            // txt_wono
            // 
            this.txt_wono.Location = new System.Drawing.Point(32, 159);
            this.txt_wono.Name = "txt_wono";
            this.txt_wono.Size = new System.Drawing.Size(114, 22);
            this.txt_wono.TabIndex = 49;
            // 
            // rdb_A
            // 
            this.rdb_A.AutoSize = true;
            this.rdb_A.Location = new System.Drawing.Point(32, 215);
            this.rdb_A.Name = "rdb_A";
            this.rdb_A.Size = new System.Drawing.Size(54, 16);
            this.rdb_A.TabIndex = 50;
            this.rdb_A.Text = "A Side";
            this.rdb_A.UseVisualStyleBackColor = true;
            // 
            // REEL_LOT
            // 
            this.REEL_LOT.AutoSize = true;
            this.REEL_LOT.Location = new System.Drawing.Point(15, 315);
            this.REEL_LOT.Name = "REEL_LOT";
            this.REEL_LOT.Size = new System.Drawing.Size(15, 14);
            this.REEL_LOT.TabIndex = 90;
            this.REEL_LOT.UseVisualStyleBackColor = true;
            // 
            // txt_program
            // 
            this.txt_program.Location = new System.Drawing.Point(180, 313);
            this.txt_program.Name = "txt_program";
            this.txt_program.Size = new System.Drawing.Size(110, 22);
            this.txt_program.TabIndex = 62;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label6.Location = new System.Drawing.Point(30, 298);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 16);
            this.label6.TabIndex = 68;
            this.label6.Text = "零件LOT";
            // 
            // TRACE_LOG_KITID
            // 
            this.TRACE_LOG_KITID.AutoSize = true;
            this.TRACE_LOG_KITID.Location = new System.Drawing.Point(15, 161);
            this.TRACE_LOG_KITID.Name = "TRACE_LOG_KITID";
            this.TRACE_LOG_KITID.Size = new System.Drawing.Size(15, 14);
            this.TRACE_LOG_KITID.TabIndex = 84;
            this.TRACE_LOG_KITID.UseVisualStyleBackColor = true;
            // 
            // PN_PN
            // 
            this.PN_PN.AutoSize = true;
            this.PN_PN.Location = new System.Drawing.Point(15, 266);
            this.PN_PN.Name = "PN_PN";
            this.PN_PN.Size = new System.Drawing.Size(15, 14);
            this.PN_PN.TabIndex = 88;
            this.PN_PN.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label11.Location = new System.Drawing.Point(178, 144);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(48, 16);
            this.label11.TabIndex = 73;
            this.label11.Text = "Reel ID";
            // 
            // txt_feederid
            // 
            this.txt_feederid.Location = new System.Drawing.Point(180, 264);
            this.txt_feederid.Name = "txt_feederid";
            this.txt_feederid.Size = new System.Drawing.Size(110, 22);
            this.txt_feederid.TabIndex = 60;
            // 
            // c_side
            // 
            this.c_side.AutoSize = true;
            this.c_side.Location = new System.Drawing.Point(15, 216);
            this.c_side.Name = "c_side";
            this.c_side.Size = new System.Drawing.Size(15, 14);
            this.c_side.TabIndex = 86;
            this.c_side.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(619, 399);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(8, 12);
            this.label16.TabIndex = 48;
            this.label16.Text = "/";
            // 
            // lb_totalPage
            // 
            this.lb_totalPage.AutoSize = true;
            this.lb_totalPage.Location = new System.Drawing.Point(633, 399);
            this.lb_totalPage.Name = "lb_totalPage";
            this.lb_totalPage.Size = new System.Drawing.Size(11, 12);
            this.lb_totalPage.TabIndex = 49;
            this.lb_totalPage.Text = "0";
            // 
            // txtPageNumber
            // 
            this.txtPageNumber.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPageNumber.Location = new System.Drawing.Point(513, 399);
            this.txtPageNumber.Name = "txtPageNumber";
            this.txtPageNumber.ReadOnly = true;
            this.txtPageNumber.Size = new System.Drawing.Size(100, 15);
            this.txtPageNumber.TabIndex = 50;
            this.txtPageNumber.Text = "0";
            this.txtPageNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // progressBar2
            // 
            this.progressBar2.Location = new System.Drawing.Point(247, 383);
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.Size = new System.Drawing.Size(44, 10);
            this.progressBar2.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar2.TabIndex = 94;
            this.progressBar2.Visible = false;
            // 
            // txt_kyworld
            // 
            this.txt_kyworld.Location = new System.Drawing.Point(726, 396);
            this.txt_kyworld.Name = "txt_kyworld";
            this.txt_kyworld.Size = new System.Drawing.Size(150, 22);
            this.txt_kyworld.TabIndex = 94;
            this.txt_kyworld.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_kyworld_KeyPress);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(955, 422);
            this.Controls.Add(this.txt_kyworld);
            this.Controls.Add(this.progressBar2);
            this.Controls.Add(this.txtPageNumber);
            this.Controls.Add(this.lb_totalPage);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnNextPage);
            this.Controls.Add(this.btnPreviousPage);
            this.Controls.Add(this.dataGridView1);
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "Bartector資料查詢";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnPreviousPage;
        private System.Windows.Forms.Button btnNextPage;
        private System.Windows.Forms.Button btn_Serach;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txt_supplier;
        private System.Windows.Forms.TextBox txt_pcbid;
        private System.Windows.Forms.TextBox txt_engsr;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txt_date2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox c_engsr;
        private System.Windows.Forms.TextBox txt_position;
        private System.Windows.Forms.TextBox txt_date1;
        private System.Windows.Forms.TextBox txt_reelID;
        private System.Windows.Forms.CheckBox checkBox9;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.CheckBox TRACE_LOG_TIMESTAMP;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox TRACE_LOG_PCBID;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txt_datecode;
        private System.Windows.Forms.CheckBox REEL_RID;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox TRACE_LOG_TRACE_STATION;
        private System.Windows.Forms.TextBox txt_station;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox TRACE_LOG_FCODE;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox TRACE_LOG_MPROG;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txt_slot;
        private System.Windows.Forms.RadioButton rdb_B;
        private System.Windows.Forms.CheckBox TRACE_LOG_LOC;
        private System.Windows.Forms.TextBox txt_PN;
        private System.Windows.Forms.TextBox txt_lotcode;
        private System.Windows.Forms.CheckBox REEL_DATECODE;
        private System.Windows.Forms.CheckBox REEL_SUPPLIER;
        private System.Windows.Forms.TextBox txt_wono;
        private System.Windows.Forms.RadioButton rdb_A;
        private System.Windows.Forms.CheckBox REEL_LOT;
        private System.Windows.Forms.TextBox txt_program;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox TRACE_LOG_KITID;
        private System.Windows.Forms.CheckBox PN_PN;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txt_feederid;
        private System.Windows.Forms.CheckBox c_side;
        private System.Windows.Forms.Button btn_export;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label lb_totalPage;
        private System.Windows.Forms.TextBox txtPageNumber;
        private System.Windows.Forms.Button btn_clear;
        private System.Windows.Forms.RadioButton rdb_N;
        private System.Windows.Forms.ProgressBar progressBar2;
        private System.Windows.Forms.TextBox txt_kyworld;
    }
}

