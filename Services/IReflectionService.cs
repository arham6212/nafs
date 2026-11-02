using Nafs.Api.Models;

namespace Nafs.Api.Services;

public interface IReflectionService
{
    string GetServiceStatus();
    
    Task<PagedResult<Reflection>> GetAllAsync(string? search,int page, int pageSize, CancellationToken cancellationToken);


     Task<Reflection?> GetByIdAsync(int id, CancellationToken cancellationToken);
    
     Task<Reflection> CreateReflectionAsync(string text, CancellationToken cancellationToken);


}