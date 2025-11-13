namespace WebUI.Services.Interfaces;

public interface ISession<T> where T : class
{
    T? Current { get; }
    bool HasValue { get; }

    void Clear();
    Task ClearAsync();
    Task<T?> GetAsync();
    void Set(T? value);
    Task SetAsync(T? value);
}
