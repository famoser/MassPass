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
        private readonly StackItem<T> _default;

        public FastThreadSafeStack()
        {
            _head = new StackItem<T>(default(T));
            _default = _head;
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
            return active.Item;
        }

        public void Clear()
        {
            _head = _default;
        }
    }
}
