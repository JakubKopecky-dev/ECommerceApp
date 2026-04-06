using CartService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CartService.IntegrationTests.Common
{
    public static class CartTestHelper
    {
        public static Cart CreateCart(Guid userId, IEnumerable<CartItem>? items = null)
        {
            Cart cart = Cart.Create(userId);

            if (items is not null)
            {
                var field = typeof(Cart).GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance);
                var list = (List<CartItem>)field!.GetValue(cart)!;
                list.AddRange(items);
            }

            return cart;
        }

        public static CartItem CreateCartItem(Guid cartId, Guid productId, string productName, decimal unitPrice, uint quantity)
            => CartItem.Create(cartId, productId, productName, unitPrice, quantity);
    }
}
