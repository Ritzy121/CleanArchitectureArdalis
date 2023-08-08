﻿using Ardalis.ApiEndpoints;
using Clean.Architecture.Core.ProjectAggregate;
using Clean.Architecture.Core.ProjectAggregate.Specifications;
using Ardalis.SharedKernel;
using Microsoft.AspNetCore.Mvc;
//using Swashbuckle.AspNetCore.Annotations;
using Clean.Architecture.Web.Endpoints.ProjectEndpoints;

namespace Clean.Architecture.Web.ProjectEndpoints;

public class GetById : EndpointBaseAsync
  .WithRequest<GetProjectByIdRequest>
  .WithActionResult<GetProjectByIdResponse>
{
  private readonly IRepository<Project> _repository;

  public GetById(IRepository<Project> repository)
  {
    _repository = repository;
  }

  [HttpGet(GetProjectByIdRequest.Route)]
  //[SwaggerOperation(
  //  Summary = "Gets a single Project",
  //  Description = "Gets a single Project by Id",
  //  OperationId = "Projects.GetById",
  //  Tags = new[] { "ProjectEndpoints" })
  //]
  public override async Task<ActionResult<GetProjectByIdResponse>> HandleAsync(
    [FromRoute] GetProjectByIdRequest request,
    CancellationToken cancellationToken = new())
  {
    var spec = new ProjectByIdWithItemsSpec(request.ProjectId);
    var entity = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (entity == null)
    {
      return NotFound();
    }

    var response = new GetProjectByIdResponse
    (
      id: entity.Id,
      name: entity.Name,
      items: entity.Items.Select(
        item => new ToDoItemRecord(item.Id,
          item.Title,
          item.Description,
          item.IsDone,
          item.ContributorId))
        .ToList()
    );

    return Ok(response);
  }
}