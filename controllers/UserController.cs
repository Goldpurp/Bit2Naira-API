using Microsoft.AspNetCore.Mvc;
using Bit2Naira_API.datas;
using Bit2Naira_API.models;
using System.Linq;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.AspNetCore.Identity;
using Bit2Naira_API.dtos;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Bit2Naira_API.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _db;
        private readonly PasswordHasher<User> passwordHasher = new();

        public UserController(ApplicationDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        // GET: api/User/get-all-users
        [HttpGet("get-all-users")]
        public IActionResult GetAllUsers()
        {
            var users = _db.Users.Include(w => w.Wallets).ToList();
            return Ok(users);
        }

        [Authorize]
        [HttpGet("logged-in-user")]
        public IActionResult LoggedInUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("Invalid token: user id missing");
            }

            int userId = int.Parse(userIdClaim.Value);

            var user = _db.Users
                .Include(u => u.Wallets)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user);
        }


        [HttpPost("register")]
        public IActionResult Register(RegisterDto request)
        {
            // Check if user already exists
            if (_db.Users.Any(u => u.Email == request.email))
                return BadRequest("User already exists.");

            var newUser = new User
            {
                FullName = request.fullName,
                Email = request.email,
                Password = passwordHasher.HashPassword(new User(), request.password)
            };

            // newUser.Password = 

            _db.Users.Add(newUser);
            _db.SaveChanges();


            var btcWallet = new Wallet
            {
                UserId = newUser.Id,
                Currency = "BTC",
                Balance = 0
            };

            var ngnWallet = new Wallet
            {
                UserId = newUser.Id,
                Currency = "NGN",
                Balance = 0
            };

            _db.Wallets.AddRange(btcWallet, ngnWallet);

            _db.SaveChanges();

            return Ok(new
            {
                Message = "Registration successful",
                newUser.Id,
                Wallet = new[] { btcWallet, ngnWallet }
            });
        }

        [HttpPost("update-user-profile")]
        public IActionResult UpdateProfile([FromForm] UpdateProfileDto request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("Invalid token: user id missing");
            }

            int userId = int.Parse(userIdClaim.Value);
            var user = _db.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return NotFound("user not found");
            }

            if (!string.IsNullOrEmpty(request.fullName))
            {
                user.FullName = request.fullName;
            }
            if (!string.IsNullOrEmpty(request.phoneNumber))
            {
                user.PhoneNumber = request.phoneNumber;
            }
            if (!string.IsNullOrEmpty(request.occupation))
            {
                user.Occupation = request.occupation;
            }
            if (!string.IsNullOrEmpty(request.location))
            {
                user.Location = request.location;
            }

            if (request.profilePicture != null && request.profilePicture.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "avatars");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var filePath = Path.Combine(uploadsFolder, $"user-{user.Id}.png"); // always overwrite
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    request.profilePicture?.CopyTo(stream);
                }
                user.ProfilePicture = $"/avatars/user-{user.Id}.png";
            }

            _db.SaveChanges();
            return Ok(new
            {
                Message = "profile update successfully",
                user
            });
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto request)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == request.email);

            if (user == null)
            {
                return Unauthorized("email is not recorgnized");
            }

            var result = passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized("password error");
            }


            var token = GenerateJwtToken(user);
            return Ok(new
            {
                Message = "login successfully",
                token
            });
        }


        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
    };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
