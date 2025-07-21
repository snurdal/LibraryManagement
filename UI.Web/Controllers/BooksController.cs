using Core.Abstracts.IServices;
using Microsoft.AspNetCore.Mvc;
using UI.Web.Helpers;
using UI.Web.Models.Book;

namespace UI.Web.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ICategoryService _categoryService;
        private readonly IAuthorService _authorService;
        private readonly IFileUploadService _fileUploadService;

        public BooksController(IBookService bookService, ICategoryService categoryService,
            IAuthorService authorService, IFileUploadService fileUploadService)
        {
            _bookService = bookService;
            _categoryService = categoryService;
            _authorService = authorService;
            _fileUploadService = fileUploadService;
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
            // Handle new category creation
            if (!string.IsNullOrEmpty(viewModel.NewCategoryName))
            {
                var categoryResult = await _categoryService.CreateAsync(viewModel.NewCategoryName);
                if (categoryResult.Success)
                    viewModel.Book.CategoryId = categoryResult.Data.Id;
                else
                {
                    TempData["Error"] = "Category creation failed: " + categoryResult.Message;
                    await LoadCreateViewModelData(viewModel);
                    return View(viewModel);
                }
            }

            // Handle new author creation
            if (!string.IsNullOrEmpty(viewModel.NewAuthorFirstName) && !string.IsNullOrEmpty(viewModel.NewAuthorLastName))
            {
                var authorResult = await _authorService.CreateAsync(viewModel.NewAuthorFirstName, viewModel.NewAuthorLastName);
                if (authorResult.Success)
                    viewModel.Book.AuthorId = authorResult.Data.Id;
                else
                {
                    TempData["Error"] = "Author creation failed: " + authorResult.Message;
                    await LoadCreateViewModelData(viewModel);
                    return View(viewModel);
                }
            }

            // Validate that we have CategoryId and AuthorId
            if (viewModel.Book.CategoryId <= 0)
            {
                TempData["Error"] = "Please select or create a category.";
                await LoadCreateViewModelData(viewModel);
                return View(viewModel);
            }

            if (viewModel.Book.AuthorId <= 0)
            {
                TempData["Error"] = "Please select or create an author.";
                await LoadCreateViewModelData(viewModel);
                return View(viewModel);
            }

            // Handle cover image upload
            if (coverImage != null && coverImage.Length > 0)
            {
                var imagePath = await _fileUploadService.UploadFileAsync(coverImage, "bookuploads"); // to "uploads/bookuploads" folder 
                if (!string.IsNullOrEmpty(imagePath))
                {
                    viewModel.Book.CoverImagePath = imagePath;
                }
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
                var imagePath = await _fileUploadService.UploadFileAsync(coverImage, "bookuploads");
                if (!string.IsNullOrEmpty(imagePath))
                {
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(viewModel.Book.CoverImagePath))
                        _fileUploadService.DeleteFile(viewModel.Book.CoverImagePath);

                    viewModel.Book.CoverImagePath = imagePath;
                }
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
    }
}
