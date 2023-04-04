using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Filmy.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;






namespace Filmy.Controllers
{
    [Route("collection")]
    [ApiController]
    [Authorize]
    public class CollectionController : ControllerBase
    {
        private readonly MyBoardsContext _context;
        public CollectionController(MyBoardsContext context)
        {
            _context = context;
        }


        private Guid GetUserId()
        {
            var handler = new JwtSecurityTokenHandler();
            string authHeader = Request.Headers["Authorization"];
            authHeader = authHeader.Replace("Bearer ", "");
            var tokenS = handler.ReadToken(authHeader) as JwtSecurityToken;
            string userIdstring = tokenS.Claims.First(claim => claim.Type == "userId").Value;
            var userId = Guid.Parse(userIdstring);
            return userId;
        }

        #region getMoviesList
        [HttpGet("")]
        public IEnumerable<UserCollectionModel> Get()
        {
            var userId = GetUserId();
            var collection = _context.Users.Where(u => u.Id == userId).Include(c => c.UserMovies).ThenInclude(c => c.Movie).FirstOrDefault().UserMovies;


            return collection;
        }
        #endregion



        #region addMovie
        [HttpPost("add")]
        public IActionResult Add([FromBody] UserCollectionDTO model)
        {

            var userId = GetUserId();

            try
            {
                var user = _context.Users.Where(p => p.Id == userId).FirstOrDefault();
                var movie = _context.Movies.Where(p => p.Title == model.Title).FirstOrDefault();
                if (movie == null){ throw new Exception("Movie doesn't exist in our database"); }

                var movieToAdd = new UserCollectionModel() { UserId = userId, MovieId = movie.Id, Rate = model.Rate };
                user.UserMovies.Add(movieToAdd);
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
        public IActionResult Delete([FromRoute] string title)
        {
            var userId = GetUserId();

            try
            {
                var movieId = _context.Movies.Where(m => m.Title == title).FirstOrDefault().Id;


                var movieToDelete = _context.UsersColections
                           .Where(p => p.UserId == userId && p.MovieId == movieId)
                           .FirstOrDefault();

                if (movieToDelete != null)
                {
                    _context.UsersColections.Remove(movieToDelete);
                    _context.SaveChanges();
                    return Ok();
                }
                else
                {
                    throw new Exception("Movie doesn't exist in your catalogue");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        #endregion



        #region editMovie
        [HttpPut("edit")]
        public IActionResult Edit([FromBody] UserCollectionDTO model)
        {

            var userId = GetUserId();

            try
            {
                var movie = _context.Movies.Where(p => p.Title == model.Title).FirstOrDefault();
                if (movie == null) { throw new Exception("Movie doesn't exist in our database"); }

                var movieToEdit = _context.UsersColections
                           .Where(p => p.MovieId == movie.Id && p.UserId == userId)
                           .FirstOrDefault();


                if (movieToEdit != null)
                {
                    movieToEdit.Rate = model.Rate;

                    _context.SaveChanges();
                    return Ok();
                }
                else
                {
                    throw new Exception("Movie doesn't exist in your catalogue");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        #endregion
    }
}
