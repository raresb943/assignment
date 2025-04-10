using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Movie.Core.Interfaces;
using Movie.Infrastructure.Services;
using System.Net;
using Xunit;

namespace Movie.Tests.MovieServiceTests
{
    public partial class MovieServiceTests
    {
        protected readonly Mock<IConfiguration> _configurationMock;
        protected readonly Mock<ICommentRepository> _commentRepositoryMock;
        protected readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        protected readonly HttpClient _httpClient;
        protected readonly MovieService _movieService;
        protected readonly string _testApiKey = "test_api_key";

        protected MovieServiceTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _commentRepositoryMock = new Mock<ICommentRepository>();
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

            _configurationMock.Setup(x => x["MovieDbApi:ApiKey"]).Returns(_testApiKey);

            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://api.themoviedb.org/")
            };

            _movieService = new MovieService(_httpClient, _configurationMock.Object, _commentRepositoryMock.Object);
        }

        protected void SetupHttpMessageHandlerMock(string url, string responseContent, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains(url)),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(responseContent)
                });
        }
    }
}