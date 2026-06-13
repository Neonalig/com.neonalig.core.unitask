using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Neonalig.Core
{
    public static class SingletonMBTasks
    {
        public static async UniTask<T> WaitForInstanceAsync<T>(CancellationToken token = default) where T : SingletonMB<T>
        {
            var tcs = new UniTaskCompletionSource<T>();

            void Handler(T instance)
            {
                SingletonMB<T>.InstanceAssigned -= Handler;
                tcs.TrySetResult(instance);
            }

            SingletonMB<T>.InstanceAssigned += Handler;

            await using (token.Register(() =>
            {
                SingletonMB<T>.InstanceAssigned -= Handler;
                tcs.TrySetCanceled();
            }))
            {
                return await tcs.Task;
            }
        }

        public static async UniTask<T> WaitForInstanceAsync<T>(this T _, CancellationToken token = default) where T : SingletonMB<T>
        {
            return await WaitForInstanceAsync<T>(token);
        }
    }
}
