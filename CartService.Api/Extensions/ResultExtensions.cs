using CartService.Application.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CartService.Api.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult<TValue>(this Result<TValue, CartItemError> result, ControllerBase controller)
        {
            if (result.IsSuccess)
                return controller.Ok(result.Value);

            return result.Error switch
            {
                CartItemError.ProductNotFound => controller.NotFound(new { Message = "Product not found." }),
                CartItemError.OutOfStock => controller.BadRequest(new { Message = "Not enough stock available." }),
                CartItemError.InvalidQuantity => controller.BadRequest(new { Message = "Quantity must be greater than zero." }),
                _ => controller.StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred." })
            };
        }



        public static IActionResult ToCreatedAtActionResult<TValue>(this Result<TValue, CartItemError> result, ControllerBase controller, string actionName, object routeValues)
        {
            if (result.IsSuccess)
                return controller.CreatedAtAction(actionName, routeValues, result.Value);

            return result.ToActionResult(controller);

        }


        public static IActionResult ToActionResult(this Result<CheckoutResult, CartError> result, ControllerBase controller)
        {

            if (result.IsSuccess)
            {
                if (result.Value is CheckoutResult checkout && checkout.Success == false)
                {
                    return controller.BadRequest(new
                    {
                        Message = "Some items in your cart are out of stock.",
                        Products = checkout.BadProducts
                    });
                }

                return controller.Ok(result.Value);

            }
            return result.Error switch
            {
                CartError.CartNotFound => controller.NotFound(new { Message = "Cart not found." }),
                CartError.DeliveryNotCreated => controller.BadRequest(new { Message = "Order created but its delivery not created." }),
                CartError.PaymentCheckoutUrlNotCreated => controller.BadRequest(new { Message = "Order and delivery created but its payment checkout url not created." }),
                CartError.DeliveryAndPaymentCheckoutNotCreated => controller.BadRequest(new { Message = "Order created but its delivery and payment checkout url not created. " }),
                _ => controller.StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred." })
            };
        }






    }
}
