using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Core.Abstracts;
using Core.Abstracts.IServices;
using Core.Concretes.DTOs.Book;
using Core.Concretes.DTOs.Search;
using Core.Concretes.Entities;
using Data;
using Utilities.Results;

namespace Business.Services
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public BookService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        }

        public async Task<IDataResult<BookDetailDTO>> CreateAsync(BookCreateDTO bookDto)
        {
            try
            {
                var author = await _unitOfWork.AuthorRepository.FindFirstAsync(a => a.Id == bookDto.AuthorId);
                if (author == null)
                {
                    return DataResult<BookDetailDTO>.Failed("Selected author not found.");
                }

                var category = await _unitOfWork.CategoryRepository.FindFirstAsync(c => c.Id == bookDto.CategoryId);
                if (category == null)
                {
                    return DataResult<BookDetailDTO>.Failed("Selected category not found.");
                }

                var existingBook = await _unitOfWork.BookRepository.FindFirstAsync(
                    b => b.Title.ToLower() == bookDto.Title.ToLower() && !b.Deleted);

                if (existingBook != null)
                {
                    return DataResult<BookDetailDTO>.Failed("A book with this title already exists.");
                }

                var book = _mapper.Map<Book>(bookDto);
                book.CreateDate = DateTime.Now;

                await _unitOfWork.BookRepository.CreateOneAsync(book);
                await _unitOfWork.CommitAsync();

                var createdBook = await _unitOfWork.BookRepository.FindManyAsync(
                    b => b.Id == book.Id, "Category", "Author");

                var result = _mapper.Map<BookDetailDTO>(createdBook.First());
                return DataResult<BookDetailDTO>.Successful(result, "Book created successfully.");
            }
            catch (Exception ex)
            {
                return DataResult<BookDetailDTO>.Failed($"An error occurred while creating the book: {ex.Message}");
            }
        }

        public async Task<IResult> DeleteAsync(int id)
        {
            try
            {
                var book = await _unitOfWork.BookRepository.FindFirstAsync(
                    b => b.Id == id && !b.Deleted);

                if (book == null)
                {
                    return Result.Failed("Book to be deleted not found.");
                }

                book.Deleted = true;
                book.Active = false;

                await _unitOfWork.BookRepository.UpdateOneAsync(book);
                await _unitOfWork.CommitAsync();

                return Result.Successful("Book deleted successfully.");
            }
            catch (Exception ex)
            {
                return Result.Failed($"An error occurred while deleting the book: {ex.Message}");
            }
        }

        public async Task<IDataResult<List<BookListDTO>>> GetAllAsync()
        {
            try
            {
                var books = await _unitOfWork.BookRepository.FindManyAsync(
                    b => !b.Deleted && b.Active,
                    "Category", "Author");

                var bookDtos = _mapper.Map<List<BookListDTO>>(books);
                return DataResult<List<BookListDTO>>.Successful(bookDtos, "Books retrieved successfully.");
            }
            catch (Exception ex)
            {
                return DataResult<List<BookListDTO>>.Failed($"An error occurred while retrieving the books: {ex.Message}");
            }
        }


        // Combines two expressions for filtering books
        private Expression<Func<Book, bool>> CombineExpressions(
            Expression<Func<Book, bool>> first,
            Expression<Func<Book, bool>> second)
        {
            var parameter = Expression.Parameter(typeof(Book), "b");
            var body = Expression.AndAlso(
                Expression.Invoke(first, parameter),
                Expression.Invoke(second, parameter));

            return Expression.Lambda<Func<Book, bool>>(body, parameter);
        }

        public async Task<IDataResult<List<BookListDTO>>> GetByAuthorAsync(int authorId)
        {
            try
            {
                var books = await _unitOfWork.BookRepository.FindManyAsync(
                    b => b.AuthorId == authorId && !b.Deleted && b.Active,
                    "Category", "Author");

                var bookDtos = _mapper.Map<List<BookListDTO>>(books);
                return DataResult<List<BookListDTO>>.Successful(bookDtos,
                    "Books related to the author retrieved.");
            }
            catch (Exception ex)
            {
                return DataResult<List<BookListDTO>>.Failed(
                    $"An error occurred while retrieving books for the author: {ex.Message}");
            }
        }

        public async Task<IDataResult<List<BookListDTO>>> GetByCategoryAsync(int categoryId)
        {
            try
            {
                var books = await _unitOfWork.BookRepository.FindManyAsync(
                    b => b.CategoryId == categoryId && !b.Deleted && b.Active,
                    "Category", "Author");

                var bookDtos = _mapper.Map<List<BookListDTO>>(books);
                return DataResult<List<BookListDTO>>.Successful(bookDtos,
                    "Books related to the category retrieved.");
            }
            catch (Exception ex)
            {
                return DataResult<List<BookListDTO>>.Failed(
                    $"An error occurred while retrieving books for the category: {ex.Message}");
            }
        }

        public async Task<IDataResult<BookDetailDTO>> GetByIdAsync(int id)
        {
            try
            {
                var book = await _unitOfWork.BookRepository.FindFirstAsync(
                    b => b.Id == id && !b.Deleted && b.Active);

                if (book == null)
                {
                    return DataResult<BookDetailDTO>.Failed("Book not found.");
                }

                var bookWithRelations = await _unitOfWork.BookRepository.FindManyAsync(
                    b => b.Id == id, "Category", "Author");

                var bookDetail = bookWithRelations.FirstOrDefault();
                var bookDto = _mapper.Map<BookDetailDTO>(bookDetail);

                return DataResult<BookDetailDTO>.Successful(bookDto, "Book details retrieved.");
            }
            catch (Exception ex)
            {
                return DataResult<BookDetailDTO>.Failed($"An error occurred while retrieving book details: {ex.Message}");
            }
        }

        public async Task<IDataResult<BookEditDTO>> GetForEditAsync(int id)
        {
            try
            {
                var book = await _unitOfWork.BookRepository.FindFirstAsync(
                    b => b.Id == id && !b.Deleted && b.Active);

                if (book == null)
                {
                    return DataResult<BookEditDTO>.Failed("Book to be edited not found.");
                }

                var bookEditDto = _mapper.Map<BookEditDTO>(book);
                return DataResult<BookEditDTO>.Successful(bookEditDto, "Book information for editing retrieved.");
            }
            catch (Exception ex)
            {
                return DataResult<BookEditDTO>.Failed($"An error occurred while retrieving book editing information: {ex.Message}");
            }
        }

        public async Task<IDataResult<List<BookListDTO>>> SearchAsync(BookSearchDTO searchDto)
        {
            try
            {
                Expression<Func<Book, bool>> filter = b => !b.Deleted && b.Active;

                if (!string.IsNullOrEmpty(searchDto.SearchTerm))
                {
                    var searchTerm = searchDto.SearchTerm.ToLower();
                    filter = CombineExpressions(filter, b =>
                        b.Title.ToLower().Contains(searchTerm) ||
                        b.Description.ToLower().Contains(searchTerm) ||
                        b.Author.FirstName.ToLower().Contains(searchTerm) ||
                        b.Author.LastName.ToLower().Contains(searchTerm));
                }

                if (searchDto.CategoryId.HasValue)
                {
                    filter = CombineExpressions(filter, b => b.CategoryId == searchDto.CategoryId.Value);
                }

                if (searchDto.AuthorId.HasValue)
                {
                    filter = CombineExpressions(filter, b => b.AuthorId == searchDto.AuthorId.Value);
                }

                if (searchDto.PublishYear.HasValue)
                {
                    filter = CombineExpressions(filter, b => b.PublishYear == searchDto.PublishYear.Value);
                }

                var books = await _unitOfWork.BookRepository.FindManyAsync(filter, "Category", "Author");

                var pagedBooks = books
                    .Skip((searchDto.Page - 1) * searchDto.PageSize)
                    .Take(searchDto.PageSize)
                    .ToList();

                var bookDtos = _mapper.Map<List<BookListDTO>>(pagedBooks);
                return DataResult<List<BookListDTO>>.Successful(bookDtos,
                    $"Search results retrieved. {bookDtos.Count} books found.");
            }
            catch (Exception ex)
            {
                return DataResult<List<BookListDTO>>.Failed($"An error occurred while performing the search: {ex.Message}");
            }
        }

        public async Task<IResult> UpdateAsync(BookEditDTO bookDto)
        {
            try
            {
                var existingBook = await _unitOfWork.BookRepository.FindFirstAsync(
                    b => b.Id == bookDto.Id && !b.Deleted);

                if (existingBook == null)
                {
                    return Result.Failed("Book to be updated not found.");
                }

                var duplicateBook = await _unitOfWork.BookRepository.FindFirstAsync(
                    b => b.Title.ToLower() == bookDto.Title.ToLower() &&
                         b.Id != bookDto.Id && !b.Deleted);

                if (duplicateBook != null)
                {
                    return Result.Failed("Another book with this title already exists.");
                }

                _mapper.Map(bookDto, existingBook);
                await _unitOfWork.BookRepository.UpdateOneAsync(existingBook);
                await _unitOfWork.CommitAsync();

                return Result.Successful("Book updated successfully.");
            }
            catch (Exception ex)
            {
                return Result.Failed($"An error occurred while updating the book: {ex.Message}");
            }
        }
    }
}
