using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace NorthOps.Services.SmsService
{
    public class HybridAsyncData<T> where T : ResponseData
    {
        private readonly Func<Task<T>> func;
        private bool loopRunning;
        private TaskCompletionSource<T> newestValue = new TaskCompletionSource<T>();
        private bool reqired;
        private Timer timer;
        private TimeSpan updateInterval = Timeout.InfiniteTimeSpan;

        public event Action<T> ValueUpdated;

        public HybridAsyncData(Func<Task<T>> func)
        {
            this.func = func ?? throw new ArgumentNullException(nameof(func));
            timer = new Timer(TimerMain);
        }

        /// <summary>
        /// 次のタイミングに取得される最新データを取得します。
        /// </summary>
        public Task<T> NewestValue => newestValue.Task;
        /// <summary>
        /// 取得済みの最後のデータを取得します。
        /// </summary>
        public T RecentValue { get; private set; }
        public TimeSpan UpdateInterval
        {
            get => updateInterval;
            set
            {
                if (updateInterval == value)
                    return;
                if (value == TimeSpan.MaxValue)
                    value = Timeout.InfiniteTimeSpan;
                else if (value < TimeSpan.Zero && value != Timeout.InfiniteTimeSpan)
                    throw new ArgumentOutOfRangeException();

                updateInterval = value;
                timer.Change(value, Timeout.InfiniteTimeSpan);
            }
        }

        /// <summary>
        /// 待機時間を無視して直ちに最新データを取得します。
        /// </summary>
        /// <returns></returns>
        public async Task<T> ForceGetNewestAsync()
        {
            var task = NewestValue;
            if (task.IsCompleted)
                return task.Result;

            timer.Change(0, Timeout.Infinite);
            return await task;
        }
        /// <summary>
        /// 最終データの取得日時が指定された日時以下の場合、直ちに最新データを取得します。
        /// </summary>
        /// <returns></returns>
        public async Task<T> ForceGetNewestAsync(DateTime dateTime)
        {
            var data = RecentValue;
            if (data != null && data.AcquisitionTime > dateTime)
                return data;
            return await ForceGetNewestAsync();
        }

        public override string ToString() => RecentValue?.ToString();

        private async void TimerMain(object state)
        {
            lock (timer)
            {
                reqired = true;
                if (loopRunning)
                    return;
                loopRunning = true;
            }

            while (true)
            {
                // メソッド実行
                try
                {
                    RecentValue = await func();
                    newestValue.SetResult(RecentValue);
                    ValueUpdated?.Invoke(RecentValue);
                    reqired = false;
                    newestValue = new TaskCompletionSource<T>();
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc);
                }
                lock (this)
                {
                    if (!reqired)
                    {
                        loopRunning = false;
                        return;
                    }
                }
            }
        }
    }
}