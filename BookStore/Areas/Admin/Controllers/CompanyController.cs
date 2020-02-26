using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStoreDataAccess.Repository.IRepository;
using BookStoreDataAccess.Repository.Page;
using BookStoreModels;
using BookStoreModels.DTO;
using BookStoreUtility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ROLE_ADMIN)]
    public class CompanyController : Controller
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyController(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            Company company = new Company();

            if (id == null)
            {
                return View(company);
            }

            company = await this._companyRepository.GetCompany(id.GetValueOrDefault());

            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.Id == 0)
                {
                    _companyRepository.Add<Company>(company);
                }
                else
                {
                    _companyRepository.Update<Company>(company);
                }

                await _companyRepository.SaveAll();
                return RedirectToAction(nameof(Index));
            }

            return View(company);
        }


        /*
         * ------------------------------------------------------------------------------
         * API INTERNA DE LA APLICACION
         * ------------------------------------------------------------------------------
         */

        // GET Admin/Company/datatable
        [HttpGet("/Admin/Company/datatable")]
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

        [HttpDelete("/Admin/Company/{id}")]
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