using System.Linq.Expressions;

namespace Shared.Repository.Abstractions;

public interface IIncludeSpecification<T> where T : class
{
    List<Expression<Func<T, object>>> Includes { get; }
    List<string> IncludeStrings { get; }
}
