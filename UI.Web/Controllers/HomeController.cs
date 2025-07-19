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
            // Initialize search DTO if null
            searchDto ??= new BookSearchDTO();

            // Get books based on search criteria
            var booksResult = string.IsNullOrEmpty(searchDto.SearchTerm) &&
                             !searchDto.CategoryId.HasValue &&
                             !searchDto.AuthorId.HasValue &&
                             !searchDto.PublishYear.HasValue
                ? await _bookService.GetAllAsync()
                : await _bookService.SearchAsync(searchDto);

            if (!booksResult.Success)
            {
                TempData["Error"] = booksResult.Message;
                return View(new List<Core.Concretes.DTOs.Book.BookListDTO>());
            }

            // Get authors and categories for filter dropdowns
            await PrepareViewData();

            ViewBag.SearchDto = searchDto;
            return View(booksResult.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Search(BookSearchDTO searchDto)
        {
            return await Index(searchDto);
        }

        private async Task PrepareViewData()
        {
            // Get all authors for dropdown
            var authorsResult = await _authorService.GetAllAsync();
            if (authorsResult.Success)
            {
                ViewBag.Authors = authorsResult.Data.Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.FullName
                }).ToList();
            }
            else
            {
                ViewBag.Authors = new List<SelectListItem>();
            }

            // Get all categories for dropdown
            var categoriesResult = await _categoryService.GetAllAsync();
            if (categoriesResult.Success)
            {
                ViewBag.Categories = categoriesResult.Data.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();
            }
            else
            {
                ViewBag.Categories = new List<SelectListItem>();
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
