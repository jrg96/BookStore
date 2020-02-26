using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStoreDataAccess.Repository.IRepository;
using BookStoreDataAccess.Repository.Page;
using BookStoreModels;
using BookStoreModels.DTO;
using BookStoreModels.ViewModels;
using BookStoreUtility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ROLE_ADMIN)]
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
        public async Task<IActionResult> Upsert(ProductViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {
                if (productViewModel.Product.Id == 0)
                {
                    _productRepository.Add<Product>(productViewModel.Product);
                }
                else
                {
                    _productRepository.Update<Product>(productViewModel.Product);
                }

                await _productRepository.SaveAll();
                return RedirectToAction(nameof(Index));
            }

            return View(productViewModel);
        }


        /*
         * ------------------------------------------------------------------------------
         * API INTERNA DE LA APLICACION
         * ------------------------------------------------------------------------------
         */

        // GET api/products/datatable
        [HttpGet("/Admin/Product/datatable")]
        public async Task<IActionResult> GetProductsDatatable([FromQuery]DatatableForSelectDTO datatableForSelectDTO)
        {
            // Convert to UserPageParams the API knows
            var userPageParams = new UserPageParams();
            userPageParams.PageNumber = (int)Math.Ceiling(datatableForSelectDTO.Start / (double)datatableForSelectDTO.Length) + 1;
            userPageParams.PageSize = datatableForSelectDTO.Length;


            // Get Data
            var products = await _productRepository.GetProduts(userPageParams);

            // Wrap result in a format Datatable Knows
            var result = new DatatableResponseDTO();
            result.aaData = products;
            result.draw = datatableForSelectDTO.Draw;
            result.iTotalDisplayRecords = products.TotalCount;
            result.iTotalRecords = products.TotalCount;

            Response.AddPagination(products.CurrentPage, products.PageSize, products.TotalCount, products.TotalPages);
            return Ok(result);
        }

        // DELETE api/products/1
        [HttpDelete("/Admin/Product/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            /*
             * ---------------------------------------------------------------------------
             * ZONA DE VALIDACION
             * ---------------------------------------------------------------------------
             */
            // Chequear si el producto existe
            var productDB = await _productRepository.GetProduct(id);

            if (productDB == null)
            {
                throw new Exception($"Product with id {id} does not exist");
            }


            /*
             * --------------------------------------------------------------------------
             * ZONA DE PROCESAMIENTO DE LA PETICION
             * --------------------------------------------------------------------------
             */
            _productRepository.Delete<Product>(productDB);
            await _productRepository.SaveAll();
            return NoContent();
        }
    }
}