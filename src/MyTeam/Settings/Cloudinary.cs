using System;
using System.Text.RegularExpressions;
using MyTeam.Models.Enums;

namespace MyTeam.Settings
{
    public static class Cloudinary
    {
        public static string Image(string res, string fallback = "")
        {
            if (string.IsNullOrWhiteSpace(res)) res = fallback; 
            return "http://res.cloudinary.com/drdo17bnj/" + res;
        }

        public static string DefaultArticle = "image/upload/c_scale,q_95,w_900/v1448309373/article_default_hnwnxo.jpg";

    }
}