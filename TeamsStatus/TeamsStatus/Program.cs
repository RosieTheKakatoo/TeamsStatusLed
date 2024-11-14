using System.Drawing;
using TeamsStatus;

Console.WriteLine("Start");

var previousColor = Color.FromArgb(0, 0, 10);

while (true) 
{
    using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10))) // 10-second timeout
    {
        var teamReader = new TeamsReader();
        string presenceStatus = await teamReader.GetTeamsPresence(cts.Token);

        var ledColor = LedColorSelector.PickColor(presenceStatus);

        if (ledColor == null)
        {
            ledColor = previousColor;
        }

        Console.ForegroundColor = LedColorSelector.ClosestConsoleColor(ledColor.Value.R, ledColor.Value.G, ledColor.Value.B); 
        Console.WriteLine($"{presenceStatus}");

        previousColor = ledColor.Value;

        var rgbColor = $"{ledColor.Value.A},{ledColor.Value.R},{ledColor.Value.G},{ledColor.Value.B};";

        SerialPortSender.SendString(rgbColor);

        Thread.Sleep(1000);
    }
}

