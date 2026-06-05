using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using Web_Api.Data;
using Web_Api.Models;
using Web_Api.Models.Entities;

namespace Web_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public ServiceRequestsController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetAllRequests()
        {
            return Ok(dbContext.ServiceRequests.ToList());
        }

        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetRequestsById(int id)
        {
            var request = dbContext.ServiceRequests.Find(id);
            if (request is null) return NotFound();
            return Ok(request);
        }

        [HttpPost]
        public IActionResult AddRequests(AddServiceRequestDto addRequest)
        {
            var requestEntity = new ServiceRequests()
            {
                ServiceStatus = addRequest.ServiceStatus,
                RequestDescription = addRequest.RequestDescription,
                ContractCost = addRequest.ContractCost,
                RequestDate = addRequest.RequestDate,
                RequestTypes = addRequest.RequestTypes
            };

            dbContext.ServiceRequests.Add(requestEntity);
            dbContext.SaveChanges();
            return Ok(requestEntity);
        }

        [HttpPut]
        [Route("{id:int}")]
        public IActionResult UpdateRequest(int id, UpdateServiceRequestDto updateRequestDto)
        {
            var requests = dbContext.ServiceRequests.Find(id);
            if (requests is null) return NotFound();

            requests.ServiceStatus = updateRequestDto.ServiceStatus;

            requests.RequestDescription = updateRequestDto.RequestDescription;

            requests.ContractCost = updateRequestDto.ContractCost;

            requests.RequestDate = updateRequestDto.RequestDate;

            requests.RequestTypes = updateRequestDto.RequestTypes;

            dbContext.SaveChanges();
            return Ok(requests);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public IActionResult DeleteRequest(int id)
        {
            var request = dbContext.ServiceRequests.Find(id);
            if (request is null) return NotFound();

            dbContext.ServiceRequests.Remove(request);
            dbContext.SaveChanges();
            return Ok();
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> PatchProduct(int id,
            [FromBody] JsonPatchDocument<ServiceRequests> patchDoc)
        {
            if (patchDoc == null) return BadRequest();

            var request = await dbContext.ServiceRequests.FindAsync(id);
            if (request == null) return NotFound();

            patchDoc.ApplyTo(request, error =>
            {
                ModelState.AddModelError(error.Operation.op, error.ErrorMessage);
            });

            if (!ModelState.IsValid) return BadRequest(ModelState);

            await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}