using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStoreDataAccess.Repository.IRepository;
using BookStoreDataAccess.Repository.Page;
using BookStoreModels.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // GET api/products
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery]UserPageParams userPageParams)
        {
            var products = await _productRepository.GetProduts(userPageParams);

            Response.AddPagination(products.CurrentPage, products.PageSize, products.TotalCount, products.TotalPages);
            return Ok(products);
        }

        [HttpGet("datatable")]
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
    }
}