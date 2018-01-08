using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace MyTeam.TagHelpers
{
    /// <summary>
    /// <see cref="ITagHelper"/> implementation targeting &lt;label&gt; elements with an <c>asp-for</c> attribute.
    /// </summary>
    [HtmlTargetElement("label", Attributes = ForAttributeName)]
    public class CustomLabelTagHelper : TagHelper
    {
        private const string ForAttributeName = "mt-for";
        private const string IconAttributeName = "mt-icon";

        [HtmlAttributeName(IconAttributeName)]
        public string Icon { get; set; }

        public CustomLabelTagHelper(IHtmlGenerator generator)
        {
            Generator = generator;
        }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        protected IHtmlGenerator Generator { get; }

     [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }

  public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var tagBuilder = Generator.GenerateLabel(
                ViewContext,
                For.ModelExplorer,
                For.Name,
                labelText: null,
                htmlAttributes: null);

            tagBuilder.Attributes["title"] = For.Metadata.DisplayName;

            if (tagBuilder != null)
            {
                output.MergeAttributes(tagBuilder);

                // We check for whitespace to detect scenarios such as:
                // <label for="Name">
                // </label>
                if (!output.IsContentModified)
                {
                    var childContent = await output.GetChildContentAsync();

                    if (childContent.IsEmptyOrWhiteSpace)
                    {
                        // Provide default label text since there was nothing useful in the Razor source.
                        output.Content.SetHtmlContent($"<i class='fa fa-{Icon}'></i>");
                    }
                    else
                    {
                        output.Content.SetHtmlContent(childContent.GetContent());
                    }
                }
            }
        }
    }
}