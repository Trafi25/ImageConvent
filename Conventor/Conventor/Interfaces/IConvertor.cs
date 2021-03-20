using System;
using System.Collections.Generic;
using System.Text;
using Conventor.ImageConcept;


namespace Conventor.Interfaces
{
    public interface IConvertor
    {
        public void Convert(Image image, string destinationPath);
    }
}
