using E_CommerceApp.DTOs;
using E_CommerceApp.Models;
using E_CommerceApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;

namespace E_CommerceApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        private readonly TagService _tagService;

        public ProductController(ProductService productService, CategoryService categoryService, TagService tagService) { 
            _productService = productService;
            _categoryService = categoryService;
            _tagService = tagService;
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
            } catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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

        /*
        [HttpGet("view-product/{productId}")]
        [Authorize]
        public async Task<IActionResult> ViewProduct(string productId) {
            Product? product = await _productService.GetById(productId);
            if (product == null) return NotFound("Product not found");

            ProductDTO productDTO = new ProductDTO();
            productDTO.Name = product.Name;
            productDTO.Description = product.Description;
            productDTO.Price = product.Price;
            productDTO.Stock = product.Stock;

            List<string> productCategories = new List<string>();
            List<string> productTags = new List<string>();

            foreach (var productCategory in product.ProductCategories) {
                productCategories.Add(productCategory.Category.Name);
            }

            foreach (var productTag in product.ProductTags) { 
                productTags .Add(productTag.Tag.Name);
            }

            productDTO.Categories = productCategories;
            productDTO.Tags = productTags;
            return Ok(productDTO);
        }
        */

    }
}
