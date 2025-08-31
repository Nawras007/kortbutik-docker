namespace Lab2dotnet3.Models;

public class DeckCard
{
    public int DeckId { get; set; }
    public int CardId { get; set; }

    public virtual Deck Deck { get; set; }
    public virtual Card Card { get; set; }
}
