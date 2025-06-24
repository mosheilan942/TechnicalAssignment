using HelloWorldApp;
using Xunit;

namespace HelloWorldApp.Tests
{
    public class GreeterTests
    {
        [Fact]
        public void GetMessage_ReturnsHelloWorld()
        {
            var greeter = new Message();
            var str = greeter.GetMessage();

            Assert.Equal("Hello World!", str);
        }
    }
}