﻿using System.Collections.Generic;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Mvc.TagHelpers;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;
using Microsoft.AspNet.Razor.TagHelpers;
using MyTeam.Models.Enums;
using MyTeam.Models.Structs;

namespace MyTeam.TagHelpers
{
    [TargetElement("div", Attributes = Name)]
    public class AlertTagHelper : TagHelper
    {

        public const string Name = "mt-alert";

        [HtmlAttributeName(Name)]
        public AlertType Type { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var message = context.GetChildContentAsync().Result.GetContent();


            var alert = new Alert(Type, message);

            var hidden = string.IsNullOrWhiteSpace(message) ? "hidden" : "";
            output.Attributes["class"] = $"alert alert-{Type.ToString().ToLower()} {hidden}";
            output.Attributes["id"] = Type.ToString().ToLower();

            var innertag = new TagBuilder("i");
            innertag.AddCssClass($"fa fa-{alert.Icon}");
            output.Content.Append(innertag.ToString());
            output.Content.Append($" <span class='alert-content'>{alert.Message}</span>");
            output.Content.Append("<button type='button' class='close' data-dismiss='alert' aria-hidden='true'>&times;</button>");
        }

      
    }
}


