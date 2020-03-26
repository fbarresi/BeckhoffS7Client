using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;

namespace TFU002.Interfaces.Extensions
{
    public static class ObservableExtensions
	{
		public static IObservable<T> LogAndRetry<T>(this IObservable<T> source, ILogger logger, string message)
		{
			return source
				.Do(
					_ => { },
					ex => logger?.LogError(ex, message))
				.Retry();
		}

		public static IObservable<T> RetryAfterDelay<T>(
			this IObservable<T> source,
			TimeSpan retryDelay)
		{
			return RedoAfterDelay(source, retryDelay, -1, null, Observable.Retry, Observable.Retry);
		}
		
		public static IObservable<T> RetryAfterDelay<T>(
			this IObservable<T> source,
			TimeSpan retryDelay,
			int retryCount,
			IScheduler scheduler)
		{
			return RedoAfterDelay(source, retryDelay, retryCount, scheduler, Observable.Retry, Observable.Retry);
		}

		public static IObservable<T> RepeatAfterDelay<T>(
			this IObservable<T> source,
			TimeSpan retryDelay)
		{
			return RedoAfterDelay(source, retryDelay, -1, null, Observable.Repeat, Observable.Repeat);
		}
		public static IObservable<T> RepeatAfterDelay<T>(
			this IObservable<T> source,
			TimeSpan retryDelay,
			int repeatCount,
			IScheduler scheduler)
		{
			return RedoAfterDelay(source, retryDelay, repeatCount, scheduler, Observable.Repeat, Observable.Repeat);
		}

		public static IObservable<T> LogAndRetryAfterDelay<T>(
			this IObservable<T> source,
			ILogger logger,
			TimeSpan retryDelay,
			string message)
		{
			return LogAndRetryAfterDelay(source, logger, retryDelay, message, -1, null);
		}
		public static IObservable<T> LogAndRetryAfterDelay<T>(
			this IObservable<T> source,
			ILogger logger,
			TimeSpan retryDelay,
			string message,
			int retryCount,
			IScheduler scheduler)
		{
			var sourceLogged =
				source
					.Do(
						_ => { },
						ex => logger?.LogError(ex, message));

			return RetryAfterDelay(sourceLogged, retryDelay, retryCount, scheduler);
		}

		private static IObservable<T> RedoAfterDelay<T>(IObservable<T> source, TimeSpan retryDelay, int retryCount, IScheduler scheduler, Func<IObservable<T>, IObservable<T>> reDo,
			Func<IObservable<T>, int, IObservable<T>> reDoCount)
		{
			scheduler = scheduler ?? TaskPoolScheduler.Default;
			var attempt = 0;

			var deferedObs = Observable.Defer(() => ((++attempt == 1) ? source : source.DelaySubscription(retryDelay, scheduler)));
			return retryCount > 0 ? reDoCount(deferedObs, retryCount) : reDo(deferedObs);
		}

		public static IObservable<T> DisposeMany<T>(this IObservable<T> source)
		{
			return Observable.Create<T>(obs =>
			{
				var serialDisposable = new SerialDisposable();
				var subscription =
					source.Subscribe(
						item =>
						{
							serialDisposable.Disposable = item as IDisposable;
							obs.OnNext(item);
						},
						obs.OnError,
						obs.OnCompleted);
				return new CompositeDisposable(serialDisposable, subscription);
			});
		}
	}
}
