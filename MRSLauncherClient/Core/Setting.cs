using Newtonsoft.Json;
using System.IO;
using log4net;

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

        private static ILog log = LogManager.GetLogger("Setting");

        private static Setting LoadSetting()
        {
            log.Info("Get Setting File");

            if (File.Exists(Launcher.SettingPath)) // 설정파일 있을때
            {
                var filecontent = File.ReadAllText(Launcher.SettingPath);

                var serializer = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore // JSON에 없는 값은 무시
                };

                log.Info("Deserializing");
                var obj = JsonConvert.DeserializeObject<Setting>(filecontent, serializer); // 역직렬화
                return obj;
            }
            else
            {
                log.Info("Create new Empty Setting");
                return new Setting(); // 설정파일 없을때
            }
        }

        public static void SaveSetting()
        {
            if (instance == null) return;

            Directory.CreateDirectory(Path.GetDirectoryName(Launcher.SettingPath)); // 폴더 생성

            log.Info("Save Setitng");
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

        [JsonProperty]
        public bool HideLauncher { get; set; } = true;
    }
}
