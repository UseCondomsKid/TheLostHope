using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLostHopeEngine.EngineCode.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class HeaderAttribute : Attribute
    {
        public string Header { get; set; } = "";

        public HeaderAttribute(string header)
        {
            Header = header;
        }
    }
}
