using Xunit;

namespace ApiTests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var result = false;
        Assert.False(result, "sample test");
    }
}