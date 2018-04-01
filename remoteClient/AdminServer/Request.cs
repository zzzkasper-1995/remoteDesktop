using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace AdminServer
{
    public class Request//То что мы отправляем клиенту
    {
        public string command;
        public List<string> parameters;

        public Request(string Command)
        {
            command = Command;
            parameters = new List<string>();
        }

        public Request(string Command, List<string> Parameters)
        {
            command = Command;
            parameters = Parameters;
        }

        public void Send()//отправить запрос на сервер
        {
            if (Repeater.ConnectServer)
            {
                if (Repeater.eClient != null)
                {
                    string jsonreq = JsonConvert.SerializeObject(this);

                    string enc = Encrypt(jsonreq, "1234") + "\r\n";
                    byte[] dataWrite = Encoding.UTF8.GetBytes(enc);
                    
                    try
                    {
                        Repeater.networkStream.Write(dataWrite, 0, dataWrite.Length);
                    }
                    catch (Exception exc)
                    {
                        Program.logger.Debug("send() ошибка: " + exc);
                    }
                }
            }
            else
            {
                if (command == "Hello" || command == "Exit") { }
                else
                {
                    if (command != "GetListClients")
                        MessageBox.Show("Нельзя выполнить эту команду пока нет соединения с сервером", "Ошибка");
                }           
            }
        }

        public static string Encrypt(string ishText, string pass,
                                     string sol = "doberman", string cryptographicAlgorithm = "SHA1",
                                     int passIter = 2, string initVec = "a8doSuDitOz1hZe#",
                                     int keySize = 256)//метод шифрования строки
        {
            if (string.IsNullOrEmpty(ishText))
                return "";

            byte[] initVecB = Encoding.ASCII.GetBytes(initVec);
            byte[] solB = Encoding.ASCII.GetBytes(sol);
            byte[] ishTextB = Encoding.UTF8.GetBytes(ishText);

            PasswordDeriveBytes derivPass = new PasswordDeriveBytes(pass, solB, cryptographicAlgorithm, passIter);
            byte[] keyBytes = derivPass.GetBytes(keySize / 8);
            RijndaelManaged symmK = new RijndaelManaged
            {
                Mode = CipherMode.CBC
            };

            byte[] cipherTextBytes = null;

            using (ICryptoTransform encryptor = symmK.CreateEncryptor(keyBytes, initVecB))
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(ishTextB, 0, ishTextB.Length);
                        cryptoStream.FlushFinalBlock();
                        cipherTextBytes = memStream.ToArray();
                        memStream.Close();
                        cryptoStream.Close();
                    }
                }
            }

            symmK.Clear();
            return Convert.ToBase64String(cipherTextBytes);
        }
    }
}
