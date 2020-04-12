using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using LinnworksCategoryApi.Controllers;
using NUnit.Framework;
using LinnworksCategoryApi.Headers;
using LinnworksCategoryApi.HttpClients;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace LinnworksCategoryApiTests
{
    [TestFixture]
    public class RequestConfiguratorTests
    {
        private IConfigurator<HttpContext, HttpRequestMessage> _sut;
        private CategoryController _controller;

        [OneTimeSetUp]
        public void Setup()
        {
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


            _sut = new RequestHeadersConfigurator();

        }

        [Test]
        public void RequestConfigurator_Copies_Authorization_From_Source_To_Target_Test()
        {
            var requestMessage = new HttpRequestMessage();
            var expectedAuthValue = Guid.NewGuid().ToString();
            var expectedValue = new AuthenticationHeaderValue(expectedAuthValue);

            _controller.HttpContext.Request.Headers["Authorization"] = expectedAuthValue;

            // Act
            _sut.Apply(_controller.HttpContext, requestMessage);

            Assert.That(requestMessage.Headers.Authorization, Is.EqualTo(expectedValue));
        }


        [Test]
        public void RequestConfigurator_Copies_AcceptHeaders_From_Source_To_Target_Test()
        {
            var requestMessage = new HttpRequestMessage();

            var expectedAcceptHeaderValues = "application/json,text/html,application/xhtml+xml";

            _controller.HttpContext.Request.Headers.Add("Accept", expectedAcceptHeaderValues);

            // Act
            _sut.Apply(_controller.HttpContext, requestMessage);

            requestMessage.Headers.TryGetValues("Accept", out var requestMessageResponseAcceptHeaders);

            Assert.That(
                string.Join(",", requestMessageResponseAcceptHeaders.ToArray()), 
                Is.EqualTo(expectedAcceptHeaderValues));
        }

    }
}