using FluentAssertions;
using HubSpot.NET.Core;
using Xunit;

namespace HubSpot.NET.Tests.Core
{
    public class HubSpotApiTests
    {
        private string apiKey = "";

        [Fact]
        public void ctor_WhenCalled_ShouldCreateApiClasses()
        {
            HubSpotApi sut = CreateSut();

            sut.Company.Should().NotBeNull();
            sut.Contact.Should().NotBeNull();
            sut.Deal.Should().NotBeNull();
            sut.Engagement.Should().NotBeNull();
            sut.File.Should().NotBeNull();
            sut.Owner.Should().NotBeNull();
            sut.CompanyProperties.Should().NotBeNull();
            sut.EmailSubscriptions.Should().NotBeNull();
        }


        private HubSpotApi CreateSut()
        {
            return new HubSpotApi(apiKey);
        }
    }
}
