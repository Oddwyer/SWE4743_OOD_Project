using System.Timers;
namespace SmartHome.Domain.Simulations;

public class SimulationTicker
{
    /// <summary>   
    /// TODO: Kataali: Implement periodical ticking for the simulation speed, allowing it to update device states and generate events.
    /// <summary>
    
    private System.Timers.Timer _timer;
    private SimulationSpeed _defaultSpeed = SimulationSpeed.OneX;
    private const int baseTimerInterval = 5000; // accounting for "temperature changes by 1 F every 5s at 1x speed" condition
    // the actual interval will be baseTimerInterval divided by the speed multiplier (e.g., 2x speed means timer ticks every 2.5s)

    public event Action OnTick; // Action to execute on each tick, set by SimulationService to update simulation state
    public SimulationTicker()
    {
        _timer = new System.Timers.Timer(baseTimerInterval);
        _timer.Elapsed += OnTimerElapsed;
        _timer.AutoReset = true;
    }

    private void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
        OnTick?.Invoke();
    }

    public void setSimulationTickerSpeed(SimulationSpeed speed)
    {
        _defaultSpeed = speed;
        double newInterval = baseTimerInterval / (int)speed;
        _timer.Interval = newInterval;
    }

    public void Start()
    {
        _timer.Start();
    }

    public void Stop()
    {
        _timer.Stop();
    }
}