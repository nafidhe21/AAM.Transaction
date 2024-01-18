using AAM.Transaction.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AAM.Transaction.Controllers
{
    [Route("api/[controller]")]
    public class TransactionRecordController : Controller
    {
        private readonly AppDbContext db;

        public TransactionRecordController(AppDbContext appDbContext) =>
            db = appDbContext;

        [HttpGet("transaction")]
        public async Task<IActionResult> Get()
        {
            var transactionList = await db.Transactions.ToListAsync();
            return Ok(new { transactionList });
        }

        [HttpGet("create-transaction")]
        public async Task<IActionResult> Post(TransactionRecordDto dto)
        {
            try
            {
                var client = new RestClient("https://localhost:44323");
                var request = new RestRequest("api/Product/process-transaction", Method.Post);
                request.AddJsonBody(dto);

                RestResponse response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    Console.WriteLine(response.Content);
                    var transactionResponse = JsonConvert.DeserializeObject<TransactionResponseDto>(response.Content);

                    var transactionRecord = new TransactionRecord
                    {
                        ProductName = transactionResponse.ProductName,
                        Amount = dto.Amount,
                        Price = transactionResponse.Price,
                    };

                    await db.Transactions.AddAsync(transactionRecord);
                    db.SaveChanges();

                    return Ok(transactionRecord);
                }
                else
                {
                    return StatusCode((int)response.StatusCode, response.ErrorMessage);
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
