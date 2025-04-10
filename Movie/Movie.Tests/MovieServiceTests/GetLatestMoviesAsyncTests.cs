using FluentAssertions;
using Movie.Core.Entities;
using System.Net;
using Xunit;

namespace Movie.Tests.MovieServiceTests
{
    public partial class MovieServiceTests
    {
        public class GetLatestMoviesAsyncTests : MovieServiceTests
        {
            [Fact]
            public async Task GetLatestMoviesAsync_WhenEmptyResults_ShouldReturnEmptyList()
            {
                // Arrange
                var jsonResponse = @"
                {
                    ""page"": 1,
                    ""results"": [],
                    ""total_pages"": 0,
                    ""total_results"": 0
                }";

                SetupHttpMessageHandlerMock("movie/now_playing", jsonResponse);

                // Act
                var result = await _movieService.GetLatestMoviesAsync();

                // Assert
                result.Should().NotBeNull();
                result.Should().BeEmpty();
            }

            [Fact]
            public async Task GetLatestMoviesAsync_ShouldUseCorrectEndpoint()
            {
                // Arrange
                var jsonResponse = @"
                {
                    ""page"": 1,
                    ""results"": [],
                    ""total_pages"": 0,
                    ""total_results"": 0
                }";

                string expectedEndpoint = $"movie/now_playing?api_key={_testApiKey}";
                SetupHttpMessageHandlerMock(expectedEndpoint, jsonResponse);

                // Act & Assert
                await _movieService.GetLatestMoviesAsync();
            }
        }
    }
}