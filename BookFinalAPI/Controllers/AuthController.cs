using AutoMapper;
using BookFinalAPI.DTOs;
using BookFinalAPI.DTOs.Auth;
using BookFinalAPI.Models;
using BookFinalAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookFinalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IOTPService _otpService;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        public AuthController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOTPService otpService,
            IConfiguration config,
            IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _otpService = otpService;
            _config = config;
            _mapper = mapper;
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpDto dto)
        {
            await _otpService.SendOtpAsync(dto.Mobile);
            return Ok(new { message = "OTP sent (or static OTP active)." });
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            var valid = await _otpService.VerifyOtpAsync(dto.Mobile, dto.Otp);
            if (!valid) return BadRequest("Invalid OTP");

            var user = await _userManager.FindByNameAsync(dto.Mobile);
            if (user == null)
                return NotFound("User not registered. Please register first.");

            var token = GenerateJwtToken(user);
            return Ok(new { token, user = _mapper.Map<UserDto>(user) });
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("pincode", user.Pincode ?? "")
            };

            var roles = _userManager.GetRolesAsync(user).Result;
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var existing = await _userManager.FindByNameAsync(dto.Mobile);
            if (existing != null) return BadRequest("User already exists");

            var user = new ApplicationUser
            {
                UserName = dto.Mobile,
                PhoneNumber = dto.Mobile,
                FullName = dto.FullName,
                Pincode = dto.Pincode
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded) return BadRequest(result.Errors);

            // assign default roles
            foreach (var role in dto.Roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole(role));

                await _userManager.AddToRoleAsync(user, role);
            }

            return Ok(new { message = "User registered successfully" });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            return Ok(_mapper.Map<UserDto>(user));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Mobile);
            if (user == null)
                return NotFound("User not registered. Please register first.");

            var valid = await _otpService.VerifyOtpAsync(dto.Mobile, dto.Otp);
            if (!valid) return BadRequest("Invalid OTP");

            var token = GenerateJwtToken(user);

            // Map user → UserDto
            var userDto = _mapper.Map<UserDto>(user);
            userDto.Roles = (await _userManager.GetRolesAsync(user)).ToList();

            return Ok(new { token, user = userDto });
        }
    }
}
