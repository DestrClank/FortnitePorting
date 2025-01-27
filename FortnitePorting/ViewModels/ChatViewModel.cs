using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media.Imaging;
using Clowd.Clipboard;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CUE4Parse;
using DynamicData;
using FluentAvalonia.UI.Controls;
using FortnitePorting.Models;
using FortnitePorting.Models.Chat;
using FortnitePorting.OnlineServices.Packet;
using FortnitePorting.Services;
using FortnitePorting.Shared.Framework;
using FortnitePorting.Shared.Models;
using FortnitePorting.Shared.Models.Clipboard;
using FortnitePorting.Shared.Services;
using Serilog;
using SixLabors.ImageSharp.PixelFormats;
using Globals = FortnitePorting.Shared.Globals;

namespace FortnitePorting.ViewModels;

public partial class ChatViewModel : ViewModelBase
{
    [ObservableProperty] private ScrollViewer _scroll;
    [ObservableProperty] private TeachingTip _imageFlyout;

    [ObservableProperty] private EPermissions _permissions;
    [ObservableProperty] private string _text = string.Empty;
    [ObservableProperty] private Bitmap _selectedImage;
    [ObservableProperty] private string _selectedImageName;

    [ObservableProperty] private ObservableCollection<ChatUser> _users = [];
    [ObservableProperty] private ObservableCollection<ChatMessage> _messages = [];
    [ObservableProperty] private ObservableCollection<string> _commands = [];
    [ObservableProperty] private bool _areServicesDown;
    
    public override async Task Initialize()
    {
        Messages.CollectionChanged += (sender, args) =>
        {
            TaskService.RunDispatcher(() =>
            {
                var isScrolledToEnd = Math.Abs(Scroll.Offset.Y - Scroll.Extent.Height + Scroll.Viewport.Height) < 500;
                if (isScrolledToEnd)
                    Scroll.ScrollToEnd();
            });
        };
    }

    public override async void OnApplicationExit()
    {
        base.OnApplicationExit();
    }

    [RelayCommand]
    public async Task OpenImage()
    {
        if (await BrowseFileDialog(fileTypes: Globals.ChatAttachmentFileType) is { } path)
        {
            SelectedImageName = Path.GetFileName(path);
            SelectedImage = new Bitmap(path);
            ImageFlyout.IsOpen = true;
        }
    }

    public async Task ClipboardPaste()
    {
        if (await AvaloniaClipboard.GetImageAsync() is not { } image) return;
        
        SelectedImageName = "image.png";
        SelectedImage = image;
        ImageFlyout.IsOpen = true;
    }

}

