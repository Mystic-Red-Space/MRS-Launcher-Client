using System.IO;
using System.Net;
using System.Text;

namespace MRSLauncherClient
{
    class Web // 웹 요청 관련 클래스
    {
        public static string Request(string url) // HTTP GET (UTF8)
        {
            return Request(url, Encoding.UTF8);
        }

        public static string Request(string url, Encoding enc) // HTTP GET
        {
            using (var wc = new WebClient())
            {
                var data = wc.DownloadData(url);
                return enc.GetString(data);
            }
        }

        public static string Request(string url, Encoding enc, string post) // HTTP POST
        {
            var req = WebRequest.CreateHttp(url);

            var reqStream = req.GetRequestStream();
            var reqData = enc.GetBytes(post);
            reqStream.Write(reqData, 0, reqData.Length);
            reqStream.Dispose();

            var res = req.GetResponse();
            var resStream = new StreamReader(res.GetResponseStream(), enc);
            var resData = resStream.ReadToEnd();

            return resData;
        }
    }
}
