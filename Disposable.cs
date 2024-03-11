namespace CVV
{
    using System;
    using System.Diagnostics;

    public abstract class Disposable : System.IDisposable, IDisposable
    {
        private bool _disposed;

        ~Disposable()
        {
            this.Dispose(false);
        }

        public event EventHandler OnDispose;

        protected object NoDisposeWhileLocked { get; } = new object();

        protected bool Disposed
        {
            get
            {
                lock (this.NoDisposeWhileLocked)
                {
                    return this._disposed;
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        // ReSharper disable once FlagArgument
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (this.NoDisposeWhileLocked)
                {
                    if (this._disposed)
                    {
                        return;
                    }

                    try
                    {
                        this.OnDispose?.Invoke(this, EventArgs.Empty);
                    }
                    finally
                    {
                        this.CleanUpResources();
                        this._disposed = true;
                    }
                }
            }
            else
            {
                Debug.WriteLineIf(
                    Debugger.IsAttached,
                    $"Object of type {this.GetType().Name} with hash code {this.GetHashCode()} was not disposed of and was garbage collected");
                this.CleanUpResources();
            }
        }

        protected bool AssertSafe() => this.Disposed
            ? throw new ObjectDisposedException($"Object of type {this.GetType().Name} with hash code {this.GetHashCode()} has been disposed")
            : true;

        protected virtual void CleanUpResources()
        {
            // Intentionally empty.
        }
    }
}
