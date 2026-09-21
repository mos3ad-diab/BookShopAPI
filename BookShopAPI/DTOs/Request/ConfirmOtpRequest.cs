namespace BookShopAPI.DTOs.Request
{
    public class ConfirmOtpRequest
    {
        public string OTP { get; set; }
        public string UserId { get; set; }
    }
}
