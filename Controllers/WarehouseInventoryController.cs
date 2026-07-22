using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseInventoryController : ControllerBase
    {

        private readonly ApplicationDbContext _context;


        public WarehouseInventoryController(ApplicationDbContext context)
        {
            _context = context;
        }



        // GET: api/WarehouseInventory

        [HttpGet]
        public async Task<IActionResult> GetInventory()
        {

            var inventory = await _context.Warehouseinventories
                .Include(w => w.Warehouse)
                .Include(w => w.Product)
                .ToListAsync();


            return Ok(inventory);

        }



        // POST: api/WarehouseInventory

        [HttpPost]
        public async Task<IActionResult> AddInventory(
            WarehouseInventoryDTO dto)
        {

            var inventory = new Warehouseinventory
            {

                WarehouseId = dto.WarehouseId,

                ProductId = dto.ProductId,

                Quantity = dto.Quantity,

                UpdatedDate = DateTime.Now

            };


            _context.Warehouseinventories.Add(inventory);


            await _context.SaveChangesAsync();


            return Ok(new
            {
                message = "Inventory added",

                inventoryId = inventory.WarehouseInventoryId
            });

        }



        // PUT: api/WarehouseInventory/{id}

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateInventory(
            int id,
            WarehouseInventoryDTO dto)
        {

            var inventory =
                await _context.Warehouseinventories
                .FindAsync(id);


            if (inventory == null)
            {
                return NotFound("Inventory not found");
            }


            inventory.WarehouseId = dto.WarehouseId;

            inventory.ProductId = dto.ProductId;

            inventory.Quantity = dto.Quantity;

            inventory.UpdatedDate = DateTime.Now;


            await _context.SaveChangesAsync();


            return Ok(new
            {
                message = "Inventory updated"
            });

        }



        // DELETE: api/WarehouseInventory/{id}

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteInventory(int id)
        {

            var inventory =
                await _context.Warehouseinventories
                .FindAsync(id);


            if (inventory == null)
            {
                return NotFound("Inventory not found");
            }


            _context.Warehouseinventories.Remove(inventory);


            await _context.SaveChangesAsync();


            return Ok(new
            {
                message = "Inventory deleted"
            });

        }


    }

}