using System.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Configuration;
using NLog;
using System.Security.Principal;
using System.Windows.Forms;
using System.ComponentModel;
using Newtonsoft.Json;

namespace update
{
    class Program
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            logger.Error("Main() UpdateForAdmin");
            //Проверяем запущена программа с правами администратора или нет
            WindowsPrincipal pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            bool hasAdministrativeRight = pricipal.IsInRole(WindowsBuiltInRole.Administrator);

            if (hasAdministrativeRight == false)
            {
                logger.Error("Main() UpdateForAdmin if (hasAdministrativeRight == false)");
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    Verb = "runas", //в данном случае указываем, что процесс должен быть запущен с правами администратора
                    FileName = Application.ExecutablePath //указываем исполняемый файл (программу) для запуска
                }; //создаем новый процесс
                try
                {
                    Process.Start(processInfo); //пытаемся запустить процесс
                }
                catch (Win32Exception)
                {
                    //Ничего не делаем, потому что пользователь, возможно, нажал кнопку "Нет" в ответ на вопрос о запуске программы в окне предупреждения UAC (для Windows 7)
                }
                Application.Exit(); //закрываем текущую копию программы (в любом случае, даже если пользователь отменил запуск с правами администратора в окне UAC)
            }
            else //имеем права администратора, значит, стартуем
            {
                logger.Error("Main() UpdateForAdmin if (hasAdministrativeRight != false)");
                UpdateRun();
            }
        }

        public static void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                logger.Debug("AddUpdateAppSettings ошибка: ConfigurationErrorsException");
                Console.WriteLine("Ошибка записи настроек");
            }
        }

        public static string ReadSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                string result = appSettings[key] ?? "Not Found";
                return result;
            }
            catch (ConfigurationErrorsException)
            {
                logger.Debug("ReadSetting ошибка: ConfigurationErrorsException");
                Console.WriteLine("Ошибка записи настроек");
                return null;
            }
        }

        public static int UpdateRun()
        {
            logger.Error("UpdateRun() UpdateForAdmin");
            string remoteUri = ReadSetting("remoteUri");
            string ListFiles = ReadSetting("ListFiles");

            try
            {
                //проверяем доступность сервера обновлений
                WebRequest webRequestAccessHttp = WebRequest.Create(remoteUri);
                webRequestAccessHttp.Method = "HEAD";
                try
                {
                    using (WebResponse webResponse = webRequestAccessHttp.GetResponse())
                    {
                        Console.WriteLine("Сервер обновлений доступен");
                    }
                }
                catch (WebException ex)
                {
                    Console.WriteLine("Сервер обновлений не доступен");
                    return 0;
                }

                //Выключаем все процессы клиентской программы
                string StartFile = ReadSetting("StartFile");// + ".exe";

                //копируем файл с информацией о составе программы
                string myStringWebResource = ReadSetting("remoteUri") + "/" + ListFiles;
                var client = new WebClient();
                client.DownloadFile(myStringWebResource, ListFiles);

                string str = "";
                foreach (string line in File.ReadLines(ListFiles))
                { str += line; }
                Response r = JsonConvert.DeserializeObject<Response>(str);

                //проверяем наличие файлов на сервере
                foreach (string lineFile in r.parameters)
                {
                    myStringWebResource = ReadSetting("remoteUri") + "/RemoteAdmin/" + lineFile;
                    logger.Debug("Updater: " + myStringWebResource.ToString());
                    WebRequest webRequest = WebRequest.Create(myStringWebResource);
                    webRequest.Method = "HEAD";
                    try
                    {
                        using (WebResponse webResponse = webRequest.GetResponse())
                        {
                            Console.WriteLine(lineFile + ": существует");
                        }
                    }
                    catch (WebException ex)
                    {
                        logger.Debug("При обновлении не удалось получить доступ к файлу на сервере. Ошибка: " + ex);
                        Console.WriteLine("Не существует: " + ex.Message);
                        return 0;
                    }
                }

                try
                {
                    Process[] pr = Process.GetProcessesByName(StartFile);
                    if (pr.Length != 0)
                    {
                        foreach (Process p in pr)
                        {
                            logger.Debug("НАчали закрывать процесс: ");
                            p.Kill();
                            logger.Debug("Закрыли процесс: ");
                            p.WaitForExit();
                        }
                    }
                }catch(Exception ex) { logger.Error(ex); }

                //Скачиваем необходимые файлы
                foreach (string lineFile in r.parameters)
                {
                    if (lineFile!="")
                    {
                        myStringWebResource = ReadSetting("remoteUri") + "/RemoteAdmin/" + lineFile;
                        logger.Debug("Updater: " + myStringWebResource.ToString());
                        client.DownloadFile(myStringWebResource, lineFile);
                        //client.DownloadFile(myStringWebResource, parentPath + "\\" + lineFile);
                    }
                }
                File.Delete(ListFiles);

                //выясняем новую версию client и updater
                //копируем файл с информацией о версии с сервера
                string FileithInfo = ReadSetting("remoteUri") + "/" + ReadSetting("fileNameWithVersion");
                logger.Debug("Updater: " + FileithInfo.ToString());
                client.DownloadFile(FileithInfo, ReadSetting("fileNameWithVersion"));
                string NewVersionClient = "";

                str = "";
                foreach (string line in File.ReadLines(ReadSetting("fileNameWithVersion")))
                { str += line; }
                r = JsonConvert.DeserializeObject<Response>(str);

                foreach (string line in r.parameters)
                {
                    if (line.Contains("VersionAdmin"))
                    {
                        NewVersionClient = "";
                        for (int i = "VersionAdmin".Length + 1; i < line.Length; i++)
                        {
                            NewVersionClient += line[i];
                        }
                        break;
                    }
                }

                string ConfigFile = ReadSetting("ConfigFile");
                if (File.Exists(ConfigFile))
                {
                    List<string> l = new List<string>();
                    foreach (string lineFile in File.ReadLines(ConfigFile))
                    {
                        if (lineFile.Contains("thisVersion"))
                        {
                            l.Add("<add key=\"thisVersion\" value=\"" + NewVersionClient + "\" />\"");
                            continue;
                        }
                        l.Add(lineFile);
                    }

                    File.WriteAllLines(ConfigFile, l, Encoding.UTF8);
                }

                ProcessStartInfo procInfo = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = ReadSetting("StartFile") + ".exe"  //The file in that DIR.
                };
                logger.Debug("Updater: " + procInfo.FileName.ToString());
                procInfo.Verb = "runas";
                Process ProcessUpdate = Process.Start(procInfo);  //Start that process.
                Process.GetCurrentProcess().Kill();
                return 1;
            }
            catch (Exception exc) { logger.Error("" + exc); return 0; }
            return 1;
        }

        public static string GetParent(string path)
        {
            try
            {
                DirectoryInfo directoryInfo = Directory.GetParent(path);
                return directoryInfo.ToString();
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine("Path is a null reference.");
                return null;
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Path is an empty string, " +
                    "contains only white spaces, or " +
                    "contains invalid characters.");
                return null;
            }
        }
    }
}
