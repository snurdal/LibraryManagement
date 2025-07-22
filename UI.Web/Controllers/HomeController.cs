using System.Diagnostics;
using Core.Abstracts.IServices;
using Core.Concretes.DTOs.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UI.Web.Models;

namespace UI.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;
        private readonly ICategoryService _categoryService;

        public HomeController(IBookService bookService, IAuthorService authorService, ICategoryService categoryService)
        {
            _bookService = bookService;
            _authorService = authorService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(BookSearchDTO? searchDto = null)
        {
            searchDto ??= new BookSearchDTO();

            var booksResult = string.IsNullOrEmpty(searchDto.SearchTerm) &&
                             !searchDto.CategoryId.HasValue &&
                             !searchDto.AuthorId.HasValue &&
                             !searchDto.PublishYear.HasValue
                ? await _bookService.GetAllAsync()
                : await _bookService.SearchAsync(searchDto);

            ViewBag.Authors = await GetAuthorsSelectList();
            ViewBag.Categories = await GetCategoriesSelectList();
            ViewBag.SearchDto = searchDto;

            if (!booksResult.Success)
            {
                ViewBag.ErrorMessage = booksResult.Message;
                return View(new List<Core.Concretes.DTOs.Book.BookListDTO>());
            }

            return View(booksResult.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Search(BookSearchDTO searchDto)
        {
            var result = await _bookService.SearchAsync(searchDto);

            if (result.Success)
            {
                ViewBag.SearchDto = searchDto;
                ViewBag.Authors = await GetAuthorsSelectList();
                ViewBag.Categories = await GetCategoriesSelectList();
                return View("Index", result.Data);
            }

            ViewBag.SearchDto = searchDto;
            ViewBag.Authors = await GetAuthorsSelectList();
            ViewBag.Categories = await GetCategoriesSelectList();
            ViewBag.ErrorMessage = result.Message;

            return View("Index", new List<Core.Concretes.DTOs.Book.BookListDTO>());
        }


        private async Task<List<SelectListItem>> GetAuthorsSelectList()
        {
            try
            {
                var authorsResult = await _authorService.GetAllAsync();
                if (authorsResult.Success)
                {
                    return authorsResult.Data.Select(a => new SelectListItem
                    {
                        Value = a.Id.ToString(),
                        Text = $"{a.FirstName} {a.LastName}"
                    }).ToList();
                }
                return new List<SelectListItem>();
            }
            catch
            {
                return new List<SelectListItem>();
            }
        }

        private async Task<List<SelectListItem>> GetCategoriesSelectList()
        {
            try
            {
                var categoriesResult = await _categoryService.GetAllAsync();
                if (categoriesResult.Success)
                {
                    return categoriesResult.Data.Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name 
                    }).ToList();
                }
                return new List<SelectListItem>();
            }
            catch
            {
                return new List<SelectListItem>();
            }
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }


    }
}
