using E_CommerceApp.CustomExceptions;
using E_CommerceApp.DTOs;
using E_CommerceApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security.Claims;

namespace E_CommerceApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;
        public OrderController(OrderService orderService) { 
            _orderService = orderService;
        }
        
        [HttpPost("Place/{addressId}")]
        [Authorize]
        public async Task<IActionResult> PlaceOrder(string addressId) {

            if (addressId == null) return BadRequest("Must send address Id");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try {
                await _orderService.PlaceOrder(userId, addressId);
                return Ok("Order placed");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (NotAddedOrUpdatedException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }

        }

        [HttpGet("Past-Orders")]
        [Authorize]
        public async Task<IActionResult> ViewPastOrders() {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                List<ViewOrderDTO> orderDTOs = await _orderService.ViewPastOrders(userId);
                return Ok(orderDTOs);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("View-Order/{orderId}")]
        [Authorize]
        public async Task<IActionResult> ViewOrder(string orderId) {
            if (orderId == null) return BadRequest("Must enter order id");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                ViewOrderDTO orderDTO = await _orderService.ViewOrder(userId, orderId);
                return Ok(orderDTO);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }

        }

        [HttpPut("Cancel/{orderId}")]
        [Authorize]
        public async Task<IActionResult> CancelOrder(string orderId) {
            if (orderId == null) return BadRequest("Must enter order id");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                await _orderService.CancelOrder(userId, orderId);
                return Ok("Order cancelled");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("Update-Status/{orderId},{status}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(string orderId, int status = 1)
        {
            if (orderId == null) return BadRequest("Must enter an order id");
            if (status < 0 || status > 4) return BadRequest("Status not valid");

            try
            {
                await _orderService.UpdateStatus(orderId, status);
                return Ok("Status updated");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (NotAddedOrUpdatedException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("Status-Sort")]
        [Authorize(Roles = "Admin")]
        //Pending -> Processing -> Delivered -> Cancelled -> Returned

        public async Task<IActionResult> GetOrdersStatusSorted(bool asc, int pageNumber = 1)
        {
            try
            {
                List<ShowOrdersDTO> orderDTOs = await _orderService.GetOrdersStatusSorted(asc, pageNumber);
                return Ok(orderDTOs);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }

        }

        [HttpGet("Date-Sort")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrdersDateSorted(bool asc, int pageNumber = 1)
        {
            try
            {
                List<ShowOrdersDTO> orderDTOs = await _orderService.GetOrdersDateSorted(asc, pageNumber);
                return Ok(orderDTOs);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }

        }


    }
}
