﻿using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Mvc.TagHelpers;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;
using Microsoft.AspNet.Razor.TagHelpers;
using MyTeam.Models.Enums;

namespace MyTeam.TagHelpers
{
    [TargetElement("*", Attributes = Name)]
    public class EventTypeTagHelper : TagHelper
    {

        public const string Name = "mt-eventtype";

        [HtmlAttributeName(Name)]
        public EventType EventType { get; set; }
        [HtmlAttributeName("mt-hidename")]
        public bool HideName { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

            var innertag = new TagBuilder("i");
            innertag.AddCssClass($"fa fa-{GetIconName(EventType)}");

            output.Content.Append($"{innertag.ToString()}");
            if (!HideName) output.Content.Append($"&nbsp;&nbsp;{EventType}");
        }
        
        private string GetIconName(EventType type)
        {
            switch (type)
            {
                case EventType.Alle:
                    return "calendar";
                case EventType.Kamp:
                    return "trophy";
                case EventType.Trening:
                    return "flag";
                case EventType.Diverse:
                    return "beer";
                default:
                    return string.Empty;
                    
            }
        }

    }
}

