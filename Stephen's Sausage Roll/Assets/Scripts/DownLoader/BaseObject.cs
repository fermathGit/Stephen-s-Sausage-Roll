using System;

public class BaseObject : System.IDisposable
{
    bool _disposed = false;

    ~BaseObject() {
        Dispose(false);
    }

    private void Dispose(bool dispoing)
    {
        if (_disposed) {
            return;
        }

        if (dispoing) {
            DisposeMgred();
        }

        DisposeUnmgred();
        _disposed = true;
    }

    protected virtual void DisposeUnmgred()
    {
        
    }

    protected virtual void DisposeMgred()
    {
        
    }

    public void Dispose()
    {
        
    }
}

