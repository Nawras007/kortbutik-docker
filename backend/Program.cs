using Microsoft.EntityFrameworkCore;
using Lab2dotnet3;
using Lab2dotnet3.Models;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// --- Database connection ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Database connection string is missing!");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure();
    }));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

// --- Create DB + Seed data with retries ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var retries = 10;
    var delay = TimeSpan.FromSeconds(5);

    for (int i = 0; i < retries; i++)
    {
        try
        {
            var created = db.Database.EnsureCreated();
            Console.WriteLine(created ? "✅ Database created." : "ℹ️  Database already exists.");

            // Seed once (only if Cards table is empty)
            if (!db.Cards.Any())
            {
                db.Cards.AddRange(
                    new Card { Name = "Lightning Bolt", Type = "Instant", ManaCost = "R", Description = "Deal 3 damage to any target.", Price = 1.50m },
                    new Card { Name = "Counterspell", Type = "Instant", ManaCost = "UU", Description = "Counter target spell.", Price = 2.00m },
                    new Card { Name = "Giant Growth", Type = "Instant", ManaCost = "G", Description = "Target creature gets +3/+3.", Price = 0.25m }
                );
                db.SaveChanges();
                Console.WriteLine("✅ Seeded Cards.");
            }

            break; // success
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️  Attempt {i + 1}/{retries} failed: {ex.Message}");
            Thread.Sleep(delay);
        }
    }
}

// --- Middleware ---
app.UseExceptionHandler("/error");
app.Map("/error", (HttpContext httpContext) =>
{
    var exception = httpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
    return Results.Problem(detail: exception?.Message, statusCode: 500);
});

app.UseCors("AllowAll");

app.UseSwagger();
app.UseSwaggerUI();

// Optional: quick health check
app.MapGet("/health", () => Results.Ok("OK"));

// --- Endpoints ---

// DeckCards
app.MapGet("/deckcards", async (AppDbContext db) =>
{
    try { return Results.Ok(await db.DeckCards.ToListAsync()); }
    catch (Exception ex) { return Results.Problem(detail: ex.Message, statusCode: 500); }
});

app.MapGet("/deckcards/{deckId:int}/{cardId:int}", async (int deckId, int cardId, AppDbContext db) =>
{
    try
    {
        var deckCard = await db.DeckCards.FindAsync(deckId, cardId);
        return deckCard is not null ? Results.Ok(deckCard) : Results.NotFound();
    }
    catch (Exception ex) { return Results.Problem(detail: ex.Message, statusCode: 500); }
});

app.MapPost("/deckcards", async (DeckCard deckCard, AppDbContext db) =>
{
    try
    {
        db.DeckCards.Add(deckCard);
        await db.SaveChangesAsync();
        return Results.Created($"/deckcards/{deckCard.DeckId}/{deckCard.CardId}", deckCard);
    }
    catch (Exception ex) { return Results.Problem(detail: ex.Message, statusCode: 500); }
});

app.MapDelete("/deckcards/{deckId:int}/{cardId:int}", async (int deckId, int cardId, AppDbContext db) =>
{
    try
    {
        var deckCard = await db.DeckCards.FindAsync(deckId, cardId);
        if (deckCard is null) return Results.NotFound();
        db.DeckCards.Remove(deckCard);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    catch (Exception ex) { return Results.Problem(detail: ex.Message, statusCode: 500); }
});

// Cards
app.MapGet("/cards", async (AppDbContext db) =>
{
    try { return Results.Ok(await db.Cards.ToListAsync()); }
    catch (Exception ex) { return Results.Problem(detail: ex.Message, statusCode: 500); }
});

app.MapGet("/cards/{id:int}", async (int id, AppDbContext db) =>
{
    try
    {
        var card = await db.Cards.FindAsync(id);
        return card is not null ? Results.Ok(card) : Results.NotFound();
    }
    catch (Exception ex) { return Results.Problem(detail: ex.Message, statusCode: 500); }
});

app.MapPost("/cards", async (Card card, AppDbContext db) =>
{
    try
    {
        db.Cards.Add(card);
        await db.SaveChangesAsync();
        return Results.Created($"/cards/{card.Id}", card);
    }
    catch (Exception ex) { return Results.Problem(detail: ex.Message, statusCode: 500); }
});

app.MapPut("/cards/{id:int}", async (int id, Card updatedCard, AppDbContext db) =>
{
    try
    {
        var card = await db.Cards.FindAsync(id);
        if (card is null) return Results.NotFound();

        card.Name = updatedCard.Name;
        card.Description = updatedCard.Description;
        // keep Type, ManaCost, Price as-is unless provided in your DTO
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    catch (Exception ex) { return Results.Problem(detail: ex.Message, statusCode: 500); }
});

app.MapDelete("/cards/{id:int}", async (int id, AppDbContext db) =>
{
    try
    {
        var card = await db.Cards.FindAsync(id);
        if (card is null) return Results.NotFound();
        db.Cards.Remove(card);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    catch (Exception ex) { return Results.Problem(detail: ex.Message, statusCode: 500); }
});

// Decks
app.MapGet("/decks", async (AppDbContext db) =>
{
    try { return Results.Ok(await db.Decks.ToListAsync()); }
    catch (Exception ex) { return Results.Problem(detail: ex.Message, statusCode: 500); }
});

app.MapGet("/decks/{id:int}", async (int id, AppDbContext db) =>
{
    try
    {
        var deck = await db.Decks.FindAsync(id);
        return deck is not null ? Results.Ok(deck) : Results.NotFound();
    }
    catch (Exception ex) { return Results.Problem(detail: ex.Message, statusCode: 500); }
});

app.MapPost("/decks", async (Deck deck, AppDbContext db) =>
{
    try
    {
        db.Decks.Add(deck);
        await db.SaveChangesAsync();
        return Results.Created($"/decks/{deck.Id}", deck);
    }
    catch (Exception ex) { return Results.Problem(detail: ex.Message, statusCode: 500); }
});

app.MapPut("/decks/{id:int}", async (int id, Deck updatedDeck, AppDbContext db) =>
{
    try
    {
        var deck = await db.Decks.FindAsync(id);
        if (deck is null) return Results.NotFound();

        deck.Name = updatedDeck.Name;
        deck.Description = updatedDeck.Description;
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    catch (Exception ex) { return Results.Problem(detail: ex.Message, statusCode: 500); }
});

app.MapDelete("/decks/{id:int}", async (int id, AppDbContext db) =>
{
    try
    {
        var deck = await db.Decks.FindAsync(id);
        if (deck is null) return Results.NotFound();
        db.Decks.Remove(deck);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    catch (Exception ex) { return Results.Problem(detail: ex.Message, statusCode: 500); }
});

app.Run();
