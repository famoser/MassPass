using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Famoser.MassPass.Common
{
    public class FastThreadSafeStack<T>
    {
        private StackItem<T> _head;
        private readonly StackItem<T> _default = new StackItem<T>(default(T));
        private int _count;

        public FastThreadSafeStack()
        {
            Reset();
        }

        public FastThreadSafeStack(IEnumerable<T> items)
        {
            Reset();
            foreach (var item in items)
            {
                Push(item);
            }
        }

        private void Reset()
        {
            _head = _default;
            _count = 0;
        }

        public void Push(T item)
        {
            var node = new StackItem<T>(item);
            var localHead = _head;
            node.Next = localHead;
            while (localHead != Interlocked.CompareExchange(ref _head, node, localHead))
            {
                localHead = _head;
                node.Next = localHead;
            }
            Interlocked.Increment(ref _count);
        }

        public T Pop()
        {
            var active = _head;
            if (active == _default)
                return default(T);
            var next = active.Next;
            while (active != Interlocked.CompareExchange(ref _head, next, active))
            {
                active = _head;
                if (active == _default)
                    return default(T);
                next = active.Next;
            }
            Interlocked.Decrement(ref _count);
            return active.Item;
        }

        public void Clear()
        {
            Reset();
        }
    }
}
