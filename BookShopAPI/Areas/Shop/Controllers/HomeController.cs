using BookShopAPI.DTOs.Response;
using BookShopAPI.Models;
using BookShopAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookShopAPI.Areas.Shop.Controllers
{
    [Area("Shop")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IRepository<Book> _bookRepository;

        public HomeController(IRepository<Book> bookRepository)
        {
            _bookRepository = bookRepository;
        }
        [HttpGet("BestSellers")]
        public async Task<IActionResult> BestSellers()
        {
            var books = await _bookRepository.GetAllAsync(ordering: e=>e.Sold , top: 1 , order:1);

            return Ok(new ApiResponse<IEnumerable<Book>>()
            {
                IsSuccess = true,
                Message = "Data returned Successfully",
                Data = books
            });

        }
        [HttpGet("ForYou")]
        public async Task<IActionResult> ForYou()
        {
            var books = await _bookRepository.GetAllAsync(ordering: e => e.Rate, top: 1, order: 1);

            return Ok(new ApiResponse<IEnumerable<Book>>()
            {
                IsSuccess = true,
                Message = "Data returned Successfully",
                Data = books
            });

        }
    }
}
