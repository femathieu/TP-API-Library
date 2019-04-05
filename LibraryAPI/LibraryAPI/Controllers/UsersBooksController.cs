using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersBooksController : ControllerBase
    {
        private readonly LibraryContext _context;

        public UsersBooksController(LibraryContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retourne toutes les relations entre les utilisateurs et les livres
        /// </summary>
        /// <returns>200 les relations utilisateurs-livres</returns>
        // GET: api/UsersBooks
        [Authorize (Roles ="user, admin")]
        [HttpGet]
        public IEnumerable<UsersBooks> GetUsersBooks()
        {
            return _context.UsersBooks.Where(ub => ub.UsersId.ToString() == AppConfig.getTokenClaims(HttpContext, ClaimTypes.NameIdentifier));
        }

        /// <summary>
        /// Retourne une relation utilisateur-livre
        /// </summary>
        /// <param name="id">l'id de la relation a retourner</param>
        /// <returns>400 paramètres invalides</returns>
        /// <returns>404 relation non trouvée</returns>
        /// <returns>200 relation utilisateur-livre</returns>
        // GET: api/UsersBooks/5
        [Authorize (Roles ="user, admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsersBooks([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usersBooks = await _context.UsersBooks.FindAsync(id);

            if (!(usersBooks.UsersId.ToString() == AppConfig.getTokenClaims(HttpContext, ClaimTypes.NameIdentifier)) &&
                AppConfig.getTokenClaims(HttpContext, ClaimTypes.Role) != "admin"
                )
            {
                return BadRequest();
            }

            if (usersBooks == null)
            {
                return NotFound();
            }

            return Ok(usersBooks);
        }

        /// <summary>
        /// Modifie une relation utilisateur-livre
        /// </summary>
        /// <param name="id">l'id de la reation a modifié</param>
        /// <param name="usersBooks">les données a mettre</param>
        /// <returns>400 paramètres invalides</returns>
        /// <returns>404 relation non trouvée</returns>
        /// <returns>204 la modification s'est effectuée</returns>
        // PUT: api/UsersBooks/5
        [Authorize (Roles ="user, admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsersBooks([FromRoute] int id, [FromBody] UsersBooks usersBooks)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != usersBooks.Id)
            {
                return BadRequest();
            }

            if (!(AppConfig.getTokenClaims(HttpContext, ClaimTypes.NameIdentifier) == usersBooks.UsersId.ToString()) &&
                AppConfig.getTokenClaims(HttpContext, ClaimTypes.Role) != "admin"
            )
            {
                return BadRequest();
            }

            _context.Entry(usersBooks).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsersBooksExists(id))
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
        /// Ajoute une relation utilisateur-livre
        /// </summary>
        /// <param name="usersBooks">donnée de la relation</param>
        /// <returns>400 paramètres invalides</returns>
        /// <returns>409 relation déjà existante</returns>
        /// <returns>201 ajout effectué avec les données ajoutées</returns>
        ///  
        // POST: api/UsersBooks
        [Authorize (Roles ="user, admin")]
        [HttpPost]
        public async Task<IActionResult> PostUsersBooks([FromBody] UsersBooks usersBooks)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(usersBooks.UsersId.ToString() != AppConfig.getTokenClaims(HttpContext, ClaimTypes.NameIdentifier) &&
                AppConfig.getTokenClaims(HttpContext, ClaimTypes.Role) != "admin"
              )
            {
                return BadRequest();
            }

            bool isUser = _context.Users.Any((Users u) => u.Id == usersBooks.UsersId);
            bool isBook = _context.Books.Any((Books b) => b.Id == usersBooks.BooksId);

            try
            {
                if (isUser && isBook)
                {
                    _context.UsersBooks.Add(usersBooks);
                }
                else
                {
                    throw new Exception($"books {usersBooks.BooksId} or user {usersBooks.UsersId} does not exists");
                }
                await _context.SaveChangesAsync();
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UsersBooksExists(usersBooks.Id))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetUsersBooks", new { id = usersBooks.Id }, usersBooks);
        }

        /// <summary>
        /// Supprime une relation utilisateur-livre
        /// </summary>
        /// <param name="id">id de la relation a supprimer</param>
        /// <param name="user">l'utilisateur souhaite effectuer la modification</param>
        /// <returns>400 paramètres invalide</returns>
        /// <returns>404 relation non trouvée</returns>
        /// <returns>200 relation supprimée</returns>
        // DELETE: api/UsersBooks/5
        [Authorize (Roles ="user, admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsersBooks([FromRoute] int id, [FromBody] Users user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usersBooks = await _context.UsersBooks.FindAsync(id);
            if (usersBooks == null)
            {
                return NotFound();
            }

            if(!(usersBooks.UsersId == user.Id))
            {
                return BadRequest();
            }

            _context.UsersBooks.Remove(usersBooks);
            await _context.SaveChangesAsync();

            return Ok(usersBooks);
        }

        private bool UsersBooksExists(int id)
        {
            return _context.UsersBooks.Any(e => e.Id == id);
        }
    }
}