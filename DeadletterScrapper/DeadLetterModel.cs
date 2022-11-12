using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadLetterProcessing;

[Table("DeadLetterMessages")]
internal record DeadLetterModel
{
    [Key]
    public Guid Id { get; set; } = new Guid();
    [Required]
    public string SBNamespace { get; set; }
    [Required]
    public string TopicName { get; set; }
    [Required]
    public string SubscriptionName { get; set; }
    [Required]
    public string DeadletterErrorDescription { get; set; }
    public string DeadletterSource { get; set; }
    [Required]
    public string DeadletterReason { get; set; }
    [Required]
    public DateTime EnqueueTime { get; set; }
    [Required]
    public long EnqueueSequenceNumber { get; set; }
    public string Reciepent { get; set; }
    public string Body { get; set; }

}
