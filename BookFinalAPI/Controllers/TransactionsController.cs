using BookFinalAPI.Data;
using BookFinalAPI.DTOs;
using BookFinalAPI.Models;
using BookFinalAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BookFinalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IOTPService _otpService;

        public TransactionsController(ApplicationDbContext db, IOTPService otpService)
        {
            _db = db;
            _otpService = otpService;
        }

        // Step 1: Request a book (no OTP yet)
        [HttpPost("request")]
        public async Task<IActionResult> RequestBook([FromBody] RequestBookDto dto)
        {
            var book = await _db.Books.FindAsync(dto.BookId);
            if (book == null) return NotFound("Book not found");

            var tx = new BookTransaction
            {
                BookId = dto.BookId,
                FromUserId = "1",
                ToUserId = dto.ToUserId,
                Price = dto.Price,
                RentUntil = dto.RentUntil
            };

            _db.BookTransactions.Add(tx);
            await _db.SaveChangesAsync();

            return Ok(new { transactionId = tx.Id, message = "Request created. OTP will be generated later." });
        }

        // Step 2: Send OTP when owner is ready to handover
        [HttpPost("{id:int}/send-otp")]
        public async Task<IActionResult> SendOtp(Guid id)
        {
            var tx = await _db.BookTransactions.FindAsync(id);
            if (tx == null) return NotFound("Transaction not found");

            await _otpService.SendOtpAsync(tx.ToUserId); // ToUserId = receiver's mobile/whatsapp

            return Ok(new { transactionId = tx.Id, message = "OTP sent to receiver" });
        }

        // Step 3: Confirm handover with OTP
        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmTransaction([FromBody] ConfirmTransactionDto dto)
        {
            var tx = await _db.BookTransactions.FindAsync(dto.TransactionId);
            if (tx == null) return NotFound("Transaction not found");

            var isValid = await _otpService.VerifyOtpAsync(tx.ToUserId, dto.Otp);
            if (!isValid) return BadRequest("Invalid OTP");

            tx.IsConfirmed = true;
            tx.ConfirmedAt = DateTime.UtcNow;

            // Update book availability
            var book = await _db.Books.FindAsync(tx.BookId);
            if (book != null)
            {
                if (tx.Mode == "Sell" || tx.Mode == "Donate")
                {
                    book.IsActive = false;
                }
                else if (tx.Mode == "Rent" && dto != null)
                {
                    //book.RentUntil = tx.RentUntil;
                }
            }

            await _db.SaveChangesAsync();
            return Ok(new { message = "Transaction confirmed successfully", transactionId = tx.Id });
        }


    }
}
