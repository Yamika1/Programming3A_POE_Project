using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Web_Api.Data;
using Web_Api.Models;
using Web_Api.Models.Entities;

namespace Web_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public ClientsController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetAllClients()
        {
            return Ok(dbContext.Clients.ToList());
        }

        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetClientById(int id)
        {
            var client = dbContext.Clients.Find(id);

            if (client is null)
                return NotFound();

            return Ok(client);
        }

        [HttpPost]
        public IActionResult AddClients(AddClientDTO addClient)
        {
            var clientEntity = new Client()
            {
                ClientFirstName = addClient.ClientFirstName,
                ClientLastName = addClient.ClientLastName,
                EmailAddress = addClient.EmailAddress,
                ContactNumber = addClient.ContactNumber,
                Region = addClient.Region
            };

            dbContext.Clients.Add(clientEntity);
            dbContext.SaveChanges();
            return Ok(clientEntity);
        }

        [HttpPut]
        [Route("{id:int}")]
        public IActionResult UpdateClient(int id, UpdateClientDto updateClientDto)
        {
            var client = dbContext.Clients.Find(id);

            if (client is null)
                return NotFound();

            client.ClientFirstName = updateClientDto.ClientFirstName;
            client.ClientLastName = updateClientDto.ClientLastName;
            client.EmailAddress = updateClientDto.EmailAddress;
            client.ContactNumber = updateClientDto.ContactNumber;
            client.Region = updateClientDto.Region;

            dbContext.SaveChanges();
            return Ok(client);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public IActionResult DeleteClient(int id)
        {
            var client = dbContext.Clients.Find(id);

            if (client is null)
                return NotFound();

            dbContext.Clients.Remove(client);
            dbContext.SaveChanges();
            return Ok();
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> PatchClient(int id,
            [FromBody] JsonPatchDocument<Client> patchDoc)
        {
            if (patchDoc == null) return BadRequest();

            var client = await dbContext.Clients.FindAsync(id);
            if (client == null) return NotFound();

            patchDoc.ApplyTo(client, error =>
            {
                ModelState.AddModelError(error.Operation.op, error.ErrorMessage);
            });

            if (!ModelState.IsValid) return BadRequest(ModelState);

            await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}