namespace MyTeam.ViewModels.Admin
{
    public class AddPlayersViewModel
    {
        public string FacebookAppId { get; set; }


        public AddPlayersViewModel(string facebookAppId)
        {
            FacebookAppId = facebookAppId;
        }    
    }
}