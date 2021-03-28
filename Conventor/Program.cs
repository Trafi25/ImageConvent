using Conventor.Reader;
using System;
using Conventor.ImageConcept;

namespace Conventor
{
    class Program
    {
        static void Main(string[] args)
        {
            string Destination = "";
            string Original = "";
            Conventor conventor = new Conventor();
            conventor.Convert(Destination, Original);
        }
    }
}
