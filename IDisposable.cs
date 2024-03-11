namespace CVV
{
    using System;

    public interface IDisposable
    {
        event EventHandler OnDispose;

        void Dispose();
    }
}
