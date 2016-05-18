﻿using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace MyTeam.TagHelpers
{
    [HtmlTargetElement("div", Attributes = Name)]
    public class SidebarTagHelper : TagHelper
    {

        public const string Name = "mt-sidebar";
        public const string InnerIdName = "inner-id";
        public const string ClassName = "class";

        [HtmlAttributeName(InnerIdName)]
        public string InnerId { get; set; }

        [HtmlAttributeName(ClassName)]
        public string Class { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes["class"] = $" pull-right {Class}";

            var innertag = new TagBuilder("div");
            innertag.AddCssClass("mt-container");

            var content = output.GetChildContentAsync().Result.GetContent();
            innertag.InnerHtml.AppendHtml(content);

            if (!string.IsNullOrWhiteSpace(InnerId)) innertag.Attributes["id"] = InnerId;

            output.Content.Append(innertag);
        }


    }
}


