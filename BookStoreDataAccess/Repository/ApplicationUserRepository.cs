using BookStoreDataAccess.Repository.IRepository;
using BookStoreDataAccess.Repository.Page;
using BookStoreModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using BookStoreUtility;

namespace BookStoreDataAccess.Repository
{
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signinManager;

        public ApplicationUserRepository(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signinManager)
        {
            _userManager = userManager;
            _signinManager = signinManager;
        }

        public async Task<int> AddAsNormalUser<ApplicationUser>(BookStoreModels.ApplicationUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                if (user.CompanyId > 0)
                {
                    await _userManager.AddToRoleAsync(user, SD.ROLE_USER_COMPANY);
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, SD.ROLE_USER_INDIVIDUAL);
                }

                return 1;
            }
            return 0;
        }

        public void Delete<ApplicationUser>(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public void Update<ApplicationUser>(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser> GetUser(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<ApplicationUser>> GetUsers(UserPageParams userPageParams)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveAll()
        {
            throw new NotImplementedException();
        }

        public async Task<IdentityUser> Login(string email, string password)
        {
            // Obtener el usuario con email determinado
            var user = await _userManager.FindByEmailAsync(email);
            var result = await _signinManager.CheckPasswordSignInAsync(user, password, false);

            if (result.Succeeded)
            {
                return user;
            }

            return null;
        }
    }
}
