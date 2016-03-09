using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EmoteDisplayer
{
    class IrcClient
    {
        private string password;
        private string username;
        private string currentRoom;

        private TcpClient tcpClient;
        private StreamReader inputStream;
        private StreamWriter outputStream;

        public IrcClient(string ip, int port, string username, string password)
        {
            this.username = username;
            this.password = password;

            tcpClient = new TcpClient(ip, port);
            inputStream = new StreamReader(tcpClient.GetStream());
            outputStream = new StreamWriter(tcpClient.GetStream());

            outputStream.WriteLine("PASS " + password);
            outputStream.WriteLine("NICK " + username);
            outputStream.WriteLine("USER " + username + " 8 * :" + username);
            outputStream.Flush();

        }

        public void joinRoom(string room)
        {
            this.currentRoom = room;
            outputStream.WriteLine("JOIN #" + room);
            outputStream.Flush();
        }

        public void sendRaw(string message)
        {
            outputStream.WriteLine(message);
            outputStream.Flush();
        }

        public void sendMessage(string message)
        {
            sendRaw(":" + username + "!" + username + "@" + username + ".tmi.twitch.tv PRIVMSG #" + currentRoom + " :" + message);
        }

        public string readMessage()
        {
            string message = inputStream.ReadLine();
            return message;
        }


    }
}
