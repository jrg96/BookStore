using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStoreDataAccess.Repository.IRepository;
using BookStoreModels;
using BookStoreModels.DTO.CoverType;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoverTypesController : ControllerBase
    {
        private readonly ICoverTypeRepository _coverTypeRepository;

        public CoverTypesController(ICoverTypeRepository coverTypeRepository)
        {
            _coverTypeRepository = coverTypeRepository;
        }

        // GET api/covertypes
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var covers = await _coverTypeRepository.GetCoverTypes();

            return Ok(covers);
        }

        // GET api/covertypes/id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCover(int id)
        {
            var cover = await _coverTypeRepository.GetCoverType(id);

            if (cover != null)
            {
                throw new Exception($"Cover with id {id} does not exist");
            }

            return Ok(cover);
        }

        // POST api/covertypes/id
        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] CoverTypeForInsertDTO coverTypeForInsertDTO)
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
            var cover = new CoverType
            {
                Name = coverTypeForInsertDTO.Name
            };

            _coverTypeRepository.Add<CoverType>(cover);
            return Ok();
        }

        // DELETE api/covertypes/id
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cover = await _coverTypeRepository.GetCoverType(id);

            if (cover == null)
            {
                throw new Exception($"Cover with id {id} does not exist");
            }

            _coverTypeRepository.Delete<CoverType>(cover);
            await _coverTypeRepository.SaveAll();
            return NoContent();
        }
    }
}