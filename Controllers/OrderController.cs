using Ecommerce.Data;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        private readonly ApplicationDbContext _context;


        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }



        [HttpPost("Create")]
        public async Task<IActionResult> CreateOrder(OrderDTO dto)
        {

            using var transaction =
                await _context.Database.BeginTransactionAsync();


            try
            {

                var cart = await _context.Carts
                    .FirstOrDefaultAsync(c =>
                        c.CartId == dto.CartId &&
                        c.CustomerId == dto.CustomerId);


                if (cart == null)
                {
                    return BadRequest(
                        "Cart does not belong to customer"
                    );
                }



                var cartItems =
                    await _context.Cartitems
                    .Where(c => c.CartId == dto.CartId)
                    .ToListAsync();



                if (cartItems.Count == 0)
                {
                    return BadRequest(
                        "Cart is empty"
                    );
                }



                decimal totalAmount = 0;


                var items =
                    new List<(Cartitem cart, Sellerproduct sellerProduct)>();



                foreach (var item in cartItems)
                {

                    var sellerProduct =
                        await _context.Sellerproducts
                        .FirstOrDefaultAsync(
                            sp =>
                            sp.SellerProductId ==
                            item.SellerProductId
                        );


                    if (sellerProduct == null)
                    {
                        return BadRequest(
                            "Seller product not found"
                        );
                    }



                    decimal price =
                        sellerProduct.Price;



                    totalAmount +=
                        price * item.Quantity;



                    items.Add(
                        (
                        item,
                        sellerProduct
                        )
                    );

                }




                var order = new Order
                {

                    CustomerId = dto.CustomerId,

                    AddressId = dto.AddressId,

                    OrderDate = DateTime.Now,

                    TotalAmount = totalAmount,

                    OrderStatus = "Placed"

                };



                _context.Orders.Add(order);

                await _context.SaveChangesAsync();




                foreach (var item in items)
                {

                    var orderItem =
                        new Orderitem
                        {

                            OrderId =
                            order.OrderId,


                            SellerProductId =
                            item.sellerProduct.SellerProductId,


                            Quantity =
                            item.cart.Quantity,


                            Price =
                            item.sellerProduct.Price

                        };


                    _context.Orderitems.Add(orderItem);

                }



                await _context.SaveChangesAsync();



                _context.Cartitems.RemoveRange(cartItems);


                await _context.SaveChangesAsync();



                await transaction.CommitAsync();



                return Ok(new
                {

                    message =
                    "Order created successfully",

                    orderId =
                    order.OrderId,

                    totalAmount =
                    order.TotalAmount

                });


            }
            catch (Exception ex)
            {

                await transaction.RollbackAsync();


                return StatusCode(500, new
                {
                    message =
                    "Order creation failed",

                    error =
                    ex.Message
                });

            }


        }





        [HttpGet("Customer/{customerId}")]

        public async Task<IActionResult>
        GetCustomerOrders(int customerId)
        {


            var orders =
                await _context.Orders

                .Where(o =>
                o.CustomerId == customerId)

                .Include(o =>
                o.Orderitems)

                .ThenInclude(oi =>
                oi.SellerProduct)

                .ThenInclude(sp =>
                sp.Product)

                .ToListAsync();



            return Ok(orders);

        }





        [HttpGet("{id}")]

        public async Task<IActionResult>
        GetOrder(int id)
        {


            var order =
                await _context.Orders

                .Include(o =>
                o.Orderitems)

                .ThenInclude(oi =>
                oi.SellerProduct)

                .ThenInclude(sp =>
                sp.Product)

                .FirstOrDefaultAsync(
                    o => o.OrderId == id
                );



            if (order == null)
            {
                return NotFound(
                    "Order not found"
                );
            }



            return Ok(order);

        }



    }

}