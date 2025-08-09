namespace Talabat.APIs.Errors
{
    public class ApiExceptionResponse : ApiResponse
    {
        public string? Details { get; set; } // = null If I'm In Production Environment

        public ApiExceptionResponse(int statusCode, string? message = null, string details = null) : base(statusCode, message)
        {
            Details = details;
        }
    }
}
