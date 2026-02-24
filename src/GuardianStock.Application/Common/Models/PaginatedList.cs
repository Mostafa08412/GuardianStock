using System.Text.Json.Serialization;

namespace GuardianStock.Application.Common.Models;

public class PaginatedList<T> : IPaginationMetadata
{
    public List<T> Items { get; init; } = [];
    [JsonIgnore]
    public int PageNumber { get; init; }
    [JsonIgnore]
    public int PageSize { get; init; }
    [JsonIgnore]
    public int TotalCount { get; init; }

    [JsonIgnore]
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    [JsonIgnore]
    public bool HasPrevious => PageNumber > 1;

    [JsonIgnore]
    public bool HasNext => PageNumber < TotalPages;

    public PaginatedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = count;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public static PaginatedList<T> Create(List<T> items, int count, int pageNumber, int pageSize)
    {
        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }
}

public interface IPaginationMetadata
{

    int PageNumber { get; }
    int PageSize { get; }
    int TotalCount { get; }
    int TotalPages { get; }
    bool HasPrevious { get; }
    bool HasNext { get; }
}