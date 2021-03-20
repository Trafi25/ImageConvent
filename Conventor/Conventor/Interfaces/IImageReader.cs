using System;
using System.Collections.Generic;
using System.Text;
using Conventor.ImageConcept;

namespace Conventor.Interfaces
{
    interface IImageReader
    {
        public Image Read(string path);
    }
}
