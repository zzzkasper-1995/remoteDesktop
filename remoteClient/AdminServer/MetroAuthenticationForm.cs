using System;
using System.Windows.Forms;
using MetroFramework.Forms;
using MetroFramework;
using System.Diagnostics;

namespace AdminServer
{
    public partial class MetroAuthenticationForm : MetroForm
    {
        public MetroAuthenticationForm()
        {
            InitializeComponent();
            Program.logger.Debug("MetroAuthenticationForm() ");
        }

        public void Auth()//Метод авторизации
        {
            if (textBox_login.Text != "" && textBox_pas.Text != "")
            {
                try
                {
                    //отправляем запрос на авторизацию на сервер
                    Request req = new Request("Hello");
                    Program.login = textBox_login.Text;
                    Program.pas = textBox_pas.Text;
                    req.parameters.Add(Program.login);
                    req.parameters.Add(Program.pas);
                    req.Send();

                    try
                    {
                        //Создаем наш класс, при этом срабатывает конструктор, который сразу авторизируется в Битрикс24
                        Program.bx_logon = new Bitrix24(Program.login, Program.pas);//Если тут ошибка значит логин пароль не верный

                        Visible = false;
                        label_warning.Visible = false;
                        textBox_login.Style = MetroColorStyle.White;
                        textBox_pas.Style = MetroColorStyle.White;
                        Program.MainForm.Show();
                    }
                    catch
                    {
                        textBox_login.Style = MetroColorStyle.Red;
                        textBox_pas.Style = MetroColorStyle.Red;
                        label_warning.Visible = true;
                    }
                }
                catch (Exception exc)
                {
                    Program.logger.Debug("Auth() " + exc);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Auth();
        }

        private void textBox_login_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                Auth();
            }
        }

        private void textBox_pas_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                Auth();
            }
        }

        private void MetroAuthenticationForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                Auth();
            }
        }

        private void MetroAuthenticationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
