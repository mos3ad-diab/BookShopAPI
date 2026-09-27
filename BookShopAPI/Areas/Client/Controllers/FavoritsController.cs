using BookShopAPI.DTOs.Response;
using BookShopAPI.Models;
using BookShopAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookShopAPI.Areas.Client.Controllers
{
    [Area("Client")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize]
    public class FavoritsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Favorit> _favoritesRepository;
        private readonly IRepository<Book> _bookRepository;

        public FavoritsController(
            UserManager<ApplicationUser> userManager,
            IRepository<Favorit> favoritesRepository,
            IRepository<Book> bookRepository)
        {
            _userManager = userManager;
            _favoritesRepository = favoritesRepository;
            _bookRepository = bookRepository;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var favorits = await _favoritesRepository.GetAllAsync(
                f => f.ApplicationUserId == user.Id,
                includes: [f => f.Book]
            );

            if (favorits == null || !favorits.Any())
            {
                return NotFound(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "No favorites found",
                });
            }

            return Ok(new ApiResponse<IEnumerable<Favorit>>()
            {
                IsSuccess = true,
                Message = "Favorites returned successfully",
                Data = favorits 
            });
        }

        [HttpGet("GetOne")]
        public async Task<IActionResult> GetOne(int bookId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var favorit = await _favoritesRepository.GetOneAsync(
                f => f.ApplicationUserId == user.Id && f.BookId == bookId,
                includes: [f => f.Book]
            );

            if (favorit == null)
            {
                return NotFound(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "This book is not in your favorites",
                });
            }

            return Ok(new ApiResponse<Favorit>()
            {
                IsSuccess = true,
                Message = "Favorite returned successfully",
                Data = favorit
            });
        }

        [HttpPost("AddToFavorit")]
        public async Task<IActionResult> AddToFavorit(int bookId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            
            var book = await _bookRepository.GetOneAsync(b => b.Id == bookId);
            if (book == null)
            {
                return NotFound(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "Book not found"
                });
            }

            
            var favorit = await _favoritesRepository.GetOneAsync(f => f.ApplicationUserId == user.Id && f.BookId == bookId);
            if (favorit != null)
            {
                return BadRequest(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "The book is already in your favorites",
                });
            }

            var newFav = new Favorit()
            {
                ApplicationUserId = user.Id,
                BookId = bookId,
            };

            await _favoritesRepository.InsertAsync(newFav);
            await _favoritesRepository.CommitAsync();

            return Ok(new ApiResponse<object>()
            {
                IsSuccess = true,
                Message = "Book added to your favorites successfully"
            });
        }

        [HttpDelete("RemoveFromFavorit")]
        public async Task<IActionResult> RemoveFromFavorit(int bookId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var favorit = await _favoritesRepository.GetOneAsync(f => f.ApplicationUserId == user.Id && f.BookId == bookId);
            if (favorit == null)
            {
                return NotFound(new ApiResponse<object>()
                {
                    IsSuccess = false,
                    Message = "Book is not in your favorites"
                });
            }

            _favoritesRepository.Delete(favorit);
            await _favoritesRepository.CommitAsync();

            return Ok(new ApiResponse<object>()
            {
                IsSuccess = true,
                Message = "Book removed from favorites successfully"
            });
        }
    }
}