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
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IApplicationUserRepository _applicationUserReposiory;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthController(IApplicationUserRepository applicationUserRepository, IMapper mapper,
            IConfiguration config, UserManager<IdentityUser> userManager)
        {
            _applicationUserReposiory = applicationUserRepository;
            _mapper = mapper;
            _config = config;
            _userManager = userManager;
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


        // POST api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(AppUserForLoginDTO appUserForLoginDTO)
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

            // Paso 1: Realizamos el login
            var user = await _applicationUserReposiory.Login(appUserForLoginDTO.UserName, appUserForLoginDTO.Password);

            // Paso 2: Verificamos si el login fue exitoso
            if (user != null)
            {
                return Ok(new {
                    user,
                    token = await GenerateToken(user)
                });
            }

            return Unauthorized();
        }

        private async Task<string> GenerateToken(IdentityUser user)
        {
            // Creando los claims del token JWT
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}