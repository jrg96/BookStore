using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStoreDataAccess.Repository.IRepository;
using BookStoreModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BookStoreModels.DTO;
using BookStoreModels.DTO.ApplicationUser;
using AutoMapper;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IApplicationUserRepository _applicationUserReposiory;
        private readonly IMapper _mapper;

        public AuthController(IApplicationUserRepository applicationUserRepository, IMapper mapper)
        {
            _applicationUserReposiory = applicationUserRepository;
            _mapper = mapper;
        }

        // POST api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(AppUserForCommonInsertDTO appUserForCommonInsertDTO)
        {
            /*
             * ---------------------------------------------------------------------------
             * ZONA DE VALIDACION
             * ---------------------------------------------------------------------------
             */

            // Verificamos si la contraseña y confirmar son iguales
            if (appUserForCommonInsertDTO.Password != appUserForCommonInsertDTO.ConfirmPassword)
            {
                throw new Exception("Confirm password must match password");
            }

            /*
             * --------------------------------------------------------------------------
             * ZONA DE PROCESAMIENTO DE LA PETICION
             * --------------------------------------------------------------------------
             */

            // Paso 1: Crear la entidad final y mapear con DTO
            ApplicationUser appUser = new ApplicationUser();
            _mapper.Map(appUserForCommonInsertDTO, appUser);

            // Paso 2: Insertamos el usuario
            await _applicationUserReposiory.AddAsNormalUser<ApplicationUser>(appUser, appUserForCommonInsertDTO.Password);

            // Paso 3: Retornamos mensaje de exito
            return Ok();
        }
    }
}