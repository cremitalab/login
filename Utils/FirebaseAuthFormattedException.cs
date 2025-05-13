using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace fase1.Utils
{
    public class FirebaseAuthFormattedException : Exception
    {
        public string FirebaseCode { get; }
        public string RawJson { get; }

        public FirebaseAuthFormattedException(string code, string rawJson)
            : base(code)
        {
            FirebaseCode = code;
            RawJson = rawJson;
        }
    }
}
