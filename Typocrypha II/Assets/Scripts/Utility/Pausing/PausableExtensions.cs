public static class PausableExtensions
{
    public static bool IsPaused(this IPausable self) => self.PH.Pause;
    public static void Pause(this IPausable self) => self.PH.Pause = true;
    public static void Unpause(this IPausable self) => self.PH.Pause = false;
}