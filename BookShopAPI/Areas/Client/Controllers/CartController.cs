using BookShopAPI.DTOs.Request;
using BookShopAPI.DTOs.Response;
using BookShopAPI.Models;
using BookShopAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
            if (user == null) return Unauthorized(new ApiResponse<object>()
            {
                IsSuccess = false,
                Message = "User is unauthorized",
            });

            var carts = await _cartRepository.GetAllAsync(e => e.ApplicationUserId == user.Id, includes: [m => m.Book]);
            if (carts == null || !carts.Any()) return NotFound(new ApiResponse<object>()
            {
                IsSuccess = false,
                Message = "No cart items found",
            });

            return Ok(new ApiResponse<IEnumerable<Cart>>()
            {
                IsSuccess = true,
                Message = "Data returned successfully",
                Data = carts
            });
        }

        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart(AddToCartRequest addToCartRequest)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized(new ApiResponse<object>()
            {
                IsSuccess = false,
                Message = "User is unauthorized",
            });

            var book = await _bookRepository.GetOneAsync(e => e.Id == addToCartRequest.bookId);
            if (book == null) return NotFound(new ApiResponse<object>()
            {
                IsSuccess = false,
                Message = "Invalid book",
            });

            var cartInDb = await _cartRepository.GetOneAsync(e => e.BookId == addToCartRequest.bookId && e.ApplicationUserId == user.Id);

            if (cartInDb != null)
            {
                if (cartInDb.Count + addToCartRequest.count > book.Amount)
                {
                    return BadRequest(new ApiResponse<object>()
                    {
                        IsSuccess = false,
                        Message = $"Cannot add more items. Available stock is {book.Amount}"
                    });
                }

                cartInDb.Count += addToCartRequest.count;
                await _cartRepository.CommitAsync();

                return Ok(new ApiResponse<object>()
                {
                    IsSuccess = true,
                    Message = "Book already exists, count updated successfully"
                });
            }

            if (addToCartRequest.count > book.Amount)
            {
                return BadRequest(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = $"Requested count exceeds available stock ({book.Amount})"
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
                Message = "Book added to cart successfully"
            });
        }

        [HttpPut("Increment")]
        public async Task<IActionResult> Increment(int bookId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized(new ApiResponse<object>()
            {
                IsSuccess = false,
                Message = "User is unauthorized",
            });

            var book = await _bookRepository.GetOneAsync(e => e.Id == bookId);
            if (book == null) return NotFound(new ApiResponse<object>()
            {
                IsSuccess = false,
                Message = "Invalid book",
            });

            var cart = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.BookId == bookId);
            if (cart == null) return NotFound(new ApiResponse<object>
            {
                IsSuccess = false,
                Message = "Item not found in cart"
            });

            if (cart.Count < book.Amount)
            {
                cart.Count++;
                await _cartRepository.CommitAsync();
                return Ok(new ApiResponse<object>()
                {
                    IsSuccess = true,
                    Message = "Count increased successfully"
                });
            }

            return BadRequest(new ApiResponse<object>()
            {
                IsSuccess = false,
                Message = $"Cannot increase count. Stock limit ({book.Amount}) reached"
            });
        }

        [HttpPut("Decrement")]
        public async Task<IActionResult> Decrement(int bookId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized(new ApiResponse<object>()
            {
                IsSuccess = false,
                Message = "User is unauthorized",
            });

            var book = await _bookRepository.GetOneAsync(e => e.Id == bookId);
            if (book == null) return NotFound(new ApiResponse<object>()
            {
                IsSuccess = false,
                Message = "Invalid book",
            });

            var cart = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.BookId == bookId);
            if (cart == null) return NotFound(new ApiResponse<object>
            {
                IsSuccess = false,
                Message = "Item not found in cart"
            });

            if (cart.Count > 1)
            {
                cart.Count--;
                await _cartRepository.CommitAsync();
                return Ok(new ApiResponse<object>()
                {
                    IsSuccess = true,
                    Message = "Count decremented successfully"
                });
            }

            return BadRequest(new ApiResponse<object>()
            {
                IsSuccess = false,
                Message = "Minimum count is 1. Use remove to delete this item."
            });
        }

        [HttpDelete("Remove")]
        public async Task<IActionResult> Remove(int bookId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized(new ApiResponse<object>()
            {
                IsSuccess = false,
                Message = "User is unauthorized",
            });

            var cart = await _cartRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.BookId == bookId);
            if (cart == null) return NotFound(new ApiResponse<object>()
            {
                IsSuccess = false,
                Message = "Item not found in cart"
            });

            _cartRepository.Delete(cart);
            await _cartRepository.CommitAsync();

            return Ok(new ApiResponse<object>()
            {
                IsSuccess = true,
                Message = "Item removed from cart successfully"
            });
        }
    }
}