using RepositoryContracts;

namespace CLI.UI.Users;

public class ManageUsersView
{
    private readonly IUserRepository userRepository;
    private readonly CreateUserView createUserView;
    private readonly ListUsersView listUsersView;
    public ManageUsersView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
        createUserView = new CreateUserView(userRepository);
        listUsersView = new ListUsersView(userRepository);
    }

    public async Task RunAsync()
    {
        bool inMenu = true;

        while (inMenu)
        {
            Console.WriteLine();
            Console.WriteLine("---- Manage Users ----");
            Console.WriteLine("1. Create new user");
            Console.WriteLine("2. View all users");
            Console.WriteLine("0. Back");
            Console.Write("Choose an option: ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await createUserView.RunAsync();
                    break;
                case "2":
                    await listUsersView.RunAsync();
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