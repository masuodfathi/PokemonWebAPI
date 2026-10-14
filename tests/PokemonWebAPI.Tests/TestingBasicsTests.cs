using Xunit;

namespace PokemonWebAPI.Tests;

public class TestingBasicsTests
{
    [Fact]
    public void AdditionOfTwoAndThreeIsFive()
    {
        //Arrange: prepare the inputs
        var firstNumber = 2;
        var secondNumber = 3;

        //Act: perform the operation.
        var result = firstNumber + secondNumber;

        //Assert: verify the expected outcome.
        Assert.Equal(5,result);

    }
}