using CLI.UI.Users;
using CLI.UI.Posts;
using RepositoryContracts;

namespace CLI.UI;

public class CliUiApp
{
    private readonly IUserRepository userRepository;
    private readonly ICommentRepository commentRepository;
    private readonly IPostRepository postRepository;
    private readonly ManageUsersView manageUsersView;
    private readonly ManagePostsView managePostsView;

    public CliUiApp(IUserRepository userRepository, ICommentRepository commentRepository, IPostRepository postRepository)
    {
        this.userRepository = userRepository;
        this.commentRepository = commentRepository;
        this.postRepository = postRepository;
        manageUsersView = new ManageUsersView(userRepository);
        managePostsView = new ManagePostsView(postRepository, userRepository, commentRepository);
    }

    public async Task StartAsync()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine();
            Console.WriteLine("==== Forum Application ====");
            Console.WriteLine("1. Manage users");
            Console.WriteLine("2. Manage posts");
            Console.WriteLine("0. Exit");
            Console.Write("Choose an option: ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await manageUsersView.RunAsync();
                    break;
                case "2":
                    await managePostsView.RunAsync();
                    break;
                case "0":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Unknown option, please try again.");
                    break;
            }
        }

        Console.WriteLine("Goodbye!");
    }
}   