using System.ComponentModel.DataAnnotations;

namespace BookShopAPI.DTOs.Request
{
    public class ChangePasswordRequest
    {
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}
