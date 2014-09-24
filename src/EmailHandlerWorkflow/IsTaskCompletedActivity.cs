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
using EmailHandler.Common.Tasks;

namespace EmailHandlerWorkflow
{
    /// <summary>
    /// Check if a task is completed
    /// </summary>
    /// <returns>True if the task is completed</returns>
    public sealed class IsTaskCompletedActivity : CodeActivity<bool>
    {
        /// <summary>
        /// Guid of the task to be checked
        /// </summary>
        public InArgument<string> TaskGuid { get; set; }

        protected override bool Execute(CodeActivityContext context)
        {
            var taskGuid = context.GetValue(TaskGuid);

            return TasksManager.IsTaskCompleted(taskGuid);
        }
    }
}