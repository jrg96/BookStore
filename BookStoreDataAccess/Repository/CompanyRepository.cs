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
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyRepository(ApplicationDbContext context)
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

        public async Task<PagedList<Company>> GetCompanies(UserPageParams userPageParams)
        {
            // Joining the CoverType and Category
            var companies = _context.Companies.AsQueryable();
            return await PagedList<Company>.CreateAsync(companies, userPageParams.PageNumber, userPageParams.PageSize);
        }

        public async Task<IEnumerable<Company>> GetAllCompanies()
        {
            return await _context.Companies.ToListAsync();
        }

        public async Task<Company> GetCompany(int id)
        {
            return await _context.Companies.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
