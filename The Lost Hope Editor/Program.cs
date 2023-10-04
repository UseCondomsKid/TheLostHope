
using System;

internal class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        using var game = new TheLostHopeEditor.Game1();
        game.Run();
    }
}