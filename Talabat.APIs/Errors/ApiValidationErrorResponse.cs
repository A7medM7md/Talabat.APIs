namespace Talabat.APIs.Errors
{
    public class ApiValidationErrorResponse : ApiResponse
    {

        public IEnumerable<string> Errors { get; set; }

        public ApiValidationErrorResponse() : base(400) // Validation Error Is a Bad Request Error
        {
            Errors = new List<string>();
        }


    }
}
