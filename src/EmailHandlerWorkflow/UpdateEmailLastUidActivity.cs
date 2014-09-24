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
using EmailHandler.Common.Emails;

namespace EmailHandlerWorkflow
{
    /// <summary>
    /// Update the LastUid field in the supplied account Settings
    /// </summary>
    public sealed class UpdateEmailLastUidActivity : CodeActivity
    {
        // Message to be updated
        public InArgument<MessageInfo> Message { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            var msg = context.GetValue(Message);

            EmailsManager.SetLastUid(msg.ImapAccountId, msg.MessageUid);
        }
    }
}