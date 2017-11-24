using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace ImageGetter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();


            //HttpWebRequestを作成
            System.Net.HttpWebRequest webreq =
                (System.Net.HttpWebRequest)
                    System.Net.WebRequest.Create("https://matome.naver.jp/odai/2141253870948308001");

            //サーバーからの応答を受信するためのHttpWebResponseを取得
            System.Net.HttpWebResponse webres =
                (System.Net.HttpWebResponse)webreq.GetResponse();

            //応答データを受信するためのStreamを取得
            System.IO.Stream st = webres.GetResponseStream();
            //文字コードを指定して、StreamReaderを作成
            System.IO.StreamReader sr =
                new System.IO.StreamReader(st, System.Text.Encoding.UTF8);
            //データをすべて受信
            string htmlSource = sr.ReadToEnd();
            //閉じる
            sr.Close();
            st.Close();
            webres.Close();

            //取得したソースを表示する
            List<string> imageUrl = new List<string>();
            htmlSource= htmlSource.Trim('\n');
            var matches=Regex.Matches(htmlSource, "<img src=\"[^\"]*\"");
            foreach (Match reg in matches) {
                var str = reg.Value;
                imageUrl.Add( str.Substring(10,str.Length-1-10));
            }

            matches = Regex.Matches(htmlSource, "image: url([^)]*)");
            foreach (Match reg in matches)
            {
                var str = reg.Value;
                imageUrl.Add(str.Substring(11, str.Length - 1 - 10));
            }

            SetImage(imageUrl.ToArray());

        }




        public void SetImage(string[] urlList)
        {
            int width = 300;
            int height = 300;
            int y = 0;
            foreach (var url in urlList)
            {
                var image = UrlToImage.GetImage(url);
                if (image == null) continue;
                var canvas = new Bitmap(width, height);
                Graphics g = Graphics.FromImage(canvas);
                g.InterpolationMode = InterpolationMode.Default;
                g.DrawImage(image,0,0,width,height);
                var pictureBox = new PictureBox();
                pictureBox.Location = new Point(0,y);
                pictureBox.Size =new Size( width,height);
                pictureBox.Image = canvas;
                y += height;
                panel1.Controls.Add(pictureBox);
            }
        }


    }
}
