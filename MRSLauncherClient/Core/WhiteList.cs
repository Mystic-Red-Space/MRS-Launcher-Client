using System.Text;
using System.Collections.Specialized;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MRSLauncherClient.Core
{
    public class WhiteListLoader
    {
        private static ILog log = LogManager.GetLogger("WhiteListLoader");

        public static WhiteList GetWhiteList(string id)
        {
            log.Info("Get WhiteList : " + id);

            var query = new NameValueCollection();
            query.Add("name" , id);

            var res = Web.Request(Launcher.WhiteListUrl, Encoding.UTF8, query);
            var json = JArray.Parse(res);

            var whitelist = new WhiteListFile[json.Count];
            for (int i = 0; i < json.Count; i++)
            {
                whitelist[i] = json[i].ToObject<WhiteListFile>();
            }

            return new WhiteList(res, whitelist);
        }
    }

    public class WhiteList
    {
        public WhiteList(string rawresponse, WhiteListFile[] files)
        {
            RawResponse = rawresponse;
            Files = files;
        }

        public string RawResponse { get; private set; }
        public WhiteListFile[] Files { get; private set; }
    }

    public class WhiteListFile
    {
        [JsonProperty("dir")]
        public bool IsDirectory { get; private set; }

        [JsonProperty("md5")]
        public string Hash { get; private set; }

        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("path")]
        public string Path { get; private set; }
    }
}
