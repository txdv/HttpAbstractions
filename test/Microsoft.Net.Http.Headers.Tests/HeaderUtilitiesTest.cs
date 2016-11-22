// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace Microsoft.Net.Http.Headers
{
    public class HeaderUtilitiesTest
    {
        private const string Rfc1123Format = "r";

        [Theory]
        [MemberData(nameof(TestValues))]
        public void ReturnsSameResultAsRfc1123String(DateTimeOffset dateTime, bool quoted)
        {
            var formatted = dateTime.ToString(Rfc1123Format);
            var expected = quoted ? $"\"{formatted}\"" : formatted;
            var actual = HeaderUtilities.FormatDate(dateTime, quoted);

            Assert.Equal(expected, actual);
        }

        public static TheoryData<DateTimeOffset, bool> TestValues
        {
            get
            {
                var data = new TheoryData<DateTimeOffset, bool>();

                var now = DateTimeOffset.Now;

                foreach (var quoted in new[] { true, false })
                {
                    for (var i = 0; i < 60; i++)
                    {
                        data.Add(now.AddSeconds(i), quoted);
                        data.Add(now.AddMinutes(i), quoted);
                        data.Add(now.AddDays(i), quoted);
                        data.Add(now.AddMonths(i), quoted);
                        data.Add(now.AddYears(i), quoted);
                    }
                }

                return data;
            }
        }

        [Theory]
        [InlineData("h=1", "h", 1)]
        [InlineData("header1=3, header2=10", "header1", 3)]
        [InlineData("header1   =45, header2=80", "header1", 45)]
        [InlineData("header1=   89   , header2=22", "header1", 89)]
        [InlineData("header1=   89   , header2= 42", "header2", 42)]
        public void TryParseTimeSpan_Succeeds(string headerValues, string targetValue, int expectedValue)
        {
            TimeSpan? value;
            Assert.True(HeaderUtilities.TryParseTimeSpan(new StringValues(headerValues), targetValue, out value));
            Assert.Equal(TimeSpan.FromSeconds(expectedValue), value);
        }

        [Theory]
        [InlineData("h=", "h")]
        [InlineData("header1=, header2=10", "header1")]
        [InlineData("header1   , header2=80", "header1")]
        [InlineData("h=10", "header")]
        [InlineData("", "")]
        [InlineData(null, null)]
        public void TryParseTimeSpan_Fails(string headerValues, string targetValue)
        {
            TimeSpan? value;
            Assert.False(HeaderUtilities.TryParseTimeSpan(new StringValues(headerValues), targetValue, out value));
        }
    }
}
