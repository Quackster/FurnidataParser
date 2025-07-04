# FurnidataParser

A lightweight .NET library for downloading and parsing Habbo furnidata.

## Other Languages

It’s also available in other languages for your preferred tech stack.

### Available Ports

- **Java:** [furnidataparser-java](https://github.com/Quackster/FurnidataParser/tree/master/java/org/oldskooler/furnidataparser)  

  A full-featured Java implementation of FurnidataParser, using Java 11+.


## Features

- Fetches Habbo furnidata from official or private URLs (private Habbo servers supported)  
- Parses **both**:
    - The quirky, non-standard furnidata chunks (multiple JSON-like arrays back-to-back)
    - The classic XML furnidata format (`<furnidata>...</furnidata>`)
- Maps data to strongly-typed `FurniItem` objects  
- Async methods

## Usage

Fetch from Habbo’s XML endpoint.

```csharp
using FurnidataParser;

var client = new FurnidataClient();

var itemsFromXml = await client.FetchFurnidataAsync(
    "https://www.habbo.com/gamedata/furnidata_xml/1");

foreach (var item in itemsFromXml)
{
    Console.WriteLine($"ID {item.Id}: {item.Name} [{item.ClassName}]");
    Console.WriteLine($"  Type: {item.Type} | Category: {item.Category} | Revision: {item.Revision}");
    Console.WriteLine($"  Dimensions: {item.XDim}x{item.YDim} | Colors: {item.PartColors}");
    Console.WriteLine($"  Description: {item.Description}");
    Console.WriteLine(new string('-', 50));
}

Console.WriteLine($"Fetched {itemsFromXml.Count} items from chunked JSON endpoint.");
```

Fetch from Habbo’s chunked JSON endpoint.

```csharp
using FurnidataParser;

var client = new FurnidataClient();

var itemsFromJson = await client.FetchFurnidataAsync(
    "https://www.habbo.com/gamedata/furnidata/1");

foreach (var item in itemsFromJson)
{
    // ...
}

Console.WriteLine($"Fetched {itemsFromJson.Count} items from XML endpoint.");
```

Parse from raw data (e.g., local file) using ParseFurnidataAsync.

```csharp
using FurnidataParser;

var client = new FurnidataClient();

var rawData = await File.ReadAllTextAsync("furnidata.xml"); // can be either [[...]] furnidata or XML furnidata
var itemsFromFile = await client.ParseFurnidataAsync(rawData);

foreach (var item in itemsFromFile)
{
    // ...
}

Console.WriteLine($"Parsed {itemsFromFile.Count} items from local furnidata file.");
```

## Installation

Install the package via NuGet.

All NuGet versions can be found here: https://www.nuget.org/packages/FurnidataParser

```sh
dotnet add package FurnidataParser
```

Or via this command.

```sh
NuGet\Install-Package FurnidataParser
```



## License

This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
