using UnityEngine;

public static class Yielders
{
    public static WaitForFixedUpdate FixedUpdate { get; } = new WaitForFixedUpdate();
    public static WaitWhile Paused(IPausable pausable, ref WaitWhile pauseYielder)
    {
        if (pauseYielder == null)
        {
            pauseYielder = new WaitWhile(pausable.IsPaused);
        }
        return pauseYielder;
    }
}
