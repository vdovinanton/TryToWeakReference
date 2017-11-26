using System;
using System.Threading;
using System.Threading.Tasks;

namespace TryToWeakReference
{
    /// <summary>
    /// Custom type with destructor
    /// </summary>
    public class Finalizable
    {
        ~Finalizable() {
            Console.WriteLine("Finalizable.dtor");
        }
    }

    /// <summary>
    /// Sample <see cref="WeakReference"/> tracker
    /// </summary>
    public class WeakReferenceTracker
    {
        private readonly WeakReference _wr;
        public event Action ReferenceDied = () => { };

        public WeakReferenceTracker(object o, bool trackerResurection)
        {
            _wr = new WeakReference(o, trackerResurection);

            Task.Factory.StartNew(TrackDeath);
        }

        private void TrackDeath()
        {
            while (true)
            {
                if (!_wr.IsAlive)
                {
                    ReferenceDied();
                    break;
                }
                Thread.Sleep(1);
            }
        }

    }

    class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Creating two trackers...");

            var finalizable = new Finalizable();

            // detect object's access when there isn't reference from root application
            var weakTracker = new WeakReferenceTracker(finalizable, false);
            weakTracker.ReferenceDied += () => Console.WriteLine("Short weak reference is dead");

            // detect object's finalize when destructor worked 
            var resurectionTracker = new WeakReferenceTracker(finalizable, true);
            resurectionTracker.ReferenceDied += () => Console.WriteLine("Long weak reference is dead");

            Console.WriteLine("Forcing 0th generation GC...");
            GC.Collect(0);
            // moved finalizable object to freachable queue
            Thread.Sleep(100);

            Console.WriteLine("Forcing 1th generation GC...");
            GC.Collect(1);
            // removed object from freachable queue and finalize destroy them


            GC.KeepAlive(weakTracker);
            GC.KeepAlive(resurectionTracker);

            Console.ReadLine();
        }
    }
}
