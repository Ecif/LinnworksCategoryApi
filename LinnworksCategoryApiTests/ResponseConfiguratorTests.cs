using System.Net.Http;
using System.Net.Http.Headers;
using AutoFixture;
using LinnworksCategoryApi.Controllers;
using LinnworksCategoryApi.Headers;
using LinnworksCategoryApi.HttpClients;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace LinnworksCategoryApiTests
{
    [TestFixture]
    public class ResponseConfiguratorTests
    {
        private IConfigurator<HttpResponseMessage, HttpResponse> _sut;
        private CategoryController _controller;
        private IFixture _fixture;

        [OneTimeSetUp]
        public void SetUp()
        {
            _fixture = new Fixture();

            _controller = new CategoryController(new Mock<ICategoryApiClient>().Object,
                new Mock<IDashboardApiClient>().Object,
                new Mock<ILogger<CategoryController>>().Object,
                new Mock<IConfigurator<HttpResponseMessage, HttpResponse>>().Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };


            _sut = new ResponseHeadersConfigurator();
        }

        [Test]
        public void ResponseConfigurator_Copies_Content_Type_From_Source_To_Target_Test()
        {
            var httpContent = new Mock<HttpContent>();

            var responseMessage = new HttpResponseMessage { Content = httpContent.Object };

            var expectedContentType = new MediaTypeHeaderValue("text/plain");

            responseMessage.Content.Headers.ContentType = expectedContentType;

            // Act
            _sut.Apply(responseMessage, _controller.Response);

            _controller.Response.Headers.TryGetValue("Content-Type", out var contentTypeValueString);

           
            Assert.That(contentTypeValueString.ToString(), Is.EqualTo(expectedContentType.ToString()));
        }
    }
}