using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace ChessDiploma.Models
{
    public  static class SendCodeForTwoFA
    {
        private const string _checkString = "ABCDEFGHIGKLMNOPQRSTUVWXZabcdefghigklmnopqrstuvwxz";
        private const int _amountOfSymbolsInMessage = 5;

        public static string  accountSid = "AC18ea5d73765f39cda6b0c8f2a5b398a5";
        public static string authToken = "20c9c2ade41b32aa74233245fcece619";

        public static string _code;

        public static void SendMessage(string phoneNumber)
        {
            string testPhone = "+380992893550";
            _code = GetMessageToSend();

            TwilioClient.Init(accountSid, authToken);

            var messageOptions = new CreateMessageOptions(
              new PhoneNumber(phoneNumber));
            messageOptions.From = new PhoneNumber("+18484004294");
            messageOptions.Body = _code;


            var message = MessageResource.Create(messageOptions);
            Console.WriteLine(message.Body);
        }
        private static string GetMessageToSend()
        {
            Random rnd = new Random();
            string res = "";

            for(int i = 0; i < _amountOfSymbolsInMessage; i++)
            {
                res += _checkString[rnd.Next(0, _checkString.Length - 1)];
            }

            return res;
        }
    }
}
