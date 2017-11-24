using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageGetter
{
    class Server
    {

        static System.Text.Encoding enc = System.Text.Encoding.UTF8;

        static public string GetUrl(System.Net.Sockets.NetworkStream ns) {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            int resSize = 0;
            byte[] resBytes = new byte[256];
            do
            {
                //データの一部を受信する
                resSize = ns.Read(resBytes, 0, resBytes.Length);

                //Readが0を返した時はクライアントが切断したと判断
                if (resSize == 0)
                {
                    return null;
                }
                //受信したデータを蓄積する
                ms.Write(resBytes, 0, resSize);
                //まだ読み取れるデータがあるか、データの最後が\nでない時は、
                // 受信を続ける
            } while (ns.DataAvailable);

            

            ms.Close();

            //受信したデータを文字列に変換
            return enc.GetString(ms.GetBuffer(), 0, (int)ms.Length);

        }

        static public string[] GetUrlList()
        {
            //ListenするIPアドレス
            string ipString = "localhost";
            System.Net.IPAddress ipAdd = System.Net.IPAddress.Parse(ipString);
            //Listenするポート番号
            int port = 8080;

            //TcpListenerオブジェクトを作成する
            System.Net.Sockets.TcpListener listener =
                new System.Net.Sockets.TcpListener(ipAdd, port);

            //Listenを開始する
            listener.Start();

            //接続要求があったら受け入れる
            System.Net.Sockets.TcpClient client = listener.AcceptTcpClient();

            //NetworkStreamを取得
            System.Net.Sockets.NetworkStream ns = client.GetStream();

            //読み取り、書き込みのタイムアウトを10秒にする
            //デフォルトはInfiniteで、タイムアウトしない
            //(.NET Framework 2.0以上が必要)
            ns.ReadTimeout = 10000;
            ns.WriteTimeout = 10000;

            //クライアントから送られたデータを受信する
            bool disconnected = false;
            List<string> urlList=new List<string>();

            while (true)
            {
                string url = GetUrl(ns);
                if (url == null)
                {
                    disconnected = true;
                    break;
                }
                else
                {
                    urlList.Add(url);
                }
            }

            if (!disconnected)
            {
                //クライアントにデータを送信する
                //クライアントに送信する文字列を作成
                //文字列をByte型配列に変換
                byte[] sendBytes = enc.GetBytes("草生える" + '\n');
                //データを送信する
                ns.Write(sendBytes, 0, sendBytes.Length);
            }

            //閉じる
            ns.Close();
            client.Close();

            //リスナを閉じる
            listener.Stop();

            return urlList.ToArray();

        }
    }
}
