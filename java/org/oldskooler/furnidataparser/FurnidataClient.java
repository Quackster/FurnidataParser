package org.oldskooler.furnidataparser;

import java.io.StringReader;
import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.util.*;
import java.util.regex.*;
import javax.xml.parsers.*;
import org.w3c.dom.*;
import org.xml.sax.InputSource;

/**
 * A client for downloading and parsing Habbo furnidata in either XML or chunked JSON format.
 */
public class FurnidataClient {

    private final HttpClient httpClient;

    /**
     * Initializes a new instance of the FurnidataClient class.
     */
    public FurnidataClient() {
        this.httpClient = HttpClient.newHttpClient();
    }

    /**
     * Initializes a new instance of the FurnidataClient class.
     * Allows injecting a custom HttpClient or uses a default one.
     */
    public FurnidataClient(HttpClient httpClient) {
        this.httpClient = httpClient != null ? httpClient : HttpClient.newHttpClient();
    }

    /**
     * Fetches furnidata from the specified URL and parses it into a list of FurniItem objects.
     * Automatically determines if the data is XML or chunked JSON.
     */
    public List<FurniItem> fetchFurnidata(String url) throws Exception {
        HttpRequest request = HttpRequest.newBuilder()
                .uri(URI.create(url))
                .header("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                        "AppleWebKit/537.36 (KHTML, like Gecko) " +
                        "Chrome/120.0.0.0 Safari/537.36")
                .GET()
                .build();

        HttpResponse<String> response = httpClient.send(request, HttpResponse.BodyHandlers.ofString());

        if (response.statusCode() == 307 || response.statusCode() == 308) {
            String location = response.headers().firstValue("Location")
                    .orElseThrow(() -> new RuntimeException("Redirect location not provided!"));
            // System.out.println("Redirected to: " + location);

            HttpRequest redirectRequest = HttpRequest.newBuilder()
                    .uri(URI.create(location))
                    .header("User-Agent", request.headers().firstValue("User-Agent").orElse(""))
                    .GET()
                    .build();

            response = httpClient.send(redirectRequest, HttpResponse.BodyHandlers.ofString());
        }

        // proceed as normal
        String data = response.body();
        return parseFurnidata(data);
    }

    /**
     * Parses the furnidata from a string containing either XML or chunked JSON,
     * automatically detecting the format.
     */
    private List<FurniItem> parseFurnidata(String data) throws Exception {
        if (data == null || data.trim().isEmpty()) return new ArrayList<>();

        if (isXmlFile(data)) {
            return parseXml(data);
        } else {
            return parseChunkedJson(data);
        }
    }

    private List<FurniItem> parseChunkedJson(String data) {
        List<FurniItem> items = new ArrayList<>();
        Matcher chunkMatcher = Pattern.compile("\\[\\[.*?\\]\\]", Pattern.DOTALL).matcher(data);

        while (chunkMatcher.find()) {
            String chunk = chunkMatcher.group();
            Matcher itemMatcher = Pattern.compile("\\[(.*?)\\]").matcher(chunk);

            while (itemMatcher.find()) {
                List<String> fields = splitFields(itemMatcher.group(1));

                if (fields.size() < 1) continue;

                FurniItem item = new FurniItem();
                item.type = getField(fields, 0);
                item.id = tryParseInt(getField(fields, 1), 0);
                item.className = getField(fields, 2);
                item.revision = tryParseInt(getField(fields, 3), 0);
                item.category = getField(fields, 4);
                item.xDim = tryParseInt(getField(fields, 5), 0);
                item.yDim = tryParseInt(getField(fields, 6), 0);
                item.partColors = getField(fields, 7);
                item.name = getField(fields, 8);
                item.description = getField(fields, 9);
                item.adUrl = getField(fields, 10);
                item.offerId = tryParseInt(getField(fields, 11), 0);
                item.buyout = isTrue(getField(fields, 12));
                item.rentOfferId = tryParseInt(getField(fields, 13), 0);
                item.rentBuyout = tryParseInt(getField(fields, 14), 0);
                item.bc = isTrue(getField(fields, 15));
                item.excludedDynamic = isTrue(getField(fields, 16));
                item.bcOfferId = tryParseInt(getField(fields, 17), 0);
                item.customParams = getField(fields, 18);
                item.specialType = tryParseInt(getField(fields, 19), 0);
                item.canStandOn = isTrue(getField(fields, 20));
                item.canSitOn = isTrue(getField(fields, 21));
                item.canLayOn = isTrue(getField(fields, 22));
                item.furniLine = getField(fields, 23);
                item.environment = getField(fields, 24);
                item.rare = isTrue(getField(fields, 25));

                items.add(item);
            }
        }

        return items;
    }

