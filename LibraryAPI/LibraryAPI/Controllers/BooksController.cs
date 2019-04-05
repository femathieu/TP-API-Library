using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly LibraryContext _context;

        public BooksController(LibraryContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retourne la liste de tous les livres
        /// </summary>
        /// <returns>200 la liste des livres</returns>
        // GET: api/Books
        [HttpGet]
        public IEnumerable<Books> GetBooks()
        {
            return _context.Books;
        }

        /// <summary>
        /// Retourn un libre
        /// </summary>
        /// <param name="id">l'id du livre a retourner</param>
        /// <returns>400 paramètres incorrects</returns>
        /// <returns>404 livre introuvable</returns>
        /// <returns>200 le livre trouvé</returns>
        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooks([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var books = await _context.Books.FindAsync(id);

            if (books == null)
            {
                return NotFound();
            }

            return Ok(books);
        }

        /// <summary>
        /// Modifie un livre
        /// </summary>
        /// <param name="id">l'id du livre a modifier</param>
        /// <param name="books">les nouvelles données du livre</param>
        /// <returns>400 paramètres invalides ou livre non trouvé</returns>
        /// <returns>404 livre non trouvé</returns>
        /// <returns>204 les modifications se sont effectuées</returns>
        // PUT: api/Books/5
        [Authorize (Roles ="admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBooks([FromRoute] int id, [FromBody] Books books)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != books.Id)
            {
                return BadRequest();
            }

            _context.Entry(books).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BooksExists(id))
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
        /// Ajouter un nouveau livre
        /// </summary>
        /// <param name="books">les données du livre à ajouter</param>
        /// <returns>400 les paramètres sont invalides</returns>
        /// <returns>201 livre ajouté avec les données du livre ajouté</returns>
        // POST: api/Books
        [Authorize (Roles ="admin")]
        [HttpPost]
        public async Task<IActionResult> PostBooks([FromBody] Books books)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Books.Add(books);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBooks", new { id = books.Id }, books);
        }

        /// <summary>
        /// Supprime un livre
        /// </summary>
        /// <param name="id">l'id du livre a supprimer</param>
        /// <returns>400 paramètres invalides</returns>
        /// <returns>404 livre non trouvé</returns>
        /// <returns>200 livre supprimé avec les données du livre supprimé</returns>
        // DELETE: api/Books/5
        [Authorize (Roles ="admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooks([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var books = await _context.Books.FindAsync(id);
            if (books == null)
            {
                return NotFound();
            }

            _context.Books.Remove(books);
            await _context.SaveChangesAsync();

            return Ok(books);
        }

        private bool BooksExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}