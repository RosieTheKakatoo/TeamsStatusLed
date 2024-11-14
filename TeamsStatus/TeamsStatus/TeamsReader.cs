using System.Windows.Automation;

namespace TeamsStatus
{

    public class TeamsReader
    {
        public bool isChecking { get; private set; }
        private AutomationElement storedTeamsWindow = null;

        public async Task<string> GetTeamsPresence(CancellationToken token)
        {
            isChecking = true;
            string presenceStatus = "Unknown";

            try
            {
                var rootElement = await Task.Run(() => AutomationElement.RootElement, token);

                // Check if we already have a valid storedTeamsWindow
                if (storedTeamsWindow != null)
                {
                    try
                    {
                        // Try to access a property to check if it's still valid
                        var cachedWindowName = storedTeamsWindow.Current.Name;
                    }
                    catch
                    {
                        // If accessing the property fails, the stored window is no longer valid
                        storedTeamsWindow = null;
                    }
                }

                if (storedTeamsWindow == null)
                {
                    AutomationElement teamsWindow = null;

                    // Find all windows
                    var windows = await Task.Run(() =>
                    {
                        var windowCondition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window);
                        return rootElement.FindAll(TreeScope.Children, windowCondition);
                    }, token); 

                    // Iterate through all of the found Windows to find the one for MS Teams.
                    // Teams does NOT need to be the active window. It CAN be minimized to the system tray and it will still be found.
                    foreach (AutomationElement window in windows)
                    {
                        if (window.Current.Name.Contains("Microsoft Teams"))
                        {
                            // Store the window that belongs to Teams as teamsWindow
                            teamsWindow = window;
                            // Store the found Teams window AutomationElement
                            storedTeamsWindow = teamsWindow;
                            break;
                        }
                    }

                    if (teamsWindow == null)
                    {
                        isChecking = false;

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Microsoft Teams not found.");

                        return presenceStatus; // Return early if no Teams window is found
                    }
                }

                // Look for the presence status element within the Teams window
                var presenceElements = await Task.Run(() =>
                {
                    var presenceCondition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button);
                    return storedTeamsWindow.FindAll(TreeScope.Descendants, presenceCondition);
                }, token);

                foreach (AutomationElement element in presenceElements)
                {
                    // On my system, with the "new" Teams UI installed, I had to look for the string:
                    // "Your profile picture with status displayed as"
                    // and then look at the next word, which is my current status.
                    if (!string.IsNullOrEmpty(element.Current.Name) && element.Current.Name.Contains("Your profile")) //'Your profile, status In a meeting'
                    {
                        // Let's grab the status by looking at everything after "displayed as ", removing the trailing ".",
                        // and setting it to lowercase. I set it to lowercase because that is how I have my ESP32-C3 
                        // set up to read the data that this C# app sends to it.
                        int statusStart = element.Current.Name.IndexOf("status ") + "status ".Length;
                        presenceStatus = element.Current.Name.Substring(statusStart).Trim().Trim('"').Replace(".", "").ToLower();
                        break;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Operation was cancelled
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
            finally
            {
                isChecking = false;
            }

            return presenceStatus;
        }
    }
}
