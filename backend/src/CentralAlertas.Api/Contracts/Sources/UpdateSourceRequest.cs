namespace CentralAlertas.Api.Contracts.Sources;

public class UpdateSourceRequest
{
    public string? Name { get; set; }

    public int ExpectedIntervalMinutes { get; set; }
}