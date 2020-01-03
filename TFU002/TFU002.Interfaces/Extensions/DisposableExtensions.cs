using System;
using System.Reactive.Disposables;

namespace TFU002.Interfaces.Extensions
{
    public static class DisposableExtensions
    {
        public static void AddDisposableTo(this IDisposable disposable, CompositeDisposable compositeDisposable)
        {
            compositeDisposable.Add(disposable);
        }
    }
}