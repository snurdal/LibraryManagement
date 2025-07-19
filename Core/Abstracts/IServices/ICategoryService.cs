using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Concretes.DTOs.Category;
using Utilities.Results;

namespace Core.Abstracts.IServices
{
    public interface ICategoryService
    {
       Task<IDataResult<List<CategoryListDTO>>> GetAllAsync();
        Task<IDataResult<CategoryDetailDTO>> GetByIdAsync(int id);
        Task<IDataResult<CategoryEditDTO>> GetForEditAsync(int id);
        Task<IDataResult<CategoryDetailDTO>> CreateAsync(CategoryCreateDTO categoryDto);
        Task<IResult> UpdateAsync(CategoryEditDTO categoryDto);
        Task<IResult> DeleteAsync(int id);
        Task<IDataResult<CategoryDetailDTO>> CreateAsync(string name);
    }
}
