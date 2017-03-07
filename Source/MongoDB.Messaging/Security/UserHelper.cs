using System;
using System.Security.Principal;

namespace MongoDB.Messaging.Security
{
    /// <summary>
    /// A class to get user information
    /// </summary>
    public static class UserHelper
    {
        private static string GetCurrentUserName()
        {
            return "unimplemented";
        }

        /// <summary>
        /// Gets the current logged in user name
        /// </summary>
        /// <returns>The current user name</returns>
        public static string Current()
        {
            string username = GetCurrentUserName();
            string name = username;

            var parts = username.Split('\\');
            if (parts.Length == 2)
                name = parts[1];

            return name;
        }
    }
}
