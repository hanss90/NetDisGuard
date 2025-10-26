
using System.Net.NetworkInformation;

namespace NetDisGuard;

public class NetworkHandler
{
    public readonly string[] DnsHosts = ["8.8.8.8", "9.9.9.9", "8.8.4.4", "1.1.1.1"];
    private int Successes { get; set; } = 0;
    private int Failures { get; set; } = 0;
    private int NewFailures { get; set; } = 0;
    private List<string> CurrentFailureTypes { get; set; } = [];
    private int PingCount { get; set; } = 0;
    private int CurrentDnsIndex { get; set; } = 0;

    public int SleepMilliseconds { get; private set; } = 1000;
    public int PingTimeoutMilliseconds { get; private set; } = 2000;
    public int InfoEveryPings { get; private set; } = 60;


    public NetworkHandler() { }

    public NetworkHandler(int sleepMs = 1000, int pingTimeoutMs = 2000, int infoEveryPings = 60)
    {
        SleepMilliseconds = sleepMs;
        PingTimeoutMilliseconds = pingTimeoutMs;
        InfoEveryPings = infoEveryPings;
    }

    public NetworkHandler(Dictionary<ProgramArgumentEvaluator.ProgramArguments, int> options)
    {
        if (options.TryGetValue(ProgramArgumentEvaluator.ProgramArguments.SleepMilliseconds, out int sleepMs))
            SleepMilliseconds = sleepMs;
        if (options.TryGetValue(ProgramArgumentEvaluator.ProgramArguments.PingTimeoutMilliseconds, out int pingTimeoutMs))
            PingTimeoutMilliseconds = pingTimeoutMs;
        if (options.TryGetValue(ProgramArgumentEvaluator.ProgramArguments.InfoEveryPings, out int infoEveryPings))
            InfoEveryPings = infoEveryPings;
    }

    public async Task StartNetworkCheck()
    {
        Console.WriteLine("Starting to check connection, to quit press q!");
        while (true)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.Q)
                    break;
            }

            await PingOnce(DnsHosts[CurrentDnsIndex]);
            ++PingCount;
            if (PingCount == InfoEveryPings)
                PrintInfo();

            CurrentDnsIndex = CurrentDnsIndex + 1 >= DnsHosts.Length ? 0 : CurrentDnsIndex + 1;
            Thread.Sleep(SleepMilliseconds);
        }
        Console.WriteLine("Stopped checking connection!");
    }

    private void PrintInfo()
    {
        PingCount = 0;
        Console.WriteLine($"The network check has a total of {Successes} successes and {Failures} failures.");
        if (NewFailures == 0)
            return;

        Console.WriteLine($"Those include {NewFailures} new failures. Among:");
        foreach (string f in CurrentFailureTypes)
            Console.WriteLine(f);
        NewFailures = 0;
    }

    private async Task PingOnce(string host)
    {
        using Ping ping = new();
        PingReply reply = await ping.SendPingAsync(host, PingTimeoutMilliseconds);

        if (reply.Status != IPStatus.Success)
        {
            Failures++;
            NewFailures++;
            CurrentFailureTypes.Add($"Failure: {reply.Status} - Host: {host} - Time {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            return;
        }

        Successes++;
        return;
    }
}
