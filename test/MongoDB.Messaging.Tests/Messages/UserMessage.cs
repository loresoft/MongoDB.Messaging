using System;

namespace MongoDB.Messaging.Tests.Messages
{
    public class UserMessage
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }


        public static UserMessage Tester()
        {
            return new UserMessage
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test.user@gmail.com",
                Phone = "888-555-1212"
            };
        }
    }
}