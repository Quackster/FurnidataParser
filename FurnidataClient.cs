using System.Text.RegularExpressions;
using System.Xml;

namespace FurnidataParser
{
    /// <summary>
    /// A client for downloading and parsing Habbo furnidata in either XML or chunked JSON format.
    /// </summary>
    public class FurnidataClient
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="FurnidataClient"/> class.
        /// Allows injecting a custom <see cref="HttpClient"/> or uses a default one.
        /// </summary>
        /// <param name="httpClient">Optional custom HttpClient instance.</param>
        public FurnidataClient(HttpClient? httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient();
        }

        /// <summary>
        /// Fetches furnidata from the specified URL, and parses it into a list of <see cref="FurniItem"/> objects.
        /// Automatically determines if the data is XML or chunked JSON.
        /// </summary>
        /// <param name="url">The URL to fetch furnidata from.</param>
        /// <param name="cancellationToken">Optional cancellation token for the request.</param>
        /// <returns>A list of parsed <see cref="FurniItem"/> instances.</returns>
        public async Task<List<FurniItem>> FetchFurnidataAsync(string url)
        {
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                "AppleWebKit/537.36 (KHTML, like Gecko) " +
                "Chrome/120.0.0.0 Safari/537.36");

            var data = await _httpClient.GetStringAsync(url);

