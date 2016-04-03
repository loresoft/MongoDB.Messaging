using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Messaging.Extensions;
using Xunit;

namespace MongoDB.Messaging.Tests.Extensions
{
    public class DateTimeExtensionsTests
    {

        [Fact]
        public void ToUnix()
        {
            var dateTime = new DateTime(2016, 3, 24, 21, 44, 35, DateTimeKind.Utc);
            var unixTime = dateTime.ToUnixTimeSeconds();

            unixTime.Should().Be(1458855875);
        }

        [Fact]
        public void Convert()
        {
            var dateTime = DateTime.UtcNow;

            var unixTime = dateTime.ToUnixTimeSeconds();
            var fromUnix = unixTime.FromUnixTimeSeconds();

        }
    }
}
