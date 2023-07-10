using Newtonsoft.Json;
using System.Text;

namespace OlağanWeb.Models;

public class CommentModel
{
    public int? CommentId { get; set; }
    public int? TextId { get; set; }
    public string Comments { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public string? Title { get; set; }

    public string token { get; set; }

    public DateTime? CommentDate { get; set; }

}
public class CaptchaResponse
{
    public bool success { get; set; }
    public double score { get; set; }
    public string action { get; set; }
    public DateTime challenge_ts { get; set; }
    public string hostname { get; set; }
    [JsonProperty("error-codes")]
    public List<string> error_codes { get; set; }
}
