# Attempt to understand to GC WeakReference.

Try to understand to GC WeakReference.

If you hold a strong reference to an object directly in a static variable or in a local variable, it can't be collected. Also, if such an object holds references to other objects, those other objects can't be collected either. That is, an entire graph of objects that have "roots" is considered alive and can't be collected. This is why objects that you are using aren't collected while in use, they are used directly or indirectly through static variables or local variables on the callstack.

P.S. For correct wrok of an example, you are will must run from release build and without debugging (Ctr + F5)
