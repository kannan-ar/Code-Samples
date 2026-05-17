using System.Reflection;
using Microsoft.Data.SqlClient;

namespace DbMigration.Tests.Helpers;

internal static class SqlExceptionFactory
{
    internal static SqlException Create(int errorNumber, string message = "Simulated SQL error")
    {
        var collection = CreateErrorCollection();
        var error = CreateSqlError(errorNumber, message);
        AddErrorToCollection(collection, error);
        return CreateException(collection);
    }

    private static SqlErrorCollection CreateErrorCollection() =>
        (SqlErrorCollection)Activator.CreateInstance(
            typeof(SqlErrorCollection),
            BindingFlags.NonPublic | BindingFlags.Instance,
            binder: null, args: null, culture: null)!;

    private static SqlError CreateSqlError(int errorNumber, string message)
    {
        var ctors = typeof(SqlError)
            .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
            .OrderBy(c => c.GetParameters().Length);

        foreach (var ctor in ctors)
        {
            try
            {
                var args = BuildArgs(ctor.GetParameters(), errorNumber, message);
                var error = (SqlError)ctor.Invoke(args);
                if (error.Number == errorNumber)
                    return error;
            }
            catch { /* try next overload */ }
        }

        throw new InvalidOperationException(
            $"Could not create SqlError with number {errorNumber}. " +
            "The internal constructor signature may have changed.");
    }

    private static void AddErrorToCollection(SqlErrorCollection collection, SqlError error) =>
        typeof(SqlErrorCollection)
            .GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Instance)!
            .Invoke(collection, [error]);

    private static SqlException CreateException(SqlErrorCollection collection) =>
        (SqlException)typeof(SqlException)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .First(m => m.Name == "CreateException"
                     && m.GetParameters().Length == 2
                     && m.GetParameters()[0].ParameterType == typeof(SqlErrorCollection))
            .Invoke(null, [collection, "11.0.0.0"])!;

    private static object?[] BuildArgs(ParameterInfo[] parameters, int errorNumber, string message)
    {
        var intIndex = 0;
        return parameters.Select(p =>
        {
            if (p.ParameterType == typeof(int))
                // First int = errorNumber (infoNumber); subsequent ints (e.g. lineNumber) = 0
                return (object?)(intIndex++ == 0 ? errorNumber : 0);
            if (p.ParameterType == typeof(byte)) return (object?)(byte)1;
            if (p.ParameterType == typeof(string)) return (object?)message;
            if (p.ParameterType == typeof(uint)) return (object?)(uint)0;
            if (p.ParameterType == typeof(bool)) return (object?)false;
            return p.ParameterType.IsValueType
                ? Activator.CreateInstance(p.ParameterType)
                : null;
        }).ToArray();
    }
}
