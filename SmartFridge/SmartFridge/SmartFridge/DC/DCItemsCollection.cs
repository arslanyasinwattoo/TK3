using System;
using Microsoft.SPOT;
using System.Collections;

namespace SmartFridge
{
    class DCItemsCollection : ArrayList
    {        
        public int Count { get { return this.Count; } }

        public DCItemsCollection()
        {            
        }

        public void Add(DCItem item)
        {
            this.Add(item);
        }

        public void AddRange(DCItemsCollection items)
        {
            foreach (DCItem item in items)
                this.Add(item);
        }

        public void Remove(DCItem item)
        {
            this.Remove(item);
        }

        public void RemoveAt(int index)
        {
            this.RemoveAt(index);
        }

        public IEnumerator GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public DCItem this[int i]
        {
            get { return (DCItem)this[i]; }
            set { this[i] = value; }
        }
    }

}
