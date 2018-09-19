using System;

namespace FakeApi.Listener
{
    public class Token
    {
        public bool Valid { get; set; }
        public Guid Value { get; set; }
        public string Stamp { get; set; }


        public Token(bool valid)
        {
            Valid = valid;
            Value = Valid ? Guid.NewGuid() : new Guid();
            Stamp = DateTime.Now.ToString("yyyyMMdd_hhmmssfff");
        }
    }
}
