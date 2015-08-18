using System;
using System.Security.Principal;
using System.Web;
using System.Web.Hosting;

namespace MongoDB.Messaging.Security
{
    /// <summary>
    /// A class to get user information
    /// </summary>
    public static class UserHelper
    {
        private static string GetCurrentUserName()
        {
            if (!HostingEnvironment.IsHosted)
                return Environment.UserName;

            IPrincipal currentUser = null;
            HttpContext current = HttpContext.Current;
            if (current != null)
                currentUser = current.User;

            if ((currentUser != null))
                return currentUser.Identity.Name;

            return Environment.UserName;
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
