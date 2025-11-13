using Blazored.LocalStorage;

namespace WebUI.Services.Interfaces;

public abstract class Session<T> : ISession<T> where T : class
{
    protected abstract string Key { get; }
    private readonly ILocalStorageService _localStorageService;
    private T? _value;
    public T? Current => _value;
    public bool HasValue => _value is not null;

    public Session(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }
    public void Set(T value) => _value = value ?? throw new ArgumentNullException(nameof(value));
    public async Task SetAsync(T value)
    {
        Set(value);
        await _localStorageService.SetItemAsync(Key, _value);
    }
    public async Task<T?> GetAsync()
    {
        if (HasValue)
        {
            return _value;
        }
        var value = await _localStorageService.GetItemAsync<T>(Key);
        if (value is null)
        {
            return null;
        }
        Set(value);
        return value;
    }
    public void Clear() => _value = default;
    public async Task ClearAsync()
    { 
        Clear(); 
        await _localStorageService.RemoveItemAsync(Key); 
    }
}
