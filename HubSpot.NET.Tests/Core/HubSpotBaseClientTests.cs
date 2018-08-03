using System;
using System.Collections.Generic;
using System.Net;
using FakeItEasy;
using FluentAssertions;
using HubSpot.NET.Core;
using HubSpot.NET.Core.Interfaces;
using Newtonsoft.Json;
using RestSharp;
using Xunit;

namespace HubSpot.NET.Tests.Core
{
    public class HubSpotBaseClientTests
    {
        public HubSpotBaseClientTests()
        {
            client = A.Fake<IRestClient>();
            request = null;
            response = null;
        }

        private readonly string apiKey = "apiKey";
        private readonly IRestClient client;
        private IRestResponse response;
        private IRestRequest request;

        [Theory]
        [InlineData(Method.GET)]
        [InlineData(Method.POST)]
        [InlineData(Method.PUT)]
        public void Execute_WhenCalled_ShouldBuildCorrectRequest(Method expectedMethod)
        {
            string expectedUri = "expectedUri";
            object expectedEntity = new
            {
                Test = "Test"
            };

            ConfigureResponseForRequest(HttpStatusCode.OK, ResponseStatus.Completed);

            HubSpotBaseClient sut = CreateSut();

            sut.Execute(expectedUri, expectedEntity, expectedMethod);

            AssertRequest(expectedMethod, expectedUri);
        }

        
        [Fact]
        public void Execute_WhenCalled_ShouldThrowHubSpotExceptionOnError()
        {
            const string expectedUri = "expectedUri";
            object expectedEntity = new
            {
                Test = "Test"
            };

            ConfigureResponseForRequest(HttpStatusCode.NotFound, ResponseStatus.Error);

            HubSpotBaseClient sut = CreateSut();

            Action execute = () => sut.Execute(expectedUri, expectedEntity);

            execute.Should().Throw<HubSpotException>();
        }

        [Theory]
        [InlineData(Method.GET)]
        [InlineData(Method.POST)]
        [InlineData(Method.PUT)]
        public void ExecuteBatch_WhenCalled_ShouldBuildCorrectRequest(Method expectedMethod)
        {
            const string expectedUri = "expectedUri";
            List<object> expectedEntities = new List<object>
            {
                A.Fake<IHubSpotModel>()
            };

            ConfigureResponseForRequest(HttpStatusCode.OK, ResponseStatus.Completed);

            HubSpotBaseClient sut = CreateSut();

            sut.ExecuteBatch(expectedUri, expectedEntities, expectedMethod);

            AssertRequest(expectedMethod, expectedUri);
        }

        private void ConfigureResponseForRequest(HttpStatusCode statusCode, ResponseStatus responseStatus)
        {
            response = A.Fake<IRestResponse>();

            A.CallTo(() => response.StatusCode)
                .Returns(statusCode);

            A.CallTo(() => response.ResponseStatus)
                .Returns(responseStatus);

            A.CallTo(() => client.Execute(A<IRestRequest>.Ignored))
                .ReturnsLazily((IRestRequest actualRequest) =>
                {
                    request = actualRequest;
                    return response;
                });
        }

        private void AssertRequest(Method expectedMethod, string expectedUri)
        {
            request.Should().NotBeNull();
            request.Method.Should().Be(expectedMethod);
            request.Resource.Should().Be($"{expectedUri}?hapikey={apiKey}");
            request.Parameters.Should().HaveCount(1);
            request.Parameters[0].Name.Should().Be("application/json");
            request.Parameters[0].Type.Should().Be(ParameterType.RequestBody);
        }


        private HubSpotBaseClient CreateSut()
        {
            return new HubSpotBaseClient(apiKey, client);
        }
    }
}