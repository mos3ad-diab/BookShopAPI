using BookShopAPI.DTOs.Request;
using BookShopAPI.DTOs.Response;
using BookShopAPI.Models;
using BookShopAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace BookShopAPI.Areas.User.Controllers
{
    [Area("Shop")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {

        private readonly IRepository<Book> _bookRepository;

        public BooksController(IRepository<Book> bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var books = await _bookRepository.GetAllAsync();

           // List<string> book = new List<string>();
            if(books == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "No data found"
                });
            }
            return Ok(new ApiResponse<IEnumerable<Book>>
            {
                IsSuccess = true,
                Message = "Data returned successfully",
                Data = books
            });
            
        }
        [HttpGet("GetFiltered")]
        public async Task<IActionResult> GetFiltered(int? AutherId , int? CategoryId)
        {
            Expression<Func<Book, bool>> Filter = e => ( AutherId == e.AuthorId)
            && ( CategoryId == e.CategoryId);

            var books = await _bookRepository.GetAllAsync(filter: Filter );

            if(books == null || !books.Any())
            {
                return NotFound(new ApiResponse<Object>
                {
                    IsSuccess = false,
                    Message = "No book match the inputs"
                });
            }
            return Ok(new ApiResponse<IEnumerable<Book>>
            {
                IsSuccess = true,
                Message = "Data returned successfully",
                Data = books
            });
        }
        [HttpGet("GetOne")]
        public async Task<IActionResult> GetOne(int id)
        {
            var book = await _bookRepository.GetOneAsync(e=>e.Id == id);

            // List<string> book = new List<string>();
            if (book == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "No data found"
                });
            }
            return Ok(new ApiResponse<Book>
            {
                IsSuccess = true,
                Message = "Data returned successfully",
                Data = book
            });

        }

    }
}
