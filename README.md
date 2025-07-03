# FurnidataParser

A lightweight .NET library for downloading and parsing Habbo furnidata.

## Features

- Fetches Habbo furnidata from the official or private URLs ('private' Habbo servers are supported)
- Parses the quirky non-standard furnidata chunks (multiple JSON-like arrays back-to-back)  
- Maps to strongly-typed `FurniItem` objects  
- Supports dependency injection of `HttpClient`  
- Async, cancellation-friendly API

## Usage

```csharp
using FurnidataParser;

var client = new FurnidataClient();
var items = await client.FetchFurnidataAsync("https://www.habbo.com/gamedata/furnidata/1", cancellationToken);

foreach (var item in items)
{
    Console.WriteLine($"{item.Id}: {item.Name} ({item.ClassName})");
}
```
