using Entities;
using RepositoryContracts;

namespace CLI.UI.Posts;

public class CreatePostView
{
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository;

    public CreatePostView(IPostRepository postRepository, IUserRepository userRepository)
    {
        this.postRepository = postRepository;
        this.userRepository = userRepository;
    }

    public async Task RunAsync()
    {
        Console.WriteLine();
        Console.WriteLine("---- Create New Post ----");

        Console.Write("Title: ");
        string title = Console.ReadLine() ?? "";

        Console.Write("Body: ");
        string body = Console.ReadLine() ?? "";

        Console.Write("Your user ID: ");
        if (!int.TryParse(Console.ReadLine(), out int userId))
        {
            Console.WriteLine("Invalid user ID.");
            return;
        }

        bool userExists = userRepository.GetManyAsync().Any(u => u.Id == userId);
        if (!userExists)
        {
            Console.WriteLine($"No user found with ID {userId}.");
            return;
        }

        Post post = new Post { Title = title, Body = body, UserId = userId };
        Post created = await postRepository.AddAsync(post);

        Console.WriteLine($"Post '{created.Title}' created with ID {created.Id}.");
    }
}