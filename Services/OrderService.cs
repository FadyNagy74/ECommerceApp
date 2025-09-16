using E_CommerceApp.CustomExceptions;
using E_CommerceApp.DTOs;
using E_CommerceApp.Models;
using E_CommerceApp.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace E_CommerceApp.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IApplicationUserRepository _applicationUserRepository;
        private readonly ICartProductRepository _cartProductRepository;
        private readonly IProductRepository _productRepository;
        public OrderService(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, ICartRepository cartRepository, IAddressRepository addressRepository, IApplicationUserRepository applicationUserRepository, ICartProductRepository cartProductRepository, IProductRepository productRepository) { 
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _cartRepository = cartRepository;
            _addressRepository = addressRepository;
            _applicationUserRepository = applicationUserRepository;
            _cartProductRepository = cartProductRepository;
            _productRepository = productRepository;
        }

        public async Task PlaceOrder(string userId, string addressId) { 

            ApplicationUser? user = await _applicationUserRepository.FindUserByIdAsync(userId);
            UserAddress? address = user.UserAddresses.FirstOrDefault(address => address.Id == addressId);
            
            if (address == null) throw new KeyNotFoundException("This address doesn't exist");
            if (address.UserId != userId) throw new UnauthorizedAccessException("Cannot choose this address");

            Cart? cart = await _cartRepository.GetCartAndCartProductsAsync(userId);
            if (cart == null) throw new KeyNotFoundException("Cart doesn't exist");
            if (cart.TotalPrice < AppConstants.MinOrderAmount) throw new InvalidOperationException($"Minimum order amount is {AppConstants.MinOrderAmount}");

            bool hasItems = cart.CartProducts.Any();
            if (!hasItems) throw new InvalidOperationException("Cannot place an order on an empty cart");

            foreach (var cartProduct in cart.CartProducts)
            {
                if (cartProduct.Product.Stock < cartProduct.Quantity)
                {
                    throw new InvalidOperationException(
                        $"Not enough stock for product {cartProduct.Product.Name}. " +
                        $"Available: {cartProduct.Product.Stock}, Requested: {cartProduct.Quantity}");
                }
            }

            decimal taxValue = cart.TotalPrice * AppConstants.TaxRate;

            var order = new Order
            {
                ShippingAddress = address.Address,
                SubTotal = cart.TotalPrice,
                ShippingTotal = AppConstants.ShippingFee,
                Tax = taxValue,
                Total = cart.TotalPrice + AppConstants.ShippingFee + taxValue,
                Status = OrderStatus.Pending,
                UserId = userId
            };

            order.OrderItems = cart.CartProducts.Select(cp => new OrderItem
            {
                ProductId = cp.ProductId,
                Quantity = cp.Quantity,
                UnitPrice = cp.Product.Price
            }).ToList();

            _orderRepository.Add(order);

            cart.TotalPrice = 0.00m;
            foreach (var cartProduct in cart.CartProducts) {
                cartProduct.Product.Stock -= cartProduct.Quantity;
                _productRepository.Update(cartProduct.Product);
                _cartProductRepository.Remove(cartProduct);
            }
            _cartRepository.Update(cart);


            int result = await _orderRepository.SaveChangesAsync();
            if (result == 0) throw new NotAddedOrUpdatedException();

        }

        public async Task<List<ViewOrderDTO>> ViewPastOrders(string userId)
        {
            List<Order> orders = await _applicationUserRepository.FindUserOrders(userId);
            if (!orders.Any()) throw new KeyNotFoundException("No past orders");

            List<ViewOrderDTO> viewOrderDTOs = new List<ViewOrderDTO>();

            foreach (var order in orders)
            {
                ViewOrderDTO orderDTO = new ViewOrderDTO
                {
                    PlacedAt = order.PlacedAt,
                    SubTotal = order.SubTotal,
                    ShippingTotal = order.ShippingTotal,
                    Tax = order.Tax,
                    Total = order.Total,
                    ShippingAddress = order.ShippingAddress,
                    Status = order.Status.ToString(),
                    OrderItemDTOs = order.OrderItems.Select(item => new OrderItemDTO
                    {
                        ProductName = item.Product.Name,
                        UnitPrice = item.UnitPrice,
                        Quantity = item.Quantity
                    }).ToList()
                };

                viewOrderDTOs.Add(orderDTO);
            }

            return viewOrderDTOs;
        }

        public async Task<ViewOrderDTO> ViewOrder(string userId, string orderId) {

            List<Order> orders = await _applicationUserRepository.FindUserOrders(userId);
            if (!orders.Any()) throw new KeyNotFoundException("Order not found");

            Order? order = orders.FirstOrDefault(order => order.Id == orderId);
            if (order == null) throw new KeyNotFoundException("Order not found");
            //No need to check if order belongs to this user because we only look through this user's orders

            ViewOrderDTO orderDTO = new ViewOrderDTO
            {
                PlacedAt = order.PlacedAt,
                SubTotal = order.SubTotal,
                ShippingTotal = order.ShippingTotal,
                Tax = order.Tax,
                Total = order.Total,
                ShippingAddress = order.ShippingAddress,
                Status = order.Status.ToString(),
                OrderItemDTOs = order.OrderItems.Select(item => new OrderItemDTO
                {
                    ProductName = item.Product.Name,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity
                }).ToList()
            };

            return orderDTO;
        }

        public async Task CancelOrder(string userId, string orderId)
        {
            List<Order> orders = await _applicationUserRepository.FindUserOrders(userId);
            if (!orders.Any()) throw new KeyNotFoundException("Order not found");

            Order? order = orders.FirstOrDefault(order => order.Id == orderId);
            if (order == null) throw new KeyNotFoundException("Order not found");
            //No need to check if order belongs to this user because we only look through this user's orders

            if (order.Status == OrderStatus.Pending || order.Status == OrderStatus.Processing)
            {
                // Restore stock
                foreach (var item in order.OrderItems)
                {
                    if (item.Product != null)
                    {
                        item.Product.Stock += item.Quantity;
                        _productRepository.Update(item.Product);
                    }
                }

                order.Status = OrderStatus.Cancelled;
                _orderRepository.Update(order);

                int result = await _orderRepository.SaveChangesAsync();
                if (result == 0) throw new NotAddedOrUpdatedException();
            }
            else if (order.Status == OrderStatus.Cancelled)
            {
                throw new InvalidOperationException("Order already cancelled");
            }
            else
            {
                throw new InvalidOperationException("Cannot cancel this order");
            }
        }

        public async Task UpdateStatus(string orderId, int status) {

            if (!Enum.IsDefined(typeof(OrderStatus), status))
                throw new ArgumentException("Invalid status value");

            Order? order;
            if (status == (int)OrderStatus.Pending) throw new InvalidOperationException("Admins cannot place orders");
            if (status == 3)
            {
                order = await _orderRepository.GetOrderWithItems(orderId);
            }
            else
            {
                order = await _orderRepository.GetByIdAsync(orderId);
            }
            if (order == null) throw new KeyNotFoundException("Order not found");

            if (order.Status == OrderStatus.Cancelled) throw new InvalidOperationException("Cannot update a cancelled order");

            if (status == 3)
            {
                foreach(var item in order.OrderItems)
                {
                    if (item.Product != null)
                    {
                        item.Product.Stock += item.Quantity;
                        _productRepository.Update(item.Product);
                    }
                }
            }

            order.Status = (OrderStatus)status;

            _orderRepository.Update(order);
            int result = await _orderRepository.SaveChangesAsync();
            if (result == 0) throw new NotAddedOrUpdatedException();
        }

        public async Task<List<ShowOrdersDTO>> GetOrdersStatusSorted(bool asc, int pageNumber)
        {
            List<Order> orders = await _orderRepository.GetOrdersStatusSorted(asc, pageNumber);
            if (!orders.Any()) throw new KeyNotFoundException("No orders found");

            return orders.Select(order => new ShowOrdersDTO
            {
                OrderId = order.Id,
                Status = order.Status.ToString(),
                PlacedAt = order.PlacedAt
            }).ToList();
        }

        public async Task<List<ShowOrdersDTO>> GetOrdersDateSorted(bool asc, int pageNumber)
        {
            List<Order> orders = await _orderRepository.GetOrdersDateSorted(asc, pageNumber);
            if (!orders.Any()) throw new KeyNotFoundException("No orders found");

            return orders.Select(order => new ShowOrdersDTO
            {
                OrderId = order.Id,
                Status = order.Status.ToString(),
                PlacedAt = order.PlacedAt
            }).ToList();
        }

    }
}
