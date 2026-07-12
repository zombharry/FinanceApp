using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Configuration;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Key { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public bool ValidateLifetime { get; init; } = true;
}
