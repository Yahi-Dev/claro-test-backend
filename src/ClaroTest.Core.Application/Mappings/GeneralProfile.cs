using AutoMapper;
using ClaroTest.Core.Application.Features.Authors.Commands.CreateAuthor;
using ClaroTest.Core.Application.Features.Authors.Commands.UpdateAuthor;
using ClaroTest.Core.Application.Features.Books.Commands.CreateBook;
using ClaroTest.Core.Application.Features.Books.Commands.UpdateBook;
using ClaroTest.Core.Application.ViewModels.Authors;
using ClaroTest.Core.Application.ViewModels.Books;
using ClaroTest.Core.Domain.Entities;

namespace ClaroTest.Core.Application.Mappings;

public class GeneralProfile : Profile
{
    public GeneralProfile()
    {
        // Libros — entidad <-> view models
        CreateMap<Book, BookViewModel>().ReverseMap();
        CreateMap<Book, SaveBookViewModel>().ReverseMap();
        CreateMap<SaveBookViewModel, BookViewModel>().ReverseMap();

        // Libros — commands -> entidad
        CreateMap<CreateBookCommand, Book>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateBookCommand, Book>();

        // Autores — entidad <-> view models
        CreateMap<Author, AuthorViewModel>()
            .ForMember(dest => dest.BookCount, opt => opt.Ignore())
            .ReverseMap();
        CreateMap<Author, SaveAuthorViewModel>().ReverseMap();
        CreateMap<SaveAuthorViewModel, AuthorViewModel>()
            .ForMember(dest => dest.BookCount, opt => opt.Ignore())
            .ReverseMap();

        // Autores — commands -> entidad
        CreateMap<CreateAuthorCommand, Author>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateAuthorCommand, Author>();
    }
}
