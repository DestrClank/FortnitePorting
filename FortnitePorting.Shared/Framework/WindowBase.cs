using Avalonia.Controls;
using FortnitePorting.Shared.Services;

namespace FortnitePorting.Shared.Framework;

public abstract class WindowBase<T> : Window where T : ViewModelBase, new()
{
    protected readonly T ViewModel;

    public WindowBase(bool initializeViewModel = true)
    {
        ViewModel = ViewModelRegistry.Register<T>();
        
        if (initializeViewModel)
        {
            TaskService.Run(async () => await ViewModel.Initialize());
        }
    }
}