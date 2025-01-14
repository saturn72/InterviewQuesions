We have a middleware that invokes the following code:
```
 public Task Invoke(HttpContext context)
 {
     var wc = context.RequestRequiredServices<IWorkContext>();
     _ = DoSomethingSmartAsync(wc);
     return _next(context);
 }
```
Which problem do you see in the code and how would you fix them?
Assume that `DoSomethingSmartAsync` works without any issues what so ever
Answers:
`wc` may be null and throw exception. Using `context.RequestServices.GetService<IWorkContext>()` returns null if object not registered
