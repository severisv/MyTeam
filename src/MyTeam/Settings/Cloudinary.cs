using System;
using System.Linq;
using System.Text.RegularExpressions;
using MyTeam.Models.Enums;

namespace MyTeam.Settings
{
    public static class Cloudinary
    {

        public static string BaseLocation = "http://res.cloudinary.com/drdo17bnj/";
        public static string DefaultArticle = "image/upload/c_scale,q_95,w_900/v1448309373/article_default_hnwnxo.jpg";
        public static string DefaultMember = "image/upload/v1448559418/default_player_dnwac0.gif";


        public static string Image(string res, string fallback = "")
        {
            if (string.IsNullOrWhiteSpace(res)) res = fallback; 
            return BaseLocation + res;
        }


        public static string MemberImage(string res, int? width = null)
        {
            if (string.IsNullOrEmpty(res))
            {
                res = DefaultMember;
            }
            else if (res.Contains("http")) return res;

            return Resize($"{BaseLocation}{res}", width);
        }

        private static string Resize(string imageUrl, int? width)
        {
            if (width == null) return imageUrl;

            var urlList = imageUrl.Split('/').ToList();
            int insertAt = 1337;
            for (int i = 0; i < urlList.Count(); i++)
            {
                if (urlList[i] == "upload") insertAt = i + 1;
            }

            urlList.Insert(insertAt, $"c_scale,w_{width},q_100");

            return string.Join("/", urlList);
        }
    }
}