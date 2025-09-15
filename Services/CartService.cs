using E_CommerceApp.Models;
using E_CommerceApp.Repositories;
using Microsoft.AspNetCore.Identity;
using E_CommerceApp.CustomExceptions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using E_CommerceApp.DTOs;

namespace E_CommerceApp.Services
{
    public class CartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICartProductRepository _cartProductRepository;
        private readonly IApplicationUserRepository _applicationUserRepository;
        public CartService(ICartRepository cartRepository, IProductRepository productRepository, ICartProductRepository cartProductRepository, IApplicationUserRepository userRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _cartProductRepository = cartProductRepository;
            _applicationUserRepository = userRepository;
        }



        public async Task AddToCart(string userId, string productId, int quantity)
        {
            Cart? cart = await _cartRepository.GetCartAndCartProductsAsync(userId);
            Product? product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
                throw new KeyNotFoundException("Product with this Id doesn't exist");

            if (product.Stock < quantity)
                throw new InvalidOperationException("Quantity selected is larger than available amount of this product");

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _cartRepository.Add(cart);
            }

            CartProduct? cartProduct = cart.CartProducts.FirstOrDefault(cp => cp.ProductId == product.Id);
            if (cartProduct != null)
            {
                if (product.Stock < cartProduct.Quantity + quantity)
                    throw new InvalidOperationException("Quantity exceeds available stock");

                cartProduct.Quantity += quantity;
                _cartProductRepository.Update(cartProduct);
            }
            else
            {
                cartProduct = new CartProduct
                {
                    ProductId = productId,
                    CartId = cart.Id,
                    Quantity = quantity
                };
                _cartProductRepository.Add(cartProduct);
            }

            // Recalculate total to be safe
            cart.TotalPrice += (product.Price * quantity);  

            int result = await _cartRepository.SaveChangesAsync();
            if (result == 0) throw new NotAddedOrUpdatedException();
        }

        public async Task<ViewCartDTO> ViewCart(string userId) {
            Cart? cart = await _cartRepository.GetCartAndCartProductsAsync(userId);
            if (cart == null) throw new KeyNotFoundException("User has no cart");

            List<CartProduct> cartProducts = cart.CartProducts.Where(cp => cp.CartId == cart.Id).ToList();
            ViewCartDTO viewCartDTO = new ViewCartDTO();
            foreach (var cartProduct in cartProducts) {
                viewCartDTO.ProductAndQuantity.Add(cartProduct.Product.Name, cartProduct.Quantity);
            }
            viewCartDTO.TotalPrice = cart.TotalPrice;
            return viewCartDTO;
        }

        public async Task UpdateQuantity(string userId, string productId, bool add) {
            Cart? cart = await _cartRepository.GetCartAndCartProductsAsync(userId);
            if (cart == null) throw new KeyNotFoundException("User has no cart");

            CartProduct? cartProduct = cart.CartProducts.FirstOrDefault(cp => cp.ProductId == productId);
            if (cartProduct == null)
            {
                Product? product = await _productRepository.GetByIdAsync(productId);
                if (product == null) throw new KeyNotFoundException("Product doesn't exist");
                if (add)
                {
                    if (product.Stock == 0) {
                        throw new InvalidOperationException("Product out of stock");
                    }
                    cartProduct = new CartProduct();
                    cartProduct.ProductId = productId;
                    cartProduct.CartId = cart.Id;
                    cartProduct.Quantity = 1;
                    _cartProductRepository.Add(cartProduct);
                    cart.TotalPrice += product.Price;
                    _cartRepository.Update(cart);
                }
                else
                {
                    throw new InvalidOperationException("Cannot decrement this product");
                }
            }
            else {

                if (add)
                {
                    if (cartProduct.Product.Stock == cartProduct.Quantity) throw new InvalidOperationException("Cannot increment this product");
                    cartProduct.Quantity = cartProduct.Quantity + 1;
                    _cartProductRepository.Update(cartProduct);
                    cart.TotalPrice += cartProduct.Product.Price;
                    _cartRepository.Update(cart);

                }
                else {
                    if (cartProduct.Quantity == 1)
                    {
                        _cartProductRepository.Remove(cartProduct);
                        cart.TotalPrice -= cartProduct.Product.Price;
                        _cartRepository.Update(cart);
                    }
                    else if (cartProduct.Quantity == 0)
                    {
                        throw new InvalidOperationException("Cannot decrement this product");
                    }
                    else { 
                        cartProduct.Quantity -= 1;
                        _cartProductRepository.Update(cartProduct);
                        cart.TotalPrice -= cartProduct.Product.Price;
                        _cartRepository.Update(cart);
                    }
                }
            }
            int result = await _cartProductRepository.SaveChangesAsync();
            if (result == 0) throw new NotAddedOrUpdatedException();
        }

        public async Task RemoveProduct(string userId, string productId) {

            Product? product = await _productRepository.GetByIdAsync(productId);
            if (product == null) throw new KeyNotFoundException("Product doesn't exist");

            Cart? cart = await _cartRepository.GetCartAndCartProductsAsync(userId);
            if (cart == null) throw new KeyNotFoundException("User has no cart");

            CartProduct? cartProduct = cart.CartProducts.FirstOrDefault(cp => cp.ProductId == productId);
            if(cartProduct == null) throw new KeyNotFoundException("User doesn't have this product in his cart");

            cart.TotalPrice -= (cartProduct.Product.Price * cartProduct.Quantity);
            _cartProductRepository.Remove(cartProduct);
            _cartRepository.Update(cart);

            int result = await _cartProductRepository.SaveChangesAsync();
            if (result == 0) throw new NotAddedOrUpdatedException();
        }

        public async Task ClearCart(string userId) {
            Cart? cart = await _cartRepository.GetCartAndCartProductsAsync(userId);
            if (cart == null) throw new KeyNotFoundException("User has no cart");

            List<CartProduct> cartProducts = cart.CartProducts.Where(cp => cp.CartId == cart.Id).ToList();
            if (!cartProducts.Any())
            {
                //Make sure to clear cart total price
                cart.TotalPrice = 0;
                _cartRepository.Update(cart);
            }
            else {
                foreach (var cartProduct in cartProducts) {
                    _cartProductRepository.Remove(cartProduct);
                }
                cart.TotalPrice = 0;
                _cartRepository.Update(cart);
            }
            int result = await _cartProductRepository.SaveChangesAsync();
            if (result == 0) throw new NotAddedOrUpdatedException();
        }

    }
}
