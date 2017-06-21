using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RA
{
    class Packet
    {
        public string rawPacket { get; set; }
        public string Header { get; set; }
        public Dictionary<string, string> Commands = new Dictionary<string, string> { };

        public Packet(string header, Dictionary<string, string> commands)
        {
            rawPacket += Convert.ToBase64String(Encoding.UTF8.GetBytes(header)) + ":";

            foreach (KeyValuePair<string, string> entry in commands)
            {
                rawPacket += Convert.ToBase64String(Encoding.UTF8.GetBytes(entry.Key)) + "|";
                rawPacket += Convert.ToBase64String(Encoding.UTF8.GetBytes(entry.Value)) + "~";
            }

            Encrypt();
        }

        public Packet(string encPacket)
        {
            encPacket = new RNCryptor.Decryptor().Decrypt(encPacket, Environment.UserName);
            string[] broken = encPacket.Split(':');
            Header = Encoding.UTF8.GetString(Convert.FromBase64String(broken[0]));
            string[] brokenCommands = broken[1].Split('~');

            foreach (string command in brokenCommands)
            {
                if (command != "")
                {
                    string[] c = command.Split('|');
                    Commands.Add(Encoding.UTF8.GetString(Convert.FromBase64String(c[0])), Encoding.UTF8.GetString(Convert.FromBase64String(c[1])));
                }
            }
        }

        public string ePacket()
        {
            return Encrypt();
        }

        public string Encrypt()
        {
            return new RNCryptor.Encryptor().Encrypt(rawPacket, Environment.UserName);
        }
    }
}
