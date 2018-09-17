using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FW
{
    public class ConcurrentByteArrayQueue
    {
        Queue<Byte[]> queue = new Queue<byte[]>();

        public int Count {
            get {
                int count = 0;
                lock (this)
                {
                    count = queue.Count;
                }
                return count;
            }
        }

        public void Enqueue(byte[] item)
        {
            lock (this)
            {
                queue.Enqueue(item);
            }

        
        }

        public byte[] Dequeue()
        {
            byte[] item = null;
            lock (this)
            {
                if (queue.Count == 0)
                {
                    return null;
                }
                item = queue.Dequeue();
            }
            return item;
        }

        public void Clear()
        {
            lock (this)
            {
                queue.Clear();
            }
        }
    }
}
