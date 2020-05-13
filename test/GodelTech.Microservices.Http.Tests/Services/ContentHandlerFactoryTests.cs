using System;
using System.IO;
using FakeItEasy;
using FluentAssertions;
using GodelTech.Microservices.Http.Services;
using GodelTech.Microservices.Http.Services.ResponseHandlers;
using Xunit;

namespace GodelTech.Microservices.Http.Tests.Services
{
    public class ContentHandlerFactoryTests
    {
        private readonly ResponseHandlerFactory _underTest;

        public ContentHandlerFactoryTests()
        {
            _underTest = new ResponseHandlerFactory(A.Fake<IJsonSerializer>());
        }

        [Theory]
        [InlineData(typeof(string), typeof(StringResponseHandler))]
        [InlineData(typeof(Stream), typeof(StreamResponseHandler))]
        [InlineData(typeof(byte[]), typeof(ByteResponseHandler))]
        [InlineData(typeof(ContentHandlerFactoryTests), typeof(DefaultResponseHandler))]
        public void Create_Should_Create_Handler_Of_Expected_Type(Type dataType, Type expectedHandlerType)
        {
            _underTest.Create(dataType).Should().BeOfType(expectedHandlerType);
        }
    }
}
