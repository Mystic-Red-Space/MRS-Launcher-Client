using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MRSLauncherClient
{
    // 해쉬된 파일이 같은지 검사하는 클래스
    public class HashedFileEqualityCompaser : EqualityComparer<HashedFile>
    {
        // 파일이 같은지 확인
        public override bool Equals(HashedFile x, HashedFile y)
        {
            if (x.Path != y.Path) // 경로가 같은지 확인
                return false;

            if (File.Exists(x.Path) && File.Exists(y.Path)) // 파일이 존재하는지 확인
            {
                var xhash = x.Hash; // 해쉬가 같은지 확인
                if (x.Hash == "")
                    xhash = GetFileHash(x.Path);

                var yhash = y.Hash;
                if (y.Hash == "")
                    yhash = GetFileHash(yhash);

                return xhash == yhash;
            }
            else // 파일이 없으면 같지 않은 것으로 간주
                return false;
        }

        // 파일의 해쉬를 가져옴
        public override int GetHashCode(HashedFile obj)
        {
            byte[] hash;

            if (obj.Hash == "")
                hash = GetFileHashArray(obj.Path);
            else
                hash = HexToByteArray(obj.Hash);

            return BitConverter.ToInt32(hash, 0);
        }

        // 파일의 해쉬를 구해 HEX 문자를 반환
        public static string GetFileHash(string path)
        {
            var binaryHash = GetFileHashArray(path);
            return BitConverter.ToString(binaryHash).Replace("-", "").ToLower();
        }

        // 파일 해쉬 계산
        public static byte[] GetFileHashArray(string path)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(path))
                {
                    var binaryHash = md5.ComputeHash(stream);
                    return binaryHash;
                }
            }
        }

        private static byte[] HexToByteArray(string hex) // HEX 문자열을 바이너리로
        {
            if (hex.Length % 2 == 1)
                throw new Exception("Invalid Hex. Hex Length cannot be odd number");

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1]))); // bit-operate
            }

            return arr;
        }

        private static int GetHexVal(char hex) // HEX 문자를 숫자로
        {
            int val = (int)hex;
            return val - (val < 58 ? 48 : 87); // 소문자 a 부터 f 까지
        }
    }
}
