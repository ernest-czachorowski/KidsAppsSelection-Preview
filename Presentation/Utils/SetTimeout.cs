namespace Presentation.Utils;

public class SetTimeout : IDisposable
{
    protected System.Timers.Timer timer;
    protected int waitTime = 1000;
    protected EventCallback eventCallback;
    protected System.Timers.ElapsedEventHandler timedEventCallback;

    public SetTimeout(EventCallback callback, int time = 1000)
    {
        eventCallback = callback;
        waitTime = time;

        timer = new System.Timers.Timer(this.waitTime);
        timer.Enabled = false;
        timer.AutoReset = false;

        timedEventCallback = async (s, e) =>
        {
            await eventCallback.InvokeAsync();
            timer.Enabled = false;
            timer.Stop();
        };

        timer.Elapsed += timedEventCallback;
    }

    public void Start()
    {
        timer.Enabled = true;
        timer.Start();
    }

    public void Stop()
    {
        timer.Enabled = false;
        timer.Stop();
    }

    public void Reset()
    {
        if (timer.Enabled)
        {
            timer.Stop();
            timer.Start();
        }
        else
        {
            timer.Enabled = true;
            timer.Start();
        }
    }

    public void Dispose()
    {
        timer.Enabled = false;
        timer.Stop();
        timer.Elapsed -= timedEventCallback;
        timer.Dispose();
    }
}