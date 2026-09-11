using RepositoryContracts;

namespace CLI.UI.Users;

public class ListUsersView
{
    private readonly IUserRepository userRepository;

    public ListUsersView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public Task RunAsync()
    {
        Console.WriteLine();
        Console.WriteLine("---- All Users ----");

        var users = userRepository.GetManyAsync().ToList();

        foreach (var user in users)
        {
            Console.WriteLine($"[{user.Id}] {user.Username}");
        }

        return Task.CompletedTask;
    }
}