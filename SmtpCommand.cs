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

    internal sealed class SmtpCommand
    {
        public const string TerminationSequence = "\r\n";
        public const string DataTerminationSequence = "\r\n.\r\n";

        private readonly string commandCode;
        private readonly string parameters;

        public SmtpCommand(string commandCode, string parameters)
        {
            if (string.IsNullOrEmpty(commandCode))
            {
                throw new ArgumentNullException("commandCode");
            }

            this.commandCode = commandCode.ToUpperInvariant();
            this.parameters = parameters;
        }

        public string Parameters
        {
            get { return this.parameters; }
        }

        public string CommandCode
        {
            get { return this.commandCode; }
        }

        public static SmtpCommand Parse(string message)
        {
            if (message.Length < 4)
            {
                return null;
            }

            var commandCode = message.Substring(0, 4);

            string parameters = null;
            if (message.Length > 4)
            {
                parameters = message.Substring(4, message.Length - 4).Trim();
            }

            return new SmtpCommand(commandCode, parameters);
        }
    }
}
