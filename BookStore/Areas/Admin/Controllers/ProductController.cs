using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStoreDataAccess.Repository.IRepository;
using BookStoreModels;
using BookStoreModels.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICoverTypeRepository _coverTypeRepository;

        public ProductController(IProductRepository productRepository, 
            ICategoryRepository categoryRepository,
            ICoverTypeRepository coverTypeRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _coverTypeRepository = coverTypeRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            Product product = new Product();

            if (id != null)
            {
                product = await this._productRepository.GetProduct(id.GetValueOrDefault());

                if (product == null)
                {
                    return NotFound();
                }
            }

            // Creating ViewModel for specific page
            ProductViewModel productViewModel = new ProductViewModel();
            productViewModel.Product = product;
            productViewModel.CategoryList = (await _categoryRepository.GetCategories())
                .Select(i => new SelectListItem { 
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
            productViewModel.CoverTypeList = (await _coverTypeRepository.GetCoverTypes())
                .Select(i => new SelectListItem {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });

            return View(productViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.Id == 0)
                {
                    _categoryRepository.Add<Category>(category);
                }
                else
                {
                    _categoryRepository.Update<Category>(category);
                }

                await _categoryRepository.SaveAll();
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }
    }
}