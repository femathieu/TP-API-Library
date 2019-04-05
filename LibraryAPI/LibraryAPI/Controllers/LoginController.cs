using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly LibraryContext _context;
        private readonly AppConfig _Config = new AppConfig();

        public LoginController(LibraryContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Connecte un utilisateur
        /// </summary>
        /// <param name="user">l'utilisateur souhaitant se connecter</param>
        /// <returns>400 paramètres invalides</returns>
        /// <returns>404  login ou pwd invalides</returns>
        /// <returns>200 connexion ok renvoie le jwt</returns>
        // POST: api/Users/login
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] Users user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            bool userExist = _context.Users.Any((Users u) => u.Login == user.Login && u.Pwd == user.Pwd);
            if (!userExist)
            {
                return NotFound();
            }

            Users userContext = _context.Users.Where((Users u) => u.Login == user.Login).First();
            HttpContext.Session.SetString("_userId", userContext.Id.ToString());
            HttpContext.Session.SetString("_role", userContext.Role);
            return Ok(generateToken(userContext));
        }

        private string generateToken(Users user)
        {
            var claims = new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("Svc", "Informatique"),
                new Claim(JwtRegisteredClaimNames.Exp, new
                       DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Nbf, new
                       DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString())
            };

            var token = new JwtSecurityToken(
                new JwtHeader(new SigningCredentials(_Config.getSecretPassPhrase(), SecurityAlgorithms.HmacSha256)),
                new JwtPayload(claims)
            );

            string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return jwtToken;
        }
    }
}
