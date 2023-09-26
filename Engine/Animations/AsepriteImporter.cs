using System;
using System.Globalization;
using System.IO;
using LostHope.Engine.ContentLoading;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LostHope.Engine.Animations
{
    public partial class AsepriteJsonExportRoot
    {
        [JsonProperty("frames")]
        public FrameElement[] Frames { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }

    public partial class FrameElement
    {
        [JsonProperty("filename")]
        public string Filename { get; set; }

        [JsonProperty("frame")]
        public SpriteSourceSizeClass Frame { get; set; }

        [JsonProperty("rotated")]
        public bool Rotated { get; set; }

        [JsonProperty("trimmed")]
        public bool Trimmed { get; set; }

        [JsonProperty("spriteSourceSize")]
        public SpriteSourceSizeClass SpriteSourceSize { get; set; }

        [JsonProperty("sourceSize")]
        public Size SourceSize { get; set; }

        [JsonProperty("duration")]
        public long Duration { get; set; }
    }

    public partial class SpriteSourceSizeClass
    {
        [JsonProperty("x")]
        public long X { get; set; }

        [JsonProperty("y")]
        public long Y { get; set; }

        [JsonProperty("w")]
        public long W { get; set; }

        [JsonProperty("h")]
        public long H { get; set; }
    }

    public partial class Size
    {
        [JsonProperty("w")]
        public long W { get; set; }

        [JsonProperty("h")]
        public long H { get; set; }
    }

    public partial class Meta
    {
        [JsonProperty("app")]
        public Uri App { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("format")]
        public string Format { get; set; }

        [JsonProperty("size")]
        public Size Size { get; set; }

        [JsonProperty("scale")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Scale { get; set; }

        [JsonProperty("frameTags")]
        public FrameTag[] FrameTags { get; set; }

        [JsonProperty("slices")]
        public object[] Slices { get; set; }
    }

    public partial class FrameTag
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("from")]
        public long From { get; set; }

        [JsonProperty("to")]
        public long To { get; set; }

        [JsonProperty("direction")]
        public string Direction { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("repeat", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Repeat { get; set; }
    }

    public partial class AsepriteJsonExportRoot
    {
        public static AsepriteJsonExportRoot FromJson(string json) => JsonConvert.DeserializeObject<AsepriteJsonExportRoot>(json, LostHope.Engine.Animations.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this AsepriteJsonExportRoot self) => JsonConvert.SerializeObject(self, LostHope.Engine.Animations.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }



    public class AsepriteExportData
    {
        public Texture2D Texture { get; set; }
        public AsepriteJsonExportRoot AsepriteData { get; set; }

        public AsepriteExportData(string asepriteAnimationFileName)
        {
            // Loading the texture
            ContentLoader.LoadTexture(asepriteAnimationFileName, asepriteAnimationFileName);
            Texture = ContentLoader.GetTexture(asepriteAnimationFileName);

            // Loading the json file
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", asepriteAnimationFileName + ".json");
            string json = File.ReadAllText(path);
            AsepriteData = AsepriteJsonExportRoot.FromJson(json);
        }
    }
}
