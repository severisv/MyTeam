using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Mvc.TagHelpers;
using Microsoft.AspNet.Razor.TagHelpers;

namespace MyTeam.TagHelpers
{

    [HtmlTargetElement("a", Attributes = ForAttributeName)]
    public class TooltipTagHelper : TagHelper
    {
        private const string ForAttributeName = "mt-tooltip";

        [HtmlAttributeName(ForAttributeName)]
        public string Content { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var tagBuilder = new TagBuilder("a");

            tagBuilder.Attributes["href"] = "javascript:void(0);";
            tagBuilder.Attributes["class"] = "mt-popover";
            tagBuilder.Attributes["data-container"] = "body";
            tagBuilder.Attributes["data-toggle"] = "popover";
            tagBuilder.Attributes["data-placement"] = "right";
            tagBuilder.Attributes["data-content"] = Content;
            output.Content.AppendHtml("<i class='fa fa-question-circle'></i>");

            output.MergeAttributes(tagBuilder);

        }
    }
}