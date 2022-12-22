namespace Processor.Dto
{
    public class MailgunRequest
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public bool Tracking { get; set; }
        public bool TrackingClicks { get; set; }
        public bool TrackingOpens { get; set; }
        public string Tag { get; set; }
        public string Template { get; set; }
        public string Variables { get; set; }
        public string Description { get; set; }
        public string TemplateHtml { get; set; }

        public MailgunRequest()
        {
            Tracking = true;
            TrackingClicks = true;
            TrackingOpens = true;        }
    }
}
