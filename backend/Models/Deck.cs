namespace Lab2dotnet3.Models;

public partial class Deck
{
    public int DeckId { get; set; }

    public string Name { get; set; } = null!;

    // Add the missing Description property
    public string? Description { get; set; }

    public virtual ICollection<Card> Cards { get; set; } = new List<Card>();

    // Add the missing Id property
    public int Id => DeckId;
}
