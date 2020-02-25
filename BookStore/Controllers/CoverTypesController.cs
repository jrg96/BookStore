using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public CoverTypesController(ICoverTypeRepository coverTypeRepository, IMapper mapper)
        {
            _coverTypeRepository = coverTypeRepository;
            _mapper = mapper;
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

            if (cover == null)
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
            var cover = new CoverType();
            _mapper.Map(coverTypeForInsertDTO, cover);

            _coverTypeRepository.Add<CoverType>(cover);
            await _coverTypeRepository.SaveAll();
            return Ok();
        }

        // PUT api/covertypes/id
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CoverTypeForUpdateDTO coverTypeForUpdateDTO)
        {
            var coverDB = await _coverTypeRepository.GetCoverType(id);
            _mapper.Map(coverTypeForUpdateDTO, coverDB);

            if (await _coverTypeRepository.SaveAll())
            {
                return NoContent();
            }

            throw new Exception($"Error while updating cover with id: {id}");
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