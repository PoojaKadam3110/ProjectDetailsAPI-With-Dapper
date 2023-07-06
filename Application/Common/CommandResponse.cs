using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common
{
    public class CommandResponse : Response
    {
        public dynamic Data { get; set; }
        public CommandResponse() { }
        public CommandResponse(string Message, bool status = false)
        {
            IsSuccessful = status;
            Errors.Add(Message);
        }

        public CommandResponse(List<string> Messages, bool status = false)
        {
            IsSuccessful = status;
            Errors.AddRange(Messages);
        }
    }
}
