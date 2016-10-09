namespace MyTeam.ViewModels.Fine
{
    public class PaymentInfoViewModel
    {

        public string Image { get; }
        public string FacebookId { get; }
        public string PaymentInfo { get; }
        public int Due { get; }
        
        public PaymentInfoViewModel(string image, string facebookId, string paymentInfo, int due)
        {
            Image = image;
            FacebookId = facebookId;
            PaymentInfo = paymentInfo;
            Due = due;
        }

    }
}