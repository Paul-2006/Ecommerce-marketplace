using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "WarehouseOnly")]
    public class WarehouseController : ControllerBase
    {

        private readonly ApplicationDbContext _context;


        public WarehouseController(ApplicationDbContext context)
        {
            _context = context;
        }







        // POST: api/Warehouse/Add

        [HttpPost("Add")]

        public async Task<IActionResult> AddWarehouse(
            WarehouseDTO dto)
        {


            var warehouse = new Warehouse
            {

                WarehouseName = dto.WarehouseName,

                Location = dto.Location,

                ContactNumber = dto.ContactNumber,

                CreatedDate = DateTime.Now

            };



            _context.Warehouses.Add(warehouse);


            await _context.SaveChangesAsync();



            return Ok(new
            {

                message = "Warehouse added successfully",

                warehouseId = warehouse.WarehouseId

            });

        }









        // GET: api/Warehouse

        [HttpGet]

        public async Task<IActionResult> GetWarehouses()
        {


            var warehouses =
                await _context.Warehouses
                .ToListAsync();



            return Ok(warehouses);

        }









        // GET: api/Warehouse/1

        [HttpGet("{id}")]

        public async Task<IActionResult> GetWarehouse(
            int id)
        {


            var warehouse =
                await _context.Warehouses
                .FindAsync(id);



            if (warehouse == null)
            {
                return NotFound(
                    "Warehouse not found"
                );
            }



            return Ok(warehouse);

        }









        // PUT: api/Warehouse/1

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateWarehouse(
            int id,
            WarehouseDTO dto)
        {


            var warehouse =
                await _context.Warehouses
                .FindAsync(id);



            if (warehouse == null)
            {
                return NotFound(
                    "Warehouse not found"
                );
            }




            warehouse.WarehouseName =
                dto.WarehouseName;


            warehouse.Location =
                dto.Location;


            warehouse.ContactNumber =
                dto.ContactNumber;



            await _context.SaveChangesAsync();



            return Ok(new
            {

                message =
                "Warehouse updated"

            });

        }









        // DELETE: api/Warehouse/1

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteWarehouse(
            int id)
        {


            var warehouse =
                await _context.Warehouses
                .FindAsync(id);



            if (warehouse == null)
            {
                return NotFound(
                    "Warehouse not found"
                );
            }




            _context.Warehouses.Remove(warehouse);



            await _context.SaveChangesAsync();



            return Ok(new
            {

                message =
                "Warehouse deleted"

            });


        }


    }

}