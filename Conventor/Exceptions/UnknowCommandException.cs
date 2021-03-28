using System;
using System.Collections.Generic;
using System.Text;

namespace Conventor.Exceptions
{
    class UnknowCommandException:Exception
    {
        public string ErrorDetails;
        public UnknowCommandException(string message,string value):base(message)
        {
            ErrorDetails = $"{message} : {value}";
        }
    }
}
