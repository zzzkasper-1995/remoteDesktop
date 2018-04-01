using System;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Security.Cryptography;
using Newtonsoft.Json.Converters;
using System.Dynamic;
using NLog;
using System.Collections;

public static class RepServer //основной класс сервера репитера
{
    public static int ECHO_PORT = 34001; //порт к который слушаем
    public static List<ClientHandler> ListClientHandlers=new List<ClientHandler>(); //список клиентов
    public static List<Line> ListLines = new List<Line>();//список текущих соединений между лаунчером и Приложением мастреа (сервером)
    public static Logger logger = LogManager.GetCurrentClassLogger();

    public static void Main(string[] arg)
    {
        try
        {
            Console.WriteLine("I'm runing");
            logger.Debug("Main()");
            // Связываем сервер с локальным портом
            TcpListener clientListener = new TcpListener(ECHO_PORT);

            // Начинаем слушать
            clientListener.Start();
            logger.Debug("Main(): Начинаем слушать порт");

            while (true)
            {
                try
                {
                    //Даем согласие на соединение
                    TcpClient client = clientListener.AcceptTcpClient();

                    ClientHandler cHandler = new ClientHandler
                    {

                        //Передаем значение объекту ClientHandler
                        clientSocket = client
                    };

                    //Создаем новый поток для клиента
                    Thread clientThread = new Thread(new ThreadStart(cHandler.RunClient));
                    clientThread.Start();
                }
                catch(Exception exc)
                {
                    logger.Error("Main() ошибка: " + exc);
                }
            }
            clientListener.Stop();
        }
        catch (Exception exc)
        {
            logger.Error("Main() ошибка: "+ exc);
            Console.WriteLine("Exception: " + exc);
        }
    }
}

public class ClientHandler//класс отвечающий за работу с клиентами репитера
{
    public bool TypeUserIsAdmin = false;//данный клиент сервер или лаунчер
    public TcpClient clientSocket;
    public StreamReader readerStream = null;
    public NetworkStream networkStream = null;

    //информация о пользователе 
    public string OSVersion;
    public string PhysicalMemory;
    public string ProcName;
    public string Mac;
    public string ID = "";
    public string Pas = "";
    public string PasEncrypted;
    public string name;
    public string lastName;
    public string phone;
    public bool Auth = false;

