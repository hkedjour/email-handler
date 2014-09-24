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
using System.Activities;
using EmailHandler.Common.Configuration;
using EmailHandler.Common.Emails;

namespace EmailHandlerWorkflow
{
    /// <summary>
    /// Send an email to the supervisor telling him that the task is still undone.
    /// </summary>
    public sealed class NotifySupervisorAboutUnhandeledEmailActivity : CodeActivity
    {
        /// <summary>
        /// Information of the received email
        /// </summary>
        public InArgument<MessageInfo> Message { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var msg = context.GetValue(Message);

            var body = string.Format("The email sent to {0} by {1} at {2:dd/MM/yyyy HH:mm} about {3} still not handled.", msg.MailBoxName, msg.From, msg.MessageDate.AddHours(1), msg.Subject);

            EmailsManager.SendEmail(ConfigurationManager.GetSupervisorEmailAddress(), "Unhandled Email", body);
        }
    }
}