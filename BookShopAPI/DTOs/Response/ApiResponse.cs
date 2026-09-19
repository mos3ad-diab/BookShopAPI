namespace BookShopAPI.DTOs.Response
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T? MyProperty { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}
