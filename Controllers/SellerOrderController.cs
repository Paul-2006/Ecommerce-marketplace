using Ecommerce.Data;
using Ecommerce.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class SellerOrderController : ControllerBase
    {

        private readonly ApplicationDbContext _context;


        public SellerOrderController(ApplicationDbContext context)
        {
            _context = context;
        }




        // GET: api/SellerOrder/Seller/1

        [HttpGet("Seller/{sellerId}")]

        public async Task<IActionResult> GetSellerOrders(int sellerId)
        {


            var orders = await _context.Orderitems

                .Include(o => o.Order)

                .Include(o => o.SellerProduct)

                    .ThenInclude(sp => sp.Product)


                .Where(o =>
                    o.SellerProduct.SellerId == sellerId
                )


                .Select(o => new
                {

                    orderItemId = o.OrderItemId,

                    orderId = o.OrderId,


                    productId =
                    o.SellerProduct.ProductId,


                    productName =
                    o.SellerProduct.Product.ProductName,


                    quantity =
                    o.Quantity,


                    price =
                    o.Price,


                    orderStatus =
                    o.Order.OrderStatus,


                    orderDate =
                    o.Order.OrderDate


                })

                .ToListAsync();



            return Ok(orders);

        }









        // GET: api/SellerOrder/Order/1


        [HttpGet("Order/{orderId}")]

        public async Task<IActionResult> GetOrderDetails(int orderId)
        {


            var order = await _context.Orders

                .Include(o => o.Orderitems)

                    .ThenInclude(oi => oi.SellerProduct)

                        .ThenInclude(sp => sp.Product)


                .FirstOrDefaultAsync(
                    o => o.OrderId == orderId
                );




            if (order == null)
            {
                return NotFound(
                    "Order not found"
                );
            }



            return Ok(order);


        }









        // PUT: api/SellerOrder/UpdateStatus


        [HttpPut("UpdateStatus")]

        public async Task<IActionResult> UpdateStatus(
            SellerOrderStatusDTO dto)
        {



            var order =
                await _context.Orders
                .FindAsync(dto.OrderId);



            if (order == null)
            {
                return NotFound(
                    "Order not found"
                );
            }




            order.OrderStatus = dto.Status;



            await _context.SaveChangesAsync();




            return Ok(new
            {

                message =
                "Order status updated",


                status =
                dto.Status

            });


        }



    }

}