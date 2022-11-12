using DeadLetterProcessing;
using Spectre.Console.Cli;

internal class Program
{
    private static int Main(string[] args)
    {
        
        CommandApp<DeadLetterCommand> app = new();
        app.Configure(config =>
        {
            string[] args = new string[1] { "-c <ConnectionString> -t <TopicName> -s <SubscriptionName>" };
            config.AddExample(args); 
        });
        int retruncode = app.Run(args);
        return retruncode;
    }
}