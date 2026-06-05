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
    public class ContractsController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public ContractsController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetAllContracts()
        {
            return Ok(dbContext.Contracts.ToList());
        }

        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetContractById(int id)
        {
            var contract = dbContext.Contracts.Find(id);
            if (contract is null) return NotFound();
            return Ok(contract);
        }

        [HttpPost]
        public IActionResult AddContracts(AddContractDTO addContract)
        {
            var contractEntity = new Contracts()
            {
                ContractName = addContract.ContractName,
                ContractType = addContract.ContractType,
                ContractDescription = addContract.ContractDescription,
                StartDate = addContract.StartDate,
                EndDate = addContract.EndDate
            };

            dbContext.Contracts.Add(contractEntity);
            dbContext.SaveChanges();
            return Ok(contractEntity);
        }

        [HttpPut]
        [Route("{id:int}")]
        public IActionResult UpdateContract(int id, UpdateContractDto updateContractDto)
        {
            var contract = dbContext.Contracts.Find(id);
            if (contract is null) return NotFound();

            contract.ContractName = updateContractDto.ContractName;
            contract.ContractDescription = updateContractDto.ContractDescription;
            contract.ContractType = updateContractDto.ContractType;
            contract.StartDate = updateContractDto.StartDate;
            contract.EndDate = updateContractDto.EndDate;

            dbContext.SaveChanges();
            return Ok(contract);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public IActionResult DeleteContract(int id)
        {
            var contract = dbContext.Contracts.Find(id);
            if (contract is null) return NotFound();

            dbContext.Contracts.Remove(contract);
            dbContext.SaveChanges();
            return Ok();
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> PatchProduct(int id,
            [FromBody] JsonPatchDocument<Contracts> patchDoc)
        {
            if (patchDoc == null) return BadRequest();

            var contract = await dbContext.Contracts.FindAsync(id);
            if (contract == null) return NotFound();

            patchDoc.ApplyTo(contract, error =>
            {
                ModelState.AddModelError(error.Operation.op, error.ErrorMessage);
            });

            if (!ModelState.IsValid) return BadRequest(ModelState);

            await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
    }
