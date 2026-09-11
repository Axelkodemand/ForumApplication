using CLI.UI;
using InMemoryRepositories;
using RepositoryContracts;


Console.WriteLine ("Starting CLI application");
IUserRepository userRepository = new UserInMemoryRepository();
ICommentRepository commentRepository = new CommentInMemoryRepository();
IPostRepository postRepository = new PostInMemoryRepository();

CliUiApp cliUiApp = new CliUiApp(userRepository, commentRepository, postRepository);
await cliUiApp.StartAsync();