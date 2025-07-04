# FurnidataParser

A lightweight library ported from C# to Java for downloading and parsing Habbo furnidata.

## Features

- Fetches Habbo furnidata from official or private URLs (private Habbo servers supported)  
- Parses **both**:
    - The quirky, non-standard furnidata chunks (multiple JSON-like arrays back-to-back)
    - The classic XML furnidata format (`<furnidata>...</furnidata>`)
- Maps data to strongly-typed `FurniItem` objects 

## Usage

Fetch from Habbo’s XML endpoint.

```csharp
FurnidataClient client = new FurnidataClient();

List<FurniItem> itemsFromXml = client.fetchFurnidata(
    "https://www.habbo.com/gamedata/furnidata_xml/1");

for (FurniItem item : itemsFromXml) {
    System.out.println(String.format("ID %d: %s [%s]",
            item.id, item.name, item.className));
    System.out.println(String.format("  Type: %s | Category: %s | Revision: %d",
            item.type, item.category, item.revision));
    System.out.println(String.format("  Dimensions: %dx%d | Colors: %s",
            item.xDim, item.yDim, item.partColors));
    System.out.println("  Description: " + item.description);
    System.out.println("--------------------------------------------------");
}

System.out.println(String.format("Fetched %d items from XML endpoint.", itemsFromXml.size()));
```

Fetch from Habbo’s chunked JSON endpoint.

```csharp
FurnidataClient client = new FurnidataClient();

List<FurniItem> itemsFromJson = client.fetchFurnidata(
    "https://www.habbo.com/gamedata/furnidata/1");

for (FurniItem item : itemsFromJson) {
    // ...
}

System.out.println(String.format("Fetched %d items from chunked JSON endpoint.", itemsFromJson.size()));
```

Parse from raw data (e.g., local file) using ParseFurnidataAsync.

```csharp
FurnidataClient client = new FurnidataClient();

String rawData = Files.readString(Paths.get("furnidata.xml")); // can be [[...]] furnidata or XML furnidata
List<FurniItem> itemsFromFile = client.parseFurnidata(rawData);

for (FurniItem item : itemsFromFile) {
    // ...
}

System.out.println(String.format("Parsed %d items from local furnidata file.", itemsFromFile.size()));
```

## License

This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
