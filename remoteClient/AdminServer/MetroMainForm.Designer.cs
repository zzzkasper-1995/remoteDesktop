namespace AdminServer
{
    partial class MetroMainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MetroMainForm));
            this.isCon = new MetroFramework.Controls.MetroTile();
            this.metroTabControl1 = new MetroFramework.Controls.MetroTabControl();
            this.metroTabPage1 = new MetroFramework.Controls.MetroTabPage();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button_find = new MetroFramework.Controls.MetroButton();
            this.textBox_find = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.groupBoxGeneralInfo = new MetroFramework.Controls.MetroPanel();
            this.linkLabel_link = new System.Windows.Forms.LinkLabel();
            this.button_ConnectToRemovePC = new MetroFramework.Controls.MetroButton();
            this.label_stat = new MetroFramework.Controls.MetroLabel();
            this.label_address = new MetroFramework.Controls.MetroLabel();
            this.pictureBox_stat = new System.Windows.Forms.PictureBox();
            this.label_mac = new MetroFramework.Controls.MetroLabel();
            this.label_mem = new MetroFramework.Controls.MetroLabel();
            this.label_proc = new MetroFramework.Controls.MetroLabel();
            this.label_OSVersion = new MetroFramework.Controls.MetroLabel();
            this.label_number = new MetroFramework.Controls.MetroLabel();
            this.label_second_name = new MetroFramework.Controls.MetroLabel();
            this.label_name = new MetroFramework.Controls.MetroLabel();
            this.label_id = new MetroFramework.Controls.MetroLabel();
            this.metroTabPage2 = new MetroFramework.Controls.MetroTabPage();
            this.label_version = new MetroFramework.Controls.MetroLabel();
            this.textBox_IpPortRepeaterServer = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
            this.textBox_IpPortRepeaterUVNC = new MetroFramework.Controls.MetroTextBox();
            this.textBox_WayToVNC = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel4 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.metroButton1 = new MetroFramework.Controls.MetroButton();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.metroTabControl1.SuspendLayout();
            this.metroTabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBoxGeneralInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_stat)).BeginInit();
            this.metroTabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // isCon
            // 
            this.isCon.Location = new System.Drawing.Point(0, 539);
            this.isCon.Name = "isCon";
            this.isCon.Size = new System.Drawing.Size(389, 24);
            this.isCon.Style = MetroFramework.MetroColorStyle.Red;
            this.isCon.TabIndex = 16;
            this.isCon.Text = "Нет подключения к серверу";
            this.isCon.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // metroTabControl1
            // 
            this.metroTabControl1.Controls.Add(this.metroTabPage1);
            this.metroTabControl1.Controls.Add(this.metroTabPage2);
            this.metroTabControl1.Location = new System.Drawing.Point(0, 63);
            this.metroTabControl1.Name = "metroTabControl1";
            this.metroTabControl1.SelectedIndex = 0;
            this.metroTabControl1.Size = new System.Drawing.Size(378, 470);
            this.metroTabControl1.Style = MetroFramework.MetroColorStyle.Red;
            this.metroTabControl1.TabIndex = 17;
            this.metroTabControl1.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.metroTabControl1_Selecting);
            // 
            // metroTabPage1
            // 
            this.metroTabPage1.Controls.Add(this.pictureBox1);
            this.metroTabPage1.Controls.Add(this.button_find);
            this.metroTabPage1.Controls.Add(this.textBox_find);
            this.metroTabPage1.Controls.Add(this.metroLabel1);
            this.metroTabPage1.Controls.Add(this.groupBoxGeneralInfo);
            this.metroTabPage1.HorizontalScrollbarBarColor = true;
            this.metroTabPage1.Location = new System.Drawing.Point(4, 35);
            this.metroTabPage1.Name = "metroTabPage1";
            this.metroTabPage1.Size = new System.Drawing.Size(370, 431);
            this.metroTabPage1.TabIndex = 0;
            this.metroTabPage1.Text = "Удаленный доступ";
            this.metroTabPage1.VerticalScrollbarBarColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.Image = global::AdminServer.Properties.Resources.GotoListClients;
            this.pictureBox1.Location = new System.Drawing.Point(335, 414);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(17, 14);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 18;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // button_find
            // 
            this.button_find.Location = new System.Drawing.Point(266, 34);
            this.button_find.Name = "button_find";
            this.button_find.Size = new System.Drawing.Size(104, 23);
            this.button_find.TabIndex = 20;
            this.button_find.Text = "Найти";
            this.button_find.Click += new System.EventHandler(this.button_find_Click);
            // 
            // textBox_find
            // 
            this.textBox_find.FontSize = MetroFramework.MetroTextBoxSize.Medium;
            this.textBox_find.Location = new System.Drawing.Point(20, 34);
            this.textBox_find.Name = "textBox_find";
            this.textBox_find.Size = new System.Drawing.Size(208, 23);
            this.textBox_find.TabIndex = 19;
            this.textBox_find.TextChanged += new System.EventHandler(this.textBox_find_TextChanged);
            this.textBox_find.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_find_KeyPress);
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.metroLabel1.Location = new System.Drawing.Point(20, 12);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(80, 19);
            this.metroLabel1.TabIndex = 18;
            this.metroLabel1.Text = "ID клиента:";
            // 
            // groupBoxGeneralInfo
            // 
            this.groupBoxGeneralInfo.Controls.Add(this.linkLabel_link);
            this.groupBoxGeneralInfo.Controls.Add(this.button_ConnectToRemovePC);
            this.groupBoxGeneralInfo.Controls.Add(this.label_stat);
            this.groupBoxGeneralInfo.Controls.Add(this.label_address);
            this.groupBoxGeneralInfo.Controls.Add(this.pictureBox_stat);
            this.groupBoxGeneralInfo.Controls.Add(this.label_mac);
            this.groupBoxGeneralInfo.Controls.Add(this.label_mem);
            this.groupBoxGeneralInfo.Controls.Add(this.label_proc);
            this.groupBoxGeneralInfo.Controls.Add(this.label_OSVersion);
            this.groupBoxGeneralInfo.Controls.Add(this.label_number);
            this.groupBoxGeneralInfo.Controls.Add(this.label_second_name);
            this.groupBoxGeneralInfo.Controls.Add(this.label_name);
            this.groupBoxGeneralInfo.Controls.Add(this.label_id);
            this.groupBoxGeneralInfo.HorizontalScrollbarBarColor = true;
            this.groupBoxGeneralInfo.HorizontalScrollbarHighlightOnWheel = false;
            this.groupBoxGeneralInfo.HorizontalScrollbarSize = 10;
            this.groupBoxGeneralInfo.Location = new System.Drawing.Point(20, 65);
            this.groupBoxGeneralInfo.Name = "groupBoxGeneralInfo";
            this.groupBoxGeneralInfo.Size = new System.Drawing.Size(350, 350);
            this.groupBoxGeneralInfo.TabIndex = 22;
            this.groupBoxGeneralInfo.VerticalScrollbarBarColor = true;
            this.groupBoxGeneralInfo.VerticalScrollbarHighlightOnWheel = false;
            this.groupBoxGeneralInfo.VerticalScrollbarSize = 10;
            this.groupBoxGeneralInfo.Visible = false;
            // 
            // linkLabel_link
            // 
            this.linkLabel_link.BackColor = System.Drawing.Color.White;
            this.linkLabel_link.DisabledLinkColor = System.Drawing.Color.White;
            this.linkLabel_link.Font = new System.Drawing.Font("Arial Unicode MS", 9F);
            this.linkLabel_link.Location = new System.Drawing.Point(3, 60);
            this.linkLabel_link.Name = "linkLabel_link";
            this.linkLabel_link.Size = new System.Drawing.Size(344, 16);
            this.linkLabel_link.TabIndex = 32;
            this.linkLabel_link.TabStop = true;
            this.linkLabel_link.Text = "link";
            this.linkLabel_link.Click += new System.EventHandler(this.linkLabel_link_Click);
            // 
            // button_ConnectToRemovePC
            // 
            this.button_ConnectToRemovePC.Location = new System.Drawing.Point(243, 7);
            this.button_ConnectToRemovePC.Name = "button_ConnectToRemovePC";
            this.button_ConnectToRemovePC.Size = new System.Drawing.Size(104, 23);
            this.button_ConnectToRemovePC.TabIndex = 21;
            this.button_ConnectToRemovePC.Text = "Подключиться";
            this.button_ConnectToRemovePC.Visible = false;
            this.button_ConnectToRemovePC.Click += new System.EventHandler(this.button_ConnectToRemovePC_Click);
            // 
            // label_stat
            // 
            this.label_stat.AutoSize = true;
            this.label_stat.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.label_stat.Location = new System.Drawing.Point(31, 11);
            this.label_stat.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.label_stat.Name = "label_stat";
            this.label_stat.Size = new System.Drawing.Size(49, 19);
            this.label_stat.TabIndex = 30;
            this.label_stat.Text = "Offline";
            // 
            // label_address
            // 
            this.label_address.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.label_address.Location = new System.Drawing.Point(3, 168);
            this.label_address.Margin = new System.Windows.Forms.Padding(10);
            this.label_address.Name = "label_address";
            this.label_address.Size = new System.Drawing.Size(344, 19);
            this.label_address.TabIndex = 29;
            this.label_address.Text = "Адрес: ";
            // 
            // pictureBox_stat
            // 
            this.pictureBox_stat.BackColor = System.Drawing.Color.White;
            this.pictureBox_stat.Location = new System.Drawing.Point(3, 7);
            this.pictureBox_stat.Name = "pictureBox_stat";
            this.pictureBox_stat.Size = new System.Drawing.Size(23, 23);
            this.pictureBox_stat.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_stat.TabIndex = 23;
            this.pictureBox_stat.TabStop = false;
            // 
            // label_mac
            // 
            this.label_mac.AutoSize = true;
            this.label_mac.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.label_mac.Location = new System.Drawing.Point(3, 276);
            this.label_mac.Name = "label_mac";
            this.label_mac.Size = new System.Drawing.Size(84, 19);
            this.label_mac.TabIndex = 28;
            this.label_mac.Text = "Mac-адрес: ";
            // 
            // label_mem
            // 
            this.label_mem.AutoSize = true;
            this.label_mem.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.label_mem.Location = new System.Drawing.Point(3, 249);
            this.label_mem.Name = "label_mem";
            this.label_mem.Size = new System.Drawing.Size(43, 19);
            this.label_mem.TabIndex = 27;
            this.label_mem.Text = "ОЗУ: ";
            // 
            // label_proc
            // 
            this.label_proc.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.label_proc.Location = new System.Drawing.Point(3, 222);
            this.label_proc.Name = "label_proc";
            this.label_proc.Size = new System.Drawing.Size(344, 19);
            this.label_proc.TabIndex = 26;
            this.label_proc.Text = "Процессор:";
            // 
            // label_OSVersion
            // 
            this.label_OSVersion.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.label_OSVersion.Location = new System.Drawing.Point(3, 195);
            this.label_OSVersion.Name = "label_OSVersion";
            this.label_OSVersion.Size = new System.Drawing.Size(344, 19);
            this.label_OSVersion.TabIndex = 25;
            this.label_OSVersion.Text = "Операционная система:";
            // 
            // label_number
            // 
            this.label_number.AutoSize = true;
            this.label_number.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.label_number.Location = new System.Drawing.Point(3, 141);
            this.label_number.Margin = new System.Windows.Forms.Padding(10);
            this.label_number.Name = "label_number";
            this.label_number.Size = new System.Drawing.Size(119, 19);
            this.label_number.TabIndex = 24;
            this.label_number.Text = "Номер телефона:";
            // 
            // label_second_name
            // 
            this.label_second_name.AutoSize = true;
            this.label_second_name.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.label_second_name.Location = new System.Drawing.Point(3, 87);
            this.label_second_name.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.label_second_name.Name = "label_second_name";
            this.label_second_name.Size = new System.Drawing.Size(73, 19);
            this.label_second_name.TabIndex = 23;
            this.label_second_name.Text = "Фамилия: ";
            // 
            // label_name
            // 
            this.label_name.AutoSize = true;
            this.label_name.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.label_name.Location = new System.Drawing.Point(3, 114);
            this.label_name.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.label_name.Name = "label_name";
            this.label_name.Size = new System.Drawing.Size(43, 19);
            this.label_name.TabIndex = 22;
            this.label_name.Text = "Имя: ";
            // 
            // label_id
            // 
            this.label_id.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.label_id.Location = new System.Drawing.Point(3, 33);
            this.label_id.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.label_id.Name = "label_id";
            this.label_id.Size = new System.Drawing.Size(344, 19);
            this.label_id.TabIndex = 21;
            this.label_id.Text = "ID клиента:";
            // 
            // metroTabPage2
            // 
            this.metroTabPage2.Controls.Add(this.label_version);
            this.metroTabPage2.Controls.Add(this.textBox_IpPortRepeaterServer);
            this.metroTabPage2.Controls.Add(this.metroLabel3);
            this.metroTabPage2.Controls.Add(this.textBox_IpPortRepeaterUVNC);
            this.metroTabPage2.Controls.Add(this.textBox_WayToVNC);
            this.metroTabPage2.Controls.Add(this.metroLabel4);
            this.metroTabPage2.Controls.Add(this.metroLabel2);
            this.metroTabPage2.Controls.Add(this.metroButton1);
            this.metroTabPage2.HorizontalScrollbarBarColor = true;
            this.metroTabPage2.Location = new System.Drawing.Point(4, 35);
            this.metroTabPage2.Name = "metroTabPage2";
            this.metroTabPage2.Size = new System.Drawing.Size(370, 431);
            this.metroTabPage2.TabIndex = 1;
            this.metroTabPage2.Text = "Настройки";
            this.metroTabPage2.VerticalScrollbarBarColor = true;
            // 
            // label_version
            // 
            this.label_version.AutoSize = true;
            this.label_version.Cursor = System.Windows.Forms.Cursors.Default;
            this.label_version.FontSize = MetroFramework.MetroLabelSize.Small;
            this.label_version.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.label_version.Location = new System.Drawing.Point(3, 416);
            this.label_version.Name = "label_version";
            this.label_version.Size = new System.Drawing.Size(171, 15);
            this.label_version.TabIndex = 47;
            this.label_version.Text = "Текущая версия программы: ";
            this.label_version.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBox_IpPortRepeaterServer
            // 
            this.textBox_IpPortRepeaterServer.FontSize = MetroFramework.MetroTextBoxSize.Medium;
            this.textBox_IpPortRepeaterServer.Location = new System.Drawing.Point(18, 167);
            this.textBox_IpPortRepeaterServer.Name = "textBox_IpPortRepeaterServer";
            this.textBox_IpPortRepeaterServer.Size = new System.Drawing.Size(330, 23);
            this.textBox_IpPortRepeaterServer.Style = MetroFramework.MetroColorStyle.White;
            this.textBox_IpPortRepeaterServer.TabIndex = 46;
            this.textBox_IpPortRepeaterServer.UseStyleColors = true;
            this.textBox_IpPortRepeaterServer.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_IpPortRepeaterServer_KeyPress);
            // 
            // metroLabel3
            // 
            this.metroLabel3.AutoSize = true;
            this.metroLabel3.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.metroLabel3.Location = new System.Drawing.Point(19, 145);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(187, 19);
            this.metroLabel3.Style = MetroFramework.MetroColorStyle.Black;
            this.metroLabel3.TabIndex = 45;
            this.metroLabel3.Text = "IP сервера (127.0.0.1:34001)";
            // 
            // textBox_IpPortRepeaterUVNC
            // 
            this.textBox_IpPortRepeaterUVNC.FontSize = MetroFramework.MetroTextBoxSize.Medium;
            this.textBox_IpPortRepeaterUVNC.Location = new System.Drawing.Point(18, 101);
            this.textBox_IpPortRepeaterUVNC.Name = "textBox_IpPortRepeaterUVNC";
            this.textBox_IpPortRepeaterUVNC.Size = new System.Drawing.Size(330, 23);
            this.textBox_IpPortRepeaterUVNC.Style = MetroFramework.MetroColorStyle.White;
            this.textBox_IpPortRepeaterUVNC.TabIndex = 44;
            this.textBox_IpPortRepeaterUVNC.UseStyleColors = true;
            this.textBox_IpPortRepeaterUVNC.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_IpPortRepeaterUVNC_KeyPress);
            // 
            // textBox_WayToVNC
            // 
            this.textBox_WayToVNC.FontSize = MetroFramework.MetroTextBoxSize.Medium;
            this.textBox_WayToVNC.Location = new System.Drawing.Point(19, 40);
            this.textBox_WayToVNC.Name = "textBox_WayToVNC";
            this.textBox_WayToVNC.Size = new System.Drawing.Size(330, 23);
            this.textBox_WayToVNC.Style = MetroFramework.MetroColorStyle.White;
            this.textBox_WayToVNC.TabIndex = 42;
            this.textBox_WayToVNC.UseStyleColors = true;
            this.textBox_WayToVNC.Click += new System.EventHandler(this.textBox_WayToVNC_Click);
            this.textBox_WayToVNC.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_WayToVNC_KeyPress);
            // 
            // metroLabel4
            // 
            this.metroLabel4.AutoSize = true;
            this.metroLabel4.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.metroLabel4.Location = new System.Drawing.Point(19, 79);
            this.metroLabel4.Name = "metroLabel4";
            this.metroLabel4.Size = new System.Drawing.Size(254, 19);
            this.metroLabel4.Style = MetroFramework.MetroColorStyle.Black;
            this.metroLabel4.TabIndex = 41;
            this.metroLabel4.Text = "IP репитера Ultra VNC (127.0.0.1:5500)";
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.metroLabel2.Location = new System.Drawing.Point(19, 18);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(163, 19);
            this.metroLabel2.Style = MetroFramework.MetroColorStyle.Black;
            this.metroLabel2.TabIndex = 39;
            this.metroLabel2.Text = "Путь до папки UltraVNC";
            // 
            // metroButton1
            // 
            this.metroButton1.Location = new System.Drawing.Point(71, 208);
            this.metroButton1.Name = "metroButton1";
            this.metroButton1.Size = new System.Drawing.Size(218, 23);
            this.metroButton1.TabIndex = 38;
            this.metroButton1.Text = "Применить";
            this.metroButton1.Click += new System.EventHandler(this.metroButton1_Click);
            // 
            // MetroMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = MetroFramework.Drawing.MetroBorderStyle.FixedSingle;
            this.ClientSize = new System.Drawing.Size(386, 563);
            this.Controls.Add(this.isCon);
            this.Controls.Add(this.metroTabControl1);
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MetroMainForm";
            this.Style = MetroFramework.MetroColorStyle.Red;
            this.Text = "NEO-SERVICE24";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MetroMainForm_FormClosing);
            this.Load += new System.EventHandler(this.MetroMainForm_Load);
            this.LocationChanged += new System.EventHandler(this.MetroMainForm_LocationChanged);
            this.metroTabControl1.ResumeLayout(false);
            this.metroTabPage1.ResumeLayout(false);
            this.metroTabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBoxGeneralInfo.ResumeLayout(false);
            this.groupBoxGeneralInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_stat)).EndInit();
            this.metroTabPage2.ResumeLayout(false);
            this.metroTabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private MetroFramework.Controls.MetroTile isCon;
        private MetroFramework.Controls.MetroTabControl metroTabControl1;
        private MetroFramework.Controls.MetroTabPage metroTabPage1;
        private MetroFramework.Controls.MetroPanel groupBoxGeneralInfo;
        private MetroFramework.Controls.MetroLabel label_mac;
        private MetroFramework.Controls.MetroLabel label_mem;
        private MetroFramework.Controls.MetroLabel label_proc;
        private MetroFramework.Controls.MetroLabel label_OSVersion;
        private MetroFramework.Controls.MetroButton button_ConnectToRemovePC;
        private MetroFramework.Controls.MetroButton button_find;
        private MetroFramework.Controls.MetroTextBox textBox_find;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroTabPage metroTabPage2;
        private MetroFramework.Controls.MetroTextBox textBox_IpPortRepeaterUVNC;
        private MetroFramework.Controls.MetroTextBox textBox_WayToVNC;
        private MetroFramework.Controls.MetroLabel metroLabel4;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroButton metroButton1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private MetroFramework.Controls.MetroTextBox textBox_IpPortRepeaterServer;
        private MetroFramework.Controls.MetroLabel metroLabel3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox_stat;
        private MetroFramework.Controls.MetroLabel label_number;
        private MetroFramework.Controls.MetroLabel label_second_name;
        private MetroFramework.Controls.MetroLabel label_name;
        private MetroFramework.Controls.MetroLabel label_id;
        private MetroFramework.Controls.MetroLabel label_address;
        private MetroFramework.Controls.MetroLabel label_stat;
        private System.Windows.Forms.LinkLabel linkLabel_link;
        private MetroFramework.Controls.MetroLabel label_version;
    }
}