using System;
using System.Text.RegularExpressions;
using MyTeam.Models.Enums;

namespace MyTeam.Settings
{
    public class Config
    {
        public static int AllowedSignupDays = 7;
        public static int AllowedSignoffHours = 2;
        

        public static string PlayerImages(string clubShortname, string imagename, ImageSize imageSize)
        {
            if (string.IsNullOrWhiteSpace(imagename)) return string.Format("~/img/default_player.gif");

            Regex rgx = new Regex("[^a-zA-Z0-9]");
            var handle = rgx.Replace(clubShortname, "").ToLower();
            var size = GetSize(imageSize);
            return string.Format("~/img/clubs/{0}/players/{1}_{2}.jpg", handle, imagename, size);
        }

        private static string GetSize(ImageSize imageSize)
        {
            switch (imageSize)
            {
                case ImageSize.Small:
                    return "sm";
                case ImageSize.Medium:
                    return "md";
                case ImageSize.Full:
                    return "full";
                default:
                    throw new NotImplementedException(string.Format("Imagesize not recognized: {0}", imageSize));
            }
        }

    }
}