using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Concretes.DTOs.Book;
using Core.Concretes.DTOs.Category;
using Utilities.Results;
using static Core.Concretes.DTOs.Author.AuthorDTO;

namespace Core.Abstracts.IServices
{
    public interface IAuthorService
    {
       Task<IDataResult<List<AuthorListDTO>>> GetAllAsync();
        Task<IDataResult<AuthorDetailDTO>> GetByIdAsync(int id);
        Task<IDataResult<AuthorEditDTO>> GetForEditAsync(int id);
        Task<IResult> UpdateAsync(AuthorEditDTO authorDto);
        Task<IResult> DeleteAsync(int id);
        Task<IDataResult<AuthorDetailDTO>> CreateAsync(string firstName, string lastName);
    }
}
