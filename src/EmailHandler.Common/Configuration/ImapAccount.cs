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
namespace EmailHandler.Common.Configuration
{
    /// <summary>
    /// Encapsulate email account information
    /// </summary>
    public class ImapAccount
    {
        /// <summary>
        /// The account Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Friendly name of the account set by the user.
        /// </summary>
        public string AccountName { get; set; }
        public string Host { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        /// <summary>
        /// The last uid of the last handled message
        /// </summary>
        public int LastUid { get; set; }

        /// <summary>
        /// Whether the account is active or not
        /// </summary>
        public bool IsActive { get; set; }
    }
}