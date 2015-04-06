using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Mvc.TagHelpers;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;
using Microsoft.AspNet.Razor.TagHelpers;

namespace MyTeam.TagHelpers
{
    [TagName("mt-sidebar")]
    [ContentBehavior(ContentBehavior.Modify)]
    public class SidebarTagHelper : TagHelper
    {

       


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var tagBuilder = new TagBuilder("div");
            tagBuilder.AddCssClass("col-lg-3 col-md-3 visible-md visible-lg pull-right");
            var innertag = new TagBuilder("div");
            innertag.AddCssClass("mt-container");
            innertag.InnerHtml = output.Content;

            string innerId;
            output.Attributes.TryGetValue("inner-id", out innerId);
            innertag.Attributes["id"] = innerId;

            tagBuilder.InnerHtml = innertag.ToString();

            output.MergeAttributes(tagBuilder);
            output.Content = tagBuilder.InnerHtml;

            output.TagName = "div";


        }

    }
}


