using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qahr.Mobile.Role.Instance
{
    public struct RelicAttribute
    {

        public Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute Type 
        { 
        
            get
            {
                return (Qahr.Mobile.Game.MsgServer.MsgChiInfo.ChiAttribute)(Data % 100);
            }
            set
            {
                uint x = 0;
                x = Value * 1000;
                x += (uint)((Epic ? (uint)1 : (uint)0)) * (uint)100;
                x += (byte)value;
                Data = x;
            }
        
        }
        public uint Value
        {
            get
            {
                return Data / 1000;
            }
            set
            {
                uint x = 0;
                x = value * 1000;
                x += (uint)((Epic ? (uint)1 : (uint)0)) * (uint)100;
                x += (byte)Type;
                Data = x;
            }
        }
        public bool Epic
        {
            get
            {
                return (((Data % 1000) - (byte)Type) == 100);
            }
            set
            {
                uint x = 0;
                x = Value * 1000;
                x += (uint)((value ? (uint)1 : (uint)0)) * (uint)100;
                x += (byte)Type;
                Data = x;
            }
        }
        public uint Data;
        public RelicAttribute(uint value)
        {
            Data = value;
        }
        public static implicit operator UInt32(RelicAttribute attribute)
        {
            return attribute.Data;
        }
       
    }
}