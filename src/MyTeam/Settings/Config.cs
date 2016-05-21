using System;
using System.Text.RegularExpressions;
using MyTeam.Models.Enums;

namespace MyTeam.Settings
{
    public class Config
    {
        public static int AllowedSignupDays = 10;
        public static int AllowedSignoffHours = 2;
        public static int AllowedMonthsAheadInTimeForTrainingCreation = 11;
        

        public static string PlayerImages(string clubShortname, string imagename, ImageSize imageSize)
        {
            if (string.IsNullOrWhiteSpace(imagename)) return string.Format("~/img/default_player.gif");



            var rgx = new Regex("[^a-zA-Z0-9]");
            var handle = rgx.Replace(clubShortname, "").ToLower();
            var size = GetSize(imageSize);
            return $"~/img/clubs/{handle}/players/{imagename}_{size}.jpg";
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
                    throw new NotImplementedException($"Imagesize not recognized: {imageSize}");
            }
        }

    }
}