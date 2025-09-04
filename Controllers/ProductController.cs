using E_CommerceApp.DTOs;
using E_CommerceApp.Models;
using E_CommerceApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService) { 
            _productService = productService;
        }


        [HttpPost("add-product")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddProduct(ProductDTO productDTO) {
            if (ModelState.IsValid) { 

                Product product = new Product();
                product.Name = productDTO.Name;
                product.Description = productDTO.Description;
                product.Price = productDTO.Price;
                product.Stock = productDTO.Stock;

                int result = await _productService.AddProduct(product);
                if (result > 0) return Ok($"Product {product.Name} Added");
                else return BadRequest($"Product {product.Name} was not added");
            }
            return BadRequest(ModelState);
        }

        [HttpDelete("delete-product/{productId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveProduct(string productId) {

            Product product = await _productService.GetById(productId);
            if (product == null) return BadRequest("Product not found");

            int result = await _productService.RemoveProduct(product);
            if (result > 0) return Ok($"Product {product.Name} removed");
            return BadRequest($"Product {product.Name} was not removed");
        }
    }
}
