using Nafs.Api.Models;

namespace Nafs.Api.Services;

public class ReflectionService : IReflectionService
{
    private readonly List<Reflection> _reflections = new()
    {
        new Reflection
        (
            id: 1,
            text: "Today I felt focused.",
            createdAt: DateTime.UtcNow.AddHours(-2)
        ),
        new Reflection
        (
            id: 2,
            text: "I need to be more patient.",
            createdAt: DateTime.UtcNow.AddHours(-1)
        )
    };


    public string GetServiceStatus()
    {
        return "Reflection Service is running";
    }

    public Task<Reflection?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var reflection = _reflections.FirstOrDefault(x => x.Id == id);

        return Task.FromResult(reflection);
    }

    public Task<Reflection> CreateReflectionAsync(string text, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Reflection text cannot be empty", nameof(text));
        }

        var reflection = new Reflection
        (
            id: _reflections.Any() ? _reflections.Max(x => x.Id) + 1 : 1,
            text: text,
            createdAt: DateTime.UtcNow
        );
        _reflections.Add(reflection);
        return Task.FromResult(reflection);
    }


    public Task<PagedResult<Reflection>> GetAllAsync(string? search, int page, int pageSize,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (page < 1)
        {
            throw new ArgumentException(
                "Page must be greater than or equal to 1.",
                nameof(page));
        }

        if (pageSize is < 1 or > 100)

        {
            throw new ArgumentException(
                "Page size must be between 1 and 100.",
                nameof(pageSize));
        }

        IEnumerable<Reflection> query = _reflections;
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(reflection => reflection.Text.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        var totalCount = query.Count();
        var items = query.OrderByDescending(reflection => reflection.CreatedAt)
            .ThenByDescending(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        var result = new PagedResult<Reflection>
        {
            Items = items.ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
        };
        return Task.FromResult(result);
    }
}