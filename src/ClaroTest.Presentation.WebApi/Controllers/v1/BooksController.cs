using ClaroTest.Core.Application.Features.Books.Commands.CreateBook;
using ClaroTest.Core.Application.Features.Books.Commands.DeleteBook;
using ClaroTest.Core.Application.Features.Books.Commands.UpdateBook;
using ClaroTest.Core.Application.Features.Books.Queries.GetAllBooks;
using ClaroTest.Core.Application.Features.Books.Queries.GetBookById;
using ClaroTest.Core.Application.ViewModels.Books;
using ClaroTest.Core.Application.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace ClaroTest.Presentation.WebApi.Controllers.v1;

[Route("api/[controller]")]
public class BooksController : BaseApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(Response<IReadOnlyList<BookViewModel>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAllBooksQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Response<BookViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetBookByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Response<BookViewModel>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateBookCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Response<BookViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] int id,
        [FromBody] UpdateBookCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id && command.Id != 0)
        {
            return BadRequest(Response<object>.Failure("El id de la ruta no coincide con el id del cuerpo."));
        }

        command.Id = id;
        var result = await Mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(Response<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new DeleteBookCommand(id), cancellationToken);
        return Ok(result);
    }
}
