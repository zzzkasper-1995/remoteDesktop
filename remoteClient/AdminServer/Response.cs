using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdminServer
{
    public class Response//то что мы получаем от клиента
    {
        public string command;//команда
        public List<string> parameters;//параметры ответа

        public override string ToString()
        {
            string param = "";
            foreach (string parameter in parameters)
            {
                param += parameter + ", ";
            }
            return "Command:" + command + " Parameters: " + param + ".";
        }

        //Считываем информацию из ответа от клиента или сервера
        public void WorkWithResponse()
        {
            Program.logger.Info("WorkWithResponse() команда: "+ ToString());
            switch (command)
            {
                case ("GetListClients")://Получить список подключенных к серверу клиентов
                    {
                        if(parameters.Count==1)
                        {
                            MetroMainForm.ListClientsID = JsonConvert.DeserializeObject<List<string>>(parameters[0]);
                        }

                        break;
                    }
                case ("Error")://Случилось что то не хорошее
                    {
                        MetroMainForm.remoteC.StatusConnect = "noConnect";
                        break;
                    }
                case ("GetSystemInfo")://получить системные сведения о компьютере клиента
                    {
                        MetroMainForm.remoteC.StatusConnect = "noConnect";
                        try
                        {
                            //try
                            //{
                            //    string TaskListByJSON = Program.bx_logon.SendCommand("crm.lead.get", "ID=" + Program.MainForm.FindID);
                            //    Program.MainForm.WriteInfoFromBitrix(TaskListByJSON);
                            //}
                            //catch (Exception exc) { }

                            MetroMainForm.remoteC.osVersion = parameters[0];
                            MetroMainForm.remoteC.procName = parameters[1];
                            MetroMainForm.remoteC.physicalMemory = parameters[2];
                            MetroMainForm.remoteC.Mac = parameters[3];
                            MetroMainForm.remoteC.id = parameters[4];
                        }
                        catch(Exception exc)
                        {
                            Program.logger.Debug("WorkWithResponse(" + command + ") " + exc);
                        }
                        break;
                    }
                case ("RunVNC")://Запустить VNC
                    {
                        if (parameters.Count > 0)
                        {
                            if (parameters[0] != null)
                                if (parameters[0] == "yes")
                                {
                                    Process[] pr = Process.GetProcessesByName(Properties.Settings.Default.UvncStartFile);
                                    if (pr.Length != 0)
                                    {
                                        foreach (Process p in pr)
                                        {
                                            p.Kill();
                                        }
                                        Thread.Sleep(1500);
                                    }

                                    MetroMainForm.remoteC.StatusConnect = "Запуск UVNC";
                                    ProcessStartInfo procInfo = new ProcessStartInfo
                                    {
                                        UseShellExecute = true,
                                        FileName = Properties.Settings.Default.UvncStartFile + ".exe",  //The file in that DIR.
                                        WorkingDirectory = Repeater.wayToUVNC //The working DIR.
                                    };

                                    string dsmplugin = "SecureVNCPlugin.dsm";
                                    if (Environment.Is64BitOperatingSystem) dsmplugin = "SecureVNCPlugin64.dsm";

                                    procInfo.Arguments = "-proxy " + Repeater.IpUVNC + ":" + Repeater.PortUVNC + " -connect ID:" 
                                                            + MetroMainForm.remoteC.id + " -dsmplugin " + dsmplugin + " -password " 
                                                            + MetroMainForm.remoteC.pas;
                                    Process ProcessVNC = Process.Start(procInfo);  //Start that process.

                                    var outer = Task.Factory.StartNew(() =>      // внешняя задача
                                    {
                                        Thread.Sleep(2000);
                                        while (true)
                                        {
                                            Process[] pr_test = Process.GetProcessesByName(Properties.Settings.Default.UvncStartFile);
                                            if (pr_test.Length == 0)
                                            {
                                                Request req = new Request("KillConnectAdmin");
                                                req.Send();
                                                break;
                                            }
                                        }
                                    });
                                }
                                else
                                {
                                    MetroMainForm.remoteC.StatusConnect = "noConnect";
                                    MessageBox.Show("Удаленный пользователь отверг запрос на присоединение");
                                }
                        }
                        else
                        {
                            MetroMainForm.remoteC.StatusConnect = "noConnect";
                            MessageBox.Show("Пользователь прислал не коректный ответ на ваш запрос");
                            Program.logger.Debug("WorkWithResponse(" + command + ") Недостаточно параметров в запросе");
                        }

                        break;
                    }
                case ("Find")://найти клиента
                    {
                        try
                        {
                            if (parameters.Count>0)
                            {
                                if (parameters[0] == "yes")
                                {
                                    Request req = new Request("GetSystemInfo");
                                    req.parameters.Add(parameters[1]);
                                    req.Send();

                                    Program.MainForm.IsFindID = true;
                                    MetroMainForm.remoteC.StatusOnOrOff = true;
                                }
                                else
                                {
                                    MetroMainForm.remoteC.StatusConnect = "noConnect";
                                    Program.MainForm.IsFindID = true;
                                    MetroMainForm.remoteC.StatusOnOrOff = false;
                                }
                            }
                            else
                            {
                                MetroMainForm.remoteC.StatusConnect = "noConnect";
                                MessageBox.Show("Пользователь прислал не коректный ответ на ваш зпрос");
                                Program.logger.Debug("WorkWithResponse(" + command + ") Недостаточно параметров в запросе");
                            }
                        }
                        catch(Exception exc)
                        {
                            MessageBox.Show("При попытке связаться с компьютером  (ID:" + parameters[1] + ") возникла неизвестная ошибка",
                                 "NEO-SERVICE24");

                            Program.MainForm.IsFindID = true;
                            MetroMainForm.remoteC.StatusOnOrOff = false;
                            Program.logger.Debug("WorkWithResponse(" + command + ") " + exc);
                            MetroMainForm.remoteC.StatusConnect = "noConnect";
                        }
                        break;
                    }
                case ("Connect")://начать процедуру подключения к клиенту
                    {
                        try
                        {
                            if (parameters.Count > 0)
                            {
                                if (parameters[0] == "yes")
                                {
                                    Request req = new Request("NewPas");
                                    MetroMainForm.remoteC.pas = RandomString(8);
                                    MetroMainForm.remoteC.pasEncrypted = EncryptPassword(MetroMainForm.remoteC.pas);
                                    req.parameters.Add(MetroMainForm.remoteC.pasEncrypted);
                                    req.Send();
                                    MetroMainForm.remoteC.StatusConnect = "Начинаем подключение к клиенту";
                                }
                                else
                                {
                                    MetroMainForm.remoteC.StatusConnect = "noConnect";
                                    MessageBox.Show("Удаленный компьютер с ID:" + Program.MainForm.FindID + " не найден, возможно пользователь уже отключился",
                                      "NEO-SERVICE24");
                                }
                            }
                            else
                            {
                                MetroMainForm.remoteC.StatusConnect = "noConnect";
                                MessageBox.Show("Пользователь прислал не коректный ответ на ваш зпрос");
                                Program.logger.Debug("WorkWithResponse(" + command + ") Недостаточно параметров в запросе");
                            }
                        }
                        catch (Exception exc)
                        {
                            MetroMainForm.remoteC.StatusConnect = "noConnect";
                            MessageBox.Show("При попытке связаться с компьютером  (ID:" + Program.MainForm.FindID + ") возникла ошибка",
                                 "NEO-SERVICE24");

                            Program.MainForm.IsFindID = false;

                            Program.logger.Debug("WorkWithResponse(" + command + ") " + exc);
                        }
                        break;
                    }
                case ("NewPas")://Изменить пароль от UVNC у клиента
                    {
                        try
                        {
                            if (parameters.Count > 0)
                            {
                                if (parameters[0] == "OK")
                                {
                                    MetroMainForm.remoteC.StatusConnect = "Ожидайте подтверждения от клиента";
                                    Request req = new Request("RunVNC");
                                    req.Send();
                                }
                                else
                                {
                                    if (parameters.Count > 1)
                                    {
                                        MetroMainForm.remoteC.StatusConnect = "noConnect";
                                        if (parameters[1] == "Unknown")
                                            MessageBox.Show("По неизвестной причине не удается запустить UVNC на машине клиента с ID:" + Program.MainForm.FindID + "  ",
                                              "NEO-SERVICE24");

                                        if (parameters[1] == "NotFile")
                                            MessageBox.Show("На удаленном клиенте сломался файл настроек UVNC",
                                              "NEO-SERVICE24");

                                        if (parameters[1] == "NotAdmin")
                                            MessageBox.Show("Программа на удаленном ПК запущена не от имени администратора",
                                              "NEO-SERVICE24");
                                    }
                                    else
                                    {
                                        MessageBox.Show("По неизвестной причине не удается запустить UVNC на машине клиента с ID:" + Program.MainForm.FindID + "  ",
                                           "NEO-SERVICE24");
                                    }
                                }
                            }
                            else
                            {
                                MetroMainForm.remoteC.StatusConnect = "noConnect";
                                MessageBox.Show("Пользователь прислал не коректный ответ на ваш зпрос");
                                Program.logger.Debug("WorkWithResponse(" + command + ") Недостаточно параметров в запросе");
                            }
                        }
                        catch (Exception exc)
                        {
                            MetroMainForm.remoteC.StatusConnect = "noConnect";
                            MessageBox.Show("При попытке связаться с компьютером  (ID:" + Program.MainForm.FindID + ") возникла неизвестная ошибка",
                                 "NEO-SERVICE24");

                            Program.MainForm.IsFindID = false;

                            Program.logger.Debug("WorkWithResponse(" + command + ") " + exc);
                        }
                        break;
                    }
                case ("Hello")://рукопожитие с сервером
                    {
                        if (parameters.Count > 0)
                        {
                            if (parameters[0] == "Ok")
                            {
                                Program.Auth = 2;
                                try
                                {
                                    Program.bx_logon = new Bitrix24(Program.login, Program.pas);
                                }
                                catch { }
                            }
                            else
                            {
                                Program.Auth = 1;
                            }
                        }
                        else
                        {
                            Program.logger.Debug("WorkWithResponse(" + command + ") Недостаточно параметров в запросе");
                        }
                        break;
                    }
                default: { break; }
            }
        }

        public static string RandomString(int size)//генерируем новый пароль
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                //Генерируем число являющееся латинским символом в юникоде
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                //Конструируем строку со случайно сгенерированными символами
                builder.Append(ch);
            }
            return builder.ToString();
        }

        //-------------------работа с паролем для Uvnc-------------------------------------
        public static string EncryptPassword(string plainPassword)
        {
            DES des = CreateDES();
            ICryptoTransform cryptoTransfrom = des.CreateEncryptor();

            plainPassword = plainPassword + "\0\0\0\0\0\0\0\0";
            plainPassword = plainPassword.Length > 8 ? plainPassword.Substring(0, 8) : plainPassword;
            byte[] data = Encoding.ASCII.GetBytes(plainPassword);
            byte[] encryptedBytes = cryptoTransfrom.TransformFinalBlock(data, 0, data.Length);

            return ByteArrayToHex(encryptedBytes) + "00";
        }

        private static DES CreateDES()
        {
            byte[] key = { 0xE8, 0x4A, 0xD6, 0x60, 0xC4, 0x72, 0x1A, 0xE0 };
            DES des = DES.Create();
            des.Key = key;
            des.IV = key;
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.Zeros;
            return des;
        }

        public static string ByteArrayToHex(byte[] bytes)
        {
            char[] c = new char[bytes.Length * 2];
            byte b;
            for (int i = 0; i < bytes.Length; i++)
            {
                b = (byte)(bytes[i] >> 4);
                c[i * 2] = (char)(b > 9 ? b + 0x37 : b + 0x30);
                b = (byte)(bytes[i] & 0xF);
                c[(i * 2) + 1] = (char)(b > 9 ? b + 0x37 : b + 0x30);
            }
            return new string(c);
        }
        //-------------------КОНЕЦ работа с паролем для Uvnc-------------------------------
    }
}
