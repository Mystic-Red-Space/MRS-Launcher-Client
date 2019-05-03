using System.IO;

namespace MRSLauncherClient.Core
{
    class GameOption
    {
        public GameOption(string dir)
        {
            OptionPath = dir + "\\options.txt";
        }

        public string OptionPath { get; private set; }

        public string GetOption(string searchKey)
        {
            if (!File.Exists(OptionPath))
                return null;

            var file = File.ReadAllLines(OptionPath);
            foreach (var item in file)
            {
                var spl = item.Split(':');
                var key = spl[0];
                var value = spl[1];

                if (key == searchKey)
                    return value;
            }

            return null;
        }

        public void SetOption(string setKey, string setValue)
        {
            if (!File.Exists(OptionPath))
            {
                var content = KeyValue(setKey, setValue);
                File.WriteAllText(OptionPath,content);
                return;
            }



        }

        private string KeyValue(string key, string value)
        {
            return key + ":" + value + "\n";
        }
    }
}
