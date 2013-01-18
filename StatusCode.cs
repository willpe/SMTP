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
    public enum ReplyCode
    {
        SystemStatus = 211,
        HelpMessage = 214,
        ServiceReady = 220,
        ServiceClosing = 221,
        Ok = 250,
        MessageForwarded = 251,
        StartMailInput = 354,
        ServiceNotAvailable = 421,
        MailboxBusy = 450,
        LocalProcessingError = 451,
        InsufficientStorage = 452,
        CommandUnrecognized = 500,
        CommandArgumentError = 501,
        CommandNotImplemented = 502,
        BadCommandSequence = 503,
        CommandParameterNotImplemented = 504,
        MailboxNotFound = 550,
        UserNotLocal = 551,
        ExceededStorageAllocation = 552,
        MailboxSyntaxIncorrect = 553,
        TransactionFailed = 554
    }
}
