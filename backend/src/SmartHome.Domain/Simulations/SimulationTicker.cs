using System.Timers;

namespace SmartHome.Domain.Simulations;

/// <summary>
/// Drives the simulation loop by firing a periodic tick event at a rate determined by the current speed multiplier.
/// </summary>
public class SimulationTicker
{
    private System.Timers.Timer _timer;
    private SimulationSpeed _defaultSpeed = SimulationSpeed.OneX;

    /// <summary>Base tick interval at 1× speed (5 000 ms = ambient temperature changes 1 °F every 5 seconds).</summary>
    private const int baseTimerInterval = 5000;

    /// <summary>Fired on every tick; subscribed to by SimulationService to advance simulation state.</summary>
    public event Action? OnTick;

    /// <summary>Initializes the ticker at 1× speed.</summary>
    public SimulationTicker()
    {
        _timer = new System.Timers.Timer(baseTimerInterval);
        _timer.Elapsed += OnTimerElapsed;
        _timer.AutoReset = true;
        SetSimulationTickerSpeed(_defaultSpeed);
    }

    /// <summary>Invokes the OnTick event on each timer elapsed event.</summary>
    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        OnTick?.Invoke();
    }

    /// <summary>Updates the tick interval to match the new speed multiplier.</summary>
    public void SetSimulationTickerSpeed(SimulationSpeed speed)
    {
        _defaultSpeed = speed;
        double newInterval = baseTimerInterval / (int)speed;
        _timer.Interval = newInterval;
    }

    /// <summary>Starts the simulation timer.</summary>
    public void Start()
    {
        _timer.Start();
    }

    /// <summary>Stops the simulation timer.</summary>
    public void Stop()
    {
        _timer.Stop();
    }
}
