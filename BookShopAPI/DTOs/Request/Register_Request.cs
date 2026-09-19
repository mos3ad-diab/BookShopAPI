using System.ComponentModel.DataAnnotations;

namespace BookShopAPI.DTOs.Request
{
    public class Register_Request
    {
        
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Address { get; set; }
        [DataType(DataType.EmailAddress), EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password), Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
