Halcyon
=======

Halcyon is a Roslyn Diagnostic with code fix project that uses the new Roslyn API to analyze a C# project for several conditions not currently checked by the Roslyn API. More information on the Roslyn API, including a guide on how to install the needed VS2013 plugins can be found [here](roslyn.codeplex.com). Using Mono, check this [link](http://tirania.org/blog/archive/2014/Apr-09.html).

Implemented Fixes
=================

Two different diagnostics are currently implemented:

- TypeToVar: Detects whenever a static declaration is used in places where an implicit declartion with the var keyword can be used. Offers to replace the static declaration with the var keyword.
- SingleLinePropertyAccessor: Detects whenever the declaration of a single line property accessor takes up more than one line. Offers to fix the formatting by placing the property accessor on a single line.


Future fixes
============

These are the fixes I am attempting to add in the near future:

- Detect whenever a null pointer is returned by a method returning either an implementation of IEnumerable<T> or an array. Offer to fix it by returning an empty array.
- Detect whenever a NotImplementedException is thrown without a message describing the reason. No fixes proposed.


