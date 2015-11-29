using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;

namespace MyTeam.Pipeline
{
    public class PipelineConstants
    {
        public const string MemberKey = "member";
        public const string ClubKey = "club";

        public static readonly Dictionary<string, string> ClubIdlookup = new Dictionary<string, string>()
        {
            { "wamkam.no", "wamkam" },
            { "localhost:5000", "wamkam" },
            };
        
        public static string GetClubIdFromHostname(string rootdomain)
        {
            string result;
            ClubIdlookup.TryGetValue(rootdomain, out result);
            return result;
        }
    }
}
