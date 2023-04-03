public static class PausableExtensions
{
    public static bool IsPaused(this IPausable self) => self.PH.Pause;
}