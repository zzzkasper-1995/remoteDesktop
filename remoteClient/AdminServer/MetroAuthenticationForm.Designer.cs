namespace AdminServer
{
    partial class MetroAuthenticationForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MetroAuthenticationForm));
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.textBox_login = new MetroFramework.Controls.MetroTextBox();
            this.textBox_pas = new MetroFramework.Controls.MetroTextBox();
            this.button1 = new MetroFramework.Controls.MetroButton();
            this.label_warning = new MetroFramework.Controls.MetroLabel();
            this.SuspendLayout();
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.metroLabel1.Location = new System.Drawing.Point(22, 64);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(47, 19);
            this.metroLabel1.TabIndex = 10;
            this.metroLabel1.Text = "Логин";
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.metroLabel2.Location = new System.Drawing.Point(22, 103);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(56, 19);
            this.metroLabel2.TabIndex = 11;
            this.metroLabel2.Text = "Пароль";
            // 
            // textBox_login
            // 
            this.textBox_login.FontSize = MetroFramework.MetroTextBoxSize.Medium;
            this.textBox_login.Location = new System.Drawing.Point(93, 60);
            this.textBox_login.Name = "textBox_login";
            this.textBox_login.Size = new System.Drawing.Size(226, 23);
            this.textBox_login.Style = MetroFramework.MetroColorStyle.White;
            this.textBox_login.TabIndex = 12;
            this.textBox_login.TabStop = false;
            this.textBox_login.Text = "zzz.kasper.zzz.1995@gmail.com";
            this.textBox_login.UseStyleColors = true;
            this.textBox_login.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_login_KeyPress);
            // 
            // textBox_pas
            // 
            this.textBox_pas.FontSize = MetroFramework.MetroTextBoxSize.Medium;
            this.textBox_pas.Location = new System.Drawing.Point(93, 99);
            this.textBox_pas.Name = "textBox_pas";
            this.textBox_pas.PasswordChar = '●';
            this.textBox_pas.Size = new System.Drawing.Size(226, 23);
            this.textBox_pas.Style = MetroFramework.MetroColorStyle.White;
            this.textBox_pas.TabIndex = 13;
            this.textBox_pas.TabStop = false;
            this.textBox_pas.UseStyleColors = true;
            this.textBox_pas.UseSystemPasswordChar = true;
            this.textBox_pas.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_pas_KeyPress);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(91, 156);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(228, 23);
            this.button1.TabIndex = 14;
            this.button1.Text = "Войти";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label_warning
            // 
            this.label_warning.AutoSize = true;
            this.label_warning.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label_warning.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.label_warning.ForeColor = System.Drawing.Color.Red;
            this.label_warning.Location = new System.Drawing.Point(109, 125);
            this.label_warning.Name = "label_warning";
            this.label_warning.Size = new System.Drawing.Size(190, 19);
            this.label_warning.Style = MetroFramework.MetroColorStyle.Red;
            this.label_warning.TabIndex = 16;
            this.label_warning.Text = "Неверный логин или пароль";
            this.label_warning.UseStyleColors = true;
            this.label_warning.Visible = false;
            // 
            // MetroAuthenticationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = MetroFramework.Drawing.MetroBorderStyle.FixedSingle;
            this.ClientSize = new System.Drawing.Size(342, 192);
            this.Controls.Add(this.label_warning);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox_pas);
            this.Controls.Add(this.textBox_login);
            this.Controls.Add(this.metroLabel2);
            this.Controls.Add(this.metroLabel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(342, 192);
            this.MinimumSize = new System.Drawing.Size(342, 192);
            this.Name = "MetroAuthenticationForm";
            this.Text = "Авторизация";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MetroAuthenticationForm_FormClosing);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MetroAuthenticationForm_KeyPress);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroTextBox textBox_login;
        private MetroFramework.Controls.MetroTextBox textBox_pas;
        private MetroFramework.Controls.MetroButton button1;
        private MetroFramework.Controls.MetroLabel label_warning;
    }
}