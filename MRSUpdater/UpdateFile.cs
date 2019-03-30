using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MRSUpdater
{
    public class UpdateLoader
    {
        const string UpdateServerUrl = "https://api.mysticrs.tk/launcher";

        public static UpdateFile[] GetUpdateFiles()
        {
            var json = "";
            using (var wc = new WebClient())
            {
                json = wc.DownloadString(UpdateServerUrl);
            }

            var jarr = JArray.Parse(json);
            var files = new UpdateFile[jarr.Count];

            for (int i = 0; i < jarr.Count; i++)
            {
                files[i] = jarr[i].ToObject<UpdateFile>();
            }

            return files;
        }
    }

    public class UpdateFile
    {
        [JsonProperty("name")]
        public string FileName { get; private set; }

        [JsonProperty("md5")]
        public string MD5 { get; private set; }

        [JsonProperty("url")]
        public string Url { get; private set; }
    }
}
