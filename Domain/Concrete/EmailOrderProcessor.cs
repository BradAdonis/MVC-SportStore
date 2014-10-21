using Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Concrete
{
    public class EmailOrderProcessor : IOrderProcessor
    {
        private EmailSettings emailSettings;

        public EmailOrderProcessor(EmailSettings settings)
        {
            emailSettings = settings;
        }

        public void ProcessOrder(Entities.Cart cart, Entities.ShippingDetails shippingdetails)
        {
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = emailSettings.UseSSL;
                smtpClient.Host = emailSettings.ServerName;
                smtpClient.Port = emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                //smtpClient.Credentials = new NetworkCredentials(emailSettings.username, emailSettings.Password);
                if (emailSettings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = emailSettings.FileLocation;
                    smtpClient.EnableSsl = false;
                }

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("A new order has been submitted");
                sb.AppendLine("------------------------------");
                sb.AppendLine("Items : ");

                foreach (var line in cart.Lines)
                {
                    var subtotal = line.Product.Price + line.Quantity;
                    sb.AppendFormat("{0} x {1} (subtotal: {2:c})", line.Product.Price, line.Quantity, subtotal);
                }

                sb.AppendFormat("Total order value : {0:c}", cart.ComputeTotal());

                sb.AppendLine("_______________________________");
                sb.AppendLine("Ship To:");
                sb.AppendLine(shippingdetails.Name);
                sb.AppendLine(shippingdetails.Line1);
                sb.AppendLine(shippingdetails.Line2 ?? "");
                sb.AppendLine(shippingdetails.Line3 ?? "");
                sb.AppendLine(shippingdetails.City);
                sb.AppendLine(shippingdetails.State ?? "");
                sb.AppendLine(shippingdetails.Country);
                sb.AppendLine(shippingdetails.Zip);
                sb.AppendLine("________________________________");

                sb.AppendFormat("Gift Wrap : {0}", shippingdetails.GiftWrap ? "Yes" : "No");

                MailMessage mailMessage = new MailMessage(emailSettings.MailFromAddress, emailSettings.MailToAddress, "New Order", sb.ToString());

                if (emailSettings.WriteAsFile)
                {
                    mailMessage.BodyEncoding = Encoding.ASCII;
                }

                smtpClient.Send(mailMessage);
            } 
        }
    }
}
