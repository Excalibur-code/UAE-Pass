namespace UAE_Pass_Poc.Entities;

public class DocInstance
{
    public string? Name { get; set; } = null;
    public string? Value { get; set; } = null;
    public virtual Document Document { get; set; } = null!;
}