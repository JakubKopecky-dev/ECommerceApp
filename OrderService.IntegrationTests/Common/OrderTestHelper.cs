using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using System.Reflection;

namespace OrderService.IntegrationTests.Common
{
    public static class OrderTestHelper
    {
        public static Order CreateOrder(Guid userId, OrderStatus status = OrderStatus.Created, string? note = null)
        {
            // Vytvoříme order přes Create() – vždy začíná jako Created
            Order order = Order.Create(userId, note);

            // Pokud potřebujeme jiný status, nastavíme ho přes reflection
            if (status != OrderStatus.Created)
            {
                var statusProp = typeof(Order).GetProperty("Status", BindingFlags.Public | BindingFlags.Instance);
                statusProp!.SetValue(order, status);
            }

            return order;
        }

        public static OrderItem CreateOrderItem(Guid orderId, Guid productId, string productName, decimal unitPrice, uint quantity)
            => OrderItem.Create(productId, productName, unitPrice, quantity, orderId);

        public static void AddItems(Order order, IEnumerable<OrderItem> items)
        {
            var field = typeof(Order).GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance);
            var list = (List<OrderItem>)field!.GetValue(order)!;
            list.AddRange(items);
        }
    }
}