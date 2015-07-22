using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Mvc.TagHelpers;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;
using Microsoft.AspNet.Razor.TagHelpers;

namespace MyTeam.TagHelpers
{
    [TargetElement("mt-main")]
    public class MainTagHelper : TagHelper
    {

       


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var tagBuilder = new TagBuilder("div");
            tagBuilder.AddCssClass("col-lg-9 col-md-9");
            var innertag = new TagBuilder("div");
            innertag.AddCssClass("mt-container");
            innertag.InnerHtml = output.Content.GetContent();

            TagHelperAttribute innerId;
            output.Attributes.TryGetAttribute("inner-id", out innerId);
            innertag.Attributes["id"] = innerId.Value.ToString();

            tagBuilder.InnerHtml = innertag.ToString();

            output.MergeAttributes(tagBuilder);
            output.Content.Append(tagBuilder.InnerHtml);

            output.TagName = "div";


        }

    }
}