using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContactApi.Models;
using ContactApi.Convertor;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using ContactApi.ViewModel;

namespace ContactApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ContactContext _context;

        public UsersController(ContactContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserLoginDto loginDto)
        {
            if (ModelState.IsValid)
            {
                // var user = await _userManager.FindByEmailAsync(login.Email);
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);
                if (user != null)
                {
                    var passHash = MD5Hash.MD5Hashing(loginDto.Password!);
                    if (passHash == user.PasswordHash)
                    {
                        // Login is successful
                        // Generate JWT
                        return CreateJWTToken(user);
                    }
                }
            }
            return BadRequest("Invalid login data");
        }

        [Authorize]
        [HttpPost("register")]
        public async Task<ActionResult<string>> Register(UserRegisterDto userDto)
        {
            var role = HttpContext.User.FindFirstValue("role");
            if (role != "admin")
            {
                return BadRequest("Only admin can register new users");
            }
            if (ModelState.IsValid)
            {
                // Check if username already exists
                if (_context.Users.Any(u => u.Username == userDto.Username))
                {
                    return BadRequest("Username already exists");
                }

                var user = new User
                {
                    PasswordHash = MD5Hash.MD5Hashing(userDto.Password!),
                    Role = userDto.Role,
                    Username = userDto.Username
                };

                // Add the new user to the database
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return CreateJWTToken(user);
            }

            return BadRequest("Invalid user data");
        }

        [Authorize]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.User.FindFirstValue("userId");

                // Find user by reset token
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);

                if (user == null)
                {
                    return BadRequest("user not found");
                }

                if (user.PasswordHash == MD5Hash.MD5Hashing(resetPasswordDto.OldPassword!))
                {
                    user.PasswordHash = MD5Hash.MD5Hashing(resetPasswordDto.NewPassword!);

                    _context.Entry(user).State = EntityState.Modified;

                    await _context.SaveChangesAsync();

                    return Ok("Password reset successful.");

                }
                else
                {
                    BadRequest("Password is not");
                }
            }

            return BadRequest("Invalid token or missing information.");
        }


        private static string CreateJWTToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("my very good jwt_secret for testing purpose"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddDays(10);

            var claims = new[]
            {
                new Claim("userId", user.Id.ToString()!),
                new Claim("role", user.Role!),
            };

            var Sectoken = new JwtSecurityToken(expires: expires, signingCredentials: credentials, audience: "jwt_issuer", issuer: "jwt_issuer", claims: claims);

            var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);

            return token;
        }
    }
}
