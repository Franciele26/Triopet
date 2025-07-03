using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Triopet.BusinessContext;
using Triopet.BusinessContext.Entities;
using Triopet.Shared;
using Triopet.Shared.Models;


namespace Triopet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntryController : ControllerBase
    {
        private readonly IBusinessContext _businessContext;

        public EntryController(IBusinessContext businessContext)
        {
            _businessContext = businessContext;
        }
        [HttpGet("/entries")]
        public async Task<IActionResult> GetEntries()
        {
            var entries = await _businessContext.Entries.ToListAsync();

            List<EntryDto> entryList = new List<EntryDto>();

            foreach (var entry in entries)
            {
                var entryDto = new EntryDto
                {
                    Id = entry.Id,
                    DateOfEntry = entry.EntryDate


                };

                entryList.Add(entryDto);
            }

            return Ok(entryList);
        }
        [HttpGet("/entries/{id}")]
        public async Task<Entry> GetEntryById(int id)
        {
            var entry = await _businessContext.Entries
                .Where(x => x.Id == id && x.IsDeleted.Equals(false))
                .Select(x => new Entry
                {
                    Id = x.Id,
                    EntryDate = x.EntryDate
                }).FirstOrDefaultAsync();
        
            if (entry == null)
            {
                return new Entry();

            }
            return entry;
        }
        [HttpPost("/entries")]
        public async Task<IActionResult> AddEntry([FromBody] EntryDto? entry)
        {
            if (entry == null)
            {
                return BadRequest("Invalid entry data.");
            }
            var newEntry = new BusinessContext.Entities.Entry
            {
                EntryDate = entry.DateOfEntry,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _businessContext.Entries.Add(newEntry);

            await _businessContext.SaveChangesAsync(true);

            return CreatedAtAction(nameof(GetEntries), new { id = newEntry.Id }, new EntryDto
            {
                Id = entry.Id,
                DateOfEntry = entry.DateOfEntry

            });
        }
        [HttpPut("/deleteentries/id")]
        public async Task<IActionResult> DeleteEntry(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid entry data.");
            }
            var existingEntry = await _businessContext.Entries.FindAsync(id);
            if (existingEntry == null)
            {
                return NotFound("Entry not found.");
            }
            existingEntry.IsDeleted = true;
            existingEntry.UpdatedAt = DateTime.UtcNow;
            await _businessContext.SaveChangesAsync(true);
            return NoContent();
        }
        [HttpPut("/entries")]
        public async Task<IActionResult> UpdateEntry([FromBody] Entry entry)
        {
            if (entry.Id == 0)
            {
                return BadRequest("Error getting entry Id");

            }

            var existingEntry = await _businessContext.Entries.FindAsync(entry.Id);

            if (existingEntry == null)
            {
                return NotFound("Patient not found");

            }
            existingEntry.Id = entry.Id;
            existingEntry.EntryDate = DateTime.UtcNow;
            
            await _businessContext.SaveChangesAsync(true);

            return Ok();
        }
    }
}
