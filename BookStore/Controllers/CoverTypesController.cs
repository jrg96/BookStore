using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BookStoreDataAccess.Repository.IRepository;
using BookStoreModels;
using BookStoreModels.DTO.CoverType;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var covers = await _coverTypeRepository.GetCoverTypes();

            return Ok(covers);
        }

        // GET api/covertypes/id
        [AllowAnonymous]
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "InsertEditDeletePolicy")]
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "InsertEditDeletePolicy")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CoverTypeForUpdateDTO coverTypeForUpdateDTO)
        {
            /*
             * ---------------------------------------------------------------------------
             * ZONA DE VALIDACION
             * ---------------------------------------------------------------------------
             */
            var coverDB = await _coverTypeRepository.GetCoverType(id);

            if (coverDB == null)
            {
                throw new Exception($"Cover with id {id} does not exist");
            }

            /*
             * --------------------------------------------------------------------------
             * ZONA DE PROCESAMIENTO DE LA PETICION
             * --------------------------------------------------------------------------
             */
            _mapper.Map(coverTypeForUpdateDTO, coverDB);

            if (await _coverTypeRepository.SaveAll())
            {
                return NoContent();
            }

            throw new Exception($"Error while updating cover with id: {id}");
        }

        // DELETE api/covertypes/id
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "InsertEditDeletePolicy")]
        [HttpDelete("{id}")]
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