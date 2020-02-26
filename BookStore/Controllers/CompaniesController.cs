using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BookStoreDataAccess.Repository.IRepository;
using BookStoreDataAccess.Repository.Page;
using BookStoreModels;
using BookStoreModels.DTO;
using BookStoreModels.DTO.Company;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

        public CompaniesController(ICompanyRepository companyRepository, IMapper mapper)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
        }


        // GET api/companies
        [HttpGet]
        public async Task<IActionResult> GetCompanies([FromQuery]UserPageParams userPageParams)
        {
            var companies = await _companyRepository.GetCompanies(userPageParams);

            Response.AddPagination(companies.CurrentPage, companies.PageSize, companies.TotalCount, companies.TotalPages);
            return Ok(companies);
        }

        // GET api/companies/datatable
        [HttpGet("datatable")]
        public async Task<IActionResult> GetCompaniesDatatable([FromQuery]DatatableForSelectDTO datatableForSelectDTO)
        {
            // Convert to UserPageParams the API knows
            var userPageParams = new UserPageParams();
            userPageParams.PageNumber = (int)Math.Ceiling(datatableForSelectDTO.Start / (double)datatableForSelectDTO.Length) + 1;
            userPageParams.PageSize = datatableForSelectDTO.Length;


            // Get Data
            var companies = await _companyRepository.GetCompanies(userPageParams);

            // Wrap result in a format Datatable Knows
            var result = new DatatableResponseDTO();
            result.aaData = companies;
            result.draw = datatableForSelectDTO.Draw;
            result.iTotalDisplayRecords = companies.TotalCount;
            result.iTotalRecords = companies.TotalCount;

            Response.AddPagination(companies.CurrentPage, companies.PageSize, companies.TotalCount, companies.TotalPages);
            return Ok(result);
        }

        // GET api/companies/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompany(int id)
        {
            var company = await _companyRepository.GetCompany(id);

            if (company == null)
            {
                throw new Exception($"Company with id {id} does not exist");
            }

            return Ok(company);
        }


        // POST api/companies
        [HttpPost]
        public async Task<IActionResult> InsertCompany(CompanyForInsertDTO companyForInsertDTO)
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
            var company = new Company();
            _mapper.Map(companyForInsertDTO, company);

            // Paso 2: Insertar al repositorio
            _companyRepository.Add(company);
            await _companyRepository.SaveAll();

            // Paso 3: retornamos respuesta 
            return Ok();
        }


        // PUT api/products/id
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CompanyForUpdateDTO companyForUpdateDTO)
        {
            /*
             * ---------------------------------------------------------------------------
             * ZONA DE VALIDACION
             * ---------------------------------------------------------------------------
             */
            // Chequear si el producto existe
            var companyDB = await _companyRepository.GetCompany(id);

            if (companyDB == null)
            {
                throw new Exception($"Company with id {id} does not exist");
            }


            /*
             * --------------------------------------------------------------------------
             * ZONA DE PROCESAMIENTO DE LA PETICION
             * --------------------------------------------------------------------------
             */
            _mapper.Map(companyForUpdateDTO, companyDB);

            if (await _companyRepository.SaveAll())
            {
                return NoContent();
            }

            throw new Exception($"Error while updating company with id: {id}");
        }

        // DELETE api/products/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            /*
             * ---------------------------------------------------------------------------
             * ZONA DE VALIDACION
             * ---------------------------------------------------------------------------
             */
            // Chequear si el producto existe
            var companyDB = await _companyRepository.GetCompany(id);

            if (companyDB == null)
            {
                throw new Exception($"Company with id {id} does not exist");
            }


            /*
             * --------------------------------------------------------------------------
             * ZONA DE PROCESAMIENTO DE LA PETICION
             * --------------------------------------------------------------------------
             */
            _companyRepository.Delete<Company>(companyDB);
            await _companyRepository.SaveAll();
            return NoContent();
        }
    }
}