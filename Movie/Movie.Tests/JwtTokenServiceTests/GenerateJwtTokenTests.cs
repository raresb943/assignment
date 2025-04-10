using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Movie.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Xunit;

namespace Movie.Tests.JwtTokenServiceTests
{
    public partial class JwtTokenServiceTests
    {
        public class GenerateJwtTokenTests : JwtTokenServiceTests
        {
            [Fact]
            public void GenerateJwtToken_ShouldReturnValidToken()
            {
                // Arrange
                string userId = "user123";
                string username = "testuser";

                // Act
                var token = _jwtTokenService.GenerateJwtToken(userId, username);

                // Assert
                token.Should().NotBeNullOrEmpty();
            }

            [Fact]
            public void GenerateJwtToken_ShouldUseCorrectIssuer()
            {
                // Arrange
                string userId = "user123";
                string username = "testuser";
                string expectedIssuer = "TestIssuer";

                // Act
                var token = _jwtTokenService.GenerateJwtToken(userId, username);
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);

                // Assert
                jwtToken.Issuer.Should().Be(expectedIssuer);
            }

            [Fact]
            public void GenerateJwtToken_ShouldUseCorrectAudience()
            {
                // Arrange
                string userId = "user123";
                string username = "testuser";
                string expectedAudience = "TestAudience";

                // Act
                var token = _jwtTokenService.GenerateJwtToken(userId, username);
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);

                // Assert
                jwtToken.Audiences.Should().Contain(expectedAudience);
            }

            [Fact]
            public void GenerateJwtToken_WhenConfigurationMissing_ShouldThrowException()
            {
                // Arrange
                var configMock = new Mock<IConfiguration>();
                configMock.Setup(x => x["JwtSettings:Key"]).Returns((string?)null);
                configMock.Setup(x => x["JwtSettings:Issuer"]).Returns("TestIssuer");
                configMock.Setup(x => x["JwtSettings:Audience"]).Returns("TestAudience");

                var service = new JwtTokenService(configMock.Object);

                // Act & Assert
                service.Invoking(s => s.GenerateJwtToken("user123", "testuser"))
                    .Should().Throw<InvalidOperationException>()
                    .WithMessage("JWT Key not configured");
            }
        }
    }
}