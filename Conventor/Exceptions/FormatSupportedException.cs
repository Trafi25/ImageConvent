using System;
using System.Collections.Generic;
using System.Text;

namespace Conventor.Exeptions
{
    class FormatSupportedException:Exception
    {
        public string ErrorDetails; 
        FormatSupportedException(string message,string format):base(message)
        {
            ErrorDetails = $"{message} : {format}";
        }
    }
}
