using System.Linq.Expressions;

namespace Shared.Repository;

public abstract class IncludeSpecification<T> : IIncludeSpecification<T> where T : class
{
    public List<Expression<Func<T, object>>> Includes { get; } = [];
    public List<string> IncludeStrings { get; } = [];

    protected void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    protected void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }
}
