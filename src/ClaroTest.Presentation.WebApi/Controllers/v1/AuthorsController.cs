using ClaroTest.Core.Application.Features.Authors.Commands.CreateAuthor;
using ClaroTest.Core.Application.Features.Authors.Commands.DeleteAuthor;
using ClaroTest.Core.Application.Features.Authors.Commands.UpdateAuthor;
using ClaroTest.Core.Application.Features.Authors.Queries.GetAllAuthors;
using ClaroTest.Core.Application.Features.Authors.Queries.GetAuthorById;
using ClaroTest.Core.Application.Features.Authors.Queries.GetAuthorsByBookId;
using ClaroTest.Core.Application.ViewModels.Authors;
using ClaroTest.Core.Application.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace ClaroTest.Presentation.WebApi.Controllers.v1;

[Route("api/[controller]")]
public class AuthorsController : BaseApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(Response<IReadOnlyList<AuthorViewModel>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAllAuthorsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Response<AuthorViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAuthorByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpGet("by-book/{idBook:int}")]
    [ProducesResponseType(typeof(Response<IReadOnlyList<AuthorViewModel>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByBook([FromRoute] int idBook, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAuthorsByBookIdQuery(idBook), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Response<AuthorViewModel>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateAuthorCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Response<AuthorViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] int id,
        [FromBody] UpdateAuthorCommand command,
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
        var result = await Mediator.Send(new DeleteAuthorCommand(id), cancellationToken);
        return Ok(result);
    }
}
