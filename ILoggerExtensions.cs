using Microsoft.Extensions.Logging;

namespace Simply.Log;

/// <summary>
/// Use within a "using" block.
/// </summary>
public static class ILoggerExtensions
{
    /// <summary>
    /// Adds field to all logs in this scope.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="fieldName"></param>
    /// <param name="fieldValue"></param>
    /// <returns></returns>
    public static IDisposable? AddField(this ILogger logger, string fieldName, string fieldValue) =>
        AddFields(logger, (fieldName, fieldValue));

    /// <summary>
    /// Adds fields to all logs in this scope.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="fields"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static IDisposable? AddFields(this ILogger logger, params (string fieldName, string fieldValue)[] fields)
    {
        if (logger is null)
            throw new Exception($"{nameof(logger)} is required");


        // simple string or implemention of IEnumerable<KeyValuePair<string, object>> required by BeginScope
        //https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loggerextensions.beginscope?view=net-9.0-pp
        var scopeData = new Dictionary<string, object>(fields.Length);
        foreach (var (fieldName, fieldValue) in fields)
        {
            scopeData[fieldName] = fieldValue;
        }

        return logger.BeginScope(scopeData);
    }

    /// <summary>
    /// Adds entity id field to all logs in this scope.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="entityType"></param>
    /// <param name="entiryId"></param>
    /// <returns></returns>
    public static IDisposable? AddEntityId(this ILogger logger, Type entityType, string entiryId)
    {
        var idName = char.ToLowerInvariant(entityType.Name[0]) + entityType.Name[1..] + "Id";
        return AddField(logger, idName, entiryId);
    }

    /// <summary>
    /// Add field to all logs in this scope.
    /// args are concatenated as the field name, excluding last arg which is used as the field value
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static IDisposable? AddCustomField(this ILogger logger, params string[] args)
    {
        if (args.Length < 2)
            throw new ArgumentException("Must provide at least two arguments.");

        var fieldName = string.Join("", args, 0, args.Length - 1);
        return AddField(logger, fieldName, args[^1]);
    }
}