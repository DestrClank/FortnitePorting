﻿using System;
using Avalonia;
using Avalonia.ReactiveUI;
using FortnitePorting.Application;

namespace FortnitePorting;

class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI()
            .With(new Win32PlatformOptions
            {
                CompositionMode = new [] { Win32CompositionMode.WinUIComposition },
                OverlayPopups = true
            })
            .With(new X11PlatformOptions
            {
                OverlayPopups = true
            })
            .With(new AvaloniaNativePlatformOptions
            {
                OverlayPopups = true
            });
}