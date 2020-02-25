using BookStore.DataAccess.Data;
using BookStoreDataAccess.Repository.IRepository;
using BookStoreModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreDataAccess.Repository
{
    class CoverTypeRepository : ICoverTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public CoverTypeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public void Update<T>(T entity) where T : class
        {
            _context.Entry<T>(entity).State = EntityState.Modified;
        }

        public async Task<CoverType> GetCoverType(int id)
        {
            return await _context.CoverTypes.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<CoverType>> GetCoverTypes()
        {
            return await _context.CoverTypes.ToListAsync();
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
