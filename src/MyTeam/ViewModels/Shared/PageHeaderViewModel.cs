using System;

namespace MyTeam.ViewModels.Shared
{
    public class PageHeaderViewModel
    {
        public string Headline { get; private set; }
        public string Content { get; private set; }

        public PageHeaderViewModel(string headline, string content)
        {
            Headline = headline;
            Content = content;
        }
    }
}