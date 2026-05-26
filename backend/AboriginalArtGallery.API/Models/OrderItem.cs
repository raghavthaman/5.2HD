using System.Text.Json.Serialization;

namespace AboriginalArtGallery.API.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        [JsonIgnore] public Order? Order { get; set; }

        public int ArtifactId { get; set; }
        public Artifact? Artifact { get; set; }

        public string ArtifactTitle { get; set; } = string.Empty; // snapshot at purchase time
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal LineTotal { get; set; }
    }
}
