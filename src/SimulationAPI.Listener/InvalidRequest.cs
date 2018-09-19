using System;
using System.Collections.Generic;
using System.Text;

namespace FakeApi.Listener
{
    public class InvalidRequest
    {
        public Guid Reference { get; set; }
        public DateTime RequestDate { get; set; }

        public InvalidRequest()
        {
            Reference = Guid.NewGuid();
            RequestDate = DateTime.Now;
        }
    }
}
