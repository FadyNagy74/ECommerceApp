using E_CommerceApp.DTOs;
using E_CommerceApp.Models;
using E_CommerceApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Numerics;
using System.Security.Claims;

namespace E_CommerceApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        private readonly TagService _tagService;
        private readonly ReviewService _reviewService;

        public ProductController(ProductService productService, CategoryService categoryService, TagService tagService, ReviewService reviewService) { 
            _productService = productService;
            _categoryService = categoryService;
            _tagService = tagService;
            _reviewService = reviewService;
        }


        [HttpPost("add-product")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddProduct(ProductDTO productDTO) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            try
            {
                int result = await _productService.AddProduct(productDTO);
                if (result > 0) return Ok($"Product {productDTO.Name} Added");
                else return BadRequest($"Product {productDTO.Name} was not added");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // fallback for unexpected errors
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }

        }

        [HttpDelete("remove-product/{productId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveProduct(string productId) {

            Product? product = await _productService.GetById(productId);
            if (product == null) return NotFound("Product not found");

            int result = await _productService.RemoveProduct(product);
            if (result > 0) return Ok($"Product {product.Name} removed");
            return BadRequest($"Product {product.Name} was not removed");
        }

        
        [HttpPost("add-category/{categoryName}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddCategory(string categoryName) {
            int result = await _categoryService.Add(categoryName);
            if (result == -1) return BadRequest("A Category with this name already exists");
            if(result > 0) return Ok($"Category {categoryName} added");
            return BadRequest($"Category {categoryName} was not added");
        }

        [HttpDelete("remove-category/{categoryName}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveCategory(string categoryName){
            Category? category = await _categoryService.FindByName(categoryName);
            if (category == null) return NotFound($"Category with name {categoryName} was not found");

            int result = await _categoryService.Remove(category);
            if (result > 0) return Ok($"Category {categoryName} removed");
            return BadRequest($"Category {categoryName} was not removed");
        }

        [HttpPost("add-tag/{tagName}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddTag(string tagName)
        {
            int result = await _tagService.Add(tagName);
            if (result == -1) return BadRequest("A Tag with this name already exists");
            if (result > 0) return Ok($"Tag {tagName} added");
            return BadRequest($"Tag {tagName} was not added");
        }

        [HttpDelete("remove-tag/{tagName}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveTag(string tagName)
        {
            Tag? tag = await _tagService.FindByName(tagName);
            if (tag == null) return NotFound($"Tag with name {tagName} was not found");

            int result = await _tagService.Remove(tag);
            if (result > 0) return Ok($"Tag {tagName} removed");
            return BadRequest($"Tag {tagName} was not removed");
        }

        [HttpPost("add-review/{productId}")]
        [Authorize]
        public async Task<IActionResult> AddReview(string productId, ReviewDTO reviewDTO) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.FindFirstValue(ClaimTypes.Name);
            try
            {
                int result = await _reviewService.AddReview(userId, productId, reviewDTO);
                if (result >= 1) {
                    return Ok(new { message = $"User {userName} added a review" });
                }
                return BadRequest("Review was not added");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex) {
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }

        }

        [HttpDelete("remove-review/{reviewId}")]
        [Authorize]
        public async Task<IActionResult> RemoveReview(string reviewId) {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.FindFirstValue(ClaimTypes.Name);

            try {
                int result = await _reviewService.RemoveReview(userId, reviewId);
                if (result >= 1)
                {
                    return Ok(new { message = $"User {userName} deleted a review" });
                }
                return BadRequest("Review was not deleted");
            } catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
        }

        [HttpPut("edit-review/{reviewId}")]
        [Authorize]
        public async Task<IActionResult> EditReview(string reviewId, ReviewDTO reviewDTO) {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.FindFirstValue(ClaimTypes.Name);

            try
            {
                int result = await _reviewService.EditReview(userId, reviewId, reviewDTO);
                if (result >= 1)
                {
                    return Ok(new { message = $"User {userName} edited a review" });
                }
                return BadRequest("Review was not edited");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
        }

        [HttpDelete("remove-user-review/{reviewId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveUserReview(string reviewId)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.FindFirstValue(ClaimTypes.Name);

            try
            {
                int result = await _reviewService.RemoveUserReview(userId, reviewId);
                if (result >= 1)
                {
                    return Ok(new { message = $"Admin {userName} deleted a review" });
                }
                return BadRequest("Review was not deleted");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            
        }


        [HttpGet("view-product/{productId}")]
        [Authorize]
        public async Task<IActionResult> ViewProduct(string productId) {
            Product? product = await _productService.GetById(productId);
            if (product == null) return NotFound("Product not found");

            ShowProductDTO productDTO = new ShowProductDTO();
            productDTO.Name = product.Name;
            productDTO.Description = product.Description;
            productDTO.Price = product.Price;
            productDTO.Stock = product.Stock;

            return Ok(productDTO);
        }
        

    }
}
