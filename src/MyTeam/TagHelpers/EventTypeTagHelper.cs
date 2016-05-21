using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using MyTeam.Models.Enums;

namespace MyTeam.TagHelpers
{
    [HtmlTargetElement("*", Attributes = Name)]
    public class EventTypeTagHelper : TagHelper
    {

        public const string Name = "mt-eventtype";

        [HtmlAttributeName(Name)]
        public EventType EventType { get; set; }
        [HtmlAttributeName("mt-hidename")]
        public bool HideName { get; set; }
        [HtmlAttributeName("mt-text-class")]
        public string TextClass { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

            var innertag = new TagBuilder("i");
            innertag.AddCssClass($"{GetIconName(EventType)}");
            innertag.Attributes.Add("title", EventType.ToString());

            output.Content.AppendHtml(innertag);
            if (!HideName) output.Content.AppendHtml($"<span class='{TextClass}'>&nbsp;&nbsp;{EventType}</span>");
        }

        private string GetIconName(EventType type)
        {
            switch (type)
            {
                case EventType.Alle:
                    return "fa fa-calendar";
                case EventType.Kamp:
                    return "fa fa-trophy";
                case EventType.Trening:
                    return "flaticon-couple40";
                case EventType.Diverse:
                    return "fa fa-beer";
                default:
                    return string.Empty;

            }
        }

    }
}


