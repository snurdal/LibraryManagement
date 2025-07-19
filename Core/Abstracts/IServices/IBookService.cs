using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Concretes.DTOs.Book;
using Core.Concretes.DTOs.Search;
using Utilities.Results;

namespace Core.Abstracts.IServices
{
    public interface IBookService
    {
        Task<IDataResult<List<BookListDTO>>> GetAllAsync();
        Task<IDataResult<BookDetailDTO>> GetByIdAsync(int id);
        Task<IDataResult<BookEditDTO>> GetForEditAsync(int id);
        Task<IDataResult<BookDetailDTO>> CreateAsync(BookCreateDTO bookDto);
        Task<IResult> UpdateAsync(BookEditDTO bookDto);
        Task<IResult> DeleteAsync(int id);
        Task<IDataResult<List<BookListDTO>>> SearchAsync(BookSearchDTO searchDto);
        Task<IDataResult<List<BookListDTO>>> GetByCategoryAsync(int categoryId);
        Task<IDataResult<List<BookListDTO>>> GetByAuthorAsync(int authorId);
    }
}
