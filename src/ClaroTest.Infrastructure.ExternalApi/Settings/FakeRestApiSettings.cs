namespace ClaroTest.Infrastructure.ExternalApi.Settings;

public class FakeRestApiSettings
{
    public const string SectionName = "FakeRestApi";

    public string BaseUrl { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
}
