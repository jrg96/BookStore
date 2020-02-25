using BookStore.DataAccess.Data;
using BookStoreDataAccess.Repository.IRepository;
using BookStoreDataAccess.Repository.Page;
using BookStoreModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreDataAccess.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
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

        public async Task<Product> GetProduct(int id)
        {
            return await _context.Products.Include(x => x.Category).Include(x => x.CoverType)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PagedList<Product>> GetProduts(UserPageParams userPageParams)
        {
            // Joining the CoverType and Category
            var products = _context.Products.Include(x => x.Category).Include(x => x.CoverType);


            return await PagedList<Product>.CreateAsync(products, userPageParams.PageNumber, userPageParams.PageSize);
        }

        public Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
