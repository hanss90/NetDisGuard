namespace NetDisGuard;

public static class ProgramArgumentEvaluator
{
    public static readonly Dictionary<ProgramArguments, string[]> Options = new()
    {
        {ProgramArguments.SleepMilliseconds, ["SleepMilliseconds", "sleep", "s"]},
        {ProgramArguments.PingTimeoutMilliseconds, ["PingTimeoutMilliseconds", "pingTimeout", "timeout", "p", "t"]},
        {ProgramArguments.InfoEveryPings, ["InfoEveryPings", "info", "i"]}
    };

    private static string RemoveStartingMinusses(this string s)
    {
        while (s.StartsWith('-'))
            s = s[1..s.Length];
        return s;
    }

    public static Dictionary<ProgramArguments, int> EvaluateArgs(string[] args)
    {
        if (args.Length == 0) return [];

        Dictionary<ProgramArguments, int> evaluatedArgs = [];
        for (int i = 0; i < args.Length; i++)
        {
            string s = args[i].RemoveStartingMinusses().ToLower();
            if (!args[i].StartsWith('-'))
                continue;

            if (Options[ProgramArguments.SleepMilliseconds].Any(optionString => optionString.ToLower().Equals(s)))
            {
                if (i + 1 < args.Length && int.TryParse(args[i + 1], out int sleep))
                {
                    evaluatedArgs[ProgramArguments.SleepMilliseconds] = sleep;
                    i++;
                }
                else
                    Console.WriteLine($"Invalid or missing value for {args[i]}");
            }
            else if (Options[ProgramArguments.PingTimeoutMilliseconds].Any(optionString => optionString.ToLower().Equals(s)))
            {
                if (i + 1 < args.Length && int.TryParse(args[i + 1], out int timeout))
                {
                    evaluatedArgs[ProgramArguments.PingTimeoutMilliseconds] = timeout;
                    i++;
                }
                else
                    Console.WriteLine($"Invalid or missing value for {args[i]}");
            }
            else if (Options[ProgramArguments.InfoEveryPings].Any(optionString => optionString.ToLower().Equals(s)))
            {
                if (i + 1 < args.Length && int.TryParse(args[i + 1], out int info))
                {
                    evaluatedArgs[ProgramArguments.InfoEveryPings] = info;
                    i++;
                }
                else
                    Console.WriteLine($"Invalid or missing value for {args[i]}");
            }
            else
                Console.WriteLine($"Unknown arg or failure: {args[i]}");
        }

        return evaluatedArgs;
    }

    public enum ProgramArguments
    {
        SleepMilliseconds,
        PingTimeoutMilliseconds,
        InfoEveryPings
    }
}
