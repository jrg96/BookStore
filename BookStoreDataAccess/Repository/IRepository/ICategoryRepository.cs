using BookStoreModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreDataAccess.Repository.IRepository
{
    public interface ICategoryRepository
    {
        void Add<T>(T entity) where T : class;

        void Delete<T>(T entity) where T : class;

        void Update<T>(T entity) where T : class;

        Task<bool> SaveAll();

        Task<IEnumerable<Category>> GetCategories();

        Task<Category> GetCategory(int id);
    }
}
