using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace EmoteDisplayer
{

    public class Emote
    {
        public string code;
        public string name;
        public int is_bttv;

        public Emote(string name, string code, int is_bttv)
        {
            this.code = code;
            this.name = name;
            this.is_bttv = is_bttv;
        }
    }

    public partial class Form1 : Form
    {
        private String[] emotes = new String[10];
        private IrcClient client;
        private Image[] images = new Image[10];

        public Form1()
        {

            InitializeComponent();
            for (int i = 0; i < emotes.Length; i++)
            {
                emotes[i] = @"https://cdn.betterttv.net/emote/555015b77676617e17dd2e8e/1x";
            }



            backgroundWorker.RunWorkerAsync();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<Emote> db_emotes = new List<Emote>();
            client = new IrcClient("irc.twitch.tv", 6667, "420blazeitdawg", "oauth:grd7uyutlfk7ule3u4vw66obmls6ma");
            client.joinRoom("420blazeitdawg");
            for (int i = 0; i < images.Length; i++)
            {
                images[i] = Image.FromFile(@"C:\Users\pc1.0\git\ruwbot\img\emotes\Kappa.png");
            }
            string lastID = "gqwigqwbngilweufnwelg";

            SQLiteConnection db = new SQLiteConnection(@"Data Source=db/emotes.db;Version=3;New=False;Compress=True;");
            db.Open();
            SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM emotes", db);
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
                db_emotes.Add(new Emote((string)reader["name"], (string)reader["id"], (int)reader["is_bttv"]));
            db.Close();


            while (true)
            {
                string message = client.readMessage();
                if (message == null) continue;
                string[] userMessage = message.Split(':');

                try
                {
                    foreach (Emote em in db_emotes)
                    {
                        if(em.name == userMessage[userMessage.Length - 1])
                        {
                            lastID = userMessage[userMessage.Length - 2];
                            emotes[9] = emotes[8];
                            emotes[8] = emotes[7];
                            emotes[7] = emotes[6];
                            emotes[6] = emotes[5];
                            emotes[5] = emotes[4];
                            emotes[4] = emotes[3];
                            emotes[3] = emotes[2];
                            emotes[2] = emotes[1];
                            emotes[1] = emotes[0];
                            if(em.is_bttv == 1){
                                emotes[0] = @"https://cdn.betterttv.net/emote/" + em.code + "/1x";
                            }else{
                                emotes[0] = @"https://static-cdn.jtvnw.net/emoticons/v1/" + em.code + "/3.0";
                            }
                            backgroundWorker.ReportProgress(0, null);
                            Console.WriteLine("> " + userMessage[userMessage.Length - 1]);
                        }
                    }
                    Console.WriteLine("> " + userMessage[userMessage.Length - 1]);
                }
                catch (Exception ex) { }
                Thread.Sleep(10);

            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            for (int ix = Controls.Count - 1; ix >= 0; --ix) Controls[ix].Dispose();
            for (int i = 0; i < emotes.Length; i++)
            {
                PictureBox picturebox = new PictureBox();
                picturebox.Location = new Point(0, i * 48);
                picturebox.SizeMode = PictureBoxSizeMode.StretchImage;

                picturebox.ImageLocation = emotes[i];
                string imageUrl = emotes[i];
                WebRequest request = WebRequest.Create(imageUrl);
                WebResponse response = request.GetResponse();
                Image image = Image.FromStream(response.GetResponseStream());

                double divisor = image.Height / 48.0;
                double x = image.Width / divisor;
                double y = image.Height / divisor;
                
                picturebox.Size = new Size((int) x, (int) y);
                this.Controls.Add(picturebox);
            }
        }
    }
}
