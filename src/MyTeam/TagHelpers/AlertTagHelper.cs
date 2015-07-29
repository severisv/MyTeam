using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Mvc.TagHelpers;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;
using Microsoft.AspNet.Razor.TagHelpers;
using MyTeam.Models.Enums;

namespace MyTeam.TagHelpers
{
    [TargetElement("div", Attributes = Name)]
    public class AlertTagHelper : TagHelper
    {

        public const string Name = "mt-alert";

        [HtmlAttributeName(Name)]
        public string Type { get; set; }
        [HtmlAttributeName("mt-icon")]
        public string Icon { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes["class"] = $"alert alert-{Type}";
            
            if (!string.IsNullOrWhiteSpace(Icon))
            {
                var innertag = new TagBuilder("i");
                innertag.AddCssClass($"fa fa-{Icon}");
                output.Content.Append(innertag.ToString());
            }

            output.Content.Append(context.GetChildContentAsync().Result.GetContent());
            output.Content.Append(
                "<button type='button' class='close' data-dismiss='alert' aria-hidden='true'>&times;</button>");
        }
    }
}


