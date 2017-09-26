using System;

namespace Aleeda.Net.Messages
{
    public interface ISerializableObject
    {
        void Serialize(ServerMessage message);
    }
}
