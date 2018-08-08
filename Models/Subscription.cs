using System;
using System.Collections.Generic;

namespace xamarinrest.Models
{
    public class Subscription<T>
    {
        public static readonly Dictionary<string, Subscription<T>> subscriptionDictionary = new Dictionary<string, Subscription<T>>();

        public string Id { get; set; }
        public Action Callback { get; set; }

        public Subscription( Action callback )
        {
            Callback = callback;
            Id = Guid.NewGuid().ToString();
            subscriptionDictionary.Add( Id, this );
        }

        public void Unsubscribe()
        {
            subscriptionDictionary.Remove( Id );
        }
    }
}
