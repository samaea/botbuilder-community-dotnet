// Licensed under the MIT License. 

namespace Bot.Builder.Community.Dialogs.FormFlow.Luis.Models
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Microsoft.Rest;

    [Serializable]
    public partial class EntityRecommendation
    {
    }

    [Serializable]
    public partial class IntentRecommendation
    {
    }

    /// <summary>
    /// Luis entity recommendation. Look at https://www.luis.ai/Help for more
    /// information.
    /// </summary>
    public partial class EntityRecommendation
    {
        /// <summary>
        /// Initializes a new instance of the EntityRecommendation class.
        /// </summary>
        public EntityRecommendation() { }

        /// <summary>
        /// Initializes a new instance of the EntityRecommendation class.
        /// </summary>
        public EntityRecommendation(string type, string role = default(string), string entity = default(string), int? startIndex = default(int?), int? endIndex = default(int?), double? score = default(double?), IDictionary<string, object> resolution = default(IDictionary<string, object>))
        {
            Role = role;
            Entity = entity;
            Type = type;
            StartIndex = startIndex;
            EndIndex = endIndex;
            Score = score;
            Resolution = resolution;
        }

        /// <summary>
        /// Role of the entity.
        /// </summary>
        [JsonProperty(PropertyName = "role")]
        public string Role { get; set; }

        /// <summary>
        /// Entity extracted by LUIS.
        /// </summary>
        [JsonProperty(PropertyName = "entity")]
        public string Entity { get; set; }

        /// <summary>
        /// Type of the entity.
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Start index of the entity in the LUIS query string.
        /// </summary>
        [JsonProperty(PropertyName = "startIndex")]
        public int? StartIndex { get; set; }

        /// <summary>
        /// End index of the entity in the LUIS query string.
        /// </summary>
        [JsonProperty(PropertyName = "endIndex")]
        public int? EndIndex { get; set; }

        /// <summary>
        /// Score assigned by LUIS to detected entity.
        /// </summary>
        [JsonProperty(PropertyName = "score")]
        public double? Score { get; set; }

        /// <summary>
        /// A machine interpretable resolution of the entity.  For example the
        /// string "one thousand" would have the resolution "1000".  The
        /// exact form of the resolution is defined by the entity type and is
        /// documented here: https://www.luis.ai/Help#PreBuiltEntities.
        /// </summary>
        [JsonProperty(PropertyName = "resolution", ItemConverterType = typeof(ResolutionConverter))]
        public IDictionary<string, object> Resolution { get; set; }

        /// <summary>
        /// Validate the object. Throws ValidationException if validation fails.
        /// </summary>
        public virtual void Validate()
        {
            if (Type == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Type");
            }
        }

        internal class ResolutionConverter : JsonConverter
        {
            private const string UnexpectedEndError = "Unexpected end when reading IDictionary<string, object>";

            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(IDictionary<string, object>));
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return ReadValue(reader);
            }

            private static object ReadValue(JsonReader reader)
            {
                while (reader.TokenType == JsonToken.Comment)
                {
                    if (!reader.Read())
                    {
                        throw new JsonSerializationException("Unexpected token when converting IDictionary<string, object>");
                    }
                }

                switch (reader.TokenType)
                {
                    case JsonToken.StartObject:
                        return ReadObject(reader);
                    case JsonToken.StartArray:
                        return ReadArray(reader);
                    case JsonToken.Integer:
                    case JsonToken.Float:
                    case JsonToken.String:
                    case JsonToken.Boolean:
                    case JsonToken.Undefined:
                    case JsonToken.Null:
                    case JsonToken.Date:
                    case JsonToken.Bytes:
                        return reader.Value;
                    default:
                        throw new JsonSerializationException
                            (string.Format("Unexpected token when converting IDictionary<string, object>: {0}", reader.TokenType));
                }
            }

            private static object ReadArray(JsonReader reader)
            {
                IList<object> list = new List<object>();

                while (reader.Read())
                {
                    switch (reader.TokenType)
                    {
                        case JsonToken.Comment:
                            break;
                        default:
                            var value = ReadValue(reader);

                            list.Add(value);
                            break;
                        case JsonToken.EndArray:
                            return list;
                    }
                }

                throw new JsonSerializationException(UnexpectedEndError);
            }

            private static object ReadObject(JsonReader reader)
            {
                var dictionary = new Dictionary<string, object>();

                while (reader.Read())
                {
                    switch (reader.TokenType)
                    {
                        case JsonToken.PropertyName:
                            var propertyName = reader.Value.ToString();

                            if (!reader.Read())
                            {
                                throw new JsonSerializationException(UnexpectedEndError);
                            }

                            var value = ReadValue(reader);

                            dictionary[propertyName] = value;
                            break;
                        case JsonToken.Comment:
                            break;
                        case JsonToken.EndObject:
                            return dictionary;
                    }
                }

                throw new JsonSerializationException(UnexpectedEndError);
            }
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value);
            }
        }
    }
}
