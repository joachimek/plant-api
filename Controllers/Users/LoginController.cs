﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using plant_api.Data;
using plant_api.Helpers;
using plant_api.Models.User;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace plant_api.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly PlantApiContext _context;
        private IConfiguration _configuration;

        public LoginController(IConfiguration configuration, PlantApiContext _context)
        {
            _configuration = configuration;
            this._context = _context;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<UserLoginResponse>> LoginUser(UserLogin userLogin)
        {
            var user = await Authenticate(userLogin);

            if(user != null)
            {
                var response = new UserLoginResponse() { Token = Generate(user) };
                return Ok(response);
            }

            return Unauthorized();
        }

        private async Task<Models.Users?> Authenticate(UserLogin userLogin)
        {
            string hashedPassword = Cryptography.MD5Hash(userLogin.Password);

            if(_context.Users == null)
            {
                return null;
            }

            var currentUser = await _context.Users.Where(u => u.Username == userLogin.Username && u.Password == hashedPassword).FirstOrDefaultAsync();

            if(currentUser != null)
            {
                return currentUser;
            }

            return null;
        }

        private string Generate(Models.Users user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.EmailAddress),
                new Claim(ClaimTypes.Role, user.Role),
            };

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(100),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