    public void RunClient()
    {
        RepServer.logger.Debug("RunClient() ");

        // Создаем классы потоков
        try
        {
            readerStream = new StreamReader(clientSocket.GetStream());
            networkStream = clientSocket.GetStream();

            Response r = null;

            while (true)
            {
                string returnData = readerStream.ReadLine();//считываем данные от клиента
                string Repeatdate = returnData;
                returnData = Decrypt(returnData, "1234");

                if (TypeUserIsAdmin)//если клиент сервер то работаем с ним как с сервером
                {
                    if (returnData != "")
                    {
                        r = JsonConvert.DeserializeObject<Response>(returnData);//десериализуем его
                        if (r.command == "Exit")//пришел запрос на разрыв соединения
                        {
                            r.parameters.Add(this.ID);
                            WorkWithResponse(r);
                            break;
                        }

                        if (r.command == "Find")
                        {
                            WorkWithResponse(r);
                            continue;
                        }

                        if (r.command == "GetListClients")
                        {
                            WorkWithResponse(r);
                            continue;
                        }

                        if (r.command == "KillConnectAdmin")
                        {
                            WorkWithResponse(r);
                            continue;
                        }

                        Line LineFind = RepServer.ListLines.Find(x => x.server.Equals(this));

                        if (LineFind != null)//если мастер уже связан с другим клиентом то пересылаем сообщение от мастера к клиенту
                        {
                            ClientHandler remoteClient = RepServer.ListClientHandlers.Find(x => x.ID.Equals(LineFind.client.ID));
                            if (remoteClient == null)
                            {
                                RepServer.ListLines.Remove(LineFind);
                                Request req = new Request("Error");
                                req.Send(this);
                                continue;
                            }

                            switch (r.command)
                            {
                                case ("Connect"):
                                    {
                                        if (r.parameters.Count > 0)
                                        {
                                            if (LineFind.client.ID != r.parameters[0])
                                            {
                                                RepServer.ListLines.Remove(LineFind);
                                                WorkWithResponse(r);
                                            }
                                            else
                                            {
                                                Request req = new Request("Connect");
                                                ClientHandler cHandler = RepServer.ListClientHandlers.Find(x => x.ID.Equals(r.parameters[0]));
                                                if (cHandler != null)
                                                {
                                                    req.parameters.Add("yes");
                                                }
                                                else
                                                {
                                                    req.parameters.Add("no");
                                                    Console.WriteLine("Клиент не найден");
                                                }
                                                req.Send(this);
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            Request req = new Request("Connect");
                                            req.parameters.Add("no");
                                            Console.WriteLine("Клиент не найден");
                                            req.Send(this);
                                        }
                                        break;
                                    }
                                case ("RunVNC"):
                                    {
                                        if (remoteClient != null)
                                        {
                                            Request req = new Request("RunVNC");
                                            req.parameters.Add(name);
                                            req.parameters.Add(lastName);
                                            req.parameters.Add(phone);
                                            req.Send(remoteClient);
                                            continue;
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        if (remoteClient != null)
                                        {
                                            Repeatdate += "\r\n";
                                            byte[] dataWrite = Encoding.UTF8.GetBytes(Repeatdate);
                                            remoteClient.networkStream.Write(dataWrite, 0, dataWrite.Length);
                                        }
                                        break;
                                    }
                            }
                            try
                            {
                                remoteClient.networkStream.Write(Encoding.UTF8.GetBytes(" "), 0, 1);
                            }
                            catch
                            {
                                RepServer.ListLines.Remove(LineFind);
                            }
                        }
                        else//если клиент не связан с другим клиентом значит он обращается к репитеру и мы обрабатываем его запрос сами
                        {
                            WorkWithResponse(r);
                        }
                    }
                }
                else//если клиент оказался лаунчером
                {
                    if (returnData != "")
                    {
                        r = JsonConvert.DeserializeObject<Response>(returnData);
                        if (r.command == "Exit")
                        {
                            r.parameters.Add(this.ID);
                            WorkWithResponse(r);//отправляем полученный запрос на обработку
                            break;
                        }
                        if (r.command == "isConnectAdmin")
                        {
                            WorkWithResponse(r);//отправляем полученный запрос на обработку
                            continue;
                        }

                        Line LineFind = RepServer.ListLines.Find(x => x.client.Equals(this));
                        if (LineFind != null)
                        {
                            ClientHandler remoteClient = RepServer.ListClientHandlers.Find(x => x.ID.Equals(LineFind.server.ID));
                            Repeatdate += "\r\n";
                            byte[] dataWrite = Encoding.UTF8.GetBytes(Repeatdate);
                            remoteClient.networkStream.Write(dataWrite, 0, dataWrite.Length);
                        }
                        else
                        {
                            Console.WriteLine(DateTime.Now.ToString() + returnData);
                            WorkWithResponse(r);
                        }
                    }
                }
            }
        }
        catch (Exception exc)
        {
            RepServer.logger.Error("RunClient() ошибка: " + exc);
            //RunClient();
        }
        finally
        {
            clientSocket.Close();
            RepServer.ListClientHandlers.Remove(this);
        }
    }

    public void WorkWithResponse(Response r)//обрабатываем запросы от клиента к серверу
    {
        RepServer.logger.Debug("WorkWithResponse() " + r.ToString());
        switch (r.command)
        {
            case ("Find"):
                {
                    Request reqToServ = new Request("Find");

                    if (TypeUserIsAdmin)
                    {
                        if (r.parameters.Count > 0)
                        {
                            ClientHandler remoteClient = RepServer.ListClientHandlers.Find(x => x.ID.Equals(r.parameters[0]));
                            if (remoteClient != null)
                            {
                                reqToServ.parameters.Add("yes");
                                reqToServ.parameters.Add(r.parameters[0]);
                                Console.WriteLine("Клиент, которого ищет администратор, найден");
                            }
                            else
                            {
                                reqToServ.parameters.Add("no");
                                reqToServ.parameters.Add(r.parameters[0]);
                                Console.WriteLine("Клиент, которого ищет администратор, не найден");
                            }
                        }
                        else
                        {
                            reqToServ.parameters.Add("no");
                            reqToServ.parameters.Add(r.parameters[0]);
                            Console.WriteLine("Клиент, которого ищет администратор, не найден");
                        }
                    }
                    else
                    {
                        reqToServ.parameters.Add("requires more authority");
                    }
                    reqToServ.Send(this);
                    break;
                }
            case ("GetListClients"):
                {
                    Request reqToServ = new Request("GetListClients");

                    if (TypeUserIsAdmin)
                    {
                        List<string> ListClientsID = new List<string>();
                        foreach (ClientHandler c in RepServer.ListClientHandlers)
                        {
                            if (!c.TypeUserIsAdmin) ListClientsID.Add(c.ID);
                        }

                        string ListClientsJSON = JsonConvert.SerializeObject(ListClientsID);
                        reqToServ.parameters.Add(ListClientsJSON);
                    }
                    else
                    {
                        reqToServ.parameters.Add("requires more authority");
                    }
                    reqToServ.Send(this);
                    break;
                }
            case ("Connect"):
                {
                    Request req = new Request("Connect");
                    if (TypeUserIsAdmin)
                    {
                        if (r.parameters.Count > 0)
                        {
                            Line findLineServ = RepServer.ListLines.Find(x => (x.server.Equals(this)));  //смотрим есть ли уже линия с данным клиентом
                            if (findLineServ != null)//если есть линия с клиентом
                            {
                                RepServer.ListLines.Remove(findLineServ);
                            }

                            ClientHandler remoteClient = RepServer.ListClientHandlers.Find(x => x.ID.Equals(r.parameters[0]));

                            if (remoteClient != null)//проверяем есть ли искомый клиент, потмоу что если его нет то работать дальше мы не сможем
                            {
                                Line findLine = RepServer.ListLines.Find(x => (x.client.Equals(remoteClient)));  //смотрим есть ли уже линия с данным клиентом
                                if (findLine != null)//если есть линия с клиентом
                                {
                                    findLine = RepServer.ListLines.Find(x => (x.client.Equals(remoteClient) && x.server.Equals(this))); //проверяем есть ли линия объединяющая нашего клиента и нашего мастера
                                    if (findLine != null)
                                    {
                                        req.parameters.Add("yes");
                                        Console.WriteLine("Сервер связался с клиентом");
                                    }
                                    else//если если линия с клиентом есть а линии с текущим клиентом и текущим мастером нет, то значит клиент подключен к другому мастреу
                                    {
                                        req.parameters.Add("no");
                                        Console.WriteLine("выбранный клиент (ID:" + r.parameters[0] + ") уже к кому то подключен");
                                    }
                                }
                                else//если линии нет то создаем новую линию
                                {
                                    RepServer.ListLines.Add(new Line(remoteClient, this));
                                    req.parameters.Add("yes");
                                    Console.WriteLine("Сервер связался с клиентом");
                                }
                            }
                            else
                            {
                                req.parameters.Add("no");
                                Console.WriteLine("Сервер не смог связаться с клиентом, клиент не был найден");
                            }
                        }
                        else
                        {
                            req.parameters.Add("no");
                            Console.WriteLine("В запросе было недостаточно параметров для его обработки");
                        }
                    }
                    else
                    {
                        req.parameters.Add("requires more authority");
                    }
                    req.Send(this);
                    break;
                }
            case ("Exit"):
                {
                    if (r.parameters.Count > 0)
                    {
                        ClientHandler remoteClient = RepServer.ListClientHandlers.Find(x => x.ID.Equals(r.parameters[0]));
                        if (remoteClient != null)
                        {
                            RepServer.ListClientHandlers.Remove(remoteClient);
                            Line Line = null;
                            if (remoteClient.TypeUserIsAdmin)
                            {
                                Line = RepServer.ListLines.Find(x => x.server.Equals(remoteClient));
                            }
                            else
                            {
                                Line = RepServer.ListLines.Find(x => x.server.Equals(remoteClient));
                            }
                            if (Line != null) { RepServer.ListLines.Remove(Line); }

                            Console.WriteLine("Клиент с ID:{0} отключился от репитера", remoteClient.ID);
                        }
                    }

                    break;
                }
            case ("GetSystemInfo"):
                {
                    if (r.parameters.Count == 1)
                    {
                        Request req = new Request("GetSystemInfo");
                        if (TypeUserIsAdmin)
                        {
                            ClientHandler remoteClient = RepServer.ListClientHandlers.Find(x => x.ID.Equals(r.parameters[0]));
                            if (remoteClient != null)
                            {

                                if (remoteClient.OSVersion != null) req.parameters.Add(remoteClient.OSVersion.ToString());
                                else req.parameters.Add("Unknow");

                                if (remoteClient.ProcName != null) req.parameters.Add(remoteClient.ProcName.ToString());
                                else req.parameters.Add("Unknow");

                                if (remoteClient.PhysicalMemory != null) req.parameters.Add(remoteClient.PhysicalMemory.ToString());
                                else req.parameters.Add("Unknow");

                                if (remoteClient.Mac != "") req.parameters.Add(remoteClient.Mac);
                                else req.parameters.Add("Unknow");

                                if (remoteClient.ID != "") req.parameters.Add(remoteClient.ID);
                                else req.parameters.Add("Unknow");
                            }
                            else
                            {
                                req.parameters.Add("requires more authority");
                            }
                            req.Send(this);
                        }
                    }
                    else
                    {
                        try
                        {
                            OSVersion = r.parameters[0];
                            ProcName = r.parameters[1];
                            long NumberMem = Convert.ToInt64(r.parameters[2]) / 1024 / 1024;
                            PhysicalMemory = NumberMem.ToString() + " МБ";
                            Mac = r.parameters[3];
                            ID = r.parameters[4];
                        }
                        catch
                        {

                        }
                    }
                    break;
                }
            case ("FindNewID"):
                {
                    Request reqToServ = new Request("FindNewID");

                    Bitrix24 bx_logon = new Bitrix24("zzz.kasper.zzz.1995@gmail.com", "123456Qq");

                    if (r.parameters.Count > 0)
                    {
                        try
                        {
                            string TaskListByJSON = bx_logon.SendCommand("crm.lead.get", "ID=" + r.parameters[0]);
                            var converter = new ExpandoObjectConverter();
                            dynamic res = JsonConvert.DeserializeObject<ExpandoObject>(TaskListByJSON, converter);

                            if (res != null)
                            {
                                if (res.result.ID != "")
                                {
                                    reqToServ.parameters.Add("Ok");
                                }
                                else
                                {
                                    reqToServ.parameters.Add("Bad");
                                }
                            }
                            else
                            {
                                reqToServ.parameters.Add("Bad");
                            }
                        }
                        catch (Exception exc)
                        {
                            RepServer.logger.Debug("WorkWithResponse() ошибка:" + exc);
                            reqToServ.parameters.Add("Bad");
                        }
                    }
                    else
                    {
                        RepServer.logger.Debug("WorkWithResponse() ошибка: недостаточно параметров в запросе");
                        reqToServ.parameters.Add("Bad");
                    }

                    reqToServ.Send(this);
                    break;
                }
            case ("Hello"):
                {
                    do
                    {
                        if (r.command == "Hello")
                        {
                            Request reqToServ = new Request("Hello");
                            ClientHandler cHandler = RepServer.ListClientHandlers.Find(x => x.Equals(this));

                            if (r.parameters.Count == 1)//клиент здаровается с нами, у лаунчера есть только ID у сервера еще и пароль для авторизации
                            {
                                if (cHandler == null)
                                {
                                    ID = r.parameters[0];
                                    TypeUserIsAdmin = false;
                                    try
                                    {
                                        Bitrix24 bx_logon = new Bitrix24("zzz.kasper.zzz.1995@gmail.com", "123456Qq");

                                        string TaskListByJSON = bx_logon.SendCommand("crm.lead.get", "ID=" + ID);
                                        var converter = new ExpandoObjectConverter();
                                        dynamic res = JsonConvert.DeserializeObject<ExpandoObject>(TaskListByJSON, converter);

                                        if (res.result.ID == ID)
                                        {
                                            reqToServ.parameters.Add("Ok");
                                            Auth = true;
                                            Console.WriteLine("Welcome " + ID + "(Client) to the Server");


                                            Request reqTolauncher = new Request("GetSystemInfo");
                                            reqTolauncher.Send(this);
                                        }
                                        else
                                        {
                                            reqToServ.parameters.Add("Bad");
                                            Auth = false;
                                            Console.WriteLine("Welcome unknow (Client) to the Server");
                                        }

                                    }
                                    catch (Exception exc)
                                    {
                                        RepServer.logger.Debug("WorkWithResponse() ошибка:" + exc);
                                        reqToServ.parameters.Add("Bad");
                                        Auth = false;
                                        Console.WriteLine("Welcome unknow (Client) to the Server");
                                    }
                                }
                                else
                                {
                                    reqToServ.parameters.Add("Ok");
                                    Auth = true;
                                }
                            }
                            else
                            {
                                if (r.parameters.Count == 2)//клиент здаровается с нами, у лаунчера есть только ID у сервера еще и пароль для авторизации
                                {
                                    if (cHandler == null)
                                    {
                                        ID = r.parameters[0];
                                        Pas = r.parameters[1];
                                        try
                                        {
                                            Bitrix24 bx_logon = new Bitrix24(ID, Pas);//если комбинация логин пароль в системе найдена значит все гуд, иначе программа уйдет в ошибку (try/catch)
                                            string TaskListByJSON = bx_logon.SendCommand("user.current");
                                            var converter = new ExpandoObjectConverter();
                                            dynamic res = JsonConvert.DeserializeObject<ExpandoObject>(TaskListByJSON, converter);
                                            name = res.result.NAME;
                                            lastName = res.result.LAST_NAME;
                                            try
                                            {
                                                if (res.result.PHONE[0].VALUE == "" || res.result.PHONE[0].VALUE == null) phone = "";
                                                else phone = res.result.PHONE[0].VALUE;
                                            }
                                            catch (Exception exp) { phone = ""; }

                                            reqToServ.parameters.Add("Ok");
                                            Auth = true;
                                            TypeUserIsAdmin = true;
                                            Console.WriteLine("Welcome " + ID + "(Admin) to the Server");
                                        }
                                        catch (Exception exc)
                                        {
                                            RepServer.logger.Debug("WorkWithResponse() ошибка:" + exc);
                                            reqToServ.parameters.Add("Bad");
                                            Auth = false;
                                            TypeUserIsAdmin = false;
                                            Console.WriteLine("Welcome unknow (Client) to the Server");
                                        }
                                    }
                                    else
                                    {
                                        reqToServ.parameters.Add("Ok");
                                        Auth = true;
                                    }
                                }
                                else break;
                            }

                            reqToServ.Send(this);
                        }
                    } while (r.command != "Hello");

                    if (Auth)
                    {
                        if (r.parameters.Count > 0)
                        {
                            ClientHandler cHandler = RepServer.ListClientHandlers.Find(x => x.ID.Equals(r.parameters[0]));
                            if (cHandler == null)
                            {
                                RepServer.ListClientHandlers.Add(this);//добавляем клиента в список клиентов
                            }
                        }
                    }

                    break;
                }
            case ("isConnectAdmin"):
                {
                    Request req = new Request("isConnectAdmin");
                    Line LineFind = RepServer.ListLines.Find(x => x.client.Equals(this));
                    if (LineFind != null)
                    {
                        req.parameters.Add("Ok");
                    }
                    else { req.parameters.Add("No"); }

                    req.Send(this);
                    break;
                }
            case ("KillConnectAdmin"):
                {
                    Line LineFind = RepServer.ListLines.Find(x => x.server.Equals(this));
                    if (LineFind != null)
                    {
                        RepServer.ListLines.Remove(LineFind);
                    }
                    else { }
                    break;
                }
            default:
                {
                    Request req = new Request("What is it?");
                    req.Send(this);
                    break;
                }
        }
    }

    static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
    {
        RepServer.logger.Debug("DecryptStringFromBytes_Aes()");
        // Check arguments.
        if (cipherText == null || cipherText.Length <= 0)
            throw new ArgumentNullException("cipherText");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("IV");

        // Declare the string used to hold
        // the decrypted text.
        string plaintext = null;

        // Create an Aes object
        // with the specified key and IV.
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            // Create a decrytor to perform the stream transform.
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for decryption.
            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        // Read the decrypted bytes from the decrypting stream
                        // and place them in a string.
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }

        }
        return plaintext;
    }

