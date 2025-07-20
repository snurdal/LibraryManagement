using Core.Abstracts.IServices;
using Microsoft.AspNetCore.Mvc;
using UI.Web.Helpers;
using UI.Web.Models.Author;

namespace UI.Web.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly IAuthorService _authorService;
        private readonly IFileUploadService _fileUploadService;

        public AuthorsController(IAuthorService authorService, IFileUploadService fileUploadService)
        {
            _authorService = authorService;
            _fileUploadService = fileUploadService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _authorService.GetAllAsync();
            var viewModel = new AuthorListViewModel();

            if (result.Success)
                viewModel.Authors = result.Data;

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var result = await _authorService.GetByIdAsync(id);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("Index");
            }

            var viewModel = new AuthorDetailViewModel
            {
                Author = result.Data
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var result = await _authorService.GetForEditAsync(id);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("Index");
            }

            var viewModel = new AuthorEditViewModel
            {
                Author = result.Data
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AuthorEditViewModel viewModel)
        {
            try
            {
                // Handle photo upload
                if (viewModel.PhotoFile != null)
                {
                    var photoPath = await _fileUploadService.UploadFileAsync(viewModel.PhotoFile, "authors");
                    if (!string.IsNullOrEmpty(photoPath))
                    {
                        // Delete old photo if exists
                        if (!string.IsNullOrEmpty(viewModel.Author.PhotoPath))
                            _fileUploadService.DeleteFile(viewModel.Author.PhotoPath);

                        viewModel.Author.PhotoPath = photoPath;
                    }
                }

                var result = await _authorService.UpdateAsync(viewModel.Author);
                if (result.Success)
                {
                    TempData["Success"] = result.Message;
                    return RedirectToAction("Details", new { id = viewModel.Author.Id });
                }

                TempData["Error"] = result.Message;
                return View(viewModel);
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return View(viewModel);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _authorService.DeleteAsync(id);
            if (result.Success)
                TempData["Success"] = result.Message;
            else
                TempData["Error"] = result.Message;

            return RedirectToAction("Index");
        }
    }
}
