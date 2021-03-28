using System;
using System.Collections.Generic;
using System.Text;
using Conventor.ImageConcept;
using Conventor.Interfaces;
using Conventor.Reader;
using Conventor.Writer;

namespace Conventor
{
    class Conventor : IConvertor
    {
        public void Convert(string destinationPath, string originalPath)
        {

            try
            {
                ReadBMP test = new ReadBMP();
                Image img = test.Read(originalPath);
                Ppm_writer wr = new Ppm_writer();
                wr.Write(destinationPath, img);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }

        }
    }
}
