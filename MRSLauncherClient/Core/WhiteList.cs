using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MRSLauncherClient
{
    public class WhiteListLoader
    {
        public static WhiteList[] GetWhiteList(string id)
        {
            var query = new NameValueCollection();
            query.Add("name" , id);

            var res = Web.Request(Launcher.WhiteListUrl, Encoding.UTF8, query);
            var json = JArray.Parse(res);

            var whitelist = new WhiteList[json.Count];
            for (int i = 0; i < json.Count; i++)
            {
                whitelist[i] = json[i].ToObject<WhiteList>();
            }

            return whitelist;
        }
    }

    public class WhiteList
    {

    }
}
