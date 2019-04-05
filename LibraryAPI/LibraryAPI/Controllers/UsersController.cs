using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryAPI.Models;
using System.Net.Http;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly LibraryContext _context;
        private readonly AppConfig _Config = new AppConfig();

        public UsersController(LibraryContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retourne la liste de tous les utilisateurs
        /// </summary>
        /// <returns>200 avec la liste des utilisateurs</returns>
        // GET: api/Users
        [Authorize(Roles = "admin")]
        [HttpGet]
        public IEnumerable<Users> GetUsers()
        {
            return _context.Users;
        }

        /// <summary>
        /// Retourne un utilisateur
        /// </summary>
        /// <param name="id">id de l'utilisateur a retourner</param>
        /// <returns>400 si le paramètre est invalide</returns>
        /// <returns>404 si l'utilisateur n'est pas trouvé</returns>
        /// <returns>200 avec l'utilisateur sis tout s'est bien passé</returns>
        // GET: api/Users/5
        [Authorize (Roles ="user, admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsers([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var users = await _context.Users.FindAsync(id);

            if (!(users.Id.ToString() == AppConfig.getTokenClaims(HttpContext, ClaimTypes.NameIdentifier)) && 
                (AppConfig.getTokenClaims(HttpContext, ClaimTypes.Role) != "admin")
                )
            {
                return BadRequest();
            }

            if (users == null)
            {
                return NotFound();
            }

            return Ok(users);
        }

        /// <summary>
        /// Modifie un utilisateur donné
        /// </summary>
        /// <param name="id">id de l'utilisateur à modifier</param>
        /// <param name="users">nouveaux éléments concercant l'utilisateur à modifier</param>
        /// <returns>404 si l'utilisateur n'est pas trouvé</returns>
        /// <returns>400 si les paramètres donnés sont invalides</returns>
        /// <returns>204 si tout s'est bien passé</returns>
        // PUT: api/Users/5
        [Authorize (Roles ="user, admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsers([FromRoute] int id, [FromBody] Users users)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != users.Id)
            {
                return BadRequest();
            }

            if (users.Id.ToString() != AppConfig.getTokenClaims(HttpContext, ClaimTypes.NameIdentifier) &&
                AppConfig.getTokenClaims(HttpContext, ClaimTypes.Role )!= "admin")
            {
                return BadRequest();
            }

            _context.Entry(users).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsersExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Ajoute un nouvel utilisateur
        /// </summary>
        /// <param name="users">L'utilisateur a ajouté</param>
        /// <returns>201 l'ajout s'est effectué</returns>
        /// <returns>400 une erreur s'est produite</returns>
        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> PostUsers([FromBody] Users users)
        {
            if (!ModelState.IsValid && UsersExistsWithEmail(users.Email) && UsersExistsWithLogin(users.Login))
            {
                return BadRequest();
            }

            users.Role = "user";
            _context.Users.Add(users);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsers", new { id = users.Id }, users);
        }

        /// <summary>
        /// Supprime un utilisateur
        /// </summary>
        /// <param name="id">id de l'utilisateur a supprimer</param>
        /// <returns>400 si les paramètre donnés sont invalides</returns>
        /// <returns>404 si l'utilisateur n'est pas trouvé</returns>
        /// <returns>200 avec l'utilisateur supprimé si la suppression s'est effectué</returns>
        // DELETE: api/Users/5
        [Authorize (Roles ="admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsers([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var users = await _context.Users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }

            _context.Users.Remove(users);
            await _context.SaveChangesAsync();

            return Ok(users);
        }

        private bool UsersExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        private bool UsersExistsWithEmail(string email)
        {
            return _context.Users.Any(e => e.Email == email);
        }

        private bool UsersExistsWithLogin(string login)
        {
            return _context.Users.Any(e => e.Login == login);
        }
    }
}