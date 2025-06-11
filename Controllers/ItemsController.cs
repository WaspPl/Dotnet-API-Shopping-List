using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zaliczeniowy4.Models;

namespace Zaliczeniowy4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ItemsController : ControllerBase
    {
        private readonly Models.AppDbContext _context;

        public ItemsController(Models.AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Items
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Item>>> GetItems()
        {
            try
            {
                return await _context.Items.ToListAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving items", details = ex.Message });
            }
        }

        // GET: api/Items
        [HttpGet("unique")]
        public async Task<ActionResult<IEnumerable<Item>>> GetItemsUnique()
        {
            try
            {
                return await _context.Items
                    .GroupBy(i => i.Name)
                    .Select(g => g.First())
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving unique items", details = ex.Message });
            }
        }

        // GET: api/Items/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Item>> GetItem(int id)
        {
            try
            {
                var item = await _context.Items.FindAsync(id);

                if (item == null)
                {
                    return NotFound(new { error = "Item not found" });
                }

                return item;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving the item", details = ex.Message });
            }
        }

        // PUT: api/Items/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItem(int id, Item item)
        {
            try
            {
                if (id != item.Id)
                {
                    return BadRequest(new { error = "ID mismatch" });
                }

                _context.Entry(item).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(id))
                    {
                        return NotFound(new { error = "Item not found" });
                    }
                    else
                    {
                        throw;
                    }
                }

                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while updating the item", details = ex.Message });
            }
        }

        // PUT: api/Items/5/bought
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}/bought")]
        public async Task<ActionResult<Item>> PutItemCheckmark(int id, bool bought)
        {
            try
            {
                var item = await _context.Items.FindAsync(id);
                if (item == null)
                {
                    return NotFound(new { error = "Item not found" });
                }

                item.Bought = bought;
                await _context.SaveChangesAsync();

                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while updating the item's bought status", details = ex.Message });
            }
        }

        // POST: api/Items
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Item>> PostItem(Item item)
        {
            try
            {
                _context.Items.Add(item);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while creating the item", details = ex.Message });
            }
        }

        // DELETE: api/Items/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            try
            {
                var item = await _context.Items.FindAsync(id);
                if (item == null)
                {
                    return NotFound(new { error = "Item not found" });
                }

                _context.Items.Remove(item);
                await _context.SaveChangesAsync();

                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while deleting the item", details = ex.Message });
            }
        }

        private bool ItemExists(int id)
        {
            try
            {
                return _context.Items.Any(e => e.Id == id);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
