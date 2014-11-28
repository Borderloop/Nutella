﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using BorderSource.Common;
using BorderSource.ProductAssociation;

namespace BorderSource.Queue
{
    public class ProductValidationQueue
    {
        private static readonly object QueueLock = new object();

        private static readonly object InputLock = new object();

        private static readonly object InstanceLock = new object();

        private static ProductValidationQueue _Instance;

        public static ProductValidationQueue Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (InstanceLock)
                    {
                        if (_Instance == null)
                        {
                            _Instance = new ProductValidationQueue();
                        }
                    }
                }
                return _Instance;
            }
        }

        private Queue<ProductValidation> _Queue = new Queue<ProductValidation>();
        public Queue<ProductValidation> Queue
        {
            get
            {
                return _Queue;
            }
        }

        private bool _InputStopped = false;
        public bool InputStopped
        {
            get
            {
                lock (InputLock)
                {
                    return _InputStopped;
                }
            }
            set
            {
                lock (InputLock)
                {
                    _InputStopped = value;
                }
            }
        }

        /// <summary>
        /// This method will put a product in the Queue.
        /// </summary>
        /// <param name="p">The product to be enqueued.</param>
        public void Enqueue(ProductValidation t)
        {
            lock (QueueLock)
            {
                Queue.Enqueue(t);
                Monitor.Pulse(QueueLock);
            }
        }

        /// <summary>
        /// This method will take a product out of the Queue
        /// </summary>
        /// <returns></returns>
        public ProductValidation Dequeue()
        {
            lock (QueueLock)
            {
                while (Queue.Count == 0)
                {
                    if (InputStopped)
                    {
                        return null;
                    } 
                    Monitor.Wait(QueueLock, 3000);
                }
                return Queue.Dequeue();
            }
        }
    }
}
