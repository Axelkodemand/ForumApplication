using RepositoryContracts;
using CLI.UI.Comments;

namespace CLI.UI.Posts;

public class ManagePostsView
{
    private readonly CreatePostView createPostView;
    private readonly ListPostsView listPostsView;
    private readonly ViewPostView viewPostView;
    private readonly AddCommentView addCommentView;

    public ManagePostsView(IPostRepository postRepository, IUserRepository userRepository, ICommentRepository commentRepository)
    {
        createPostView = new CreatePostView(postRepository, userRepository);
        listPostsView = new ListPostsView(postRepository);
        viewPostView = new ViewPostView(postRepository, commentRepository, userRepository);
        addCommentView = new AddCommentView(commentRepository, postRepository, userRepository);
    }

    public async Task RunAsync()
    {
        bool inMenu = true;

        while (inMenu)
        {
            Console.WriteLine();
            Console.WriteLine("---- Manage Posts ----");
            Console.WriteLine("1. Create new post");
            Console.WriteLine("2. Add comment to a post");
            Console.WriteLine("3. View posts overview");
            Console.WriteLine("4. View specific post");
            Console.WriteLine("0. Back");
            Console.Write("Choose an option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    await createPostView.RunAsync();
                    break;
                case "2":
                    await addCommentView.RunAsync();
                    break;
                case "3":
                    await listPostsView.RunAsync();
                    break;
                case "4":
                    await viewPostView.RunAsync();
                    break;
                case "0":
                    inMenu = false;
                    break;
                default:
                    Console.WriteLine("Unknown option, please try again.");
                    break;
            }
        }
    }
}