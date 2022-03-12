using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ModernRonin.FluentArgumentParser;

public static class ExpressionExtensions
{
    public static string MemberName<T>(this Expression<T> self) =>
        self.Body switch
        {
            MemberExpression m => m.Member.Name,
            _ => throw new NotImplementedException(self.GetType().ToString())
        };

    public static PropertyInfo PropertyInfo<T>(this Expression<T> self) =>
        self?.Body switch
        {
            MemberExpression m => (PropertyInfo)m.Member,
            _ => throw new NotImplementedException(self?.GetType().ToString())
        };
}