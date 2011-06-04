using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Node.net
{
    public static class EventManager
    {
        private static List<object> m_Processors = new List<object>();

        /// <summary>
        /// Adds an object to the list of objects currently performing asynchronous
        /// processing loops.  The Node.net application will not exit until the
        /// list is empty.
        /// </summary>
        /// <param name="this">The object to add; provide 'this'.</param>
        public static void Add(object @this)
        {
            if (!EventManager.m_Processors.Contains(@this))
                EventManager.m_Processors.Add(@this);
        }

        /// <summary>
        /// Removes an object from the list of objects performing asynchronous
        /// processing loops.  The Node.net application will not exit until the
        /// list is empty.
        /// </summary>
        /// <param name="this">The object to remove; provide 'this'.</param>
        public static void Remove(object @this)
        {
            EventManager.m_Processors.Remove(@this);
        }

        /// <summary>
        /// Returns whether the application can now exit.
        /// </summary>
        /// <returns>Whether the application can now exit.</returns>
        public static bool IsFinished()
        {
            return (EventManager.m_Processors.Count == 0);
        }
    }
}
