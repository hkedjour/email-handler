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
using System.Globalization;
using System.Text;
using EmailHandler.Common.OutlookServices;

namespace EmailHandler.Common.Tasks
{
    /// <summary>
    /// Outlook/SmarterMail task Wrapper
    /// </summary>
    public class OutlookTask
    {
        private readonly ExTaskItem _task;

        public OutlookTask()
        {
            _task = new ExTaskItem {key = new ArrayOfString(), val = new ArrayOfString()};

            SetStringValue("priority", "5");
            SetStringValue("percentComplete", "0");
            SetStringValue("status", "InProcess");  // InProcess. Not started is not support by this web service, only (NeedsAction,InProcess,Completed) are supported

            Guid = System.Guid.NewGuid().ToString("N");
        }

        public OutlookTask(ExTaskItem task)
        {
            _task = task;
        }

        public string Subject
        {
            get { return GetStringValue("subject"); }
            set { SetStringValue("subject", value); }
        }

        public string Description
        {
            get { return GetStringValue("description"); }
            set { SetStringValue("description", value); }
        }

        public DateTime StartDate
        {
            get { return GetDateValue("startDate"); }
            set { SetDateValue("startDate", value); }
        }

        public DateTime DueDate
        {
            get { return GetDateValue("dueDate"); }
            set { SetDateValue("dueDate", value); }
        }

        public static implicit operator ExTaskItem(OutlookTask task)
        {
            return task._task;
        }

        public string Guid
        {
            get { return GetStringValue("guid"); }
            private set { SetStringValue("guid", value); }
        }

        public bool IsCompleted
        {
            get
            {
                var stringValue = GetStringValue("status");
                return stringValue == "Completed";
            }
        }

        /// <summary>
        /// Get the value of a specific key in string form
        /// </summary>
        /// <param name="keyName">Key of the value to be retrieved</param>
        /// <returns>The value for that key</returns>
        private string GetStringValue(string keyName)
        {
            var idx = _task.key.IndexOf(keyName);
            return Encoding.UTF8.GetString(Convert.FromBase64String(_task.val[idx]));
        }

        /// <summary>
        /// Set the value of the supplied key
        /// </summary>
        /// <param name="keyName">key</param>
        /// <param name="strVal">The value to be set</param>
        private void SetStringValue(string keyName, string strVal)
        {
            var idx = _task.key.IndexOf(keyName);
            var encodedValue = string.IsNullOrWhiteSpace(strVal)
                ? ""
                : Convert.ToBase64String(Encoding.UTF8.GetBytes(strVal));

            if (idx < 0)
            {
                _task.key.Add(keyName);
                _task.val.Add(encodedValue);
            }
            else
            {
                _task.val[idx] = encodedValue;
            }
        }

        /// <summary>
        /// Get the DateTime value of the specified key
        /// </summary>
        /// <param name="keyName">key of the value to be retrieved</param>
        /// <returns>The value</returns>
        private DateTime GetDateValue(string keyName)
        {
            var str = GetStringValue(keyName);

            DateTime dt;
            if (
                !DateTime.TryParseExact(str, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out dt))
                dt = DateTime.MinValue;

            return dt;
        }


        /// <summary>
        /// Set the date value of the supplied key
        /// </summary>
        /// <param name="keyName">key</param>
        /// <param name="dtVal">The value to be set</param>
        private void SetDateValue(string keyName, DateTime dtVal)
        {
            SetStringValue(keyName, dtVal.ToString("MM/dd/yyyy HH:mm:ss"));
        }

        public override string ToString()
        {
            return string.Format("Task {0}. \"{1}\"", Subject, Description);
        }
    }
}