using Discord.Interactions;

namespace Discord;

public enum ExampleEnum
{
    First,
    Second,
    Third,
    Fourth,
    [ChoiceDisplay("Twenty First")]
    TwentyFirst
}
