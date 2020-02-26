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

        public ApplicationUserRepository(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
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
    }
}
