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
        public async Task<ActionResult<PriceQuoteResponseContract>> CalculatePrice(
            [FromBody]PriceQuoteRequestContract requestContract)
        {
            try
            {
                var createdQuote = await quoteService.CreatePrice(requestContract);
                return CreatedAtAction(nameof(GetPrice), new { id = createdQuote.Id}, createdQuote);
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

        [HttpGet("{id}")]
        public async Task<ActionResult<PriceQuoteResponseContract>> GetPrice(
            [FromRoute]string id)
        {
            return Ok(await quoteService.GetById(id));
        }

        [HttpGet]
        public string Hello()
        {
            return "Hello World!";
        }
    }
}
