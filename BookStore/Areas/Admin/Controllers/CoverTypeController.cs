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
    }
}