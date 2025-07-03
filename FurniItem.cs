namespace FurnidataParser;

public class FurniItem
{
    public string Type { get; set; }
    public int Id { get; set; }
    public string ClassName { get; set; }
    public int Revision { get; set; }
    public string Category { get; set; }
    public int XDim { get; set; }
    public int YDim { get; set; }
    public string PartColors { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string AdUrl { get; set; }
    public int OfferId { get; set; }
    public bool Buyout { get; set; }
    public int RentOfferId { get; set; }
    public int RentBuyout { get; set; }
    public bool BC { get; set; }
    public bool ExcludedDynamic { get; set; }
    public int BCOfferId { get; set; }
    public string CustomParams { get; set; }
    public int SpecialType { get; set; }
    public bool CanStandOn { get; set; }
    public bool CanSitOn { get; set; }
    public bool CanLayOn { get; set; }
    public string FurniLine { get; set; }
    public string Environment { get; set; }
    public bool Rare { get; set; }
}
