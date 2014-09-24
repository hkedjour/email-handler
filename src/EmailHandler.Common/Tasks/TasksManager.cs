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
using System.Configuration;
using EmailHandler.Common.OutlookServices;

namespace EmailHandler.Common.Tasks
{
    public static class TasksManager
    {
        /// <summary>
        /// Creata a task and return its guid.
        /// </summary>
        /// <param name="subject">Task subject</param>
        /// <param name="description">Task description</param>
        /// <param name="dueDate">Task due date</param>
        /// <returns>The Guid of the newly created task</returns>
        public static string CreateTask(string subject, string description, DateTime dueDate)
        {
            using (var srv = new svcOutlookAddinSoapClient())
            {
                var task = new OutlookTask
                {
                    Subject = subject,
                    Description = description,
                    StartDate = DateTime.Today,
                    DueDate = dueDate
                };
                var res = srv.AddTask(ConfigurationManager.AppSettings["TasksUserName"], ConfigurationManager.AppSettings["TasksUserPassword"], task);

                if (res.ResultCode != 0)
                    throw new Exception(res.Message);

                return task.Guid;
            }
        }

        /// <summary>
        /// Get Whether the task is completed or not
        /// </summary>
        /// <param name="guid">Task guid</param>
        /// <returns>returns true if the task is completed</returns>
        public static bool IsTaskCompleted(string guid)
        {
            using (var srv = new svcOutlookAddinSoapClient())
            {

                var res = srv.GetTask(ConfigurationManager.AppSettings["TasksUserName"], ConfigurationManager.AppSettings["TasksUserPassword"], guid);

                if (res.ResultCode != 0)
                    throw new Exception(res.Message);

                if (res.ExTask.key == null) // Task was deleted
                {
                    return true;
                }

                return new OutlookTask(res.ExTask).IsCompleted;
            }
            
        }
    }
}