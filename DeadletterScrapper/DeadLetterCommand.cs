using Azure.Messaging.ServiceBus;
using CsvHelper;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Globalization;

namespace DeadLetterProcessing;
internal sealed class DeadLetterCommand : AsyncCommand<DeadLetterSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, DeadLetterSettings settings)
    {
        AnsiConsole.MarkupLine("[green]Started application. Stay tuned![/]");

        if (settings.ConnectionSetting is null || settings.TopicName is null || settings.SubscriptionName is null)
        {
            AnsiConsole.MarkupLine("Conneciton String, Topic Name and Subscription Name is mandatory");
            return 1;
        }

        await using ServiceBusClient client = new ServiceBusClient(settings.ConnectionSetting);
        
        string SbNamespace = client.FullyQualifiedNamespace.Split('.')[0];

        ServiceBusReceiverOptions receiverOptions = new ServiceBusReceiverOptions
        {
            ReceiveMode = settings.RecieveAndDelete ? ServiceBusReceiveMode.ReceiveAndDelete : ServiceBusReceiveMode.PeekLock,
            SubQueue = settings.TransferDeadletter ? SubQueue.TransferDeadLetter : SubQueue.DeadLetter
        };

        ServiceBusReceiver receiver = client.CreateReceiver(settings.TopicName, settings.SubscriptionName, receiverOptions);

        List<DeadLetterModel> dlMessages = new();

        int batch = settings.MaxCount == 0 ? 100 : settings.MaxCount;
        int totalRead = 0;

        for (int i = 0; i < batch; i++)
        {
            var messages = await receiver.ReceiveMessagesAsync(1);

            if (messages.Count == 0)
            {
                break;
            }

            totalRead += messages.Count;

            var message = messages[0];
            DeadLetterModel dlMessage = new DeadLetterModel
            {
                SBNamespace = SbNamespace, 
                TopicName = settings.TopicName,
                SubscriptionName = settings.SubscriptionName,
                DeadletterErrorDescription = message.DeadLetterErrorDescription,
                DeadletterReason = message.DeadLetterReason,
                DeadletterSource = message.DeadLetterSource,
                EnqueueSequenceNumber = message.EnqueuedSequenceNumber,
                EnqueueTime = message.EnqueuedTime.DateTime,
                Reciepent = message.To,
                Body = settings.PopulateBody ? message.Body.ToString() : String.Empty
            };

            dlMessages.Add(dlMessage);

        }

        int returncode = settings.useSQL ? await WriteToSQL(settings, dlMessages) : WriteToCSVFile(settings, dlMessages);

        return returncode;
    }

    private static int WriteToCSVFile(DeadLetterSettings settings, List<DeadLetterModel> dlMessages)
    {
        string filePath = GetFileName(settings);

        if (string.IsNullOrEmpty(filePath)) return 1;

        using FileStream stream = File.Open(filePath, FileMode.Append);
        using StreamWriter writer = new StreamWriter(stream);
        using CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        csv.WriteRecords(dlMessages);

        writer.Flush();

        AnsiConsole.MarkupLine("Processed [green]{0}[/] messages and stored in [yellow]{1}[/]", dlMessages.Count, filePath);
        return 0;
    }

    private static async Task<int> WriteToSQL(DeadLetterSettings settings, List<DeadLetterModel> dlMessages)
    {
        if (settings.SQLConnectionString is null)
        {
            AnsiConsole.MarkupLine("[red]useSQL flag is set but connction string was not provided[/]");
            return 1;
        }
        try
        {
            using DeadLetterDbContext db = new DeadLetterDbContext(settings.SQLConnectionString);

            AnsiConsole.MarkupLine("[green]Created Connection to db[/]");

            await db.AddRangeAsync(dlMessages);
            await db.SaveChangesAsync();

            AnsiConsole.MarkupLine("[green]Saved to db[/]");

            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
        }

        return 1;
    }

    private static string GetFileName(DeadLetterSettings settings)
    {
        string path = settings.Path ?? Directory.GetCurrentDirectory();

        if (!Directory.Exists(path))
        {
            AnsiConsole.MarkupLine($"[red]Unable to find directory {path}[/]");
            return null;
        }

        string fileName1 = settings.SubscriptionName + Guid.NewGuid().ToString() + ".csv";
        string fileName = fileName1.Replace("-", "_");
        string filePath = Path.Combine(path, fileName);

        return filePath;
    }
}
