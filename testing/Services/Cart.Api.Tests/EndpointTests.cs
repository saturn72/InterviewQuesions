using Cart.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Shouldly;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Authorization;

namespace Cart.Api.Tests;

public class EndpointTests
{

    //this test verify all endpoint urls (per-controller)
    [InlineData(typeof(OrderController), "/api/order")]
    [InlineData(typeof(StoreController), "/api/store")]
    [Theory]
    public void ValidateEndpointRoutes(Type controllerType, string expTemplate)
    {
        controllerType.GetCustomAttribute<RouteAttribute>().Template.ShouldBe(expTemplate);
    }

    //this test verify all endpoint urls (per-method)
    [InlineData(typeof(OrderController), nameof(OrderController.GetOrderByIdAsync), "GET", "{orderId}")]
    [InlineData(typeof(OrderController), nameof(OrderController.GetOrdersAsync), "GET", null)]
    [InlineData(typeof(OrderController), nameof(OrderController.CreateOrderByIdAsync), "POST", null)]
    [InlineData(typeof(StoreController), nameof(StoreController.GetStoreInfoAsync), "GET", null)]
    [InlineData(typeof(StoreController), nameof(StoreController.GetOpenHoursAsync), "GET", "open-hours")]
    [Theory]
    public void ValidateEndpointMethodRoutes(Type controllerType, string methodName, string expHttpVerb, string expTemplate)
    {
        var mi = controllerType.GetMethod(methodName);
        var atts = mi.GetCustomAttributes<HttpMethodAttribute>();
        atts.Count().ShouldBe(1);
        var h = atts.First();
        h.HttpMethods.First().ShouldBe(expHttpVerb);
        h.Template.ShouldBe(expTemplate);
    }

    //this test verify all allow-anonymous methods
    [InlineData(typeof(StoreController), nameof(StoreController.GetOpenHoursAsync))]
    [Theory]
    public void ValidateEndpointMethodAllowAnonymousRoutes(Type controllerType, string methodName)
    {
        var mi = controllerType.GetMethod(methodName);
        mi.GetCustomAttribute<AllowAnonymousAttribute>().ShouldNotBeNull();
    }
}
