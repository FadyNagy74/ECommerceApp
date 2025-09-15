using E_CommerceApp.CustomExceptions;
using E_CommerceApp.DTOs;
using E_CommerceApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_CommerceApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost("Add/{productId}")]
        [Authorize]
        public async Task<IActionResult> AddToCart(string productId, int quantity = 1) {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (productId == null) return BadRequest("Product's Id cannot be null");
            try
            {
                if (quantity < 1) quantity = 1;
                await _cartService.AddToCart(userId, productId, quantity);
                return Ok("Product added to cart");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex) 
            { 
                return BadRequest(new { message = ex.Message});
            }
            catch(NotAddedOrUpdatedException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("Update-Quantity/{productId}")] // +/- buttons
        [Authorize]
        public async Task<IActionResult> UpdateQuantity(string productId, bool add) {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (productId == null) return BadRequest("Product's Id cannot be null");
            if (add == null) return BadRequest("Must choose Increment/Decrement method");
            try
            {
                await _cartService.UpdateQuantity(userId, productId, add);
                string placeholder;
                if (add) placeholder = "Incremented";
                else placeholder = "Decremented";
                return Ok($"Product {placeholder} successfully");
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("Remove-Product/{productId}")]
        [Authorize]
        public async Task<IActionResult> RemoveProduct(string productId) {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (productId == null) return BadRequest("Product's Id cannot be null");

            try {
                await _cartService.RemoveProduct(userId, productId);
                return Ok("Product removed from your cart");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }  
            catch (NotAddedOrUpdatedException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }

        }

        [HttpDelete("Clear")]
        [Authorize]
        public async Task<IActionResult> Clear() {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try {
                await _cartService.ClearCart(userId);
                return Ok("Your cart has been cleared");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (NotAddedOrUpdatedException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("View")]
        [Authorize]
        public async Task<IActionResult> ViewCart() {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                ViewCartDTO viewCartDTO = new ViewCartDTO();
                viewCartDTO = await _cartService.ViewCart(userId);
                return Ok(viewCartDTO);
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
