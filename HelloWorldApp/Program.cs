namespace HelloWorldApp
{
    public class Message
    {
        public string GetMessage() => "Hello World!";
    }

    class Program
    {
        static void Main()
        {
            var message = new Message();
            Console.WriteLine(message.GetMessage());
        }
            
    }
}