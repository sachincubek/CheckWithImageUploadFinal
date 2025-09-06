using AutoMapper;
using BookFinalAPI.Data;
using BookFinalAPI.DTOs;
using BookFinalAPI.Models;
using BookFinalAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookFinalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinary;
        private readonly IOTPService _otpService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<BooksController> _logger;

        public BooksController(
          ApplicationDbContext db,
          IMapper mapper,
          ICloudinaryService cloudinary,
          IOTPService otpService,
          UserManager<ApplicationUser> userManager,
          ILogger<BooksController> logger)
        {
            _db = db;
            _mapper = mapper;
            _cloudinary = cloudinary;
            _otpService = otpService;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost("CreateBook")]
        public async Task<IActionResult> CreateBook([FromForm] CreateBookDto dto)
        {
            try
            {
                // OTP check per transaction (optional)
               // if (!await _otpService.VerifyOtpAsync(dto.Mobile, dto.Otp))
                    //return BadRequest("OTP verification failed");

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var book = new Book();
                book.OwnerId = userId;
                book.CreatedAt = DateTime.UtcNow;

                if (dto.Image != null)
                {
                    var (url, publicId) = await _cloudinary.UploadFileAsync(dto.Image);
                    book.ImageUrl = url;
                }

                _db.Books.Add(book);
                await _db.SaveChangesAsync();

                _logger.LogInformation("Book {BookId} created by {UserId} at {Time}", book.Id, userId, DateTime.UtcNow);


                return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, _mapper.Map<BookDto>(book));
            }
            catch (AutoMapper.AutoMapperMappingException ex)
            {
                // Log full error
                _logger.LogError(ex, "AutoMapper mapping failed. Source type: {Source}, Destination type: {Dest}",
                    ex.TypeMap?.SourceType, ex.TypeMap?.DestinationType);

                // Optionally, inspect inner exception for more details
                if (ex.InnerException != null)
                    _logger.LogError(ex.InnerException, "Inner exception: {Message}", ex.InnerException.Message);

                // Return detailed error in API (optional, not for production)
                return BadRequest(new
                {
                    Error = "Mapping failed",
                    Message = ex.Message,
                    SourceType = ex.TypeMap?.SourceType?.Name,
                    DestinationType = ex.TypeMap?.DestinationType?.Name,
                    Inner = ex.InnerException?.Message
                });
            }
            catch (Exception ex)
            {
                // Catch any other unexpected exception
                _logger.LogError(ex, "Unexpected error during mapping");
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetBooks([FromQuery] string? category, [FromQuery] string? pincode)
        {
            var query = _db.Books.Include(b => b.Category).AsQueryable();

            if (!string.IsNullOrEmpty(category))
                query = query.Where(b => b.Category.Name == category);

            if (!string.IsNullOrEmpty(pincode))
                query = query.Where(b => b.Owner.Pincode == pincode);

            var books = await query.OrderByDescending(b => b.CreatedAt).ToListAsync();

            return Ok(_mapper.Map<IEnumerable<BookDto>>(books));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            var book = await _db.Books.Include(b => b.Category).Include(b => b.Owner)
                        .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null) return NotFound();

            return Ok(_mapper.Map<BookDto>(book));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromForm] UpdateBookDto dto)
        {
            var book = await _db.Books.FindAsync(id);
            if (book == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Only owner or admin can update
            if (book.OwnerId != userId && !User.IsInRole("Admin"))
                return Forbid();

            _mapper.Map(dto, book);

            if (dto.Image != null)
            {
                var (url, publicId) = await _cloudinary.UploadFileAsync(dto.Image);
                book.ImageUrl = url;
            }

            _db.Books.Update(book);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Book {BookId} updated by {UserId} at {Time}", book.Id, userId, DateTime.UtcNow);

            return Ok(_mapper.Map<BookDto>(book));
        }

        // ===================== DELETE BOOK ======================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _db.Books.FindAsync(id);
            if (book == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Only owner or admin can delete
            if (book.OwnerId != userId && !User.IsInRole("Admin"))
                return Forbid();

            _db.Books.Remove(book);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Book {BookId} deleted by {UserId} at {Time}", book.Id, userId, DateTime.UtcNow);

            return NoContent();
        }
    }
}
