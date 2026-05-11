namespace Bot.Interfaces;

using Telegram.Bot.Types;

/// <summary>
/// Service for handling Telegram callback queries from inline keyboards.
/// </summary>
public interface ICallbackQueryService
{
    /// <summary>
    /// Processes a callback query.
    /// </summary>
    /// <param name="callbackQuery">The callback query from Telegram.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task HandleAsync(CallbackQuery callbackQuery, CancellationToken ct);
}