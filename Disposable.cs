using System;
using System.Diagnostics;

namespace CVV
{
    public abstract class Disposable : IDisposable
    {
        private bool m_Disposed = false;
        protected object NoDisposeWhileLocked = new object();
        public event EventHandler OnDispose;

        protected bool Disposed
        {
            get
            {
                lock (NoDisposeWhileLocked)
                {
                    return m_Disposed;
                }
            }
        }

        protected bool AssertSafe()
        {
            if (Disposed == true)
            {
                throw new ObjectDisposedException($"Object of type {GetType().Name} with hash code {GetHashCode()} has been disposed");
            }

            return true;
        }

        protected virtual void CleanUpResources() { }

        public void Dispose()
        {
            lock (NoDisposeWhileLocked)
            {
                if (m_Disposed == false)
                {
                    try
                    {
                        OnDispose?.Invoke(this, EventArgs.Empty); ;
                    }
                    finally
                    {
                        CleanUpResources();
                        m_Disposed = true;
                        GC.SuppressFinalize(this);
                    }
                }
            }
        }

        ~Disposable()
        {
            Debug.WriteLineIf(Debugger.IsAttached, $"Object of type {GetType().Name} with hash code {GetHashCode()} was not disposed of and was garbage collected");
            CleanUpResources();
        }
    }
}
