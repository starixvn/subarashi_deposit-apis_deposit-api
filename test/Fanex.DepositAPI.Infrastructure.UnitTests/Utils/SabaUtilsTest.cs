namespace Fanex.DepositAPI.Infrastructure.UnitTests.Utils;

using Constants;
using FluentAssertions;
using Infrastructure.Utils;

public class SabaUtilsTest
{
    [Theory]
    [InlineData(SpecialPlatform.Jollysports, true)]
    [InlineData(SpecialPlatform.JollysportCV, true)]
    [InlineData(0, false)]
    [InlineData(1, false)]
    public void IsHideSaba_Should_ReturnExpectedResult(int specialPlatform, bool expectedResult)
    {
        // Act
        var result = SabaUtils.IsHideSaba(specialPlatform);

        // Assert
        result.Should().Be(expectedResult);
    }
}