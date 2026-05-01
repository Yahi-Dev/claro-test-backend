using AutoMapper;
using ClaroTest.Core.Application.Mappings;
using ClaroTest.Tests.Common;
using FluentAssertions;

namespace ClaroTest.Tests.Mappings;

public class GeneralProfileTests
{
    [Fact]
    public void Configuration_IsValid()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<GeneralProfile>(),
            Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance);

        var act = () => config.AssertConfigurationIsValid();

        act.Should().NotThrow("la configuración de AutoMapper debe permanecer válida");
    }

    [Fact]
    public void CreateBookCommand_MapsTo_BookEntity()
    {
        var mapper = TestMapperFactory.Create();
        var command = new ClaroTest.Core.Application.Features.Books.Commands.CreateBook.CreateBookCommand
        {
            Title = "T",
            Description = "D",
            PageCount = 10,
            Excerpt = "E",
            PublishDate = DateTime.UtcNow
        };

        var entity = mapper.Map<ClaroTest.Core.Domain.Entities.Book>(command);

        entity.Title.Should().Be("T");
        entity.Id.Should().Be(0, "los commands de creación no deben asignar un id");
    }
}