            return await ParseFurnidataAsync(data);
        }

        /// <summary>
        /// Parses the furnidata from a string containing either XML or chunked JSON,
        /// automatically detecting the format.
        /// </summary>
        /// <param name="data">The raw furnidata content.</param>
        /// <param name="cancellationToken">Optional cancellation token for asynchronous XML parsing.</param>
        /// <returns>A list of parsed <see cref="FurniItem"/> instances.</returns>
        private async Task<List<FurniItem>> ParseFurnidataAsync(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                return new List<FurniItem>();

            if (await IsXmlFileAsync(data))
            {
                return ParseXml(data);
            }
            else
            {
                return ParseChunkedJson(data);
            }
        }

        /// <summary>
        /// Parses chunked pseudo-JSON furnidata and returns a list of <see cref="FurniItem"/> objects.
        /// </summary>
        /// <param name="data">The chunked JSON-like data string.</param>
        /// <returns>A list of parsed <see cref="FurniItem"/> instances.</returns>
        private List<FurniItem> ParseChunkedJson(string data)
        {
            var items = new List<FurniItem>();
            var chunkMatches = Regex.Matches(data, @"\[\[.*?\]\]", RegexOptions.Singleline);

            foreach (Match chunkMatch in chunkMatches)
            {
                string chunk = chunkMatch.Value;
                var itemMatches = Regex.Matches(chunk, @"\[(.*?)\]");

                foreach (Match itemMatch in itemMatches)
                {
                    var fields = SplitFields(itemMatch.Groups[1].Value);

                    if (fields.Count < 1) continue;

                    var item = new FurniItem
                    {
                        Type = GetField(fields, 0),
                        Id = int.TryParse(GetField(fields, 1), out var id) ? id : 0,
                        ClassName = GetField(fields, 2),
                        Revision = int.TryParse(GetField(fields, 3), out var rev) ? rev : 0,
                        Category = GetField(fields, 4),
                        XDim = int.TryParse(GetField(fields, 5), out var xdim) ? xdim : 0,
                        YDim = int.TryParse(GetField(fields, 6), out var ydim) ? ydim : 0,
                        PartColors = GetField(fields, 7),
                        Name = GetField(fields, 8),
                        Description = GetField(fields, 9),
                        AdUrl = GetField(fields, 10),
                        OfferId = int.TryParse(GetField(fields, 11), out var offerid) ? offerid : 0,
                        Buyout = IsTrue(GetField(fields, 12)),
                        RentOfferId = int.TryParse(GetField(fields, 13), out var rentofferid) ? rentofferid : 0,
                        RentBuyout = int.TryParse(GetField(fields, 14), out var rentbuyout) ? rentbuyout : 0,
                        BC = IsTrue(GetField(fields, 15)),
                        ExcludedDynamic = IsTrue(GetField(fields, 16)),
                        BCOfferId = int.TryParse(GetField(fields, 17), out var bcofferid) ? bcofferid : 0,
                        CustomParams = GetField(fields, 18),
                        SpecialType = int.TryParse(GetField(fields, 19), out var specialtype) ? specialtype : 0,
                        CanStandOn = IsTrue(GetField(fields, 20)),
                        CanSitOn = IsTrue(GetField(fields, 21)),
                        CanLayOn = IsTrue(GetField(fields, 22)),
                        FurniLine = GetField(fields, 23),
                        Environment = GetField(fields, 24),
                        Rare = IsTrue(GetField(fields, 25))
                    };

                    items.Add(item);
                }
            }

            return items;
        }

        /// <summary>
        /// Parses XML furnidata and returns a list of <see cref="FurniItem"/> objects.
        /// </summary>
        /// <param name="xml">The XML string to parse.</param>
        /// <returns>A list of parsed <see cref="FurniItem"/> instances.</returns>
        private List<FurniItem> ParseXml(string xml)
        {
            var items = new List<FurniItem>();
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            var furnitypes = doc.SelectNodes("//furnitype");
            if (furnitypes == null) return items;

            foreach (XmlNode node in furnitypes)
            {
                var item = new FurniItem
                {
                    Type = (node.ParentNode?.Name?.ToLower() ?? "") == "roomitemtypes" ? "s" : "i",
                    Id = int.TryParse(node.Attributes?["id"]?.Value, out var id) ? id : 0,
                    ClassName = node.Attributes?["classname"]?.Value ?? "",
                    Revision = int.TryParse(node.SelectSingleNode("revision")?.InnerText, out var rev) ? rev : 0,
                    Category = node.SelectSingleNode("category")?.InnerText ?? "",
                    XDim = int.TryParse(node.SelectSingleNode("xdim")?.InnerText, out var xdim) ? xdim : 0,
                    YDim = int.TryParse(node.SelectSingleNode("ydim")?.InnerText, out var ydim) ? ydim : 0,
                    PartColors = string.Join(",",
                        node.SelectNodes("partcolors/color")?
                            .Cast<XmlNode>()
                            .Select(c => c.InnerText) ?? Array.Empty<string>()),
                    Name = node.SelectSingleNode("name")?.InnerText ?? "",
                    Description = node.SelectSingleNode("description")?.InnerText ?? "",
                    AdUrl = node.SelectSingleNode("adurl")?.InnerText ?? "",
                    OfferId = int.TryParse(node.SelectSingleNode("offerid")?.InnerText, out var offerid) ? offerid : 0,
                    Buyout = IsTrue(node.SelectSingleNode("buyout")?.InnerText),
                    RentOfferId = int.TryParse(node.SelectSingleNode("rentofferid")?.InnerText, out var rentofferid) ? rentofferid : 0,
                    RentBuyout = int.TryParse(node.SelectSingleNode("rentbuyout")?.InnerText, out var rentbuyout) ? rentbuyout : 0,
                    BC = IsTrue(node.SelectSingleNode("bc")?.InnerText),
                    ExcludedDynamic = IsTrue(node.SelectSingleNode("excludeddynamic")?.InnerText),
                    BCOfferId = int.TryParse(node.SelectSingleNode("bcofferid")?.InnerText, out var bcofferid) ? bcofferid : 0,
                    CustomParams = node.SelectSingleNode("customparams")?.InnerText ?? "",
                    SpecialType = int.TryParse(node.SelectSingleNode("specialtype")?.InnerText, out var specialtype) ? specialtype : 0,
                    CanStandOn = IsTrue(node.SelectSingleNode("canstandon")?.InnerText),
                    CanSitOn = IsTrue(node.SelectSingleNode("cansiton")?.InnerText),
                    CanLayOn = IsTrue(node.SelectSingleNode("canlayon")?.InnerText),
                    FurniLine = node.SelectSingleNode("furniline")?.InnerText ?? "",
                    Environment = node.SelectSingleNode("environment")?.InnerText ?? "",
                    Rare = IsTrue(node.SelectSingleNode("rare")?.InnerText)
                };

                items.Add(item);
            }

            return items;
        }

        /// <summary>
        /// Splits a JSON-like list of fields into individual string fields, handling escaped characters.
        /// </summary>
        /// <param name="item">A single item string inside square brackets, e.g. "field1","field2",...</param>
        /// <returns>A list of unescaped field strings.</returns>
        private static List<string> SplitFields(string item)
        {
            var fields = new List<string>();
            var matches = Regex.Matches(item, @"""((?:\\.|[^""\\])*)""");
            foreach (Match m in matches)
                fields.Add(m.Groups[1].Value);
            return fields;
        }

        /// <summary>
        /// Retrieves a field at the specified index from a list of fields.
        /// Returns an empty string if the index is out of bounds.
        /// </summary>
        /// <param name="fields">List of fields.</param>
        /// <param name="index">Zero-based field index.</param>
        /// <returns>The field value or empty string.</returns>
        private static string GetField(List<string> fields, int index)
        {
            if (index < fields.Count)
                return fields[index] ?? "";
            return "";
        }

        /// <summary>
        /// Checks whether the given string content represents a valid XML document.
        /// </summary>
        /// <param name="fileContents">The file contents to check.</param>
        /// <returns>True if the content is valid XML; otherwise false.</returns>
        private static async Task<bool> IsXmlFileAsync(string fileContents)
        {
            try
            {
                using var reader = XmlReader.Create(new StringReader(fileContents), new XmlReaderSettings
                {
                    ConformanceLevel = ConformanceLevel.Document,
                    IgnoreComments = true,
                    IgnoreWhitespace = true,
                    Async = true,
                    DtdProcessing = DtdProcessing.Ignore
                });

                while (await reader.ReadAsync()) { }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Determines if a string represents a boolean true value.
        /// Accepts "1" or "true" (case-insensitive) as true.
        /// </summary>
        /// <param name="value">The string value to check.</param>
        /// <returns>True if the string represents true; otherwise false.</returns>
        private static bool IsTrue(string? value)
        {
            if (value == null) return false;

            return value == "1" || value?.ToLower() == "true";
        }
    }
}
