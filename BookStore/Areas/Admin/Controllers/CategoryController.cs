﻿using System;
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
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            Category category = new Category();

            if (id == null)
            {
                return View(category);
            }

            category = await this._categoryRepository.GetCategory(id.GetValueOrDefault());

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
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

        /*
         * ------------------------------------------------------------------------------
         * API INTERNA DE LA APLICACION
         * ------------------------------------------------------------------------------
         */
        [HttpGet("/Admin/Category/datatable")]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryRepository.GetCategories();

            return Ok(categories);
        }

        // DELETE api/categories/id
        [HttpDelete("/Admin/Category/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetCategory(id);

            if (category == null)
            {
                throw new Exception("Username with ID does not exists");
            }

            _categoryRepository.Delete<Category>(category);
            await _categoryRepository.SaveAll();
            return NoContent();
        }
    }
}