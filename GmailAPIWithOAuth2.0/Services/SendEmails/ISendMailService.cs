﻿using System.Net.Mime;

namespace GmailAPIWithOAuth2.Services.SendEmails
{
    public interface ISendMailService
    {
        void Send();

        Task SendAsync();

        ISendMailService New();

        ISendMailService Subject(string subject);

        ISendMailService From(string senderAddress, string senderName = null);

        ISendMailService To(string address, string displayName = null);

        ISendMailService Cc(string address, string displayName = null);

        ISendMailService Bcc(string address, string displayName = null);

        ISendMailService ReplyTo(string address, string displayName = null);

        ISendMailService Body(string bodyContent, bool isHtml = false);

        ISendMailService BodyFromFile(string filePath, bool isHtml = false);

		ISendMailService Header(string key, string value);

        ISendMailService AddAttachment(string filePath, string name = null);

        ISendMailService UsingTemplate<T>(string template, T model, bool isHtml = true);

        ISendMailService UsingTemplateFromFile<T>(string filePath, T model, bool isHtml = true);
    }
}
