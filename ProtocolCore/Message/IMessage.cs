﻿using ProtocolCore.Payloads.Core;

namespace ProtocolCore.Message;

public interface IMessage
{
    MessageType Type { get; }
    int Id { get; }
    string Event { get; }
    int PayloadCount { get; }
    
    T GetValue<T>(int index) where T : IReversable;
    string? GetPayloadType(int index);
}