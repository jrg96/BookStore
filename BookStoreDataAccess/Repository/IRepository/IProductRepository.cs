using BookStoreDataAccess.Repository.Page;
using BookStoreModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreDataAccess.Repository.IRepository
{
    public interface IProductRepository
    {
        void Add<T>(T entity) where T : class;

        void Delete<T>(T entity) where T : class;

        void Update<T>(T entity) where T : class;

        Task<bool> SaveAll();

        Task<PagedList<Product>> GetProduts(UserPageParams userPageParams);

        Task<Product> GetProduct(int id);
    }
}
