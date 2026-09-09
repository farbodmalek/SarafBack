using System.Text.Json.Serialization;

namespace CommonLibrary.Application.DTO.Common
{
    public class KeyValueDto : IKeyValueEntity<int>
    {
        public KeyValueDto()
        {

        }
        public int ID { get; set; }
        public int? ParentID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        [JsonPropertyName("Key")]
        public int Key
        {
            get
            {
                return this.ID;
            }
            set
            {
                this.ID = value;
            }
        }

        [JsonPropertyName("Master")]
        public IKeyValueEntity<int> Master { get; set; }

        [JsonPropertyName("Value")]
        public string Value
        {
            get
            {
                return this.Title;
            }
            set
            {
                this.Title = value;
            }
        }

        IKeyValueEntity<int> IKeyValueEntity<int>.Master
        {
            get
            {
                return null;
            }
        }
    }

    public class KeyValueDto<TKey> : IKeyValueEntity<TKey>
    {


        public int ID { get; set; }
        //public int? ParentID { get; set; }
        //public string Title { get; set; }

        [JsonPropertyName("Key")]
        public TKey Key
        {
            get; set;
        }

        [JsonPropertyName("Master")]
        public IKeyValueEntity<TKey> Master { get; set; }

        [JsonPropertyName("Value")]
        public string Value
        {
            get; set;
        }
    }
    //public class KeyValueDto : IKeyValueEntity
    //{

    //    public string Desc { get; set; }
    //    public int? ParentID { get; set; } 
    //    public int ID { get; set; }

    //    public string Title { get; set; }
    //}
    //public class BaseKeyValue 
    //{
    //    public int ID { get; set; }

    //    public string Desc { get; set; }

    //}
}