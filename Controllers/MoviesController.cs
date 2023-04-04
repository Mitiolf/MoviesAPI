using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Filmy.Models;
using System;


namespace Filmy.Controllers
{
    [Route("movies")]
    [ApiController]
    [Authorize]
    public class MoviesController : ControllerBase
    {

        private readonly MyBoardsContext _context;
        public MoviesController( MyBoardsContext context)
        {
            _context = context;
        }



        #region getMoviesList
        [HttpGet("")]
        [AllowAnonymous]
        public IEnumerable<MovieModel> Get()
        {
            return _context.Movies;
        }
        #endregion



        #region addMovie
        [HttpPost("add")]
        public IActionResult Add([FromBody] MovieModel model)
        {

            try
            {
                _context.Movies.Add(new MovieModel() { Title = model.Title, Description = model.Description, Director = model.Director });
                _context.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
        #endregion


        #region deleteMovie
        [HttpDelete("delete/{title}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Delete([FromRoute] string title)
        {
            try
            {
                var movieToDelete = _context.Movies
                       .Where(p => p.Title == title)
                       .FirstOrDefault();
                if (movieToDelete is MovieModel) {
                    _context.Movies.Remove(movieToDelete);
                    _context.SaveChanges();
                }
                else { 
                    throw new Exception("Movie don't exist"); 
                }

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        #endregion



        #region editMovie
        [HttpPut("edit")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Edit([FromBody] MovieModel model)
        {
            try
            {
                var movieToEdit = _context.Movies
                    .Where(p => p.Title == model.Title)
                    .FirstOrDefault();

                if (movieToEdit is MovieModel)
                {
                    movieToEdit.Description = model.Description == null ? movieToEdit.Description : model.Description;
                    movieToEdit.Director = model.Director == null ? movieToEdit.Director : model.Director;
                }
                else {
                    throw new Exception("Movie don't exist");
                }

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


