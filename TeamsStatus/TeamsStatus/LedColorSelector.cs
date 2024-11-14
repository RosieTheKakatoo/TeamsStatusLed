using System.Drawing;

namespace TeamsStatus
{
    public static class LedColorSelector
    {
        public static Color? PickColor(string teamsStatus)
        {
            Color? ledColor = Color.FromArgb(0, 0, 10);

            switch (teamsStatus) 
            {
                case "available":
                    ledColor = Color.FromArgb(120, Color.Green); 
                    break;
                case "busy":
                case "in a call":
                case "presenting":
                case "do not disturb":
                    ledColor = Color.FromArgb(120, Color.Red);
                    break;
                case "be right back":
                case "away":
                    ledColor = Color.FromArgb(120, Color.Orange);
                    break;
                case "offline":
                    ledColor = Color.FromArgb(0, 0, 0);
                    break;
                default:
                    ledColor = null;
                    break;
            }

            return ledColor;
        }

        public static ConsoleColor ClosestConsoleColor(byte r, byte g, byte b)
        {
            ConsoleColor ret = 0;
            double rr = r, gg = g, bb = b, delta = double.MaxValue;

            foreach (ConsoleColor cc in Enum.GetValues(typeof(ConsoleColor)))
            {
                var n = Enum.GetName(typeof(ConsoleColor), cc);
                var c = Color.FromName(n == "DarkYellow" ? "Orange" : n); // bug fix
                var t = Math.Pow(c.R - rr, 2.0) + Math.Pow(c.G - gg, 2.0) + Math.Pow(c.B - bb, 2.0);
                if (t == 0.0)
                    return cc;
                if (t < delta)
                {
                    delta = t;
                    ret = cc;
                }
            }
            return ret;
        }
    }
}
