using Telegram.Bot.Types.ReplyMarkups;

namespace MonopolyBot
{
    internal static class KeyboardMarkups
    {
        internal static readonly ReplyKeyboardMarkup loginKeyboardMarkup = new ReplyKeyboardMarkup
                    (
                    new[]
                        {
                        new KeyboardButton [] {"Register", "Login", "Delete Account"},
                        new KeyboardButton [] { "Profile", "Rooms Menu" }
                        }
                    )
        {
            ResizeKeyboard = true
        };
        internal static readonly ReplyKeyboardMarkup roomsKeyboardMarkup = new ReplyKeyboardMarkup
                    (
                    new[]
                        {
                        new KeyboardButton [] {"Create Room", "View Rooms"},
                        new KeyboardButton [] { "Profile", "Accounts menu" }
                        }
                    )
        {
            ResizeKeyboard = true
        };
        internal static readonly ReplyKeyboardMarkup gameKeyboardMarkup = new ReplyKeyboardMarkup
                    (
                    new[]
                        {
                       new KeyboardButton [] {"Game Status", "All Game Status"},
                       new KeyboardButton [] {"Roll Dice"},
                       new KeyboardButton [] {"Buy", "Pay Rent", "Pay to Leave Prison"},
                       new KeyboardButton [] {"Trade", "Accept Trade", "Cancel/Decline Trade"},
                       new KeyboardButton [] {"Level Up", "Level Down"},
                       new KeyboardButton [] {"End Action", "Leave Game"}
                        }
                    )
        {
            ResizeKeyboard = true
        };

        internal static readonly ReplyKeyboardMarkup watchGameKeyboardMarkup = new ReplyKeyboardMarkup
                            (
                            new[]
                                {
                                    new KeyboardButton [] {"Game Status", "All Game Status"},
                                    new KeyboardButton [] {"Profile", "End Watch"}
                                }
                            )
        {
            ResizeKeyboard = true
        };

        internal static readonly InlineKeyboardMarkup createRoomInlineMarkup = new
                (
                    InlineKeyboardButton.WithCallbackData("🔐 Кімната з паролем", $"CreateRoom:set"),
                    InlineKeyboardButton.WithCallbackData("🔓 Кімната без пароля", $"CreateRoom:null")
                );
    }
}
