using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Core.Abstracts;
using Core.Abstracts.IServices;
using Core.Concretes.DTOs.Book;
using Core.Concretes.DTOs.Category;
using Core.Concretes.Entities;
using Data;
using Utilities.Results;

namespace Business.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IDataResult<CategoryDetailDTO>> CreateAsync(CategoryCreateDTO categoryDto)
        {
            try
            {
                // Check if category already exists
                var existingCategory = await _unitOfWork.CategoryRepository.FindFirstAsync(
                    c => c.Name.ToLower() == categoryDto.Name.ToLower() && c.Active && !c.Deleted);

                if (existingCategory != null)
                    return DataResult<CategoryDetailDTO>.Failed("Category already exists");

                var category = _mapper.Map<Category>(categoryDto);
                await _unitOfWork.CategoryRepository.CreateOneAsync(category);
                await _unitOfWork.CommitAsync();

                var createdCategoryDto = _mapper.Map<CategoryDetailDTO>(category);
                return DataResult<CategoryDetailDTO>.Successful(createdCategoryDto, "Category created successfully");
            }
            catch (Exception ex)
            {
                return DataResult<CategoryDetailDTO>.Failed($"Error creating category: {ex.Message}");
            }
        }

        public async Task<IDataResult<CategoryDetailDTO>> CreateAsync(string name)
        {
            try
            {
                // Check if category already exists
                var existingCategory = await _unitOfWork.CategoryRepository.FindFirstAsync(
                    c => c.Name.ToLower() == name.ToLower() && c.Active && !c.Deleted);

                if (existingCategory != null)
                    return DataResult<CategoryDetailDTO>.Successful(_mapper.Map<CategoryDetailDTO>(existingCategory));

                var category = new Category { Name = name };
                await _unitOfWork.CategoryRepository.CreateOneAsync(category);
                await _unitOfWork.CommitAsync();

                var categoryDto = _mapper.Map<CategoryDetailDTO>(category);
                return DataResult<CategoryDetailDTO>.Successful(categoryDto, "Category created successfully");
            }
            catch (Exception ex)
            {
                return DataResult<CategoryDetailDTO>.Failed($"Error creating category: {ex.Message}");
            }
        }

        public async Task<IResult> DeleteAsync(int id)
        {
            try
            {
                var category = await _unitOfWork.CategoryRepository.FindOneByKeyAsync(id);

                if (category == null || !category.Active || category.Deleted)
                    return Result.Failed("Category not found");

                // Check if category has books
                var hasBooks = await _unitOfWork.BookRepository.AnyAsync(
                    b => b.CategoryId == id && b.Active && !b.Deleted);

                if (hasBooks)
                    return Result.Failed("Cannot delete category with associated books");

                category.Deleted = true;
                category.Active = false;

                await _unitOfWork.CategoryRepository.UpdateOneAsync(category);
                await _unitOfWork.CommitAsync();

                return Result.Successful("Category deleted successfully");
            }
            catch (Exception ex)
            {
                return Result.Failed($"Error deleting category: {ex.Message}");
            }
            ;
        }

        public async Task<IDataResult<List<CategoryListDTO>>> GetAllAsync()
        {
            try
            {
                var categories = await _unitOfWork.CategoryRepository
                    .FindManyAsync(c => c.Active && !c.Deleted, "Books.Author");

                var categoryDtos = categories.Select(category => new CategoryListDTO
                {
                    Id = category.Id,
                    Name = category.Name,
                    CreateDate = category.CreateDate,
                    Books = category.Books
                        .Where(b => b.Active && !b.Deleted)
                        .Select(b => new BookListDTO
                        {
                            Id = b.Id,
                            Title = b.Title,
                            CategoryName = b.Category.Name,
                            AuthorFullName = b.Author != null
                        ? $"{b.Author.FirstName} {b.Author.LastName}"
                        : "Unknown Author", // null control
                            ShortDescription = !string.IsNullOrEmpty(b.Description)
                        ? (b.Description.Length > 150
                            ? b.Description.Substring(0, 150) + "..."
                            : b.Description)
                        : "No description",
                            AuthorId = b.AuthorId
                        }).ToList()
                })
                .OrderBy(c => c.Name)
                .ToList();

                return DataResult<List<CategoryListDTO>>.Successful(categoryDtos);
            }
            catch (Exception ex)
            {
                return DataResult<List<CategoryListDTO>>.Failed($"Error getting categories: {ex.Message}");
            }
        }

        public async Task<IDataResult<CategoryDetailDTO>> GetByIdAsync(int id)
        {
            try
            {
                var category = await _unitOfWork.CategoryRepository.FindFirstAsync(
                    c => c.Id == id && c.Active && !c.Deleted);

                if (category == null)
                    return DataResult<CategoryDetailDTO>.Failed("Category not found");

                var books = await _unitOfWork.BookRepository.FindManyAsync(
                    b => b.CategoryId == id && b.Active && !b.Deleted, "Author");

                category.Books = books.ToList();
                var categoryDto = _mapper.Map<CategoryDetailDTO>(category);
                return DataResult<CategoryDetailDTO>.Successful(categoryDto);
            }
            catch (Exception ex)
            {
                return DataResult<CategoryDetailDTO>.Failed($"Error getting category: {ex.Message}");
            }
        }

        public async Task<IDataResult<CategoryEditDTO>> GetForEditAsync(int id)
        {
            try
            {
                var category = await _unitOfWork.CategoryRepository.FindFirstAsync(
                    c => c.Id == id && c.Active && !c.Deleted);

                if (category == null)
                    return DataResult<CategoryEditDTO>.Failed("Category not found");

                var categoryDto = _mapper.Map<CategoryEditDTO>(category);
                return DataResult<CategoryEditDTO>.Successful(categoryDto);
            }
            catch (Exception ex)
            {
                return DataResult<CategoryEditDTO>.Failed($"Error getting category for edit: {ex.Message}");
            }
        }

        public async Task<IResult> UpdateAsync(CategoryEditDTO categoryDto)
        {
            try
            {
                var category = await _unitOfWork.CategoryRepository.FindOneByKeyAsync(categoryDto.Id);

                if (category == null || !category.Active || category.Deleted)
                    return Result.Failed("Category not found");

                // Check if another category with same name exists
                var existingCategory = await _unitOfWork.CategoryRepository.FindFirstAsync(
                    c => c.Name.ToLower() == categoryDto.Name.ToLower() &&
                         c.Id != categoryDto.Id && c.Active && !c.Deleted);

                if (existingCategory != null)
                    return Result.Failed("Another category with this name already exists");

                _mapper.Map(categoryDto, category);
                await _unitOfWork.CategoryRepository.UpdateOneAsync(category);
                await _unitOfWork.CommitAsync();

                return Result.Successful("Category updated successfully");
            }
            catch (Exception ex)
            {
                return Result.Failed($"Error updating category: {ex.Message}");
            }
        }
    }
}
