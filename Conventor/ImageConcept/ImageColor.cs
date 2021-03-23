using System;
using System.Collections.Generic;
using System.Text;

namespace Conventor.ImageConcept
{
    class Pixel
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public int? A { get; set; }

        public Pixel(byte r, byte g, byte b)
        {
            this.R = r;
            this.G = g;
            this.B = b;
        }

    }
}
