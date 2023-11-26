using Ghak.libraries.AppBase.Models;
using Ghak.libraries.AppBase.Utils;
using Microsoft.AspNetCore.Mvc;
using TheBooks.Api.Dto.Books;
using TheBooks.Api.Repositories.Books;

namespace TheBooks.Api.Controllers;

[Microsoft.AspNetCore.Components.Route("api/[controller]")]
public class BooksController(IBooksRepository repository) 
    : BaseAuthControllers
{
    [HttpPost("books-pagination")]
    public async Task<ActionResult<ApiResponse<PaginationList<BookDto>>>> Pagination(PaginationListArgs request)
    {
        return await repository.Pagination(request);
    }

    [HttpPost("books-list")]
    public async Task<ActionResult<ApiResponse<List<ListItem<string>>>>> List(string? searchQuery = null,
        Dictionary<string, object>? args = null)
    {
        return await repository.List(searchQuery, args);
    }


    [HttpPost("create-book")]
    public async Task<ActionResult<ApiResponse<BookDto>>> Create(ModifyBookDto request)
    {
        return await repository.Create(request);
    }


    [HttpGet("get-book")]
    public async Task<ActionResult<ApiResponse<BookDto>>> Get(string id)
    {
        return await repository.Get(id);
    }

    [HttpGet("get-book-to-modify")]
    public async Task<ActionResult<ApiResponse<ToModifyBookDto>>> InitRecordModification(string? id = null)
    {
        return await repository.PrepareModification(id);
    }

    [HttpPut("update-book")]
    public async Task<ActionResult<ApiResponse<BookDto>>> Update(ModifyBookDto request)
    {
        return await repository.Update(request);
    }

    [HttpPut("update-book-activation")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateActivation(string id)
    {
        return await repository.UpdateActivation(id);
    }

    [HttpDelete("delete-book")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(string id)
    {
        return await repository.Delete(id);
    }
}