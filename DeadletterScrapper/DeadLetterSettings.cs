using Spectre.Console.Cli;
using System.ComponentModel;

namespace DeadLetterProcessing;
internal sealed class DeadLetterSettings : CommandSettings
{
    [Description("Connection String of the Azure Service Bus namespace")]
    [CommandOption("-c|--connectionstring")]
    public string ConnectionSetting { get; init; }

    [Description("Topic name of the Azure Service Bus namespace")]
    [CommandOption("-t|--topic")]
    public string TopicName { get; init; }

    [Description("Subscription name of the Azure Service Bus namespace")]
    [CommandOption("-s|--subscription")]
    public string SubscriptionName { get; init; }

    [Description("Location where you would like to store")]
    [CommandOption("-p|--path")]
    public string Path { get; init; }

    [Description("Batch size to recieve from ServiceBus (int)")]
    [CommandOption("-b|--batch")]
    public int MaxCount { get; init; } = 0;

    [Description("Flag to indicate if the Serivce bus message reciver option is to recieve and then delete the message. Default is false (Peek Mode)")]
    [CommandOption("--recieve")]
    public bool RecieveAndDelete { get; init; }

    [Description("flag indicates if you want to store the body of the message in the file. Default is false")]
    [CommandOption("--capturepayload")]
    public bool PopulateBody { get; init; } = false;

    [Description("Flag to indicate if you want to connect to transfer deadletter instead of regular deadletter. Default is false")]
    [CommandOption("--useTransferDeadletter")]
    public bool TransferDeadletter { get; init; } = false;

    [Description("Sql connection string to use if --useSQL is set to true")]
    [CommandOption("--sqlConnectionString")]
    public string SQLConnectionString { get; set; }

    [Description("Set this flag to use SQL to store the data instead of a file")]
    [CommandOption("--useSQL")]
    public bool useSQL { get; set; } = false;
}
