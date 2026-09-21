using System.ComponentModel.DataAnnotations;

namespace BookShopAPI.DTOs.Request
{
    public class ResetPassword_Request
    {
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password), Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        public string UserId { get; set; }
        public string Token { get; set; }
    }
}
