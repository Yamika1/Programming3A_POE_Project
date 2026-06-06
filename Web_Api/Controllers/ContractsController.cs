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

        [HttpGet("{id:int}")]
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
            return CreatedAtAction(nameof(GetContractById), new { id = contractEntity.Id }, contractEntity);
        }

        [HttpPut("{id:int}")]
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

        [HttpDelete("{id:int}")]
        public IActionResult DeleteContract(int id)
        {
            var contract = dbContext.Contracts.Find(id);
            if (contract is null) return NotFound();

            dbContext.Contracts.Remove(contract);
            dbContext.SaveChanges();
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> PatchContract(int id,
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
            return Ok(contract);
        }

        [HttpGet("{id:int}/files")]
        public IActionResult GetFilesByContract(int id)
        {
            var files = dbContext.contractFiles
                .Where(f => f.ContractId == id)
                .ToList();
            return Ok(files);
        }

        [HttpGet("files/{fileId:int}")]
        public IActionResult GetFileById(int fileId)
        {
            var file = dbContext.contractFiles.Find(fileId);
            if (file == null) return NotFound();
            return Ok(file);
        }

        [HttpPost("files")]
        public IActionResult AddFile([FromBody] ContractFile file)
        {
            if (file == null) return BadRequest();

            dbContext.contractFiles.Add(file);
            dbContext.SaveChanges();

            return CreatedAtAction(nameof(GetFileById), new { fileId = file.Id }, file);
        }
    }
}