namespace Quoridor.Helper;

public static class ColorHelper
{
    public static string GetHex(Color color)
    {
        return color switch
        {
            Color.Red => "#FF0000",
            Color.Blue => "#0000FF",
            _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
        };
    }

    public const Color Player1Color = Color.Blue;
    public const Color Player2Color = Color.Red;
}

public enum Color
{
    Red,
    Blue
}