using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using CmlLib.Launcher;

namespace MRSLauncherClient
{
    public class Setting // 각종 설정들을 JSON파일로 저장
    {
        #region Singleton
        private Setting() { }
        private static Setting instance;
        public static Setting Json
        {
            get
            {
                if (instance == null)
                    instance = LoadSetting();
                return instance;
            }
        }
        #endregion

        #region Setting IO

        private static Setting LoadSetting()
        {
            if (File.Exists(Launcher.SettingPath)) // 설정파일 있을때
            {
                var filecontent = File.ReadAllText(Launcher.SettingPath);

                var serializer = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore // JSON에 없는 값은 무시
                };

                var obj = JsonConvert.DeserializeObject<Setting>(filecontent, serializer); // 역직렬화
                return obj;
            }
            else
                return new Setting(); // 설정파일 없을때
        }

        public static void SaveSetting()
        {
            if (instance == null) return;

            Directory.CreateDirectory(Path.GetDirectoryName(Launcher.SettingPath)); // 폴더 생성

            var json = JsonConvert.SerializeObject(instance); // 직렬화 후 저장
            File.WriteAllText(Launcher.SettingPath, json);
        }

        #endregion

        [JsonProperty]
        public int MaxRamMb { get; set; } = 4096;

        [JsonProperty]
        public bool UseCustomJVM { get; set; } = false;

        [JsonProperty]
        public string CustomJVMArguments { get; set; } = "";

        [JsonProperty]
        public bool ShowLogWindow { get; set; } = false;
    }
}
