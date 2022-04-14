using System;

namespace HanbiroExtensionConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("StartUp");
            Application application = new Application();
            application.Start();
            Console.ReadLine();
        }
    }
}
