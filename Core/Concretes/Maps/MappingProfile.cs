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
                    .ForMember(dest => dest.BookCount, opt => opt.MapFrom(src => src.Books.Count));

            CreateMap<Author, AuthorDetailDTO>();
            CreateMap<Author, AuthorEditDTO>();
            CreateMap<AuthorEditDTO, Author>();
            #endregion

            #region Book Mapping
            CreateMap<Book, BookListDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.AuthorFullName, opt => opt.MapFrom(src => $"{src.Author.FirstName} {src.Author.LastName}"))
                .ForMember(dest => dest.ShortDescription, opt => opt.MapFrom(src =>
                    src.Description != null && src.Description.Length > 150
                        ? src.Description.Substring(0, 150) + "..."
                        : src.Description));

            CreateMap<Book, BookDetailDTO>();
            CreateMap<Book, BookEditDTO>();
            CreateMap<BookCreateDTO, Book>()
                .ForMember(dest => dest.CoverImagePath, opt => opt.Ignore());
            CreateMap<BookEditDTO, Book>()
                .ForMember(dest => dest.CoverImagePath, opt => opt.Ignore());
            #endregion

            #region Category Mapping
            CreateMap<Category, CategoryListDTO>()
                .ForMember(dest => dest.BookCount, opt => opt.MapFrom(src => src.Books.Count));

            CreateMap<Category, CategoryDetailDTO>();
            CreateMap<Category, CategoryEditDTO>();
            CreateMap<CategoryCreateDTO, Category>();
            CreateMap<CategoryEditDTO, Category>();
            #endregion
        }
    }
}
