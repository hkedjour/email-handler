/*
* Copyright (C) 2014 Hichem Kedjour
*
* This program is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with this program. If not, see <http://www.gnu.org/licenses/>.
*
*/
using System;
using EmailHandler.Common.Configuration;

namespace EmailHandler.Common.Emails
{
    /// <summary>
    /// Encapsulate Email information.
    /// </summary>
    /// <remarks>
    /// I store only the informatoin needed, not all usual email info
    /// </remarks>
    public class MessageInfo
    {
        internal MessageInfo(ImapAccount imapAccount)
        {
            ImapAccountId = imapAccount.Id;
            MailBoxName = imapAccount.AccountName;
        }

        public MessageInfo()
        {
            
        }

        /// <summary>
        /// The email UID as describe in the imap RFC
        /// </summary>
        public int MessageUid { get; set; }

        public DateTime MessageDate { get; set; }

        public string From { get; set; }

        public string Subject { get; set; }

        public string MailBoxName { get; set; }

        public int ImapAccountId { get; set; }

        public override string ToString()
        {
            return string.Format("From {0} to {1} on {2:dd/MM HH:mm} about {3}", From, MailBoxName, MessageDate.AddHours(1),
                Subject);
        }
    }
}