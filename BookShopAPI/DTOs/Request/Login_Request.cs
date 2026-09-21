using System.ComponentModel.DataAnnotations;

namespace BookShopAPI.DTOs.Request
{
    public class Login_Request
    {
        public string UserNameOrEmail { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
