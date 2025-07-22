using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Core.Concretes.DTOs.Book;
using Core.Concretes.DTOs.Category;
using Core.Concretes.Entities;
using static Core.Concretes.DTOs.Author.AuthorDTO;

namespace Core.Concretes.Maps
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region Author Mapping
            CreateMap<Author, AuthorListDTO>()
                    .ForMember(dest => dest.BookCount, opt => opt.MapFrom(src => src.Books.Count(b => !b.Deleted)));

            CreateMap<Author, AuthorDetailDTO>()
                    .ForMember(dest => dest.Books, opt => opt.MapFrom(src => src.Books.Where(b => b.Active && !b.Deleted)));

            CreateMap<Author, AuthorEditDTO>().ReverseMap();

            CreateMap<AuthorEditDTO, Author>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                .ForMember(dest => dest.Active, opt => opt.Ignore())
                .ForMember(dest => dest.Deleted, opt => opt.Ignore())
                .ForMember(dest => dest.Books, opt => opt.Ignore());
            #endregion

            #region Book Mapping
            CreateMap<Book, BookListDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.AuthorFullName, opt => opt.MapFrom(src => $"{src.Author.FirstName} {src.Author.LastName}"))
                .ForMember(dest => dest.ShortDescription, opt => opt.MapFrom(src =>
                    src.Description != null && src.Description.Length > 500
                        ? src.Description.Substring(0, 500) + "..."
                        : src.Description))
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.AuthorId));

            CreateMap<Book, BookDetailDTO>();
            CreateMap<Book, BookEditDTO>();
            CreateMap<BookCreateDTO, Book>();
            CreateMap<BookEditDTO, Book>();
            #endregion

            #region Category Mapping
            CreateMap<Category, CategoryListDTO>()
                .ForMember(dest => dest.Books, opt => opt.MapFrom(src => src.Books));

            CreateMap<Category, CategoryDetailDTO>();
            CreateMap<Category, CategoryEditDTO>();
            CreateMap<CategoryCreateDTO, Category>();
            CreateMap<CategoryEditDTO, Category>();
            #endregion
        }
    }
}
