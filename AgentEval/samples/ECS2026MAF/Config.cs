// Originally from joslat/AgentEval at samples/ECS2026MAF/Config.cs.
// Modified for AgentCamp Brisbane 2026: read AzureOpenAI:* from
// dotnet user-secrets first, fall back to AZURE_OPENAI_* env vars.
// Special thanks to Jose Luis Latorre.
//
// SPDX-License-Identifier: MIT
// Copyright (c) 2026 ECS2026 Demo

using Azure;
using Microsoft.Extensions.Configuration;

namespace ECS2026MAF;

/// <summary>
/// Azure OpenAI configuration.
/// </summary>
/// <remarks>
/// Resolved in order:
/// <list type="number">
///   <item>.NET user-secrets section <c>AzureOpenAI</c> (keys: <c>Endpoint</c>, <c>ApiKey</c>, <c>Deployment</c>).</item>
///   <item>Environment variables <c>AZURE_OPENAI_ENDPOINT</c>, <c>AZURE_OPENAI_API_KEY</c>, <c>AZURE_OPENAI_DEPLOYMENT</c> (CI fallback).</item>
/// </list>
/// User-secrets are shared with the eval project via the matching
/// <c>UserSecretsId</c> in both csproj files.
/// </remarks>
public static class Config
{
    private static readonly Lazy<IConfigurationRoot> _configuration = new(() =>
        new ConfigurationBuilder()
            .AddUserSecrets(typeof(Config).Assembly, optional: true)
            .AddEnvironmentVariables()
            .Build());

    private static string? Read(string secretKey, string envVar) =>
        NullIfBlank(_configuration.Value[secretKey])
        ?? NullIfBlank(Environment.GetEnvironmentVariable(envVar));

    private static string? NullIfBlank(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;

    private static string? EndpointValue   => Read("AzureOpenAI:Endpoint",   "AZURE_OPENAI_ENDPOINT");
    private static string? ApiKeyValue     => Read("AzureOpenAI:ApiKey",     "AZURE_OPENAI_API_KEY");
    private static string? DeploymentValue => Read("AzureOpenAI:Deployment", "AZURE_OPENAI_DEPLOYMENT");

    /// <summary>True when endpoint, key, and deployment are all present.</summary>
    public static bool IsConfigured =>
        EndpointValue is not null && ApiKeyValue is not null && DeploymentValue is not null;

    /// <summary>Azure OpenAI endpoint URI.</summary>
    public static Uri Endpoint => new(EndpointValue
        ?? throw new InvalidOperationException(
            "AzureOpenAI:Endpoint is not set. Run `azd up` or `dotnet user-secrets set AzureOpenAI:Endpoint <value>`."));

    /// <summary>Azure OpenAI API key credential.</summary>
    public static AzureKeyCredential KeyCredential => new(ApiKeyValue
        ?? throw new InvalidOperationException(
            "AzureOpenAI:ApiKey is not set. Run `azd up` or `dotnet user-secrets set AzureOpenAI:ApiKey <value>`."));

    /// <summary>Primary model deployment name.</summary>
    public static string Model => DeploymentValue
        ?? throw new InvalidOperationException(
            "AzureOpenAI:Deployment is not set. Run `azd up` or `dotnet user-secrets set AzureOpenAI:Deployment <value>`.");
}
