package org.oldskooler.furnidataparser;

/**
 * Represents a single furniture item (furni) in Habbo furnidata.
 * Contains metadata describing the item's properties.
 */
public class FurniItem {

    /**
     * The type of the furni, e.g., "s" or "i".
     */
    public String type;

    /**
     * The unique ID of the furni.
     */
    public int id;

    /**
     * The class name of the furni.
     */
    public String className;

    /**
     * The revision number of the furni asset.
     */
    public int revision;

    /**
     * The category of the furni.
     */
    public String category;

    /**
     * The X dimension (width) of the furni in tiles.
     */
    public int xDim;

    /**
     * The Y dimension (length) of the furni in tiles.
     */
    public int yDim;

    /**
     * The part colors associated with the furni, comma-separated.
     */
    public String partColors;

    /**
     * The display name of the furni.
     */
    public String name;

    /**
     * The description of the furni.
     */
    public String description;

    /**
     * The advertisement URL associated with the furni.
     */
    public String adUrl;

    /**
     * The primary offer ID of the furni, used for purchasing.
     */
    public int offerId;

    /**
     * Indicates whether the furni can be bought outright.
     */
    public boolean buyout;

    /**
     * The offer ID for renting the furni.
     */
    public int rentOfferId;

    /**
     * The buyout price of the rental offer.
     */
    public int rentBuyout;

    /**
     * Indicates if the furni is a Builders Club exclusive.
     */
    public boolean bc;

    /**
     * This field is unknown.
     */
    public boolean excludedDynamic;

    /**
     * The Builders Club-specific offer ID.
     */
    public int bcOfferId;

    /**
     * Custom parameters for the furni's behavior or appearance.
     */
    public String customParams;

    /**
     * The special type of the furni (e.g., for rare items or unique interactions).
     */
    public int specialType;

    /**
     * Indicates if the furni can be stood on.
     */
    public boolean canStandOn;

    /**
     * Indicates if the furni can be sat on.
     */
    public boolean canSitOn;

    /**
     * Indicates if the furni can be laid on.
     */
    public boolean canLayOn;

    /**
     * The furni line (furniline) or collection the item belongs to.
     */
    public String furniLine;

    /**
     * The environment type or theme associated with the furni.
     */
    public String environment;

    /**
     * Indicates whether the furni is considered a rare item.
     */
    public boolean rare;
}
