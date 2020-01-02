using System;
using System.Linq;
using System.Reactive.Disposables;

namespace TFU002.Logic.Basics
{
    public class DisposableBase : IDisposable
    {
        protected readonly CompositeDisposable Disposables = new CompositeDisposable();

        public void Dispose()
        {
            Disposables?.Dispose();
        }
    }
}