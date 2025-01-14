namespace PicusPicks.Web.Models;

public class GameDTO
{
    public int Id { get; set; }
    public int ExternalGameId { get; set; }
    public int HomeTeamId { get; set; }
    public int AwayTeamId { get; set; }
    public DateTime GameTime { get; set; }
    public DateTime PickDeadline { get; set; }
    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }
    public bool IsComplete { get; set; }
    public int Week { get; set; }
    public int Season { get; set; }
} 