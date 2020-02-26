using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BookStoreDataAccess.Repository.IRepository;
using BookStoreDataAccess.Repository.Page;
using BookStoreModels;
using BookStoreModels.DTO;
using BookStoreModels.DTO.Product;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductsController(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        // GET api/products
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery]UserPageParams userPageParams)
        {
            var products = await _productRepository.GetProduts(userPageParams);

            Response.AddPagination(products.CurrentPage, products.PageSize, products.TotalCount, products.TotalPages);
            return Ok(products);
        }


        // GET api/products/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _productRepository.GetProduct(id);

            if (product == null)
            {
                throw new Exception($"Product with id {id} does not exist");
            }

            return Ok(product);
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "InsertEditDeletePolicy")]
        [HttpPost]
        public async Task<IActionResult> InsertProduct(ProductForInsertDTO productForInsertDTO)
        {
            /*
             * ---------------------------------------------------------------------------
             * ZONA DE VALIDACION
             * ---------------------------------------------------------------------------
             */

            /*
             * --------------------------------------------------------------------------
             * ZONA DE PROCESAMIENTO DE LA PETICION
             * --------------------------------------------------------------------------
             */

            // Paso 1: Crear objeto y mapearlo con el DTO
            var product = new Product();
            _mapper.Map(productForInsertDTO, product);

            // Paso 2: Insertar al repositorio
            _productRepository.Add(product);
            await _productRepository.SaveAll();

            // Paso 3: retornamos respuesta 
            return Ok();
        }

        // PUT api/products/id
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "InsertEditDeletePolicy")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductForUpdateDTO productForUpdateDTO)
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
            _mapper.Map(productForUpdateDTO, productDB);

            if (await _productRepository.SaveAll())
            {
                return NoContent();
            }

            throw new Exception($"Error while updating product with id: {id}");
        }


        // DELETE api/products/1
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "InsertEditDeletePolicy")]
        [HttpDelete("{id}")]
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