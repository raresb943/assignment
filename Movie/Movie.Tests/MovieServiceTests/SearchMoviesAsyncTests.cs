using FluentAssertions;
using Movie.Core.Entities;
using System.Net;
using Xunit;

namespace Movie.Tests.MovieServiceTests
{
    public partial class MovieServiceTests
    {
        public class SearchMoviesAsyncTests : MovieServiceTests
        {

            [Fact]
            public async Task SearchMoviesAsync_WithEmptyQuery_ShouldReturnEmptyResults()
            {
                // Arrange
                string query = "";
                var jsonResponse = @"
                {
                    ""page"": 1,
                    ""results"": [],
                    ""total_pages"": 0,
                    ""total_results"": 0
                }";

                SetupHttpMessageHandlerMock("search/movie", jsonResponse);

                // Act
                var result = await _movieService.SearchMoviesAsync(query);

                // Assert
                result.Should().NotBeNull();
                result.Results.Should().BeEmpty();
                result.TotalResults.Should().Be(0);
            }
        }
    }
}