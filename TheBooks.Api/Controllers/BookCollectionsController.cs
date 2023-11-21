using Ghak.libraries.AppBase.Models;
using Ghak.libraries.AppBase.Utils;
using Microsoft.AspNetCore.Mvc;
using TheBooks.Api.Dto.BookCollections;
using TheBooks.Api.Repositories.BookCollections;

namespace TheBooks.Api.Controllers;

[Microsoft.AspNetCore.Components.Route("api/[controller]")]
public class BookCollectionsController(IBookCollectionsRepository repository) 
    : BaseAuthControllers
{
    [HttpPost("books-collections-pagination")]
    public async Task<ActionResult<ApiResponse<PaginationList<BookCollectionDto>>>> Pagination(PaginationListArgs request)
    {
        return await repository.Pagination(request);
    }

    [HttpPost("books-collections-list")]
    public async Task<ActionResult<ApiResponse<List<ListItem<string>>>>> List(string? searchQuery = null,
        Dictionary<string, object>? args = null)
    {
        return await repository.List(searchQuery, args);
    }


    [HttpPost("create-book-collection")]
    public async Task<ActionResult<ApiResponse<BookCollectionDto>>> Create(ModifyBookCollectionDto request)
    {
        return await repository.Create(request);
    }


    [HttpGet("get-book-collection")]
    public async Task<ActionResult<ApiResponse<BookCollectionDto>>> Get(string id)
    {
        return await repository.Get(id);
    }

    [HttpGet("get-book-collection-to-modify")]
    public async Task<ActionResult<ApiResponse<ToModifyBookCollectionDto>>> InitRecordModification(string? id = null)
    {
        return await repository.PrepareModification(id);
    }

    [HttpPut("update-book-collection")]
    public async Task<ActionResult<ApiResponse<BookCollectionDto>>> Update(ModifyBookCollectionDto request)
    {
        return await repository.Update(request);
    }

    [HttpPut("update-book-collection-activation")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateActivation(string id)
    {
        return await repository.UpdateActivation(id);
    }

    [HttpDelete("delete-book-collection")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(string id)
    {
        return await repository.Delete(id);
    }
}