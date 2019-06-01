using System.IO;
using System.Collections.Generic;

namespace MRSLauncherClient.Core
{
    class GameOption
    {
        public GameOption(string dir)
        {
            OptionPath = dir + "\\options.txt";
        }

        public string OptionPath { get; private set; }

        public Dictionary<string, string> GetOption()
        {
            if (!File.Exists(OptionPath))
                return new Dictionary<string, string>();

            var dict = new Dictionary<string, string>();
            foreach (var item in File.ReadAllLines(OptionPath))
            {
                if (!item.Contains(":"))
                    continue;

                var spl = item.Split(':');
                dict.Add(spl[0], spl[1]);
            }

            return dict;
        }

        public void SetOption(Dictionary<string, string> dict)
        {
            using (var sr = new StreamWriter(OptionPath, false, System.Text.Encoding.ASCII))
            {
                sr.NewLine = "\r\n";
                foreach (var item in dict)
                {
                    sr.WriteLine(KeyValue(item.Key, item.Value));
                }
            }
        }

        private string KeyValue(string key, string value)
        {
            return key + ":" + value + "\n";
        }
    }
}
