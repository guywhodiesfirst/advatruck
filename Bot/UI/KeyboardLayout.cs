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
        [new KeyboardButton(BotButtons.StartTrip)],
        [new KeyboardButton(BotButtons.EndTrip)],
        [new KeyboardButton(BotButtons.Logout)],
    ])
    {
        ResizeKeyboard = true,
    };
}