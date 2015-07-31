using System;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Mvc.TagHelpers;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;
using Microsoft.AspNet.Razor.TagHelpers;
using MyTeam.Models.Enums;
using MyTeam.Models.Structs;

namespace MyTeam.TagHelpers
{
    [TargetElement("div", Attributes = Name)]
    public class MainTagHelper : TagHelper
    {

        public const string Name = "mt-main";
        public const string InnerIdName = "inner-id";
        public const string ClassName = "class";

        [HtmlAttributeName(InnerIdName)]
        public string InnerId { get; set; }

        [HtmlAttributeName(ClassName)]
        public string Class { get; set; }
        
        
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes["class"] = $"col-lg-9 col-md-9 {Class}";

            var innertag = new TagBuilder("div");
            innertag.AddCssClass("mt-container");

            var content = context.GetChildContentAsync().Result.GetContent();
            innertag.InnerHtml = content;

            if (!string.IsNullOrWhiteSpace(InnerId)) innertag.Attributes["id"] = InnerId;

            output.Content.Append(innertag.ToString());
        }
    }
}