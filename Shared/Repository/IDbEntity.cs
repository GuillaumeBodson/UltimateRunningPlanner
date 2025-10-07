namespace Shared.Repository;

public interface IDbEntity<T> where T : struct
{
    T Id { get; set; }
}
