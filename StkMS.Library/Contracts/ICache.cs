namespace StkMS.Library.Contracts
{
    public interface ICache
    {
        string? this[string? key] { get; set; }
    }
}