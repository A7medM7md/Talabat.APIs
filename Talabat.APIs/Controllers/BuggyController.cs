using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.Repository.Data;

namespace Talabat.APIs.Controllers
{
    public class BuggyController : ApiBaseController
    {
        private readonly StoreContext _dbContext;
        public BuggyController(StoreContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpGet("notfound")] // GET : api/buggy/notfound
        public ActionResult GetNotFoundRequest()
        {
            var product = _dbContext.Products.Find(100); // Id NotFound
            if (product is null)
                //return NotFound();
                return NotFound(new ApiResponse(404));

            return Ok(product);
        }


        [HttpGet("servererror")] // GET : api/buggy/servererror
        public ActionResult GetServerErrorRequest()
        {
            var product = _dbContext.Products.Find(100); // product = null
            var productToReturn = product.ToString(); // Will Throw Null Reference 'Exception' => Leads To "Server Error"

            return Ok(productToReturn);
        }


        [HttpGet("badrequest")] // GET : api/buggy/badrequest
        public ActionResult GetBadRequest()
        {
            //return BadRequest();
            return BadRequest(new ApiResponse(400));
        }


        [HttpGet("badrequest/{id}")] // GET : api/buggy/badrequest/any-string
        public ActionResult GetBadRequest(int id) // [ValidationError]
        {
            return Ok();
        }


        // Any Endpoint Called Except Those Give You NotFound Endpoint Error
    }
}
