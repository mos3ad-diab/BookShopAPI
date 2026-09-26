namespace BookShopAPI.DTOs.Request
{
    public class AddToCartRequest
    {
        public int bookId { get; set; }
        public int count { get; set; }
    }
}
