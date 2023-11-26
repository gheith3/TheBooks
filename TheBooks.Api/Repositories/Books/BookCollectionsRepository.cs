using AutoMapper;
using Ghak.libraries.AppBase.Exceptions;
using Ghak.libraries.AppBase.Extensions;
using Ghak.libraries.AppBase.Models;
using Ghak.libraries.AppBase.Utils;
using Microsoft.EntityFrameworkCore;
using TheBooks.Api.Data;
using TheBooks.Api.Dto.Books;
using TheBooks.Api.Model;
using TheBooks.Api.Services.Auth;

namespace TheBooks.Api.Repositories.Books;

public class BooksRepository(AppDbContext context,
        IMapper mapper,
        IHostEnvironment hostingEnvironment,
        IAuthUserServices authUserServices)
    : IBooksRepository
{
    public IQueryable<Book> GetQuery()
    {
        return context.Books
            .Include(r => r.Owner)
            .OrderByDescending(r => r.CreatedAt)
            .AsQueryable();
    }

    public IQueryable<Book> GetFilterQuery(IQueryable<Book> query, string? searchQuery = null,
        Dictionary<string, object>? filters = null)
    {
        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            query = query.Where(r =>
                r.Title.Contains(searchQuery));
        }

        if (filters == null || !filters.Any())
        {
            return query;
        }

        if (filters.TryGetValue("Id", out var id))
        {
            query = query.Where(r => r.Id == id.ToString());
        }

        if (filters.TryGetValue("OwnerId", out var ownerId))
        {
            query = query.Where(r => r.OwnerId == ownerId.ToString());
        }

        return query;
    }

    public async Task<Book> GetRecord(string id)
    {
        var record = await GetQuery()
            .FirstOrDefaultAsync(r => r.Id == id);
        if (record == null)
            throw new AppException("record is not found",
                404,
                nameof(Book));

        return record;
    }

    public async Task<ApiResponse<PaginationList<BookDto>>> Pagination(PaginationListArgs request)
    {
        var response = new ApiResponse<PaginationList<BookDto>>();
        try
        {
            var list = GetFilterQuery(GetQuery(), request.SearchQuery, request.Args);
            var items = await list.PaginateAsync(request.GetPagNumber(),
                request.GetItemsPeerPage());
            response.Data = items.GetResponsePaginationList(mapper.Map<List<BookDto>>(items.Items));
        }
        catch (AppException exception)
        {
            response.StatusCode = exception.ErrorCode;
            response.Errors.Add(exception.ErrorTitle, exception.Message);
        }
        catch (Exception exception)
        {
            if (!hostingEnvironment.IsProduction())
            {
                response.StatusCode = 500;
                response.Errors.Add("server error", exception.Message);
            }

            Console.WriteLine($"Error, Message {exception.Message}");
        }

        return response;
    }

    public async Task<ApiResponse<List<ListItem<string>>>> List(string? searchQuery = null,
        Dictionary<string, object>? args = null)
    {
        var response = new ApiResponse<List<ListItem<string>>>();
        try
        {
            var records = GetFilterQuery(
                GetQuery().Where(r => r.IsActive),
                searchQuery, args);
            response.Data = await records
                .Select(r => new ListItem<string>
                {
                    Id = r.Id,
                    Content = r.Title,
                    Data = new Dictionary<string, object>()
                    {
                        { "OwnerId", r.OwnerId },
                        { "Title", r.Title },
                        { "Description", r.Description ?? "" },
                        { "ISBN", r.ISBN ?? "" },
                        { "Language", r.Language ?? "" },
                    }
                }).ToListAsync();
        }
        catch (AppException exception)
        {
            response.StatusCode = exception.ErrorCode;
            response.Errors.Add(exception.ErrorTitle, exception.Message);
        }
        catch (Exception exception)
        {
            if (!hostingEnvironment.IsProduction())
            {
                response.StatusCode = 500;
                response.Errors.Add("server error", exception.Message);
            }

            Console.WriteLine($"Error, Message {exception.Message}");
        }

        return response;
    }

    public async Task<ApiResponse<BookDto>> Get(string id)
    {
        var response = new ApiResponse<BookDto>();
        try
        {
            var record = await GetRecord(id);
            response.Data = mapper.Map<BookDto>(record);
            return response;
        }
        catch (AppException exception)
        {
            response.StatusCode = exception.ErrorCode;
            response.Errors.Add(exception.ErrorTitle, exception.Message);
        }
        catch (Exception exception)
        {
            if (!hostingEnvironment.IsProduction())
            {
                response.StatusCode = 500;
                response.Errors.Add("server error", exception.Message);
            }

            Console.WriteLine($"Error, Message {exception.Message}");
        }

        return response;
    }

    public async Task ModifyValidation(Book record, ModifyBookDto request)
    {
        if (string.IsNullOrEmpty(request.OwnerId) ||
            (record.OwnerId != request.OwnerId &&
             await context.Users.AllAsync(r => r.Id != request.OwnerId)))
        {
            throw new AppException("user is not found",
                101, nameof(request.OwnerId));
        }

        if (!string.IsNullOrEmpty(request.BookCollectionId) &&
            (record.BookCollectionId != request.BookCollectionId &&
             await context.BookCollections.AllAsync(r =>
                 r.Id != request.BookCollectionId)))
        {
            throw new AppException("Books Collection is not found",
                101, nameof(request.BookCollectionId));
        }
    }

    public async Task<ApiResponse<ToModifyBookDto>> PrepareModification(string? id = null)
    {
        var response = new ApiResponse<ToModifyBookDto>();
        try
        {
            response.Data = new()
            {
                 Types = Ghak.libraries.AppBase.Utils.Helpers.GetEnumAsListItems<BookType>()
            };

            if (string.IsNullOrEmpty(id))
                return response;

            var record = await GetRecord(id);
            response.Data.Data = mapper.Map<ModifyBookDto>(record);

            return response;
        }
        catch (AppException exception)
        {
            response.StatusCode = exception.ErrorCode;
            response.Errors.Add(exception.ErrorTitle, exception.Message);
        }
        catch (Exception exception)
        {
            if (!hostingEnvironment.IsProduction())
            {
                response.StatusCode = 500;
                response.Errors.Add("server error", exception.Message);
            }

            Console.WriteLine($"Error, Message {exception.Message}");
        }

        return response;
    }

    public async Task<ApiResponse<BookDto>> Create(ModifyBookDto request)
    {
        var response = new ApiResponse<BookDto>();
        try
        {
            request.OwnerId = await authUserServices.Id();
            await ModifyValidation(new Book(), request);

            var record = mapper.Map<Book>(request);
            record.Id = Ghak.libraries.AppBase.Utils.Helpers.GetStringKey();

            await context.Books.AddAsync(record);

            if (!await SaveDbChange())
                throw new AppException("there is some issue when try to Book Collection record at database", 101);


            return await Get(record.Id);
        }
        catch (AppException exception)
        {
            response.StatusCode = exception.ErrorCode;
            response.Errors.Add(exception.ErrorTitle, exception.Message);
        }
        catch (Exception exception)
        {
            if (!hostingEnvironment.IsProduction())
            {
                response.StatusCode = 500;
                response.Errors.Add("server error", exception.Message);
            }

            Console.WriteLine($"Error, Message {exception.Message}");
        }

        return response;
    }

    public async Task<ApiResponse<BookDto>> Update(ModifyBookDto request)
    {
        var response = new ApiResponse<BookDto>();
        try
        {
            if (string.IsNullOrEmpty(request.Id))
            {
                throw new AppException("Id Is Required",
                    101,
                    nameof(request.Id));
            }

            var record = await GetRecord(request.Id);

            await ModifyValidation(record, request);

            record.UpdatedAt = DateTime.Now;
            record.Title = request.Title;
            record.Description = request.Description;
            record.ISBN = request.ISBN;
            record.Language = request.Language;
            record.BookCollectionId = request.BookCollectionId;
            record.Types = request.Types;
            

            await SaveDbChange();

            return await Get(record.Id);
        }
        catch (AppException exception)
        {
            response.StatusCode = exception.ErrorCode;
            response.Errors.Add(exception.ErrorTitle, exception.Message);
        }
        catch (Exception exception)
        {
            if (!hostingEnvironment.IsProduction())
            {
                response.StatusCode = 500;
                response.Errors.Add("server error", exception.Message);
            }

            Console.WriteLine($"Error, Message {exception.Message}");
        }

        return response;
    }

    public async Task<ApiResponse<bool>> UpdateActivation(string identifier)
    {
        var response = new ApiResponse<bool>();
        try
        {
            var record = await GetRecord(identifier);

            record.IsActive = !record.IsActive;
            record.UpdatedAt = DateTime.Now;

            if (!await SaveDbChange())
                throw new AppException("there is some issue when try to Book record on database", 101);

            response.Data = true;
            return response;
        }
        catch (AppException exception)
        {
            response.StatusCode = exception.ErrorCode;
            response.Errors.Add(exception.ErrorTitle, exception.Message);
        }
        catch (Exception exception)
        {
            if (!hostingEnvironment.IsProduction())
            {
                response.StatusCode = 500;
                response.Errors.Add("server error", exception.Message);
            }

            Console.WriteLine($"Error, Message {exception.Message}");
        }

        return response;
    }

    public async Task<ApiResponse<bool>> Delete(string id)
    {
        var response = new ApiResponse<bool>();
        try
        {
            var record = await GetRecord(id);
            record.DeleteSoftly();  
            await SaveDbChange();
        }
        catch (AppException exception)
        {
            response.StatusCode = exception.ErrorCode;
            response.Errors.Add(exception.ErrorTitle, exception.Message);
        }
        catch (Exception exception)
        {
            if (!hostingEnvironment.IsProduction())
            {
                response.StatusCode = 500;
                response.Errors.Add("server error", exception.Message);
            }

            Console.WriteLine($"Error, Message {exception.Message}");
        }

        return response;
    }

    public async Task<bool> SaveDbChange()
    {
        var res = await context.SaveChangesAsync();
        if (res > 0)
        {
            Console.WriteLine($"{res} changes was made on Book Collections database table");
            return true;
        }

        Console.WriteLine("no change was made on Units database table");
        return false;
    }
}