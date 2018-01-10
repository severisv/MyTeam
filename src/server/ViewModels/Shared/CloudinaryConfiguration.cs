﻿using System;
using MyTeam.Settings;
using MyTeam.Util;

namespace MyTeam.ViewModels.Shared
{
    public class CloudinaryConfiguration
    {
        public string CloudName { get; }
        public string ApiKey { get; }
        public string ApiSecret { get; }
        public string Signature { get; }
        public int UnixTimestamp { get; }


        public CloudinaryConfiguration(CloudinaryOptions options)
        {
           CloudName = options.CloudName;
           ApiKey = options.ApiKey;
           ApiSecret = options.ApiSecret;
           UnixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

        var queryString = $"timestamp={UnixTimestamp}{ApiSecret}";
        Signature = Sha1.HashStringForUTF8String(queryString);
        }
    }
}