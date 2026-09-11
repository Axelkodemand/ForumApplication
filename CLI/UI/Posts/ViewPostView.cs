using RepositoryContracts;

namespace CLI.UI.Posts;

public class ViewPostView
{
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentRepository;
    private readonly IUserRepository userRepository;

    public ViewPostView(IPostRepository postRepository, ICommentRepository commentRepository, IUserRepository userRepository)
    {
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
        this.userRepository = userRepository;
    }

    public async Task RunAsync()
    {
        Console.WriteLine();
        Console.WriteLine("---- View Post ----");

        Console.Write("Post ID: ");
        if (!int.TryParse(Console.ReadLine(), out int postId))
        {
            Console.WriteLine("Invalid post ID.");
            return;
        }

        try
        {
            var post = await postRepository.GetSingleAsync(postId);

            Console.WriteLine();
            Console.WriteLine($"Title: {post.Title}");
            Console.WriteLine($"Body:  {post.Body}");

            Console.WriteLine();
            Console.WriteLine("Comments:");

            var comments = commentRepository.GetManyAsync()
                .Where(c => c.PostId == postId)
                .ToList();

            if (!comments.Any())
            {
                Console.WriteLine("  No comments yet.");
            }
            else
            {
                foreach (var comment in comments)
                {
                    Console.WriteLine($"  - {comment.Body}");
                }
            }
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine($"No post found with ID {postId}.");
        }
    }
}