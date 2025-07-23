using System.Text;

namespace PowCapServer.Test;

public class UnitTest1
{
    [Fact]
    public void ToHexStringTest()
    {
#pragma warning disable CA1872 // 首选“Convert.ToHexString”和“Convert.ToHexStringLower”而不是基于“BitConverter.ToString”的调用链
#pragma warning disable CA1307 // 为了清晰起见，请指定 StringComparison
        var bytes = Encoding.UTF8.GetBytes("Hello");
        var a = BitConverter.ToString(bytes).Replace("-", "").ToUpperInvariant();
        var b = Convert.ToHexString(bytes).ToUpperInvariant();
        Assert.Equal(a, b);
#pragma warning restore CA1307 // 为了清晰起见，请指定 StringComparison
#pragma warning restore CA1872 // 首选“Convert.ToHexString”和“Convert.ToHexStringLower”而不是基于“BitConverter.ToString”的调用链

    }
}
