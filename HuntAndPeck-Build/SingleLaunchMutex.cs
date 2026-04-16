namespace HuntAndPeck;

public class SingleLaunchMutex : IDisposable
{
    private readonly bool _acquiredHandle;
    private Mutex? _mutex = new Mutex(true, "HuntAndPeck-5B5486F3-15E3-4DD5-BF05-03C3F716483E");

    public SingleLaunchMutex()
    {
        try
        {
            _acquiredHandle = _mutex.WaitOne(TimeSpan.Zero, true);
        }
        catch (AbandonedMutexException)
        {
            // This will happen if the mutex isn't disposed properly, e.g. during a crash
            _acquiredHandle = true;
        }
    }

    public bool AlreadyRunning => !_acquiredHandle;

    public void Dispose()
    {
        if (_mutex != null)
        {
            if (_acquiredHandle)
            {
                _mutex.ReleaseMutex();
            }

            _mutex.Dispose();
            _mutex = null;
        }
    }
}
