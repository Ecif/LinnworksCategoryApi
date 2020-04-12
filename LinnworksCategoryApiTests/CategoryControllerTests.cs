using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using LinnworksCategoryApi.Controllers;
using LinnworksCategoryApi.Headers;
using LinnworksCategoryApi.HttpClients;
using LinnworksCategoryApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using HttpStatusCode = System.Net.HttpStatusCode;

namespace LinnworksCategoryApiTests
{
    public class CategoryControllerTests
    {

        private CategoryController _sut;
        private Mock<ICategoryApiClient> _categoryApiClient;
        private Mock<IDashboardApiClient> _dashboardApiClient;
        private IFixture Fixture { get; set; }

        [SetUp]
        public void Setup()
        {
            Fixture = new Fixture();
            _categoryApiClient = new Mock<ICategoryApiClient>();
            _dashboardApiClient = new Mock<IDashboardApiClient>();
            _sut = new CategoryController(
                _categoryApiClient.Object, 
                _dashboardApiClient.Object, 
                new Mock<ILogger<CategoryController>>().Object,
                new Mock<IConfigurator<HttpResponseMessage, HttpResponse>>().Object)
            {
                ControllerContext = new ControllerContext()
            };
        }

        [Test]
        public void GetCategories_Returns_Categories_List_From_HttpResponseMessage()
        {
            var category1 = Fixture.Create<Category>();
            var category2 = Fixture.Create<Category>();
            var expectedCategoryList = new List<Category>()
            {
                category1, category2
            };

            var stringContent = new StringContent(JsonSerializer.Serialize(expectedCategoryList));

            var apiClientResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = stringContent };

            _categoryApiClient
                .Setup(x => x.GetCategories(_sut.ControllerContext.HttpContext, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(apiClientResponse));

            var response = _sut.GetCategories(It.IsAny<CancellationToken>());

            var result = response.Result as JsonResult;

            var returnList = result.Value as List<Category>;

            Assert.That(response, Is.Not.Null);
            Assert.That(result, Is.Not.Null);
            Assert.That(returnList.Count, Is.EqualTo(expectedCategoryList.Count));
            Assert.That(returnList[0].CategoryId, Is.EqualTo(expectedCategoryList[0].CategoryId));
            Assert.That(returnList[1].CategoryName, Is.EqualTo(expectedCategoryList[1].CategoryName));
        }

        [Test]
        public void CreateCategory_Returns_New_CategoryObject()
        {
            Category createdCategory = Fixture.Create<Category>();

            var stringContent = new StringContent(JsonSerializer.Serialize(createdCategory));

            var apiClientResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = stringContent };

            _categoryApiClient
                .Setup(x => x.CreateCategory(createdCategory.CategoryName, 
                    _sut.ControllerContext.HttpContext, 
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(apiClientResponse));

            var response = _sut.CreateCategory(createdCategory.CategoryName, It.IsAny<CancellationToken>());

            var result = response.Result as JsonResult;

            var newCategory = result.Value as Category;

            Assert.That(response, Is.Not.Null);
            Assert.That(result, Is.Not.Null);
            Assert.That(newCategory.CategoryName, Is.EqualTo(createdCategory.CategoryName));
        }

        [Test]
        public void UpdateCategory_Returns_StatusCode()
        {
            Category category = Fixture.Create<Category>();

            var apiClientResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.NoContent };

            _categoryApiClient
                .Setup(x => x.UpdateCategory(category,
                    _sut.ControllerContext.HttpContext,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(apiClientResponse));

            var response = _sut.UpdateCategory(category, It.IsAny<CancellationToken>());

            var result = response.Result as JsonResult;

            var statusCode = result.Value;

            Assert.That(response, Is.Not.Null);
            Assert.That(result, Is.Not.Null);
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public void DeleteCategory_Returns_StatusCode()
        {
            Category category = Fixture.Create<Category>();

            var apiClientResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.NoContent };

            _categoryApiClient
                .Setup(x => x.DeleteCategoryById(category.CategoryId,
                    _sut.ControllerContext.HttpContext,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(apiClientResponse));

            var response = _sut.DeleteCategory(category.CategoryId, It.IsAny<CancellationToken>());

            var result = response.Result as JsonResult;

            var statusCode = result.Value;

            Assert.That(response, Is.Not.Null);
            Assert.That(result, Is.Not.Null);
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public void GetCategoriesWithProductCount_Returns_Categories_With_Count_From_HttpResponseMessage()
        {
            var category1 = Fixture.Create<Category>();
            var category2 = Fixture.Create<Category>();
            var category3 = Fixture.Create<Category>();
            var expectedCategoryList = new List<Category>()
            {
                category1, category2, category3
            };

            var stringContent = new StringContent(JsonSerializer.Serialize(new DashboardResponse(){Results = expectedCategoryList}));

            var apiClientResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = stringContent };

            var categoriesWithProductCountSql =
                "SELECT p.CategoryName, p.CategoryId, Count(S.CategoryId) AS ProductCount " +
                "FROM StockItem AS S " +
                "RIGHT JOIN ProductCategories AS P " +
                "ON P.CategoryId = S.CategoryId " +
                "GROUP BY P.CategoryId, P.CategoryName";

            _dashboardApiClient
                .Setup(x => x.ExecuteCustomScriptQuery(_sut.ControllerContext.HttpContext, 
                    It.IsAny<CancellationToken>(),
                    categoriesWithProductCountSql))
                .Returns(Task.FromResult(apiClientResponse));

            var response = _sut.CategoriesWithProductCount(It.IsAny<CancellationToken>());

            var result = response.Result as JsonResult;

            var categories = result.Value as List<Category>;


            Assert.That(response, Is.Not.Null);
            Assert.That(result, Is.Not.Null);
            Assert.That(categories.Count, Is.EqualTo(expectedCategoryList.Count));
            Assert.That(categories[0].CategoryId, Is.EqualTo(expectedCategoryList[0].CategoryId));
            Assert.That(categories[1].CategoryName, Is.EqualTo(expectedCategoryList[1].CategoryName));
            Assert.That(categories[2].ProductCount, Is.EqualTo(expectedCategoryList[2].ProductCount));
            Assert.That(categories[0].ProductCount, Is.EqualTo(expectedCategoryList[0].ProductCount));
        }
    }
}