using Core.Abstracts.IServices;
using Microsoft.AspNetCore.Mvc;
using UI.Web.Models.Book;

namespace UI.Web.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ICategoryService _categoryService;
        private readonly IAuthorService _authorService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BooksController(IBookService bookService, ICategoryService categoryService,
            IAuthorService authorService, IWebHostEnvironment webHostEnvironment)
        {
            _bookService = bookService;
            _categoryService = categoryService;
            _authorService = authorService;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Details(int id)
        {
            var bookResult = await _bookService.GetByIdAsync(id);
            if (!bookResult.Success)
            {
                TempData["Error"] = bookResult.Message;
                return RedirectToAction("Index", "Home");
            }

            // Get related books by same author
            var relatedBooksResult = await _bookService.GetByAuthorAsync(bookResult.Data.Author.Id);
            var relatedBooks = relatedBooksResult.Success ?
                relatedBooksResult.Data.Where(b => b.Id != id).Take(4).ToList() :
                new List<Core.Concretes.DTOs.Book.BookListDTO>();

            var viewModel = new BookDetailViewModel
            {
                Book = bookResult.Data,
                RelatedBooks = relatedBooks
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new BookCreateViewModel();

            var categoriesResult = await _categoryService.GetAllAsync();
            var authorsResult = await _authorService.GetAllAsync();

            if (categoriesResult.Success)
                viewModel.Categories = categoriesResult.Data;

            if (authorsResult.Success)
                viewModel.Authors = authorsResult.Data;

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BookCreateViewModel viewModel, IFormFile? coverImage)
        {
            if (!ModelState.IsValid)
            {
                await LoadCreateViewModelData(viewModel);
                return View(viewModel);
            }

            // Handle new category creation
            if (!string.IsNullOrEmpty(viewModel.NewCategoryName))
            {
                var categoryResult = await _categoryService.CreateAsync(viewModel.NewCategoryName);
                if (categoryResult.Success)
                    viewModel.Book.CategoryId = categoryResult.Data.Id;
            }

            // Handle new author creation
            if (!string.IsNullOrEmpty(viewModel.NewAuthorFirstName) && !string.IsNullOrEmpty(viewModel.NewAuthorLastName))
            {
                var authorResult = await _authorService.CreateAsync(viewModel.NewAuthorFirstName, viewModel.NewAuthorLastName);
                if (authorResult.Success)
                    viewModel.Book.AuthorId = authorResult.Data.Id;
            }

            // Handle cover image upload
            if (coverImage != null && coverImage.Length > 0)
            {
                var imagePath = await SaveImageAsync(coverImage);
                viewModel.Book.CoverImagePath = imagePath;
            }

            var result = await _bookService.CreateAsync(viewModel.Book);
            if (result.Success)
            {
                TempData["Success"] = result.Message;
                return RedirectToAction("Details", new { id = result.Data.Id });
            }

            TempData["Error"] = result.Message;
            await LoadCreateViewModelData(viewModel);
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var bookResult = await _bookService.GetForEditAsync(id);
            if (!bookResult.Success)
            {
                TempData["Error"] = bookResult.Message;
                return RedirectToAction("Index", "Home");
            }

            var viewModel = new BookEditViewModel
            {
                Book = bookResult.Data
            };

            var categoriesResult = await _categoryService.GetAllAsync();
            var authorsResult = await _authorService.GetAllAsync();

            if (categoriesResult.Success)
                viewModel.Categories = categoriesResult.Data;

            if (authorsResult.Success)
                viewModel.Authors = authorsResult.Data;

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BookEditViewModel viewModel, IFormFile? coverImage)
        {
            if (!ModelState.IsValid)
            {
                await LoadEditViewModelData(viewModel);
                return View(viewModel);
            }

            // Handle cover image upload
            if (coverImage != null && coverImage.Length > 0)
            {
                var imagePath = await SaveImageAsync(coverImage);
                viewModel.Book.CoverImagePath = imagePath;
            }

            var result = await _bookService.UpdateAsync(viewModel.Book);
            if (result.Success)
            {
                TempData["Success"] = result.Message;
                return RedirectToAction("Details", new { id = viewModel.Book.Id });
            }

            TempData["Error"] = result.Message;
            await LoadEditViewModelData(viewModel);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _bookService.DeleteAsync(id);
            if (result.Success)
                TempData["Success"] = result.Message;
            else
                TempData["Error"] = result.Message;

            return RedirectToAction("Index", "Home");
        }

        private async Task LoadCreateViewModelData(BookCreateViewModel viewModel)
        {
            var categoriesResult = await _categoryService.GetAllAsync();
            var authorsResult = await _authorService.GetAllAsync();

            if (categoriesResult.Success)
                viewModel.Categories = categoriesResult.Data;

            if (authorsResult.Success)
                viewModel.Authors = authorsResult.Data;
        }

        private async Task LoadEditViewModelData(BookEditViewModel viewModel)
        {
            var categoriesResult = await _categoryService.GetAllAsync();
            var authorsResult = await _authorService.GetAllAsync();

            if (categoriesResult.Success)
                viewModel.Categories = categoriesResult.Data;

            if (authorsResult.Success)
                viewModel.Authors = authorsResult.Data;
        }

        private async Task<string> SaveImageAsync(IFormFile image)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "books");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return "/images/books/" + uniqueFileName;
        }
    }
}
