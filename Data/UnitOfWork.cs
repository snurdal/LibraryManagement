using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Abstracts;
using Core.Abstracts.IRepositories;
using Data.Contexts;
using Data.Repositories;

namespace Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LibraryDbContext context;
        public UnitOfWork(LibraryDbContext context)
        {
            this.context = context;
        }

        private IAuthorRepository? authorRepository;
        public IAuthorRepository AuthorRepository => authorRepository ?? new AuthorRepository(context); 

        private IBookRepository? bookRepository;
        public IBookRepository BookRepository => bookRepository ?? new BookRepository(context);

        private ICategoryRepository? categoryRepository;
        public ICategoryRepository CategoryRepository => categoryRepository ?? new CategoryRepository(context);

        public async Task CommitAsync()
        {
            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception)
            {
                await DisposeAsync();
            }
        }

        public async ValueTask DisposeAsync()
        {
            await context.DisposeAsync();
        }
    }
}
