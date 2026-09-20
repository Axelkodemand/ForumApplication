using CLI.UI;
using FileRepositories;
using RepositoryContracts;

Console.WriteLine("Starting CLI application");
IUserRepository userRepository = new UserFileRepository();
ICommentRepository commentRepository = new CommentFileRepository();
IPostRepository postRepository = new PostFileRepository();

CliUiApp cliUiApp = new CliUiApp(userRepository, commentRepository, postRepository);
await cliUiApp.StartAsync();