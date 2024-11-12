// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Style", "IDE0090:Use 'new(...)'")] // Not in my code you don't.
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement")] // No, it's confusing. The point of using is to make the scope obvious.
[assembly: SuppressMessage("Style", "IDE0056:Use index operator")] // No, it's confusing.
[assembly: SuppressMessage("Style", "IDE0017:Simplify object initialization")] // Not always a good thing
