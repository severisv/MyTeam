﻿using System.Linq;
using Microsoft.Extensions.OptionsModel;

namespace MyTeam.Settings
{
    public class Cloudinary : ICloudinary
    {
        public string DefaultArticle { get; }
        private readonly string _defaultMember;
        private readonly string _cloudName;
        private string BaseLocation => $"http://res.cloudinary.com/{_cloudName}/";

        public Cloudinary(IOptions<CloudinaryOptions> options)
        {
            _cloudName = options.Value.CloudName;
            _defaultMember = options.Value.DefaultMember ?? "image/upload/v1448559418/default_player_dnwac0.gif";
            DefaultArticle = options.Value.DefaultArticle ?? "image/upload/v1448309373/article_default_hnwnxo.jpg";
        }


        public string Image(string res, int? width = null, string fallback = "")
        {
            if (string.IsNullOrWhiteSpace(res)) res = fallback;

            return Resize($"{BaseLocation}{res}", width);
        }


        public string MemberImage(string res, string facebookId, int? width = null, int? height = null)
        {
            if (string.IsNullOrEmpty(res))
            {
                res = !string.IsNullOrEmpty(facebookId)
                    ? GetFacebookImage(facebookId, width)
                    : _defaultMember;
            }

            if (res.StartsWith("http")) return res;

            return Resize($"{BaseLocation}{res}", width, height);
        }

        private string GetFacebookImage(string facebookId, int? width)
        {
            var type = "large";
            if (width < 51) type = "square";
            else if (width < 101) type = "normal";

            return $"https://graph.facebook.com/{facebookId}/picture?type={type}";
        }

        private string Resize(string imageUrl, int? width, int? height = null)
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
                    urlList.Insert((int) insertAt, $"c_fill,h_{height},w_{width},q_100");
                }
            }

            return string.Join("/", urlList);
        }
    }
}