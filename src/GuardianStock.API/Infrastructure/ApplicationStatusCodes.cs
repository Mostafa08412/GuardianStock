namespace GuardianStock.API.Infrastructure
{
    public enum ApplicationStatusCodes
    {
        Ok = 200,
        Created = 201,
        Accepted = 202,
        NoContent = 204,
        Conflict = 409,
        BadRequest = 400,
        Unauthorized = 401,
        Forbidden = 403,
        NotFound = 404,
        MethodNotAllowed = 405,
        RequestTimeout = 408,
        UnsupportedMediaType = 415,
        UnprocessableEntity = 422,
        TooManyRequests = 429,

        InternalServerError = 500,
        BadGateway = 502,
        ServiceUnavailable = 503,
        GatewayTimeout = 504
    }
}
