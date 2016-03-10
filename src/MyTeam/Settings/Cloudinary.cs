using System;
using System.Linq;
using System.Runtime;
using System.Text.RegularExpressions;
using MyTeam.Models.Enums;

namespace MyTeam.Settings
{
    public static class Cloudinary
    {

        public static string BaseLocation = "http://res.cloudinary.com/drdo17bnj/";
        public static string DefaultArticle = "image/upload/v1448309373/article_default_hnwnxo.jpg";
        public static string DefaultMember = "image/upload/v1448559418/default_player_dnwac0.gif";


        public static string Image(string res, int? width = null, string fallback = "")
        {
            if (string.IsNullOrWhiteSpace(res)) res = fallback;

            return Resize($"{BaseLocation}{res}", width);
        }


        public static string MemberImage(string res, string facebookId, int? width = null, int? height = null)
        {
            if (string.IsNullOrEmpty(res))
            {
                res = !string.IsNullOrEmpty(facebookId) ? 
                    GetFacebookImage(facebookId, width) : 
                    DefaultMember;
            }

            if (res.StartsWith("http")) return res;

            return Resize($"{BaseLocation}{res}", width, height);
        }

        private static string GetFacebookImage(string facebookId, int? width)
        {
            var type = "large";
            if (width < 51) type = "small";
            else if (width < 101) type = "normal";

            return $"https://graph.facebook.com/{facebookId}/picture?type={type}";
        }

        private static string Resize(string imageUrl, int? width, int? height = null)
        {
            if (width == null) return imageUrl;

            var urlList = imageUrl.Split('/').ToList();
            int? insertAt = null;
            for (int i = 0; i < urlList.Count(); i++)
            {
                if (urlList[i] == "upload") insertAt = i + 1;
            }
            if (insertAt != null)
            {
                if (height == null)
                {
                    urlList.Insert((int) insertAt, $"c_scale,w_{width},q_100");
                }
                else
                {
                    urlList.Insert((int)insertAt, $"c_fill,h_{height},w_{width},q_100");
                }
            }

            return string.Join("/", urlList);
        }
    }
}