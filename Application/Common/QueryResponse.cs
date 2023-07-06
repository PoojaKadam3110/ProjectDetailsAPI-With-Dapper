using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common
{
    public class QueryResponse : Response
    {
        public dynamic Data { get; set; }

        public QueryResponse() { }
        public QueryResponse(string Message, bool status = false)
        {
            IsSuccessful = status;
            Errors.Add(Message);
        }

        public QueryResponse(List<string> Messages, bool status = false)
        {
            IsSuccessful = status;
            Errors.AddRange(Messages);
        }
    }
}
