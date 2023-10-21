namespace jhtwj
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
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.selectFileButton = new System.Windows.Forms.Button();
            this.generateLinkButton = new System.Windows.Forms.Button();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.filePathLabel = new System.Windows.Forms.Label();
            this.serverStatusLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.picQrCode = new System.Windows.Forms.PictureBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.ipv6 = new System.Windows.Forms.Label();
            this.update = new System.Windows.Forms.ToolTip(this.components);
            this.ipv6tip = new System.Windows.Forms.ToolTip(this.components);
            this.updatemsg = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.picQrCode)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 10;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // selectFileButton
            // 
            this.selectFileButton.Location = new System.Drawing.Point(12, 12);
            this.selectFileButton.Name = "selectFileButton";
            this.selectFileButton.Size = new System.Drawing.Size(273, 109);
            this.selectFileButton.TabIndex = 0;
            this.selectFileButton.Text = "选择文件\r\n\r\n（可拖放）";
            this.selectFileButton.UseVisualStyleBackColor = true;
            this.selectFileButton.Click += new System.EventHandler(this.selectFileButton_Click);
            this.selectFileButton.DragEnter += new System.Windows.Forms.DragEventHandler(this.selectFileButton_DragEnter);
            // 
            // generateLinkButton
            // 
            this.generateLinkButton.Location = new System.Drawing.Point(329, 88);
            this.generateLinkButton.Name = "generateLinkButton";
            this.generateLinkButton.Size = new System.Drawing.Size(68, 33);
            this.generateLinkButton.TabIndex = 1;
            this.generateLinkButton.Text = "生成链接";
            this.generateLinkButton.UseVisualStyleBackColor = true;
            this.generateLinkButton.Click += new System.EventHandler(this.generateLinkButton_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.Location = new System.Drawing.Point(12, 237);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(158, 54);
            this.linkLabel1.TabIndex = 2;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // filePathLabel
            // 
            this.filePathLabel.Location = new System.Drawing.Point(12, 151);
            this.filePathLabel.Name = "filePathLabel";
            this.filePathLabel.Size = new System.Drawing.Size(275, 47);
            this.filePathLabel.TabIndex = 3;
            // 
            // serverStatusLabel
            // 
            this.serverStatusLabel.AutoSize = true;
            this.serverStatusLabel.Location = new System.Drawing.Point(10, 345);
            this.serverStatusLabel.Name = "serverStatusLabel";
            this.serverStatusLabel.Size = new System.Drawing.Size(77, 12);
            this.serverStatusLabel.TabIndex = 4;
            this.serverStatusLabel.Text = "服务启动检测";
            this.serverStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(182, 319);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "分享本软件";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // timer2
            // 
            this.timer2.Interval = 10;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(248, 319);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "v0.0.0";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 212);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "              ";
            // 
            // picQrCode
            // 
            this.picQrCode.Location = new System.Drawing.Point(185, 212);
            this.picQrCode.Name = "picQrCode";
            this.picQrCode.Size = new System.Drawing.Size(100, 100);
            this.picQrCode.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picQrCode.TabIndex = 8;
            this.picQrCode.TabStop = false;
            // 
            // ipv6
            // 
            this.ipv6.AutoSize = true;
            this.ipv6.Location = new System.Drawing.Point(12, 319);
            this.ipv6.Name = "ipv6";
            this.ipv6.Size = new System.Drawing.Size(29, 12);
            this.ipv6.TabIndex = 9;
            this.ipv6.Text = "ipv6";
            this.ipv6.Click += new System.EventHandler(this.ipv6_Click);
            // 
            // update
            // 
            this.update.Popup += new System.Windows.Forms.PopupEventHandler(this.update_Popup);
            // 
            // updatemsg
            // 
            this.updatemsg.Interval = 1000;
            this.updatemsg.Tick += new System.EventHandler(this.updatemsg_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(295, 340);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ipv6);
            this.Controls.Add(this.picQrCode);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.serverStatusLabel);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.filePathLabel);
            this.Controls.Add(this.generateLinkButton);
            this.Controls.Add(this.selectFileButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Form1";
            this.Text = "文件传送器 --xiaobai.pro";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picQrCode)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button selectFileButton;
        private System.Windows.Forms.Button generateLinkButton;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label filePathLabel;
        private System.Windows.Forms.Label serverStatusLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox picQrCode;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label ipv6;
        private System.Windows.Forms.ToolTip update;
        private System.Windows.Forms.ToolTip ipv6tip;
        private System.Windows.Forms.Timer updatemsg;
    }
}

