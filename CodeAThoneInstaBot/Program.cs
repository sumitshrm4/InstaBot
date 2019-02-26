using System;
using System.Threading.Tasks;

namespace CodeAThoneInstaBot
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Starting instabot ....");
            var result = Task.Run(InstaBot.MainAsync).GetAwaiter().GetResult();
            if (result)
                return;
            Console.ReadKey();
        }

    }

}
