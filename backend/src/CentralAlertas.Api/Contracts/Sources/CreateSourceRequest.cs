namespace CentralAlertas.Api.Contracts.Sources;

public class CreateSourceRequest
{
    public string? Name { get; set; }

    public int ExpectedIntervalMinutes { get; set; }
}