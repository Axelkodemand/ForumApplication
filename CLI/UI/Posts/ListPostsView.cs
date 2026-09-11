using RepositoryContracts;

namespace CLI.UI.Posts;

public class ListPostsView
{
    private readonly IPostRepository postRepository;

    public ListPostsView(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }

    public Task RunAsync()
    {
        Console.WriteLine();
        Console.WriteLine("---- Posts Overview ----");

        var posts = postRepository.GetManyAsync().ToList();

        foreach (var post in posts)
        {
            Console.WriteLine($"[{post.Id}] {post.Title}");
        }

        return Task.CompletedTask;
    }
}