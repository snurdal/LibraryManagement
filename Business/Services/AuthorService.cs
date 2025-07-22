using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Core.Abstracts;
using Core.Abstracts.IServices;
using Core.Concretes.DTOs.Author;
using Core.Concretes.DTOs.Book;
using Core.Concretes.DTOs.Category;
using Core.Concretes.Entities;
using Data;
using Utilities.Results;
using static Core.Concretes.DTOs.Author.AuthorDTO;

namespace Business.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AuthorService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IDataResult<AuthorDetailDTO>> CreateAsync(string firstName, string lastName, string? biography = null, string? photoPath = null)
        {
            try
            {
                // Check if author already exists
                var existingAuthor = await _unitOfWork.AuthorRepository.FindFirstAsync(
                    a => a.FirstName.ToLower() == firstName.ToLower() &&
                         a.LastName.ToLower() == lastName.ToLower() &&
                         a.Active && !a.Deleted);

                if (existingAuthor != null)
                    return DataResult<AuthorDetailDTO>.Successful(_mapper.Map<AuthorDetailDTO>(existingAuthor));

                var author = new Author
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Biography = biography,
                    PhotoPath = photoPath
                };

                await _unitOfWork.AuthorRepository.CreateOneAsync(author);
                await _unitOfWork.CommitAsync();

                var authorDto = _mapper.Map<AuthorDetailDTO>(author);
                return DataResult<AuthorDetailDTO>.Successful(authorDto, "Author created successfully");
            }
            catch (Exception ex)
            {
                return DataResult<AuthorDetailDTO>.Failed($"Error creating author: {ex.Message}");
            }
        }

        public async Task<IResult> DeleteAsync(int id)
        {
            try
            {
                var author = await _unitOfWork.AuthorRepository.FindOneByKeyAsync(id);

                if (author == null || !author.Active || author.Deleted)
                    return Result.Failed("Author not found");

                // Check if author has books
                var hasBooks = await _unitOfWork.BookRepository.AnyAsync(
                    b => b.AuthorId == id && b.Active && !b.Deleted);

                if (hasBooks)
                    return Result.Failed("Cannot delete author with associated books");

                author.Deleted = true;
                author.Active = false;

                await _unitOfWork.AuthorRepository.UpdateOneAsync(author);
                await _unitOfWork.CommitAsync();

                return Result.Successful("Author deleted successfully");
            }
            catch (Exception ex)
            {
                return Result.Failed($"Error deleting author: {ex.Message}");
            }
        }

        public async Task<IDataResult<List<AuthorListDTO>>> GetAllAsync()
        {
            try
            {
                var authors = await _unitOfWork.AuthorRepository.FindManyAsync(
                    a => a.Active && !a.Deleted, "Books");

                var authorDtos = authors.Select(author => new AuthorListDTO
                {
                    Id = author.Id,
                    FirstName = author.FirstName,
                    LastName = author.LastName,
                    FullName = $"{author.FirstName} {author.LastName}",
                    PhotoPath = author.PhotoPath,
                    BookCount = author.Books.Count(b => b.Active && !b.Deleted),
                    CreateDate = author.CreateDate,
                    Books = author.Books
                    .Where(b => b.Active && !b.Deleted)
                    .Select(b => new BookListDTO
                    {
                        Id = b.Id,
                        Title = b.Title,
                        CategoryName = b.Category?.Name ?? "Unknown",
                        AuthorId = b.AuthorId,
                        AuthorFullName = $"{author.FirstName} {author.LastName}",
                        CreateDate = b.CreateDate
                    })
                    .ToList()
                })
                .OrderBy(a => a.FullName)
                .ToList();

                return DataResult<List<AuthorListDTO>>.Successful(authorDtos);
            }
            catch (Exception ex)
            {
                return DataResult<List<AuthorListDTO>>.Failed($"Error getting authors: {ex.Message}");
            }
        }

        public async Task<IDataResult<AuthorDetailDTO>> GetByIdAsync(int id)
        {
            try
            {
                var author = await _unitOfWork.AuthorRepository.FindFirstAsync(
                    a => a.Id == id && a.Active && !a.Deleted);

                if (author == null)
                    return DataResult<AuthorDetailDTO>.Failed("Author not found");

                var books = await _unitOfWork.BookRepository.FindManyAsync(
                    b => b.AuthorId == id && b.Active && !b.Deleted, "Category");

                author.Books = books.ToList();
                var authorDto = _mapper.Map<AuthorDetailDTO>(author);
                return DataResult<AuthorDetailDTO>.Successful(authorDto);
            }
            catch (Exception ex)
            {
                return DataResult<AuthorDetailDTO>.Failed($"Error getting author: {ex.Message}");
            }
            ;
        }

        public async Task<IDataResult<AuthorEditDTO>> GetForEditAsync(int id)
        {
            try
            {
                var author = await _unitOfWork.AuthorRepository.FindFirstAsync(
                    a => a.Id == id && a.Active && !a.Deleted);

                if (author == null)
                    return DataResult<AuthorEditDTO>.Failed("Author not found");

                var authorDto = _mapper.Map<AuthorEditDTO>(author);
                return DataResult<AuthorEditDTO>.Successful(authorDto);
            }
            catch (Exception ex)
            {
                return DataResult<AuthorEditDTO>.Failed($"Error getting author for edit: {ex.Message}");
            }
        }

        public async Task<IResult> UpdateAsync(AuthorEditDTO authorDto)
        {
            try
            {
                var author = await _unitOfWork.AuthorRepository.FindOneByKeyAsync(authorDto.Id);

                if (author == null || !author.Active || author.Deleted)
                    return Result.Failed("Author not found");

                _mapper.Map(authorDto, author);
                await _unitOfWork.AuthorRepository.UpdateOneAsync(author);
                await _unitOfWork.CommitAsync();

                return Result.Successful("Author updated successfully");
            }
            catch (Exception ex)
            {
                return Result.Failed($"Error updating author: {ex.Message}");
            }
        }
    }
}