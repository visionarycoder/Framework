namespace VisionaryCoder.Framework.Extensions.CLI;

public static class MenuHelper
{
    public static void ShowIntroduction(string appName, int separateWidth = 72)
    {
        ShowSeparator(separateWidth);
        Console.WriteLine($"--");
        Console.WriteLine($"-- {appName}");
    }
        public static void ShowExit(int separateWidth = 72)
        {
            ShowSeparator(separateWidth);
            Console.WriteLine("Hit [ENTER] to exit.");
            Console.ReadLine();
        }
    public static void ShowSeparator(int width = 72)
    {
        Console.WriteLine("".PadRight(width, '-'));
    }
}
