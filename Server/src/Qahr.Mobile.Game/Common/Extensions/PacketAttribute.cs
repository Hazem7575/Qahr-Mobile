using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile
{
    public class PacketAttribute : Attribute
    {
        public static readonly Func<PacketAttribute,ushort> Translator = (attr) => attr.Type;
        public ushort Type { get; private set; }

        public PacketAttribute(ushort type)
        {
            this.Type = type;
        }
    }
}
