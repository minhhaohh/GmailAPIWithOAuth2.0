using FluentEmail.Core;
using System.Text;

namespace GmailAPIWithOAuth2.Services.SendEmails
{
    public class FluentMailService : ISendMailService
    {
        private readonly IFluentEmail _fluentEmail;

        public FluentMailService(IFluentEmail fluentEmail)
        {
            _fluentEmail = fluentEmail;
        }

        public virtual void Send()
        {
            var response = _fluentEmail.Send();

            if (response.Successful)
            {
                Console.WriteLine("Email sent successfully!");
            }
            else
            {
                Console.WriteLine("Failed to send email: " + string.Join(", ", response.ErrorMessages));
            }
        }

        public virtual async Task SendAsync()
        {
            var response = await _fluentEmail.SendAsync();

            if (response.Successful)
            {
                Console.WriteLine("Email sent successfully!");
            }
            else
            {
                Console.WriteLine("Failed to send email: " + string.Join(", ", response.ErrorMessages));
            }
        }

        public virtual ISendMailService New()
        {
            return this;
        }

        public virtual ISendMailService Subject(string subject)
        {
            _fluentEmail.Subject(subject);
            return this;
        }

        public virtual ISendMailService From(string senderAddress, string senderName = null)
        {
            _fluentEmail.SetFrom(senderAddress, senderName);
            return this;
        }

        public virtual ISendMailService To(string address, string displayName = null)
        {
            _fluentEmail.To(address, displayName);
            return this;
        }

        public virtual ISendMailService Cc(string address, string displayName = null)
        {
            _fluentEmail.CC(address, displayName);
            return this;
        }

        public virtual ISendMailService Bcc(string address, string displayName = null)
        {
            _fluentEmail.BCC(address, displayName);
            return this;
        }

        public virtual ISendMailService ReplyTo(string address, string displayName = null)
        {
            _fluentEmail.ReplyTo(address, displayName);
            return this;
        }

        public virtual ISendMailService Body(string bodyContent, bool isHtml = false)
        {
            _fluentEmail.Body(bodyContent, isHtml);

            return this;
        }

        public virtual ISendMailService BodyFromFile(string filePath, bool isHtml = false)
        {
            StreamReader sr;

            if (filePath.ToLower().StartsWith("http"))
            {
                var client = new HttpClient();
                sr = new StreamReader(client.GetStreamAsync(filePath).Result);
            }
            else
            {
                sr = new StreamReader(filePath, Encoding.Default);
            }

            _fluentEmail.Body(sr.ReadToEnd(), isHtml);
            sr.Close();

            return this;
        }

        public virtual ISendMailService Header(string key, string value)
        {
            _fluentEmail.Header(key, value);
            return this;
        }

        public virtual ISendMailService AddAttachment(string filePath, string name = null)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath)) return this;

            var fileName = string.IsNullOrWhiteSpace(name) ? Path.GetFileName(filePath) : name;

            _fluentEmail.AttachFromFilename(filePath,attachmentName: fileName);

            return this;
        }

        public virtual ISendMailService UsingTemplate<T>(string template, T model, bool isHtml = true)
        {
            _fluentEmail.UsingTemplate(template, model, isHtml);

            return this;
        }

        public virtual ISendMailService UsingTemplateFromFile<T>(string filePath, T model, bool isHtml = true)
        {
            _fluentEmail.UsingTemplateFromFile(filePath, model, isHtml);

            return this;
        }
    }   
}
