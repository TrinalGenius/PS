namespace RetailService
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.richTextBox_Log = new System.Windows.Forms.RichTextBox();
            this.radioButton_stop = new System.Windows.Forms.RadioButton();
            this.radioButton_start = new System.Windows.Forms.RadioButton();
            this.button1 = new System.Windows.Forms.Button();
            this.maskedTextBox1 = new System.Windows.Forms.MaskedTextBox();
            this.maskedTextBox2 = new System.Windows.Forms.MaskedTextBox();
            this.maskedTextBox3 = new System.Windows.Forms.MaskedTextBox();
            this.StatementsListBox = new System.Windows.Forms.ListBox();
            this.StatusTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // richTextBox_Log
            // 
            this.richTextBox_Log.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.richTextBox_Log.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.richTextBox_Log.Location = new System.Drawing.Point(0, 135);
            this.richTextBox_Log.Name = "richTextBox_Log";
            this.richTextBox_Log.Size = new System.Drawing.Size(666, 103);
            this.richTextBox_Log.TabIndex = 11;
            this.richTextBox_Log.Text = "";
            this.richTextBox_Log.TextChanged += new System.EventHandler(this.richTextBox_Log_TextChanged);
            // 
            // radioButton_stop
            // 
            this.radioButton_stop.AutoSize = true;
            this.radioButton_stop.Checked = true;
            this.radioButton_stop.Location = new System.Drawing.Point(36, 76);
            this.radioButton_stop.Name = "radioButton_stop";
            this.radioButton_stop.Size = new System.Drawing.Size(47, 16);
            this.radioButton_stop.TabIndex = 13;
            this.radioButton_stop.TabStop = true;
            this.radioButton_stop.Text = "Stop";
            this.radioButton_stop.UseVisualStyleBackColor = true;
            this.radioButton_stop.CheckedChanged += new System.EventHandler(this.radioButton_stop_CheckedChanged);
            // 
            // radioButton_start
            // 
            this.radioButton_start.AutoSize = true;
            this.radioButton_start.Location = new System.Drawing.Point(36, 36);
            this.radioButton_start.Name = "radioButton_start";
            this.radioButton_start.Size = new System.Drawing.Size(53, 16);
            this.radioButton_start.TabIndex = 12;
            this.radioButton_start.Text = "Start";
            this.radioButton_start.UseVisualStyleBackColor = true;
            this.radioButton_start.CheckedChanged += new System.EventHandler(this.radioButton_start_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(528, 90);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 14;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // maskedTextBox1
            // 
            this.maskedTextBox1.Location = new System.Drawing.Point(528, 12);
            this.maskedTextBox1.Name = "maskedTextBox1";
            this.maskedTextBox1.Size = new System.Drawing.Size(100, 21);
            this.maskedTextBox1.TabIndex = 15;
            this.maskedTextBox1.Text = "1000";
            this.maskedTextBox1.Visible = false;
            // 
            // maskedTextBox2
            // 
            this.maskedTextBox2.Location = new System.Drawing.Point(528, 63);
            this.maskedTextBox2.Name = "maskedTextBox2";
            this.maskedTextBox2.Size = new System.Drawing.Size(100, 21);
            this.maskedTextBox2.TabIndex = 16;
            this.maskedTextBox2.Text = "22160881";
            this.maskedTextBox2.Visible = false;
            // 
            // maskedTextBox3
            // 
            this.maskedTextBox3.Location = new System.Drawing.Point(528, 36);
            this.maskedTextBox3.Name = "maskedTextBox3";
            this.maskedTextBox3.Size = new System.Drawing.Size(100, 21);
            this.maskedTextBox3.TabIndex = 17;
            this.maskedTextBox3.Text = "213123";
            this.maskedTextBox3.Visible = false;
            // 
            // StatementsListBox
            // 
            this.StatementsListBox.FormattingEnabled = true;
            this.StatementsListBox.ItemHeight = 12;
            this.StatementsListBox.Location = new System.Drawing.Point(151, 25);
            this.StatementsListBox.Name = "StatementsListBox";
            this.StatementsListBox.Size = new System.Drawing.Size(356, 88);
            this.StatementsListBox.TabIndex = 18;
            // 
            // StatusTimer
            // 
            this.StatusTimer.Interval = 10000;
            this.StatusTimer.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(666, 238);
            this.Controls.Add(this.StatementsListBox);
            this.Controls.Add(this.maskedTextBox3);
            this.Controls.Add(this.maskedTextBox2);
            this.Controls.Add(this.maskedTextBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.radioButton_stop);
            this.Controls.Add(this.radioButton_start);
            this.Controls.Add(this.richTextBox_Log);
            this.Name = "Form1";
            this.Text = "EPOSRetailServer";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox_Log;
        private System.Windows.Forms.RadioButton radioButton_stop;
        private System.Windows.Forms.RadioButton radioButton_start;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.MaskedTextBox maskedTextBox1;
        private System.Windows.Forms.MaskedTextBox maskedTextBox2;
        private System.Windows.Forms.MaskedTextBox maskedTextBox3;
        private System.Windows.Forms.ListBox StatementsListBox;
        private System.Windows.Forms.Timer StatusTimer;
    }
}

