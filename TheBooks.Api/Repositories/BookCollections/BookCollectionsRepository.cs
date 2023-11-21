using AutoMapper;
using Ghak.libraries.AppBase.Exceptions;
using Ghak.libraries.AppBase.Extensions;
using Ghak.libraries.AppBase.Models;
using Ghak.libraries.AppBase.Utils;
using Microsoft.EntityFrameworkCore;
using TheBooks.Api.Data;
using TheBooks.Api.Dto.BookCollections;
using TheBooks.Api.Helpers;
using TheBooks.Api.Model;
using TheBooks.Api.Services.Auth;

namespace TheBooks.Api.Repositories.BookCollections;

public class BookCollectionsRepository(AppDbContext context,
    IMapper mapper,
    IHostEnvironment hostingEnvironment,
    IAuthUserServices authUserServices) 
    : IBookCollectionsRepository
{
    public IQueryable<BookCollection> GetQuery()
    {
        return context.BookCollections
            .OrderByDescending(r => r.CreatedAt)
            .AsQueryable();
    }

    public IQueryable<BookCollection> GetFilterQuery(IQueryable<BookCollection> query, string? searchQuery = null,
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

    public async Task<BookCollection> GetRecord(string id)
    {
        var record = await GetQuery()
            .FirstOrDefaultAsync(r => r.Id == id);
        if (record == null)
            throw new AppException("record is not found",
                404,
                nameof(BookCollection));

        return record;
    }

    public async Task<ApiResponse<PaginationList<BookCollectionDto>>> Pagination(PaginationListArgs request)
    {
        var response = new ApiResponse<PaginationList<BookCollectionDto>>();
        try
        {
            var list = GetFilterQuery(GetQuery(), request.SearchQuery, request.Args);
            var items = await list.PaginateAsync(request.GetPagNumber(),
                request.GetItemsPeerPage());
            response.Data = items.GetResponsePaginationList(mapper.Map<List<BookCollectionDto>>(items.Items));
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

    public async Task<ApiResponse<BookCollectionDto>> Get(string id)
    {
        var response = new ApiResponse<BookCollectionDto>();
        try
        {
            var record = await GetRecord(id);
            response.Data = mapper.Map<BookCollectionDto>(record);
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

    public async Task ModifyValidation(BookCollection record, ModifyBookCollectionDto request)
    {
        if (string.IsNullOrEmpty(request.OwnerId) || (record.OwnerId != request.OwnerId &&
            await context.Users.AllAsync(r => r.Id != request.OwnerId)))
        {
            throw new AppException("user is not found",
                101, nameof(request.OwnerId));
        }
        
        if (record.Title != request.Title &&
            await context.BookCollections.AnyAsync(r => r.Title == request.Title 
                                                        && r.OwnerId == request.OwnerId))
        {
            throw new AppException("this user already hase collection with same name",
                101, nameof(request.Title));
        }
    }

    public async Task<ApiResponse<ToModifyBookCollectionDto>> PrepareModification(string? id = null)
    {
        var response = new ApiResponse<ToModifyBookCollectionDto>();
        try
        {
            response.Data = new();

            if (string.IsNullOrEmpty(id))
                return response;

            var record = await GetRecord(id);
            response.Data.Data = mapper.Map<ModifyBookCollectionDto>(record);

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

        public async Task<ApiResponse<BookCollectionDto>> Create(ModifyBookCollectionDto request)
    {
        var response = new ApiResponse<BookCollectionDto>();
        try
        {
            request.OwnerId = await authUserServices.Id();
            await ModifyValidation(new BookCollection(), request);
            
            var record = mapper.Map<BookCollection>(request);
            record.Id = Ghak.libraries.AppBase.Utils.Helpers.GetStringKey();

            await context.BookCollections.AddAsync(record);

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

    public async Task<ApiResponse<BookCollectionDto>> Update(ModifyBookCollectionDto request)
    {
        var response = new ApiResponse<BookCollectionDto>();
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
                throw new AppException("there is some issue when try to BookCollection record on database", 101);

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
            record.DeletedAt = DateTime.Now;
            response.Data = true;
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