using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShipIt.PriceQuote.Api.Contracts;
using ShipIt.PriceQuotes.Domain;
using ShipIt.PriceQuotes.Domain.ShipItExceptions;

namespace ShipIt.PriceQuotes.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriceQuoteController(IPriceQuoteService quoteService) : ControllerBase
    {
        [HttpPost]
        public ActionResult<PriceQuoteResponseContract> CalculatePrice(
            [FromBody]PriceQuoteRequestContract requestContract)
        {
            try
            {
                return Ok(quoteService.CreatePrice(requestContract));    
            }
            catch(FromCountryWeightException e)
            {
                return BadRequest(
                    new ProblemDetails{
                        Title = "OLALALA", 
                        Detail = e.Message}
                    );
            }
            
        }

        [HttpGet]
        public string Hello()
        {
            return "Hello World!";
        }
    }
}
