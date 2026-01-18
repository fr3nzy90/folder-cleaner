using Microsoft.Extensions.Logging;

namespace FolderCleaner.Utils;

internal class CustomLoggerFactory : IDisposable
{
  private static CustomLoggerFactory _instance;
  private ILoggerFactory _factory;

  private CustomLoggerFactory()
  {
    _factory = LoggerFactory.Create(builder => builder.Setup());
    AppDomain.CurrentDomain.ProcessExit += DisposeInstanceOnProcessExit;
  }

  public void Dispose() =>
    _factory.Dispose();

  private void DisposeInstanceOnProcessExit(object? sender, EventArgs e) =>
    Dispose();

  static CustomLoggerFactory() =>
    _instance = new();

  public static ILogger<T> CreateLogger<T>() =>
    _instance._factory?.CreateLogger<T>() ?? throw new ApplicationException($"Cannot create logger {typeof(T).Name}");
}