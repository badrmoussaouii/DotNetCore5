using DotNetCore5.Models;
using DotNetCore5.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore5.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies.ToListAsync();
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
            var AllowedExtentions = new List<string> { ".png", ".jpg" };

            if (!AllowedExtentions.Contains(Path.GetExtension(Poster.FileName).ToLower()))
            {
                movie.Genres = await _context.Genres.OrderBy(x => x.Name).ToListAsync();
                ModelState.AddModelError("Poster", "N'accepte que les extensions .jpg .png");
                return View("MovieForm", movie);
            }

            if (Poster.Length > 1048576)
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

            Movie.Title = movie.Title;
            Movie.Storeline = movie.Storeline;
            Movie.Rate = movie.Rate;
            Movie.Poster = movie.Poster;
            Movie.Year = movie.Year;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

