namespace MyTeam.Settings
{
    public interface ICloudinary
    {
        string Image(string res, int? width = null, string fallback = "", int quality = 100, string format = null);
        string MemberImage(string res, string facebookId, int? width = null, int? height = null);
        string DefaultArticle { get; }

    }
}