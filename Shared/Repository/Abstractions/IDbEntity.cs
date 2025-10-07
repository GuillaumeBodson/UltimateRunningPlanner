namespace Shared.Repository.Abstractions;

public interface IDbEntity<T> where T : struct
{
    T Id { get; set; }
}
