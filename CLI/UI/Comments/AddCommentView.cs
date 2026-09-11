using Entities;
using RepositoryContracts;

namespace CLI.UI.Comments;

public class AddCommentView
{
    private readonly ICommentRepository commentRepository;
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository;

    public AddCommentView(ICommentRepository commentRepository, IPostRepository postRepository, IUserRepository userRepository)
    {
        this.commentRepository = commentRepository;
        this.postRepository = postRepository;
        this.userRepository = userRepository;
    }

    public async Task RunAsync()
    {
        Console.WriteLine();
        Console.WriteLine("---- Add Comment ----");

        Console.Write("Post ID: ");
        if (!int.TryParse(Console.ReadLine(), out int postId))
        {
            Console.WriteLine("Invalid post ID.");
            return;
        }

        bool postExists = postRepository.GetManyAsync().Any(p => p.Id == postId);
        if (!postExists)
        {
            Console.WriteLine($"No post found with ID {postId}.");
            return;
        }

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

        Console.Write("Comment: ");
        string body = Console.ReadLine() ?? "";

        Comment comment = new Comment { Body = body, UserId = userId, PostId = postId };
        Comment created = await commentRepository.AddAsync(comment);

        Console.WriteLine($"Comment added with ID {created.Id}.");
    }
}