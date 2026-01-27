namespace IMS.API.Contracts
{

    public abstract class ApiResponse
    {
        public abstract int StatusCode { get; }

        public bool IsSuccess => StatusCode >= 200 || StatusCode >= 300;
    }
    public class OkApiResponse<T> : ApiResponse
    {

        public override int StatusCode => 200;

        public T Data { get; init; }

        public OkApiResponse(T data)
        {
            Data = data;
        }
    }
    public class PaginatedApiResponse<T> : OkApiResponse<T>
    {
        public PaginatedApiResponse(T data, int totalItems, int pageNo, int pageSize, int totalPages) : base(data)
        {
            TotalPages = totalPages;
            TotalItems = totalItems;
            PageNo = pageNo;
            PageSize = pageSize;


        }

        public int PageNo { get; init; }
        public int PageSize { get; init; }
        public int TotalPages { get; init; }

        public int TotalItems { get; init; }



    }

    public class NoContentApiResponse : ApiResponse
    {
        public override int StatusCode => 204;

    }

    public class CreatedApiResponse : ApiResponse
    {
        public override int StatusCode => 201;

        public string Location { get; init; }


    }


}




