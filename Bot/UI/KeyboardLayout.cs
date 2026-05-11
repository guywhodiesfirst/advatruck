namespace Bot.UI;

using Telegram.Bot.Types.ReplyMarkups;

public static class KeyboardLayout
{
    public static ReplyKeyboardMarkup StartKeyboard => new(
    [
        [new KeyboardButton(BotButtons.Login)]
    ])
    {
        ResizeKeyboard = true,
    };

    public static ReplyKeyboardMarkup MainKeyboard => new(
    [
        [KeyboardButton.WithRequestLocation(BotButtons.SendLocation)],
        [new KeyboardButton(BotButtons.StartTracking)],
        [new KeyboardButton(BotButtons.Logout)],
    ])
    {
        ResizeKeyboard = true,
    };

    public static ReplyKeyboardMarkup ActiveTripKeyboard => new(
    [
        [KeyboardButton.WithRequestLocation(BotButtons.SendLocation)],
        [new KeyboardButton(BotButtons.LoadDetails)],
    ])
    {
        ResizeKeyboard = true,
    };

    public static InlineKeyboardMarkup LoadDetailsKeyboard(Guid? loadId) =>
        new(InlineKeyboardButton.WithCallbackData(BotButtons.LoadDetails, $"load_details:{loadId}"));
}