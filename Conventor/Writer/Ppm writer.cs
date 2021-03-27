using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Conventor.Interfaces;
using Conventor.ImageConcept;

namespace Conventor.Writer
{
    class Ppm_writer : IImageWriter
    {

        private ASCIIEncoding ASCIi = new ASCIIEncoding();

        public void Write(string path, Image image)
        {
            throw new NotImplementedException();
        }
    }
}
