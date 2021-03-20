using System;
using System.Collections.Generic;
using System.Text;

namespace Conventor.ImageConcept
{
    public abstract  class Image
    {
        public abstract ImageHeader Header { get; set; }
        public abstract string Path { get; set; }

       //  ImageColor 
    }
}
