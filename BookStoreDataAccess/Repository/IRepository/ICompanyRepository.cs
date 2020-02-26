using BookStoreDataAccess.Repository.Page;
using BookStoreModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreDataAccess.Repository.IRepository
{
    public interface ICompanyRepository
    {
        void Add<T>(T entity) where T : class;

        void Delete<T>(T entity) where T : class;

        void Update<T>(T entity) where T : class;

        Task<bool> SaveAll();

        Task<PagedList<Company>> GetCompanies(UserPageParams userPageParams);

        Task<IEnumerable<Company>> GetAllCompanies();

        Task<Company> GetCompany(int id);
    }
}
