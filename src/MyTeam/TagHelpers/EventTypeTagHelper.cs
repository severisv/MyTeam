using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Razor.TagHelpers;
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
            innertag.AddCssClass($"fa fa-{GetIconName(EventType)}");
            innertag.Attributes.Add("title", EventType.ToString());

            output.Content.Append(innertag);
            if (!HideName) output.Content.AppendHtml($"<span class='{TextClass}'>&nbsp;&nbsp;{EventType}</span>");
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


