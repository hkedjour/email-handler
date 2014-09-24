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
using System.Data.Entity;

namespace EmailHandler.Common.Configuration
{
    public class ConfigurationDb : DbContext
    {
        public ConfigurationDb() : base("ConfigurationDb")
        {
        }

        public DbSet<ImapAccount> ImapAccounts { get; set; }
    }

    public class ConfigurationDbInitializer : DropCreateDatabaseIfModelChanges<ConfigurationDb>
    {
        protected override void Seed(ConfigurationDb context)
        {
            // Adding some test accounts
            context.Set<ImapAccount>().AddRange(new[]
            {
                new ImapAccount
                {
                    Id = 1,
                    AccountName = "Hichem Kedjour",
                    Host = "vm-xp-sp3",
                    UserName = "hichem@mydomain.test",
                    Password = "pass",
                    LastUid = 0,
                    IsActive = false
                },
                new ImapAccount
                {
                    Id = 2,
                    AccountName = "User",
                    Host = "vm-xp-sp3",
                    UserName = "user@mydomain.test",
                    Password = "pass",
                    LastUid = 0,
                    IsActive = false
                }
            });
        }
    }
}