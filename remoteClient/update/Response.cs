using System.Collections.Generic;

namespace update
{
    public class Response//то что мы получаем от клиента
    {
        public string command { get; set; }
        public List<string> parameters { get; set; }

        public override string ToString()
        {
            string param = "";
            foreach (string parameter in parameters)
            {
                param += parameter + ", ";
            }
            return "Command:" + command + " Parameters: " + param + ".";
        }
    }
}
