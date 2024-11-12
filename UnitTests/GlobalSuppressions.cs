// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Assertion", "NUnit2005:Consider using Assert.That(actual, Is.EqualTo(expected)) instead of Assert.AreEqual(expected, actual)", Justification = "<Pending>", Scope = "module")]
[assembly: SuppressMessage("Assertion", "NUnit2018:Consider using Assert.That(expr, Is.Not.Null) instead of Assert.NotNull(expr)", Justification = "<Pending>", Scope = "module")]
[assembly: SuppressMessage("Style", "IDE0090:Use 'new(...)'", Justification = "<Pending>", Scope = "module")]
[assembly: SuppressMessage("Assertion", "NUnit2003:Consider using Assert.That(expr, Is.True) instead of Assert.IsTrue(expr)", Justification = "<Pending>", Scope = "module")]
[assembly: SuppressMessage("Assertion", "NUnit2001:Consider using Assert.That(expr, Is.False) instead of Assert.False(expr)", Justification = "<Pending>", Scope = "module")]
[assembly: SuppressMessage("Assertion", "NUnit2037:Consider using Assert.That(collection, Does.Contain(instance)) instead of Assert.Contains(instance, collection)", Justification = "<Pending>", Scope = "module")]
[assembly: SuppressMessage("Assertion", "NUnit2019:Consider using Assert.That(expr, Is.Not.Null) instead of Assert.IsNotNull(expr)", Justification = "<Pending>", Scope = "module")]
[assembly: SuppressMessage("Assertion", "NUnit2035:Consider using Assert.That(collection, Is.Empty) instead of Assert.IsEmpty(collection)", Justification = "<Pending>", Scope = "module")]
[assembly: SuppressMessage("Assertion", "NUnit2004:Consider using Assert.That(expr, Is.True) instead of Assert.True(expr)")] // Obviously not! Why would I do that?
