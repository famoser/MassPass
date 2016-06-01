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

        public async Task<T> Pop()
        {
            using (await _asyncLock.LockAsync())
            {
                return _stack.Pop();
            }
        }

        public async Task<T> Peek()
        {
            using (await _asyncLock.LockAsync())
            {
                return _stack.Peek();
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
