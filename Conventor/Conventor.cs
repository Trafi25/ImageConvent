using System;
using System.Collections.Generic;
using System.Text;
using Conventor.ImageConcept;
using Conventor.Interfaces;
using Conventor.Reader;

namespace Conventor
{
    class Conventor : IConvertor
    {
        public void Convert(string destinationPath, string originalPath)
        {
                     
            ReadJPEG test = new ReadJPEG();
            Image img = test.Read(originalPath);       

        }
    }
}
