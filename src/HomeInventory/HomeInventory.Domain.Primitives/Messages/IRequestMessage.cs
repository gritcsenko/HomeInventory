﻿namespace HomeInventory.Domain.Primitives.Messages;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2326:Unused type parameters should be removed", Justification = "Used to specify matching type of the response in the functions")]
public interface IRequestMessage<out TResult> : IMessage
{
}
