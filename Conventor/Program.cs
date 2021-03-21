using Conventor.Reader;
using System;

namespace Conventor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            ReadJPEG test = new ReadJPEG();
            test.Read(@"C:\Users\user\Desktop\Комп графика\Testing image.jpg");
        }
    }
}
