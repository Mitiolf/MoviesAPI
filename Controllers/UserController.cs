using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Filmy.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;


namespace Filmy.Controllers
{
    [Route("user")]
    [ApiController]



    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;

        private readonly MyBoardsContext _context;
        public UserController(IConfiguration config, MyBoardsContext context)
        {
            _config = config;
            _context = context;
        }


        #region Login
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserDTO login)
        {
            IActionResult response = Unauthorized();
            var user = AuthenticateUser(login);
            if (user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                Response.Cookies.Append("X-Access-Token", tokenString);
                response = Ok();
            }
            return response;
        }


        private string GenerateJSONWebToken(UserModel userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>() { 
                new Claim("userId", userInfo.Id.ToString()) 
            };
            if(userInfo.Username == "Wojtek")
            {
                claims.Add(new Claim(ClaimTypes.Role, "Administrator"));
            }

            var token = new JwtSecurityToken(
              issuer: _config["Jwt:Issuer"],
              audience: _config["Jwt:Issuer"],
              claims: claims,
              expires: DateTime.Now.AddMinutes(60),
              signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private UserModel AuthenticateUser(UserDTO login)
        {
            //Validate the User Credentials      
            var user = _context.Users.Where(user => login.Username == user.Username).FirstOrDefault();
            if(user == null) { return null; }
                if (Hash.VerifyHash(login.Password, user.Password))
                {
                    return user;
                }
            return null;
        }
        #endregion

        #region Register
        [HttpPost("register")]
        public IActionResult Register([FromBody] UserDTO register)
        {
            try
            {
                string passwordHash =  Hash.ComputeHash(register.Password);
                var user = new UserModel() { Id = Guid.NewGuid(), Username = register.Username, Password = passwordHash };
                var tokenString = GenerateJSONWebToken(user);
                Response.Cookies.Append("X-Access-Token", tokenString);
                _context.Users.Add(user);

                _context.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion
    }
}