//---------------------------------------------------------------------------------
// Copyright (c) 2012, Microsoft Corporation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//---------------------------------------------------------------------------------

namespace Smtp
{
    using System;
    using System.Net.Mail;

    public static class Program
    {
        public static void Main()
        {
            var server = new SmtpServer(2525);
            server.Open();
            server.MessageReceived += OnMessageReceived;

            using (var client = new SmtpClient("localhost", 2525))
            {
                client.Send("willpe@microsoft.com", "willpe@microsoft.com", "Hello World", "Dear World,\r\n\r\nHello!\r\n\r\nWill");
            }

            Console.ReadLine();
            server.Close();
        }

        private static void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine(e.Message.From);
            Console.WriteLine(string.Join(", ", e.Message.To));
            Console.WriteLine();
            Console.WriteLine(e.Message.Body);
        }
    }
}
