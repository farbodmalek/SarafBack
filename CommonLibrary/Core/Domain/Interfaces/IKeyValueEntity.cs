using System.Text.Json.Serialization;

namespace CommonLibrary.Application.DTO.Common
{
    public interface IKeyValueEntity
    {
        public int ID { get; set; }
        public string Title { get; set; }
    }
    public interface IKeyValueEntity<TKey>
    {
        [JsonPropertyName("Key")]
        TKey Key { get; }
        [JsonPropertyName("Value")]
        string Value { get; }
        [JsonPropertyName("Master")]
        IKeyValueEntity<TKey> Master { get; }
    }
}
