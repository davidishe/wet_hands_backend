using System.Text.Json.Serialization;

namespace Core.Models
{
  public class BaseEntity
  {
    [JsonPropertyName("id")]
    public int Id { get; set; }

  }
}