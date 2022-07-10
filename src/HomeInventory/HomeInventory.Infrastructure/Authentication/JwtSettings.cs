﻿namespace HomeInventory.Infrastructure.Authentication;

internal class JwtSettings
{
    public string Secret { get; init; } = null!;
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
    public TimeSpan Expiry { get; init; }
}
