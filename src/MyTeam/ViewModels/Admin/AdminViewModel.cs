namespace MyTeam.ViewModels.Admin
{
    public class AdminViewModel
    {
        public string FacebookAppId { get; set; }


        public AdminViewModel(string facebookAppId)
        {
            FacebookAppId = facebookAppId;
        }    
    }
}