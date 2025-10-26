namespace NetDisGuard;

public static class Program
{
    public static async Task Main(string[] args)
    {
        NetworkHandler networkHandler;
        if (args.Length > 0)
            networkHandler = new(ProgramArgumentEvaluator.EvaluateArgs(args));
        else
            networkHandler = new();


        Console.WriteLine("Welcome to network check!");
        await networkHandler.StartNetworkCheck();
        Console.WriteLine("Closing the program, goodbye!");
    }


}