    private List<FurniItem> parseXml(String xml) throws Exception {
        List<FurniItem> items = new ArrayList<>();
        DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
        factory.setIgnoringComments(true);
        factory.setIgnoringElementContentWhitespace(true);
        DocumentBuilder builder = factory.newDocumentBuilder();
        Document doc = builder.parse(new InputSource(new StringReader(xml)));

        NodeList furnitypes = doc.getElementsByTagName("furnitype");
        for (int i = 0; i < furnitypes.getLength(); i++) {
            Element node = (Element) furnitypes.item(i);
            FurniItem item = new FurniItem();

            String parentName = node.getParentNode() != null ? node.getParentNode().getNodeName().toLowerCase() : "";
            item.type = "roomitemtypes".equals(parentName) ? "s" : "i";
            item.id = tryParseInt(node.getAttribute("id"), 0);
            item.className = node.getAttribute("classname");
            item.revision = tryParseInt(getNodeText(node, "revision"), 0);
            item.category = getNodeText(node, "category");
            item.xDim = tryParseInt(getNodeText(node, "xdim"), 0);
            item.yDim = tryParseInt(getNodeText(node, "ydim"), 0);

            NodeList colorNodes = getNodeList(node, "partcolors/color");
            List<String> colors = new ArrayList<>();

            if (colorNodes != null) {
                for (int j = 0; j < colorNodes.getLength(); j++) {
                    colors.add(colorNodes.item(j).getTextContent());
                }
            }

            item.partColors = String.join(",", colors);

            item.name = getNodeText(node, "name");
            item.description = getNodeText(node, "description");
            item.adUrl = getNodeText(node, "adurl");
            item.offerId = tryParseInt(getNodeText(node, "offerid"), 0);
            item.buyout = isTrue(getNodeText(node, "buyout"));
            item.rentOfferId = tryParseInt(getNodeText(node, "rentofferid"), 0);
            item.rentBuyout = tryParseInt(getNodeText(node, "rentbuyout"), 0);
            item.bc = isTrue(getNodeText(node, "bc"));
            item.excludedDynamic = isTrue(getNodeText(node, "excludeddynamic"));
            item.bcOfferId = tryParseInt(getNodeText(node, "bcofferid"), 0);
            item.customParams = getNodeText(node, "customparams");
            item.specialType = tryParseInt(getNodeText(node, "specialtype"), 0);
            item.canStandOn = isTrue(getNodeText(node, "canstandon"));
            item.canSitOn = isTrue(getNodeText(node, "cansiton"));
            item.canLayOn = isTrue(getNodeText(node, "canlayon"));
            item.furniLine = getNodeText(node, "furniline");
            item.environment = getNodeText(node, "environment");
            item.rare = isTrue(getNodeText(node, "rare"));

            items.add(item);
        }

        return items;
    }

    private static List<String> splitFields(String item) {
        List<String> fields = new ArrayList<>();
        Matcher matcher = Pattern.compile("\"((?:\\\\.|[^\"])*?)\"").matcher(item);
        while (matcher.find()) {
            fields.add(matcher.group(1));
        }
        return fields;
    }

    private static String getField(List<String> fields, int index) {
        return index < fields.size() ? fields.get(index) : "";
    }

    private static boolean isXmlFile(String content) {
        try {
            DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
            factory.setValidating(false);
            factory.setNamespaceAware(false);
            DocumentBuilder builder = factory.newDocumentBuilder();
            builder.parse(new InputSource(new StringReader(content)));
            return true;
        } catch (Exception e) {
            return false;
        }
    }

    private static boolean isTrue(String value) {
        if (value == null) return false;
        return "1".equals(value) || "true".equalsIgnoreCase(value);
    }

    private static int tryParseInt(String value, int fallback) {
        try {
            return Integer.parseInt(value);
        } catch (Exception e) {
            return fallback;
        }
    }

    private static String getNodeText(Element parent, String tag) {
        NodeList nodes = parent.getElementsByTagName(tag);
        return nodes.getLength() > 0 ? nodes.item(0).getTextContent() : "";
    }

    private static NodeList getNodeList(Element parent, String xpath) {
        String[] parts = xpath.split("/");
        Element current = parent;
        for (int i = 0; i < parts.length - 1; i++) {
            NodeList nodes = current.getElementsByTagName(parts[i]);
            if (nodes.getLength() == 0) return null;
            current = (Element) nodes.item(0);
        }
        return current.getElementsByTagName(parts[parts.length - 1]);
    }
}
