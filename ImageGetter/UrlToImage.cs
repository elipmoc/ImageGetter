using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ImageGetter
{
    class UrlToImage
    {
        static public Bitmap GetImage(string url)
        {
            System.Net.HttpWebRequest webreq;
            //HttpWebRequestを作成
            try
            {

                 webreq=
                    (System.Net.HttpWebRequest)
                        System.Net.WebRequest.Create(url);
            }
            catch(Exception e)
            {
                return null;
            }

            //サーバーからの応答を受信するためのHttpWebResponseを取得
            System.Net.HttpWebResponse webres =
                (System.Net.HttpWebResponse)webreq.GetResponse();
            //または、
            //System.Net.WebResponse webres = webreq.GetResponse();

            //応答データを受信するためのStreamを取得
            //バイナリデータに変換
            var br = new System.IO.BinaryReader(webres.GetResponseStream());

            int buffSize = 65536; // 一度に読み込むサイズ
            var imgStream = new System.IO.MemoryStream();

            while (true)
            {
                byte[] buff = new byte[buffSize];

                // 応答データの取得
                int readBytes = br.Read(buff, 0, buffSize);
                if (readBytes <= 0)
                {
                    // 最後まで取得した->ループを抜ける
                    break;
                }

                // バッファに追加
                imgStream.Write(buff, 0, readBytes);
            }

            br.Close();
            webres.Close();
            return new Bitmap(imgStream);
        }

    }
}
