using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BookmarkReader.Model;
public abstract class BookmarkElement
{
    public BookmarkElement() {}
    [JsonPropertyName("date_added")]
    [JsonPropertyOrder(-7)]
    public string DateAdded {get; set;}
    [JsonPropertyName("date_last_used")]
    [JsonPropertyOrder(-6)]
    public string DateLastUsed {get; set;}
    [JsonPropertyName("guid")]
    [JsonPropertyOrder(-4)]
    public string Guid {get; set;}
    [JsonPropertyName("id")]
    [JsonPropertyOrder(-3)]
    public string Id {get; set;}
    [JsonPropertyName("name")]
    [JsonPropertyOrder(-2)]
    public string Name {get; set;}
    [JsonPropertyName("type")]
    [JsonPropertyOrder(-1)]
    public string Type {get; set;}
    public virtual string Url {get; set;}
    public virtual string DateModified {get; set;}
    public virtual List<BookmarkElement> Children{get; set;}
}

public class Bookmark : BookmarkElement
{
    public Bookmark() { }
    [JsonPropertyName("url")]
    public override string Url {get; set;} 
}

public class Folder : BookmarkElement
{
    public Folder() {}
    [JsonPropertyName("date_modified")]
    [JsonPropertyOrder(-5)]
    public override string DateModified {get; set;}
    [JsonPropertyName("children")]
    [JsonPropertyOrder(-8)]
    public override List<BookmarkElement> Children {get; set;}
}

// public class Root : BookmarkElement
// {
//     public Root() {}
//     [JsonPropertyName("date_modified")]
//     public string DateModified {get; set;}
//     [JsonPropertyName("children")]
//     public List<BookmarkElement> RootFolder {get; set;}
// }

public class BookmarkModel
{
    public BookmarkModel() {}
    [JsonPropertyName("checksum")]
    public string Checksum {get; set;}
    [JsonPropertyName("roots")]
    public Dictionary<string, Folder> Roots {get; set;}
    [JsonPropertyName("version")]
    public int Version {get; set;}
}

public class BookmarkElementConverter : JsonConverter<BookmarkElement>
{
    public override BookmarkElement? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if(reader.TokenType != JsonTokenType.StartObject) 
			throw new JsonException();

        using var jsonDocument = JsonDocument.ParseValue(ref reader);
        if (!jsonDocument.RootElement.TryGetProperty("type", out var typeProperty))
            throw new JsonException();
        return typeProperty.GetString() switch
        {
            "url" => jsonDocument.RootElement.Deserialize<Bookmark>(options),
            "folder" => jsonDocument.RootElement.Deserialize<Folder>(options),
            _ => throw new JsonException(),
        };
    }

    public override void Write(Utf8JsonWriter writer, BookmarkElement value, JsonSerializerOptions options) =>
		JsonSerializer.Serialize(writer, value, value.GetType(), options);
}