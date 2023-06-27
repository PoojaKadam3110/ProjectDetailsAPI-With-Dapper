using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ErrorHandling
{
    public class ErrorResponse
    {
        public string Message { get; set; }
        public int ErrorCode { get; set; }
        public string StackTrace { get; set; }
    }
}
