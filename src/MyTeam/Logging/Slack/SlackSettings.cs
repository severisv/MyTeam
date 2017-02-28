namespace MyTeam.Logging.Slack
{
    public class SlackSettings
    {
        public string Channel { get; }
        public string Application { get; }
        public string WebhookUrl { get; }

        public SlackSettings(string applicationName, string webhookUrl, string channel)
        {
            Application = applicationName;
            Channel = channel;
            WebhookUrl = webhookUrl;
        }
    }
}
