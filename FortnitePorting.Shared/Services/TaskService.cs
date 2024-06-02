using Avalonia.Threading;

namespace FortnitePorting.Shared.Services;

public static class TaskService
{
    public static event ExceptionDelegate Exception; 
    public delegate void ExceptionDelegate(Exception exception);
    
    public static void Run(Func<Task> function)
    {
        Task.Run(async () =>
        {
            try
            {
                await function().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Exception?.Invoke(e);
            }
        });
    }

    public static async Task RunAsync(Func<Task> function)
    {
        await Task.Run(async () =>
        {
            try
            {
                await function().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Exception?.Invoke(e);
            }
        });
    }

    public static void Run(Action function)
    {
        Task.Run(() =>
        {
            try
            {
                function();
            }
            catch (Exception e)
            {
                Exception?.Invoke(e);
            }
        });
    }

    public static async Task RunAsync(Action function)
    {
        await Task.Run(() =>
        {
            try
            {
                function();
            }
            catch (Exception e)
            {
                Exception?.Invoke(e);
            }
        });
    }

    public static void RunDispatcher(Func<Task> function, DispatcherPriority priority = default)
    {
        Dispatcher.UIThread.Invoke(async () =>
        {
            try
            {
                await function();
            }
            catch (Exception e)
            {
                Exception?.Invoke(e);
            }
        }, priority);
    }

    public static async Task RunDispatcherAsync(Func<Task> function, DispatcherPriority priority = default)
    {
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            try
            {
                await function();
            }
            catch (Exception e)
            {
                Exception?.Invoke(e);
            }
        }, priority);
    }
    
    public static void RunDispatcher(Action function, DispatcherPriority priority = default)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            try
            {
                function();
            }
            catch (Exception e)
            {
                Exception?.Invoke(e);
            }
        }, priority);
    }

    public static async Task RunDispatcherAsync(Action function, DispatcherPriority priority = default)
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            try
            {
                function();
            }
            catch (Exception e)
            {
                Exception?.Invoke(e);
            }
        }, priority);
    }
}