using BookShopAPI.DTOs.Request;
using BookShopAPI.DTOs.Response;
using BookShopAPI.Models;
using BookShopAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;

namespace BookShopAPI.Areas.Client.Controllers
{
    [Area("Client")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Book> _bookRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(IRepository<Cart> cartRepository, IRepository<Book> bookRepository, UserManager<ApplicationUser> userManager)
        {
            _cartRepository = cartRepository;
            _bookRepository = bookRepository;
            _userManager = userManager;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound(new ApiResponse<object>()
            {
                IsSuccess = false,
                Message = "No content",
            });

            var carts = await _cartRepository.GetAllAsync(e => e.ApplicationUserId == user.Id, includes: [m => m.Book]);
            return Ok(new ApiResponse<IEnumerable<Cart>>()
            {
                IsSuccess = true,
                Message = "Successfull",
                Data = carts
             
            });
        }

        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart(AddToCartRequest addToCartRequest)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound(new ApiResponse<object>()
            {
                IsSuccess = false,
                Message = "invalid user",
            });


            var book = await _bookRepository.GetOneAsync(e => e.Id == addToCartRequest.bookId);
            var cartInDb = await _cartRepository.GetOneAsync(e => e.BookId == addToCartRequest.bookId && e.ApplicationUserId == user.Id);

            if (cartInDb != null)
            {
                cartInDb.Count += addToCartRequest.count;
                await _cartRepository.CommitAsync();
                return Ok(new ApiResponse<object>()
                {
                    IsSuccess = true,
                    Message = "Book is already existed, Count added successfully"
                });
            }

            var cart = new Cart()
            {
                ApplicationUserId = user.Id,
                BookId = addToCartRequest.bookId,
                Count = addToCartRequest.count,
                Price = book.Price,
            };

            await _cartRepository.InsertAsync(cart);
            await _cartRepository.CommitAsync();

            return Ok(new ApiResponse<object>()
            {
                IsSuccess = true,
                Message = "Book addded to cart successfully"
            });
        }
        [HttpPut("Increment")]
        public async Task<IActionResult> Increment(int bookId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            var book = await _bookRepository.GetOneAsync(e => e.Id == bookId);
            if (book == null) return NotFound();

            var carts = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.BookId == bookId);
            if (carts.Count < book.Amount)
            {
                carts.Count++;
                await _cartRepository.CommitAsync();
                return Ok(new ApiResponse<object>()
                {
                    IsSuccess = true,
                    Message = "Count increased successfully"
                });
            }
            return Ok(new ApiResponse<object>()
            {
                IsSuccess = true,
                Message = "Count increased successfully"
            });
        }
        [HttpPut("Decrement")]
        public async Task<IActionResult> Decrement(int bookId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            var book = await _bookRepository.GetOneAsync(e => e.Id == bookId);
            if (book == null) return NotFound();

            var carts = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.BookId == bookId);
            if (carts.Count > 1)
            {
                carts.Count--;
                await _cartRepository.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int bookId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            var book = await _bookRepository.GetOneAsync(e => e.Id == bookId);
            if (book == null) return NotFound();

            var carts = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.BookId == bookId);

            _cartRepository.Delete(carts);
            await _cartRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
