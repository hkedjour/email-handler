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
using System.Collections.Generic;
using System.Linq;

namespace EmailHandler.Common.Configuration
{
    /// <summary>
    /// Managers application configuration
    /// </summary>
    public static class ConfigurationManager
    {
        /// <summary>
        /// Return the list of all the accounts that need to be monitored
        /// </summary>
        /// <returns></returns>
        public static List<ImapAccount> GetActiveAccounts()
        {
            using (var db = new ConfigurationDb())
            {
                var activeAccounts = db.ImapAccounts.Where(a => a.IsActive).ToList();
                return activeAccounts;
            }
        }

        public static void UpdateAccountLastUid(int accountId, int uid)
        {
            using (var db = new ConfigurationDb())
            {
                var acc = db.ImapAccounts.SingleOrDefault(a => a.Id == accountId);

                if (acc == null || acc.LastUid >= uid)
                    return;

                acc.LastUid = uid;
                db.SaveChanges();
            }
        }


        /// <summary>
        /// Return the connection string to the SQL Server database used to persiste workflow instances
        /// </summary>
        public static string GetWfStoreConnectionString()
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings["SQLPersistenceStoreDb"].ConnectionString;
        }

        /// <summary>
        /// Return the supervisor email address to be used for notifications
        /// </summary>
        /// <returns></returns>
        public static string GetSupervisorEmailAddress()
        {
            return System.Configuration.ConfigurationManager.AppSettings["SupervisorEmailAddress"];
        }
    }
}