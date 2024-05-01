using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;

using ChessLib.PlayerModels;

namespace ChessDiploma.Models
{
    public class EmailSender
    {
        private const string _smtpServer = "smtp.gmail.com";
        private const string _sysEmail = "testdiploma3485@gmail.com";
        private const string _sysAppPassword = "fltd pncl uzlv ltnq";
        private const int _smtpPrt = 587;
        private User _user;
        public EmailSender(User user)
        {
            _user = user;
        }

        public bool SendMessage()
        {
            if (!IsValidEmail()) return false;

            MailMessage mail = new MailMessage(_sysEmail, _user.Email);
            mail.Subject = "Chess statistic";
            mail.Body = CreateStringToSend(_user);

            SmtpClient smtpClient = new SmtpClient(_smtpServer, _smtpPrt);
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(_sysEmail, _sysAppPassword);
            smtpClient.EnableSsl = true;

            try
            {
                smtpClient.Send(mail);
                Console.WriteLine("Mail was send!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                mail.Dispose();
                smtpClient.Dispose();
            }
            return true;
        }
        public bool IsValidEmail()
        {
            try
            {
                var addr = new MailAddress(_user.Email);
                return addr.Address == _user.Email;
            }
            catch
            {

                return false;
            }
        }
        public string CreateStringToSend(User user)
        {
            return "Wins - " + user.Wons.ToString() +
                "\nLoses - " + user.Losts.ToString() +
                "\nDraws - " + user.Draws.ToString() +
                "\nAmount of games - " +
                (user.Wons + user.Losts + user.Draws).ToString();
        }
    }
}
