﻿namespace HomeInventory.Tests.Acceptance.Drivers;

public interface IHomeInventoryAPIDriver : IApiDriver
{
    IAuthenticationAPIDriver Authentication { get; }

    IAreaAPIDriver Area { get; }

    void SetToday(DateOnly today);
}