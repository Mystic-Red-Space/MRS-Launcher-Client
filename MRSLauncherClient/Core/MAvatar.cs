using System;
using System.Windows.Media.Imaging;

namespace MRSLauncherClient
{
    public class MAvatar
    {
        public static BitmapImage GetAvatarImage(string uuid) // UUID 로 게임 아바타 이미지를 가져옴
        {
            var uri = "https://crafatar.com/avatars/" + uuid + "?size=" + 30 + "&default=MHF_Steve" + "&overlay";
            return new BitmapImage(new Uri(uri));
        }
    }
}
