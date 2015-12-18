namespace WindowsFormsApplication3
{
    partial class MainForm
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button_unlock = new System.Windows.Forms.Button();
            this.maskedTextBox_tableID = new System.Windows.Forms.MaskedTextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox_log_debug = new System.Windows.Forms.CheckBox();
            this.checkBox_log_Info = new System.Windows.Forms.CheckBox();
            this.radioButton_stop = new System.Windows.Forms.RadioButton();
            this.radioButton_start = new System.Windows.Forms.RadioButton();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.timer_paidinfoXml = new System.Windows.Forms.Timer(this.components);
            this.richTextBox_Log = new System.Windows.Forms.RichTextBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(596, 282);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.richTextBox_Log);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.radioButton_stop);
            this.tabPage1.Controls.Add(this.radioButton_start);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(588, 256);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Main";
            this.tabPage1.UseVisualStyleBackColor = true;
            this.tabPage1.Click += new System.EventHandler(this.tabPage1_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button_unlock);
            this.groupBox2.Controls.Add(this.maskedTextBox_tableID);
            this.groupBox2.Location = new System.Drawing.Point(327, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(162, 90);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Unlock Table";
            // 
            // button_unlock
            // 
            this.button_unlock.Location = new System.Drawing.Point(70, 61);
            this.button_unlock.Name = "button_unlock";
            this.button_unlock.Size = new System.Drawing.Size(75, 23);
            this.button_unlock.TabIndex = 1;
            this.button_unlock.Text = "Unlock";
            this.button_unlock.UseVisualStyleBackColor = true;
            this.button_unlock.Click += new System.EventHandler(this.button_unlock_Click);
            // 
            // maskedTextBox_tableID
            // 
            this.maskedTextBox_tableID.Location = new System.Drawing.Point(6, 34);
            this.maskedTextBox_tableID.Name = "maskedTextBox_tableID";
            this.maskedTextBox_tableID.Size = new System.Drawing.Size(100, 21);
            this.maskedTextBox_tableID.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.Controls.Add(this.checkBox_log_debug);
            this.groupBox1.Controls.Add(this.checkBox_log_Info);
            this.groupBox1.Location = new System.Drawing.Point(489, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(96, 90);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Log";
            // 
            // checkBox_log_debug
            // 
            this.checkBox_log_debug.AutoSize = true;
            this.checkBox_log_debug.Location = new System.Drawing.Point(12, 42);
            this.checkBox_log_debug.Name = "checkBox_log_debug";
            this.checkBox_log_debug.Size = new System.Drawing.Size(66, 16);
            this.checkBox_log_debug.TabIndex = 1;
            this.checkBox_log_debug.Text = "Details";
            this.checkBox_log_debug.UseVisualStyleBackColor = true;
            this.checkBox_log_debug.CheckedChanged += new System.EventHandler(this.checkBox_log_debug_CheckedChanged);
            // 
            // checkBox_log_Info
            // 
            this.checkBox_log_Info.AutoSize = true;
            this.checkBox_log_Info.Checked = true;
            this.checkBox_log_Info.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_log_Info.Location = new System.Drawing.Point(12, 20);
            this.checkBox_log_Info.Name = "checkBox_log_Info";
            this.checkBox_log_Info.Size = new System.Drawing.Size(60, 16);
            this.checkBox_log_Info.TabIndex = 0;
            this.checkBox_log_Info.Text = "Simple";
            this.checkBox_log_Info.UseVisualStyleBackColor = true;
            this.checkBox_log_Info.CheckedChanged += new System.EventHandler(this.checkBox_log_Info_CheckedChanged);
            // 
            // radioButton_stop
            // 
            this.radioButton_stop.AutoSize = true;
            this.radioButton_stop.Checked = true;
            this.radioButton_stop.Location = new System.Drawing.Point(121, 23);
            this.radioButton_stop.Name = "radioButton_stop";
            this.radioButton_stop.Size = new System.Drawing.Size(47, 16);
            this.radioButton_stop.TabIndex = 3;
            this.radioButton_stop.TabStop = true;
            this.radioButton_stop.Text = "Stop";
            this.radioButton_stop.UseVisualStyleBackColor = true;
            this.radioButton_stop.CheckedChanged += new System.EventHandler(this.radioButton_stop_CheckedChanged);
            // 
            // radioButton_start
            // 
            this.radioButton_start.AutoSize = true;
            this.radioButton_start.Location = new System.Drawing.Point(47, 22);
            this.radioButton_start.Name = "radioButton_start";
            this.radioButton_start.Size = new System.Drawing.Size(53, 16);
            this.radioButton_start.TabIndex = 2;
            this.radioButton_start.Text = "Start";
            this.radioButton_start.UseVisualStyleBackColor = true;
            this.radioButton_start.CheckedChanged += new System.EventHandler(this.radioButton_start_CheckedChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(588, 256);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // timer_paidinfoXml
            // 
            this.timer_paidinfoXml.Interval = 60000;
            this.timer_paidinfoXml.Tick += new System.EventHandler(this.timer_paidinfoXml_Tick);
            // 
            // richTextBox_Log
            // 
            this.richTextBox_Log.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.richTextBox_Log.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.richTextBox_Log.Location = new System.Drawing.Point(3, 99);
            this.richTextBox_Log.Name = "richTextBox_Log";
            this.richTextBox_Log.Size = new System.Drawing.Size(582, 154);
            this.richTextBox_Log.TabIndex = 7;
            this.richTextBox_Log.Text = "";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(596, 282);
            this.Controls.Add(this.tabControl1);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "EPOS Service";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.RadioButton radioButton_stop;
        private System.Windows.Forms.RadioButton radioButton_start;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBox_log_debug;
        private System.Windows.Forms.CheckBox checkBox_log_Info;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button_unlock;
        private System.Windows.Forms.MaskedTextBox maskedTextBox_tableID;
        private System.Windows.Forms.Timer timer_paidinfoXml;
        private System.Windows.Forms.RichTextBox richTextBox_Log;

    }
}

