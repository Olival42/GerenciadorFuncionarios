using System;

public record PaginationResponse<T> 
    (
        List<T> Items, 
        int Page, 
        int PageSize, 
        int TotalItems
    )
{
    public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
    public bool IsLastPage => Page == TotalPages;
}
