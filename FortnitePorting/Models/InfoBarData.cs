using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Rendering.Composition;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using FortnitePorting.Shared.Services;

namespace FortnitePorting.Models;

public partial class InfoBarData : ObservableObject
{
    [ObservableProperty] private string _title;
    [ObservableProperty] private string _message;
    [ObservableProperty] private InfoBarSeverity _severity;
    [ObservableProperty] private bool _autoClose;
    [ObservableProperty] private string _id;
    [ObservableProperty] private float _closeTime;

    public InfoBarData(string title, string message, InfoBarSeverity severity = InfoBarSeverity.Informational, bool autoClose = true, string id = "", float closeTime = 2.0f)
    {
        Title = title;
        Message = message;
        Severity = severity;
        AutoClose = autoClose;
        Id = id;
        CloseTime = closeTime;
    }
}