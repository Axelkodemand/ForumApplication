using Entities;
using RepositoryContracts;

namespace CLI.UI.Users;

public class CreateUserView
{
    private readonly IUserRepository userRepository;

    public CreateUserView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task RunAsync()
    {
        Console.WriteLine();
        Console.WriteLine("---- Create New User ----");

        Console.Write("Username: ");
        string username = Console.ReadLine() ?? "";

        Console.Write("Password: ");
        string password = Console.ReadLine() ?? "";

        User user = new User
        {
            Username = username,
            Password = password
        };

        User created = await userRepository.AddAsync(user);

        Console.WriteLine($"User '{created.Username}' created with ID {created.Id}.");
    }
}