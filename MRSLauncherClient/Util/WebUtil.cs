using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

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

        public static string Request(string url, Encoding enc, NameValueCollection q) // HTTP GET QUERY
        {
            var query = ToQuery(q);
            var req = WebRequest.CreateHttp(url + query);

            var res = req.GetResponse();
            var resStream = new StreamReader(res.GetResponseStream(), enc);
            var resData = resStream.ReadToEnd();

            return resData;
        }

        private static string ToQuery(NameValueCollection c)
        {
            var arr = from key in c.AllKeys
                      from value in c.GetValues(key)
                      select string.Format("{0}={1}", key, value); // LINQ

            var str = string.Join("&", arr.ToArray());

            return "?" + str;
        }
    }
}
