namespace GSP_DatToExcel
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.Button_Dat2Xml = new System.Windows.Forms.Button();
            this.Button_Xml2Excel = new System.Windows.Forms.Button();
            this.Button_Xml2Dat = new System.Windows.Forms.Button();
            this.Button_Excel2Xml = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Button_Dat2Xml
            // 
            this.Button_Dat2Xml.Location = new System.Drawing.Point(27, 61);
            this.Button_Dat2Xml.Name = "Button_Dat2Xml";
            this.Button_Dat2Xml.Size = new System.Drawing.Size(90, 42);
            this.Button_Dat2Xml.TabIndex = 0;
            this.Button_Dat2Xml.Text = "Dat解密Xml";
            this.Button_Dat2Xml.UseVisualStyleBackColor = true;
            this.Button_Dat2Xml.Click += new System.EventHandler(this.Button_Dat2Xml_Click);
            // 
            // Button_Xml2Excel
            // 
            this.Button_Xml2Excel.Location = new System.Drawing.Point(143, 61);
            this.Button_Xml2Excel.Name = "Button_Xml2Excel";
            this.Button_Xml2Excel.Size = new System.Drawing.Size(90, 42);
            this.Button_Xml2Excel.TabIndex = 1;
            this.Button_Xml2Excel.Text = "Xml转Excel";
            this.Button_Xml2Excel.UseVisualStyleBackColor = true;
            this.Button_Xml2Excel.Click += new System.EventHandler(this.Button_Xml2Excel_Click);
            // 
            // Button_Xml2Dat
            // 
            this.Button_Xml2Dat.Location = new System.Drawing.Point(467, 61);
            this.Button_Xml2Dat.Name = "Button_Xml2Dat";
            this.Button_Xml2Dat.Size = new System.Drawing.Size(90, 42);
            this.Button_Xml2Dat.TabIndex = 2;
            this.Button_Xml2Dat.Text = "Xml加密Dat";
            this.Button_Xml2Dat.UseVisualStyleBackColor = true;
            this.Button_Xml2Dat.Click += new System.EventHandler(this.Button_Xml2Dat_Click);
            // 
            // Button_Excel2Xml
            // 
            this.Button_Excel2Xml.Location = new System.Drawing.Point(355, 61);
            this.Button_Excel2Xml.Name = "Button_Excel2Xml";
            this.Button_Excel2Xml.Size = new System.Drawing.Size(90, 42);
            this.Button_Excel2Xml.TabIndex = 3;
            this.Button_Excel2Xml.Text = "Excel转Xml";
            this.Button_Excel2Xml.UseVisualStyleBackColor = true;
            this.Button_Excel2Xml.Click += new System.EventHandler(this.Button_Excel2Xml_Click);
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(27, 121);
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(532, 209);
            this.txtLog.TabIndex = 4;
            this.txtLog.Text = "";
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(27, 6);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(186, 19);
            this.radioButton1.TabIndex = 5;
            this.radioButton1.Text = "老版KEY:res070821mir";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(27, 31);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(218, 19);
            this.radioButton2.TabIndex = 6;
            this.radioButton2.Text = "新版KEY:Y8vkFwSacHjCFThh";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(27, 336);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(530, 23);
            this.progressBar1.TabIndex = 7;
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(295, 6);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(97, 19);
            this.radioButton3.TabIndex = 8;
            this.radioButton3.Text = "自定义KEY";
            this.radioButton3.UseVisualStyleBackColor = true;
            this.radioButton3.CheckedChanged += new System.EventHandler(this.radioButton3_CheckedChanged);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(295, 31);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(159, 25);
            this.textBox1.TabIndex = 9;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(586, 367);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.radioButton3);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.Button_Excel2Xml);
            this.Controls.Add(this.Button_Xml2Dat);
            this.Controls.Add(this.Button_Xml2Excel);
            this.Controls.Add(this.Button_Dat2Xml);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "GSP Dat文件加解密工具 By Yang";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Button_Dat2Xml;
        private System.Windows.Forms.Button Button_Xml2Excel;
        private System.Windows.Forms.Button Button_Xml2Dat;
        private System.Windows.Forms.Button Button_Excel2Xml;
        private System.Windows.Forms.RichTextBox txtLog;
        public System.Windows.Forms.RadioButton radioButton1;
        public System.Windows.Forms.RadioButton radioButton2;
        public System.Windows.Forms.ProgressBar progressBar1;
        public System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.TextBox textBox1;
    }
}

