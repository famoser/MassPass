using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Famoser.MassPass.Common
{
    public class ThreadSafeStack<T> 
    {
        private readonly Stack<T> _stack = new Stack<T>();
        private readonly AsyncLock _asyncLock = new AsyncLock();

        public async Task Push(T item)
        {
            using (await _asyncLock.LockAsync())
            {
                _stack.Push(item);
            }
        }

        public async Task PushAll(IEnumerable<T> items)
        {
            using (await _asyncLock.LockAsync())
            {
                foreach (var item in items)
                {
                    _stack.Push(item);
                }
            }
        }

        public async Task<T> TryPop()
        {
            using (await _asyncLock.LockAsync())
            {
                if (_stack.Count > 0)
                    return _stack.Pop();
                return default(T);
            }
        }

        public async Task<T> TryPeek()
        {
            using (await _asyncLock.LockAsync())
            {
                if (_stack.Count > 0)
                    return _stack.Peek();
                return default(T);
            }
        }

        public async Task Clear()
        {
            using (await _asyncLock.LockAsync())
            {
                _stack.Clear();
            }
        }
    }
}
