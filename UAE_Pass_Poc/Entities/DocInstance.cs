namespace UAE_Pass_Poc.Entities;

public class DocInstance : Entity
{
    public string? Name { get; set; } = null;
    public string? Value { get; set; } = null;
    public Guid DocumentId { get; set; }
    public virtual Document Document { get; set; } = null!;
}