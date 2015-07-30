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
        public AlertType Type { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var content = context.GetChildContentAsync().Result.GetContent();
            if (string.IsNullOrWhiteSpace(content)) return;


            output.Attributes["class"] = $"alert alert-{Type.ToString().ToLower()}";

            var innertag = new TagBuilder("i");
            innertag.AddCssClass($"fa fa-{GetIcon(Type)}");
            output.Content.Append(innertag.ToString());

            output.Content.Append(content);
            output.Content.Append(
                "<button type='button' class='close' data-dismiss='alert' aria-hidden='true'>&times;</button>");
        }

        private string GetIcon(AlertType type)
        {
            switch (type)
            {
                case AlertType.Danger:
                    return "warning";
                case AlertType.Info:
                    return "info";
                case AlertType.Warning:
                    return "warning";
                default:
                    return "info";

            }
        }
    }
}


