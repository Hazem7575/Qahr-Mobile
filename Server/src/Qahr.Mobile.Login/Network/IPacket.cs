// * Created by ElmistRo
// * Copyright © 2010-2014
// * ElmistRo - Project

namespace Qahr.Mobile.Login.Interfaces
{
    public unsafe interface IPacket
    {
        byte[] ToArray();
        void Deserialize(byte[] buffer);
    }
}