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
using System.Activities;
using EmailHandler.Common.Emails;
using EmailHandler.Common.Tasks;

namespace EmailHandlerWorkflow
{
    /// <summary>
    /// Create a task from a message
    /// </summary>
    /// <returns>Task guid</returns>
    public sealed class CreateTaskFromMessageActivity : CodeActivity<string>
    {
        /// <summary>
        /// Information of the received email
        /// </summary>
        public InArgument<MessageInfo> Message { get; set; }

        /// <summary>
        /// Message Date + 2 days - now
        /// </summary>
        public OutArgument<TimeSpan> DueDateCountDown { get; set; }

        protected override string Execute(CodeActivityContext context)
        {
            var msg = context.GetValue(Message);

            var subject = string.Format("Handle email sent to {0} by {1} about {2}", msg.MailBoxName, msg.From, msg.Subject);
            var description = string.Format("Recieved at {0:dd/MM/yyyy HH:mm}", msg.MessageDate.AddHours(1)); // Were in UTC + 1 time zone
            var dueDate = msg.MessageDate.AddDays(2);

            var countDown = dueDate - DateTime.UtcNow;

            if (countDown.TotalSeconds < 0)
                countDown = TimeSpan.FromSeconds(0);

            context.SetValue(DueDateCountDown, countDown);
            
            return TasksManager.CreateTask(subject, description, dueDate);
        }
    }
}