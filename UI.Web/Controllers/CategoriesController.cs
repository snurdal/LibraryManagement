using Core.Abstracts.IServices;
using Microsoft.AspNetCore.Mvc;
using UI.Web.Models.Category;

namespace UI.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _categoryService.GetAllAsync();
            var viewModel = new CategoryListViewModel();

            if (result.Success)
                viewModel.Categories = result.Data;

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var result = await _categoryService.GetByIdAsync(id);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("Index");
            }

            var viewModel = new CategoryDetailViewModel
            {
                Category = result.Data
            };

            return View(viewModel);
        }

        public IActionResult Create()
        {
            var viewModel = new CategoryCreateViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var result = await _categoryService.CreateAsync(viewModel.Category);
            if (result.Success)
            {
                TempData["Success"] = result.Message;
                return RedirectToAction("Index");
            }

            TempData["Error"] = result.Message;
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var result = await _categoryService.GetForEditAsync(id);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction("Index");
            }

            var viewModel = new CategoryEditViewModel
            {
                Category = result.Data
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var result = await _categoryService.UpdateAsync(viewModel.Category);
            if (result.Success)
            {
                TempData["Success"] = result.Message;
                return RedirectToAction("Details", new { id = viewModel.Category.Id });
            }

            TempData["Error"] = result.Message;
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryService.DeleteAsync(id);
            if (result.Success)
                TempData["Success"] = result.Message;
            else
                TempData["Error"] = result.Message;

            return RedirectToAction("Index");
        }
    }
}
