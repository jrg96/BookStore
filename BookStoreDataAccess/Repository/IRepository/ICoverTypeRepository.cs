using BookStoreModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreDataAccess.Repository.IRepository
{
    public interface ICoverTypeRepository
    {
        void Add<T>(T entity) where T : class;

        void Delete<T>(T entity) where T : class;

        void Update<T>(T entity) where T : class;

        Task<bool> SaveAll();

        Task<IEnumerable<CoverType>> GetCoverTypes();

        Task<CoverType> GetCoverType(int id);
    }
}
