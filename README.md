# DotJoshJohnson.Decorators
A utility library that adds an `AddDecorator()` extension method to `IServiceCollection` to make using the decorator pattern in .NET a breeze!

## Getting Started

1. Install `DotJoshJohnson.Decorators` via NuGet.
2. Add your base service implementation first, then any decorators.
```csharp
using Microsoft.Extensions.DependencyInjection;

// ... //

services
    .AddTransient<IMyService, MyService>()
    .AddDecorator<IMyService, MyFirstDecorator>()
    .AddDecorator<IMyService, MySecondDecorator>();

// ... //

var myDecoratedService = serviceProvider.GetRequiredService<IMyService>();
```

---

<a target="_blank" href="https://icons8.com/icon/TAGnwHOyTIiM/party-balloons">Party Balloons</a> icon by <a target="_blank" href="https://icons8.com">Icons8</a>