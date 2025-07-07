namespace FurnidataParser
{
    /// <summary>
    /// Represents a single furniture item (furni) in Habbo furnidata.
    /// Contains metadata describing the item's properties.
    /// </summary>
    public struct FurniItem
    {
        /// <summary>
        /// The type of the furni, e.g., "s" or "i".
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The unique ID of the furni.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The class name of the furni.
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// The file name of the furni.
        /// </summary>
        public string FileName
        {
            get
            {
                if (!string.IsNullOrEmpty(ClassName) && ClassName.Contains("*"))
                {
                    return ClassName.Split('*')[0];
                }

                return ClassName;
            }
        }

        /// <summary>
        /// The revision number of the furni asset.
        /// </summary>
        public int Revision { get; set; }

        /// <summary>
        /// The category of the furni.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// The X dimension (width) of the furni in tiles.
        /// </summary>
        public int XDim { get; set; }

        /// <summary>
        /// The Y dimension (length) of the furni in tiles.
        /// </summary>
        public int YDim { get; set; }

        /// <summary>
        /// The part colors associated with the furni, comma-separated.
        /// </summary>
        public string PartColors { get; set; }

        /// <summary>
        /// The display name of the furni.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of the furni.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The advertisement URL associated with the furni.
        /// </summary>
        public string AdUrl { get; set; }

        /// <summary>
        /// The primary offer ID of the furni, used for purchasing.
        /// </summary>
        public int OfferId { get; set; }

        /// <summary>
        /// Indicates whether the furni can be bought outright.
        /// </summary>
        public bool Buyout { get; set; }

        /// <summary>
        /// The offer ID for renting the furni.
        /// </summary>
        public int RentOfferId { get; set; }

        /// <summary>
        /// The buyout price of the rental offer.
        /// </summary>
        public int RentBuyout { get; set; }

        /// <summary>
        /// Indicates if the furni is a Builders Club exclusive.
        /// </summary>
        public bool BC { get; set; }

        /// <summary>
        /// This field is unknown.
        /// </summary>
        public bool ExcludedDynamic { get; set; }

        /// <summary>
        /// The Builders Club-specific offer ID.
        /// </summary>
        public int BCOfferId { get; set; }

        /// <summary>
        /// Custom parameters for the furni's behavior or appearance.
        /// </summary>
        public string CustomParams { get; set; }

        /// <summary>
        /// The special type of the furni (e.g., for rare items or unique interactions).
        /// </summary>
        public int SpecialType { get; set; }

        /// <summary>
        /// Indicates if the furni can be stood on.
        /// </summary>
        public bool CanStandOn { get; set; }

        /// <summary>
        /// Indicates if the furni can be sat on.
        /// </summary>
        public bool CanSitOn { get; set; }

        /// <summary>
        /// Indicates if the furni can be laid on.
        /// </summary>
        public bool CanLayOn { get; set; }

        /// <summary>
        /// The furni line (furniline) or collection the item belongs to.
        /// </summary>
        public string FurniLine { get; set; }

        /// <summary>
        /// The environment type or theme associated with the furni.
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        /// Indicates whether the furni is considered a rare item.
        /// </summary>
        public bool Rare { get; set; }
    }
}
