using System.Text.Json.Serialization;

namespace Cards
{
    public class CardItem
    {
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;

        [JsonIgnore]
        public string DisplayQuestion => string.IsNullOrWhiteSpace(Question) ? "New question" : Question;
    }
}