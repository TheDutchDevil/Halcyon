Halcyon
=======

Halcyon is a Roslyn Diagnostic with code fix project that uses the new Roslyn API to analyze a C# project for several conditions not currently checked by the Roslyn API. More information on the Roslyn API, including a guide on how to install the needed VS2013 plugins can be found [here](roslyn.codeplex.com). If you're using Mono, check this [link](http://tirania.org/blog/archive/2014/Apr-09.html).

Implemented Fixes
=================

Two different diagnostics are currently implemented:

- TypeToVar: Detects whenever a static declaration is used in places where an implicit declartion with the var keyword can be used. Offers to replace the static declaration with the var keyword. Currently correctly detects the use of the var keyword in places like  a foreach statement or a using statement and does not offer the diagnostic in case a delegate or dynamic variable is declared.
- SingleLinePropertyAccessor: Detects whenever the declaration of a single line property accessor takes up more than one line. Offers to fix the formatting by placing the property accessor on a single line. Currently implemented with a diagnostic, looking to replace this with a refactoring.
- NoArgumentExceptions: Detects whenever exceptions of a certain type are thrown without having any arugments in their constructor. This warning is currently generated for NotImplementedExceptions and ArgumentExceptions that are thrown without any arguments in their constructor.
- NullReturnForIEnumerable: Detect whenever a null pointer is returned by a method returning either an implementation of IEnumerable<T> or an array. Currently does not work with Task<IEnumerable<T>> declarations. 

Future fixes
============

These are the fixes I am attempting to add in the near future:

NullReturnForIEnumerable: Offer a diagnostic and detect declarations like:
```csharp
IEnumerable<int> enumerable = null;
return null;
```

Offer a refactoring that can be used to refactor all calls to a static method by rewriting the calls with the new C# 6 static imports.


