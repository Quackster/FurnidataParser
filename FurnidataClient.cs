using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FurnidataParser;

public class FurnidataClient
{
    private readonly HttpClient _httpClient;

    public FurnidataClient(HttpClient? httpClient = null)
    {
        _httpClient = httpClient ?? new HttpClient();
    }

    public async Task<List<FurniItem>> FetchFurnidataAsync(string url, CancellationToken cancellationToken = default)
    {
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
            "AppleWebKit/537.36 (KHTML, like Gecko) " +
            "Chrome/120.0.0.0 Safari/537.36");

        var rawResponse = await _httpClient.GetStringAsync(url, cancellationToken);

        var items = new List<FurniItem>();

        // Find all chunks like [["..."],["..."]]
        var chunkMatches = Regex.Matches(rawResponse, @"\[\[.*?\]\]", RegexOptions.Singleline);

        foreach (Match chunkMatch in chunkMatches)
        {
            string chunk = chunkMatch.Value;

            // Find each individual item like ["s","116",...]
            var itemMatches = Regex.Matches(chunk, @"\[(.*?)\]");

            foreach (Match itemMatch in itemMatches)
            {
                var fields = SplitFields(itemMatch.Groups[1].Value);

                if (fields.Count < 1) continue; // skip empty entries

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

    private static List<string> SplitFields(string item)
    {
        var fields = new List<string>();
        var matches = Regex.Matches(item, @"""((?:\\.|[^""\\])*)""");
        foreach (Match m in matches)
            fields.Add(m.Groups[1].Value);
        return fields;
    }

    private static string GetField(List<string> fields, int index)
    {
        if (index < fields.Count)
            return fields[index] ?? "";
        return "";
    }

    private static bool IsTrue(string value)
    {
        return value == "1" || value?.ToLower() == "true";
    }
}
