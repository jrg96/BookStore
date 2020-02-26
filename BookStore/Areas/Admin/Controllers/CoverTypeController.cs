using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStoreDataAccess.Repository.IRepository;
using BookStoreModels;
using BookStoreUtility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ROLE_ADMIN)]
    public class CoverTypeController : Controller
    {
        private readonly ICoverTypeRepository _coverTypeRepository;

        public CoverTypeController(ICoverTypeRepository coverTypeRepository)
        {
            _coverTypeRepository = coverTypeRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            CoverType coverType = new CoverType();

            if (id == null)
            {
                return View(coverType);
            }

            coverType = await this._coverTypeRepository.GetCoverType(id.GetValueOrDefault());

            if (coverType == null)
            {
                return NotFound();
            }

            return View(coverType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(CoverType cover)
        {
            if (ModelState.IsValid)
            {
                if (cover.Id == 0)
                {
                    _coverTypeRepository.Add<CoverType>(cover);
                }
                else
                {
                    _coverTypeRepository.Update<CoverType>(cover);
                }

                await _coverTypeRepository.SaveAll();
                return RedirectToAction(nameof(Index));
            }

            return View(cover);
        }



        /*
         * ------------------------------------------------------------------------------
         * API INTERNA DE LA APLICACION
         * ------------------------------------------------------------------------------
         */
        // GET api/covertypes
        [HttpGet("/Admin/CoverType/datatable")]
        public async Task<IActionResult> GetAll()
        {
            var covers = await _coverTypeRepository.GetCoverTypes();

            return Ok(covers);
        }

        // DELETE api/covertypes/id
        [HttpDelete("/Admin/CoverType/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            /*
             * ---------------------------------------------------------------------------
             * ZONA DE VALIDACION
             * ---------------------------------------------------------------------------
             */
            // Chequear si el cover existe
            var cover = await _coverTypeRepository.GetCoverType(id);

            if (cover == null)
            {
                throw new Exception($"Cover with id {id} does not exist");
            }

            /*
             * --------------------------------------------------------------------------
             * ZONA DE PROCESAMIENTO DE LA PETICION
             * --------------------------------------------------------------------------
             */
            _coverTypeRepository.Delete<CoverType>(cover);
            await _coverTypeRepository.SaveAll();
            return NoContent();
        }
    }
}