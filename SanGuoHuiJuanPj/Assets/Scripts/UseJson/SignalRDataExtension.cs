public static class SignalRDataExtension
{
    public const string NullString = "null";
    public static bool IsNullArg(this string arg) => string.IsNullOrWhiteSpace(arg) || NullString.Equals(arg);
}