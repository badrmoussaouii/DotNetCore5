using DotNetCore5.Models;
using DotNetCore5.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore5.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toast;
        private List<string> AllowedExtentions = new List<string> { ".png", ".jpg" };
        private long MaxAllowedPosterSize = 1048576;

        public MoviesController(ApplicationDbContext context, IToastNotification toast)
        {
            _context = context;
            _toast = toast;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies.OrderByDescending(m => m.Rate).ToListAsync();
            return View(movies);
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new MovieFormViewModel
            {
                Genres = await _context.Genres.OrderBy(x => x.Name).ToListAsync()
            };

            return View("MovieForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieFormViewModel movie)
        {
            if (!ModelState.IsValid)
            {
                movie.Genres = await _context.Genres.OrderBy(x => x.Name).ToListAsync();
                return View("MovieForm", movie);
            }
            var files = Request.Form.Files;

            if (!files.Any())
            {
                movie.Genres = await _context.Genres.OrderBy(x => x.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Veuiller selectionner une image");
                return View("MovieForm", movie);
            }

            var Poster = files.FirstOrDefault();
            

            if (!AllowedExtentions.Contains(Path.GetExtension(Poster.FileName).ToLower()))
            {
                movie.Genres = await _context.Genres.OrderBy(x => x.Name).ToListAsync();
                ModelState.AddModelError("Poster", "N'accepte que les extensions .jpg .png");
                return View("MovieForm", movie);
            }

            if (Poster.Length > MaxAllowedPosterSize)
            {
                movie.Genres = await _context.Genres.OrderBy(x => x.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Taille du fichier > 1MB");
                return View("MovieForm", movie);
            }

            using var dataStream = new MemoryStream();

            await Poster.CopyToAsync(dataStream);

            var Movie = new Movie
            {
                Title = movie.Title,
                GenreId = movie.GenreId,
                Rate = movie.Rate,
                Storeline = movie.Storeline,
                Year = movie.Year,
                Poster = dataStream.ToArray()
            };

            await _context.Movies.AddAsync(Movie);
            await _context.SaveChangesAsync();

            _toast.AddSuccessToastMessage("Film crée avec succée");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? Id)
        {
            if(Id == null)
            {
                return BadRequest();
            }

            var movie = await _context.Movies.FindAsync(Id);

            if(movie == null)
            {
                return NotFound();
            }

            var viewModel = new MovieFormViewModel
            {
                Id = movie.Id,
                GenreId = movie.GenreId,
                Poster = movie.Poster,
                Rate = movie.Rate,
                Storeline = movie.Storeline,
                Title = movie.Title,
                Year = movie.Year,
                Genres = await _context.Genres.OrderBy(x => x.Name).ToListAsync()
            };

            return View("MovieForm", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(MovieFormViewModel movie)
        {
            if (!ModelState.IsValid)
            {
                movie.Genres = await _context.Genres.OrderBy(x => x.Name).ToListAsync();
                return View("MovieForm", movie);
            }

            var Movie = await _context.Movies.FindAsync(movie.Id);

            if (Movie == null)
            {
                return NotFound();
            }

            var files = Request.Form.Files;

            if (files.Any())
            {
                var poster = files.FirstOrDefault();
                using var dataStream = new MemoryStream();

                await poster.CopyToAsync(dataStream);

                movie.Poster = dataStream.ToArray();

                if (!AllowedExtentions.Contains(Path.GetExtension(poster.FileName).ToLower()))
                {
                    movie.Genres = await _context.Genres.OrderBy(x => x.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "N'accepte que les extensions .jpg .png");
                    return View("MovieForm", movie);
                }

                if (poster.Length > MaxAllowedPosterSize)
                {
                    movie.Genres = await _context.Genres.OrderBy(x => x.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "Taille du fichier > 1MB");
                    return View("MovieForm", movie);
                }

                Movie.Poster = movie.Poster;
            }

            Movie.Title = movie.Title;
            Movie.Storeline = movie.Storeline;
            Movie.Rate = movie.Rate;
            //Movie.Poster = movie.Poster;
            Movie.Year = movie.Year;

            await _context.SaveChangesAsync();

            _toast.AddSuccessToastMessage("Film Modifié avec succée");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? Id)
        {
            if(Id == null)
            {
                return BadRequest();
            }

            var movie = await _context.Movies.Include(m => m.Genre).SingleOrDefaultAsync(x=>x.Id == Id);

            if(movie == null)
            {
                return NotFound();
            }

            return View(movie);

        }

        public async Task<IActionResult> Delete(int? Id)
        {
            if (Id == null)
            {
                return BadRequest();
            }

            var movie = await _context.Movies.FindAsync(Id);

            if (movie == null)
            {
                return NotFound();
            }

             _context.Movies.Remove(movie);

            await _context.SaveChangesAsync();

            return Ok();

        }
    }
}

