using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text;

namespace MRSLauncherClient
{
    public class ModPackLoader // 모드팩을 불러오는 클래스
    {
        // 모든 모드팩 리스트를 불러옴
        public static string[] GetModPackList()
        {
            var res = Web.Request(Launcher.ModPackListUrl);

            return JArray.Parse(res)
                   .Select(x => x.ToString())
                   .ToArray();
        }

        // 특정 모드팩의 정보를 가져옴
        public static ModPack GetModPack(string name)
        {
            var url = Launcher.ModPackDataUrl; // API 서버 요청할 데이터
            var req = new JObject();
            req.Add("name", name);

            var json = Web.Request(url, Encoding.UTF8, req.ToString()); // 응답 파싱
            var jarr = JArray.Parse(json);

            var modFiles = new ModFile[jarr.Count]; // JSON 배열 for
            for (int i = 0; i < jarr.Count; i++)
            {
                modFiles[i] = jarr[i].ToObject<ModFile>(); // 각 배열의 요소를 직렬화해서 저장
            }

            var modPack = new ModPack(name, modFiles); // 모드팩 객체 생성
            return modPack;
        }
    }

    public class ModPack // 모드팩 클래스
    {
        public ModPack(string name, ModFile[] modFiles)
        {
            this.Name = name;
            this.ModFiles = modFiles;
        }

        public string Name { get; private set; }
        public ModFile[] ModFiles { get; private set; }
    }

    public class ModFile // 모드 파일 클래스
    {
        [JsonProperty("name")]
        public string FileName { get; private set; }

        [JsonProperty("md5")]
        public string MD5 { get; private set; }

        [JsonProperty("dir")]
        public string Path { get; private set; }

        [JsonProperty("url")]
        public string Url { get; private set; }
    }
}
