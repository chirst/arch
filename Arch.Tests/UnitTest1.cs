using FluentAssertions;

namespace Arch.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var a = 1;
        a.Should().Be(1);
    }
}
