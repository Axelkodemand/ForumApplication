namespace Entities;

public class Comment
{
    public int Id { get; set; }
    public string Body { get; set; } = "";
    public int UserId { get; set; } //fk
    public int PostId { get; set; } //fk
}