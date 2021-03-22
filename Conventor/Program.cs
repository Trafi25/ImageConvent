using Conventor.Reader;
using System;

namespace Conventor
{
    class Program
    {
        static void Main(string[] args)
        {
           // Console.WriteLine("Hello World!");
           // ReadJPEG test = new ReadJPEG();
            //test.Read(@"C:\Users\user\Desktop\Комп графика\Testing image.jpg");

            try
            {
                if (args.Length == 0)
                {
                    Console.WriteLine("Please enter a numeric argument.");
                    Console.WriteLine("Usage: Factorial <num>");
                   
                }

            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }
        }
    }
}