    //метод дешифрования строки
    public static string Decrypt(string ciphText, string pass,
           string sol = "doberman", string cryptographicAlgorithm = "SHA1",
           int passIter = 2, string initVec = "a8doSuDitOz1hZe#",
           int keySize = 256)
    {
        if (string.IsNullOrEmpty(ciphText))
            return "";

        byte[] initVecB = Encoding.ASCII.GetBytes(initVec);
        byte[] solB = Encoding.ASCII.GetBytes(sol);
        byte[] cipherTextBytes = Convert.FromBase64String(ciphText);

        PasswordDeriveBytes derivPass = new PasswordDeriveBytes(pass, solB, cryptographicAlgorithm, passIter);
        byte[] keyBytes = derivPass.GetBytes(keySize / 8);

        RijndaelManaged symmK = new RijndaelManaged
        {
            Mode = CipherMode.CBC
        };

        byte[] plainTextBytes = new byte[cipherTextBytes.Length];
        int byteCount = 0;

        using (ICryptoTransform decryptor = symmK.CreateDecryptor(keyBytes, initVecB))
        {
            using (MemoryStream mSt = new MemoryStream(cipherTextBytes))
            {
                using (CryptoStream cryptoStream = new CryptoStream(mSt, decryptor, CryptoStreamMode.Read))
                {
                    byteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                    mSt.Close();
                    cryptoStream.Close();
                }
            }
        }

        symmK.Clear();
        return Encoding.UTF8.GetString(plainTextBytes, 0, byteCount);
    }
}

