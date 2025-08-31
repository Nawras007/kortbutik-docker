namespace Lab2dotnet3.Models;

public partial class Card
{
    public int CardId { get; set; }

    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string ManaCost { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; } // 💰 New property for card price

    public virtual ICollection<Deck> Decks { get; set; } = new List<Deck>();

    public int Id => CardId;
}
