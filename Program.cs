using System;
using System.Net.Mail;

namespace Smtp
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new SmtpServer(2525);
            server.Open();

            using (var client = new SmtpClient("localhost", 2525))
            {
                client.Send("will.perry@live.com", "iguazu@backlog.pro", "Hello World", "Dear World,\r\n\r\nHello!\r\n\r\nWill");
                client.Send("will.perry@live.com", "iguazu@backlog.pro", "Hello (again) World", "Message 2");
            }

            Console.ReadLine();

        }
    }
}
