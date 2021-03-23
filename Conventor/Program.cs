using Conventor.Reader;
using System;
using Conventor.ImageConcept;

namespace Conventor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            ReadJPEG test = new ReadJPEG();
            //Image img = new Image();
            test.Read(@"C:\Users\user\Desktop\Комп графика\Testing image.jpg");

            Conventor convent = new Conventor();
            //convent.Convert(img, @"C:\Users\user\Desktop\Комп графика\Testing image.jpg", @"D:\1.bmp");
        }
    }
}
