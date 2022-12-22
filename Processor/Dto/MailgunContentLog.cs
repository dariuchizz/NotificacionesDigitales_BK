using Newtonsoft.Json;
using System;

namespace Processor.Dto
{

    public partial class MailgunContentLog
    {
        [JsonProperty("items")]
        public Item[] Items { get; set; }

        [JsonProperty("paging")]
        public Paging Paging { get; set; }
    }

    public partial class Item
    {
        [JsonProperty("tags")]
        public object[] Tags { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("timestamp")]
        public double Timestamp { get; set; }

        [JsonProperty("severity")]
        public string Severity { get; set; }

        [JsonProperty("event")]
        public string Event { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("campaigns")]
        public object[] Campaigns { get; set; }

        [JsonProperty("user-variables")]
        public UserVariables UserVariables { get; set; }

        [JsonProperty("envelope")]
        public Envelope Envelope { get; set; }

        [JsonProperty("flags")]
        public Flags Flags { get; set; }

        [JsonProperty("delivery-status")]
        public DeliveryStatus DeliveryStatus { get; set; }

        [JsonProperty("message")]
        public Message Message { get; set; }

        [JsonProperty("recipient")]
        public string Recipient { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }
    }

    public partial class DeliveryStatus
    {
        [JsonProperty("tls")]
        public bool Tls { get; set; }

        [JsonProperty("mx-host")]
        public string MxHost { get; set; }

        [JsonProperty("bounce-code")]
        public string BounceCode { get; set; }

        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("session-seconds")]
        public double SessionSeconds { get; set; }

        [JsonProperty("retry-seconds")]
        public long RetrySeconds { get; set; }

        [JsonProperty("attempt-no")]
        public long AttemptNo { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("certificate-verified")]
        public bool CertificateVerified { get; set; }
    }

    public partial class Envelope
    {
        [JsonProperty("transport")]
        public string Transport { get; set; }

        [JsonProperty("sender")]
        public string Sender { get; set; }

        [JsonProperty("sending-ip")]
        public string SendingIp { get; set; }

        [JsonProperty("targets")]
        public string Targets { get; set; }
    }

    public partial class Flags
    {
        [JsonProperty("is-authenticated")]
        public bool IsAuthenticated { get; set; }

        [JsonProperty("is-test-mode")]
        public bool IsTestMode { get; set; }
    }

    public partial class Message
    {
        [JsonProperty("headers")]
        public Headers Headers { get; set; }

        [JsonProperty("attachments")]
        public object[] Attachments { get; set; }

        [JsonProperty("recipients")]
        public string[] Recipients { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }
    }

    public partial class Headers
    {
        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("message-id")]
        public string MessageId { get; set; }

        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }
    }

    public partial class UserVariables
    {
    }

    public partial class Paging
    {
        [JsonProperty("next")]
        public Uri Next { get; set; }

        [JsonProperty("previous")]
        public Uri Previous { get; set; }
    }
}