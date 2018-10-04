namespace Miracle.Arguments.Test
{
    public static class ArgumentExtension
    {
        public static string[] ToArgs(this string s)
        {
            return s.Split(' ');
        }
    }
}