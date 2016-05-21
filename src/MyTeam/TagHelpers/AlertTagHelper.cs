﻿using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using MyTeam.Models.Enums;
using MyTeam.Models.Structs;

namespace MyTeam.TagHelpers
{
    [HtmlTargetElement("div", Attributes = Name)]
    public class AlertTagHelper : TagHelper
    {

        public const string Name = "mt-alert";

        [HtmlAttributeName(Name)]
        public AlertType Type { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var message = output.GetChildContentAsync().Result.GetContent();
            var alert = new Alert(Type, message);

            var hidden = string.IsNullOrWhiteSpace(message) ? "hidden" : "";
            output.Attributes.Add("class",$"alert alert-{Type.ToString().ToLower()} {hidden}");
            output.Attributes.Add("id",Type.ToString().ToLower());

            var innertag = new TagBuilder("i");
            innertag.AddCssClass($"fa fa-{alert.Icon}");
            output.Content.AppendHtml(innertag);
            output.Content.AppendHtml($" <span class='alert-content'>{alert.Message}</span>");
            output.Content.AppendHtml("<button type='button' class='close' data-dismiss='alert' aria-hidden='true'>&times;</button>");
        }


    }
}