public class Line//соеденительная линия между сервером и лаунчером
{
    public ClientHandler client;
    public ClientHandler server;

    public Line(ClientHandler c, ClientHandler s)
    {
        RepServer.logger.Debug("New Line()");
        client = c;
        server = s;
    }
}

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

    public void Send(ClientHandler remoteClient)
    {
        RepServer.logger.Debug("send()");
        if (remoteClient != null)
        {
            string jsonreq = JsonConvert.SerializeObject(this);

            string enc = Encrypt(jsonreq, "1234") + "\r\n";
            byte[] dataWrite = Encoding.UTF8.GetBytes(enc);
            try
            {
                remoteClient.networkStream.Write(dataWrite, 0, dataWrite.Length);
            }
            catch (ObjectDisposedException exc)
            {
                RepServer.logger.Error("send() ошибка:" + exc);
            }
        }
    }

    //метод шифрования строки
    public static string Encrypt(string ishText, string pass,
           string sol = "doberman", string cryptographicAlgorithm = "SHA1",
           int passIter = 2, string initVec = "a8doSuDitOz1hZe#",
           int keySize = 256)
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

public class Response//то что мы получаем от клиента
{
    public string command;
    public List<string> parameters;

    public override string ToString()
    {
        string param = "";
        foreach(string parameter in parameters)
        {
            param += parameter + ", ";
        }
        return "Command:" + command + " Parametrs: " + param + ".";
    }
}
