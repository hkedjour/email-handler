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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ActiveUp.Net.Mail;
using EmailHandler.Common.Configuration;

namespace EmailHandler.Common.Emails
{
    /// <summary>
    /// Manages all the operations related to emails.
    /// </summary>
    public static class EmailsManager
    {
        /// <summary>
        /// Gets all new messages from all accounts
        /// </summary>
        /// <remarks>
        /// This methond connect to the configuration database to get the monitored accounts.
        /// Connects to each account and get latest email from the last uid.
        /// This methond don't update the last uid.
        /// </remarks>
        /// <returns>New messages list</returns>
        public static List<MessageInfo> GetAllNewMessages()
        {
            return ConfigurationManager.GetActiveAccounts().SelectMany(GetNewMessages).ToList();
        }


        /// <summary>
        /// Get all new emails since the supplied uid
        /// </summary>
        /// <param name="account">The account to be checked</param>
        /// <returns>List of messages</returns>
        public static List<MessageInfo> GetNewMessages(ImapAccount account)
        {
            try
            {
                // We create Imap client
                using (var imap = new Imap4Client())
                {
                    imap.ConnectSsl(account.Host);

                    // Login to mail box
                    imap.Login(account.UserName, account.Password);

                    var inbox = imap.SelectMailbox("inbox");

                    // Fetching new messages
                    // For newly configured accounts, we fetch only unread messages for the first time. After that
                    // we look for new messaged whether or not they are read.
                    var searchQuery = account.LastUid <= 0
                        ? "UNSEEN"
                        : "ALL"; // should be string.Format("UID {0}:*", account.LastUid + 1); but my server don't support it

                    return inbox.Search(searchQuery)
                        //.Where(uid => inbox.Fetch.Uid(uid) > account.LastUid)    // Since my email server don't support SEARCH UID x:*
                        .Where(uid => uid > account.LastUid)    // TODO: This or previous line depending or if your email server support SEARCH UID x:* or not
                        .Select(m =>
                        {
                            var msg = new MessageInfo(account)
                            {
                                MessageUid = inbox.Fetch.Uid(m)
                            };

                            var header = inbox.Fetch.HeaderObject(m);
                            msg.MessageDate = header.ReceivedDate;
                            msg.Subject = header.Subject;
                            msg.From = string.IsNullOrWhiteSpace(header.From.Name) ? header.From.Email : header.From.Name;

                            return msg;
                        })
                        .ToList();
                }
            }
            catch (Exception)
            {
                return new List<MessageInfo>(); // In general we get connection errors
            }
        }

        public static void SetLastUid(int accountId, int uid)
        {
            ConfigurationManager.UpdateAccountLastUid(accountId, uid);
        }

        /// <summary>
        /// Send an email from this application
        /// </summary>
        /// <param name="to">Destination address</param>
        /// <param name="subject">Message Subject</param>
        /// <param name="body">Message body</param>
        public static void SendEmail(string to, string subject, string body)
        {
            var settings = System.Configuration.ConfigurationManager.AppSettings;

            // We create the message object
            var message = new SmtpMessage();

            // We assign the sender email from application settings
            message.From.Email = settings["AppEmailFrom"];

            // We assign the message
            message.To.Add(to);
            message.Subject = subject;
            message.BodyText.Text = body;

            try
            {
                message.Send(host: settings["AppEmailSmtpHost"],
                    username: settings["AppEmailUser"],
                    password: settings["AppEmailPassword"],
                    mechanism: SaslMechanism.CramMd5);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error While sending notfication to supervisor " + ex.Message);
            }
        }
    }
}