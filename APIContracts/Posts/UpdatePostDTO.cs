namespace APIContracts.Posts;

public class UpdatePostDTO
{
    public required string Title { get; set; }
    public required string Body { get; set; }
}