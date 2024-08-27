using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.IO;

namespace ContaFacil.Utilities
{
    public class EmailSender
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailSender()
        {
            _smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("wherrera.web@gmail.com", "eysx ogkx abxj uaoy"),
                EnableSsl = true,
            };
            _fromEmail = "wherrera.web@gmail.com";
            _fromName = "FINANSYS";
        }

        public async Task SendEmailAsync(string to, string subject, string body, List<string> cc = null, List<Attachment> attachments = null, bool isBodyHtml = false)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isBodyHtml,
            };

            mailMessage.To.Add(to);

            if (cc != null)
            {
                foreach (var ccAddress in cc)
                {
                    mailMessage.CC.Add(ccAddress);
                }
            }

            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    mailMessage.Attachments.Add(attachment);
                }
            }

            try
            {
                await _smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al enviar el correo: {ex.Message}", ex);
            }
            finally
            {
                if (attachments != null)
                {
                    foreach (var attachment in attachments)
                    {
                        attachment.Dispose();
                    }
                }
                mailMessage.Dispose();
            }
        }

        public async Task SendFacturaEmailAsync(string to, string numeroFactura, byte[] pdfBytes)
        {
            var subject = $"Factura RIDE - {numeroFactura}";
            var body = $"Estimado cliente,\n\nAdjunto encontrará la factura RIDE número {numeroFactura}.\n\nGracias por su preferencia.";

            var attachment = new Attachment(new MemoryStream(pdfBytes), "Factura.pdf", "application/pdf");
            var attachments = new List<Attachment> { attachment };

            await SendEmailAsync(to, subject, body, attachments: attachments);
        }
    }
}