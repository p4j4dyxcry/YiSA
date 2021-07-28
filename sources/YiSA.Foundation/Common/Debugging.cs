namespace YiSA.Foundation.Common
{
    public class Debugging
    {
#if DEBUG
        public static bool True = true;
#else
        public static bool True = false;
#endif
        public static bool False => !True;
    }
}