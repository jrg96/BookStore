using BookStoreDataAccess.Repository.Page;
using BookStoreModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreDataAccess.Repository.IRepository
{
    public interface IApplicationUserRepository
    {
        Task<int> AddAsNormalUser<ApplicationUser>(BookStoreModels.ApplicationUser user, string password);

        void Delete<ApplicationUser>(ApplicationUser user);

        void Update<ApplicationUser>(ApplicationUser user);

        Task<bool> SaveAll();

        Task<PagedList<ApplicationUser>> GetUsers(UserPageParams userPageParams);

        Task<ApplicationUser> GetUser(int id);
    }
}
