using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Movie.Core.Interfaces;
using Movie.Infrastructure.Services;
using Xunit;

namespace Movie.Tests.JwtTokenServiceTests
{
    public partial class JwtTokenServiceTests
    {
        protected readonly Mock<IConfiguration> _configurationMock;
        protected readonly Mock<IConfigurationSection> _jwtSettingsMock;
        protected readonly JwtTokenService _jwtTokenService;

        protected JwtTokenServiceTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _jwtSettingsMock = new Mock<IConfigurationSection>();

            _configurationMock.Setup(x => x["JwtSettings:Key"]).Returns("TestSecretKeyWithMinimum32CharactersForHmacSha256");
            _configurationMock.Setup(x => x["JwtSettings:Issuer"]).Returns("TestIssuer");
            _configurationMock.Setup(x => x["JwtSettings:Audience"]).Returns("TestAudience");

            _jwtTokenService = new JwtTokenService(_configurationMock.Object);
        }
    }
}