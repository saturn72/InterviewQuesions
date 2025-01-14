In our app `IWorkContext` interface is registered as `Scoped` (http-request)

We have a middleware that invokes the following code:
```
 public Task Invoke(HttpContext context)
 {
     var wc = context.RequestRequiredServices<IWorkContext>();
     _ = DoSomethingSmartAsync(wc);
     return _next(context);
 }

private async Task DoSomethingSmartAsync(IWorkContext wc)
{
  for(var i =0; i<3 i++)
  {
    await Task.Delay(3000);
    var t = wc.GetHashCode();
  }
}
```

Which problems do you see in the code and how would you fix them?
Assume that `DoSomethingSmartAsync` works without any issues what so ever
Answers:
1. `DoSomethingSmartAsync` is being executed in a parallel by bounded to the http-context. When request terminated the instance of `IWorkContext` is disposed and exception is thrown when trying to access it.

To fix this `wc` need to be cloned or the relevant properties should be copied:
```
 public Task Invoke(HttpContext context)
 {
     var wc = context.RequestRequiredServices<IWorkContext>();
     _ = DoSomethingSmartAsync(wc.GetHasCode());
     return _next(context);
 }

private async Task DoSomethingSmartAsync(int hashcode)
{
  for(var i =0; i<3 i++)
  {
    await Task.Delay(3000);
    var t = hashcode;
  }
}
```
