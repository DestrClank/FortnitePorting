using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Exports.Texture;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Objects.UObject;
using FluentAvalonia.UI.Controls;
using FortnitePorting.Application;
using FortnitePorting.Controls;
using FortnitePorting.Shared;
using FortnitePorting.Shared.Extensions;
using FortnitePorting.Shared.Services;

namespace FortnitePorting.Models.Assets;

public partial class AssetLoaderCollection : ObservableObject
{
    public readonly List<AssetLoaderCategory> Categories =
    [
        new AssetLoaderCategory(EAssetCategory.Cosmetics)
        {
            Loaders = 
            [
                new AssetLoader(EAssetType.Outfit)
                {
                    ClassNames = ["AthenaCharacterItemDefinition"],
                    PlaceholderIconPath = "FortniteGame/Content/Athena/Prototype/Textures/T_Placeholder_Item_Outfit",
                    IconHandler = asset =>
                    {
                        asset.TryGetValue(out UTexture2D? previewImage, "SmallPreviewImage", "LargePreviewImage");
                        if (asset.TryGetValue(out UObject hero, "HeroDefinition")) 
                            hero.TryGetValue(out previewImage, "SmallPreviewImage", "LargePreviewImage");

                        return previewImage;
                    }
                },
                new AssetLoader(EAssetType.Backpack)
                {
                    ClassNames = ["AthenaBackpackItemDefinition"],
                    HideNames = ["_STWHeroNoDefaultBackpack", "_TEST", "Dev_", "_NPC", "_TBD"]
                },
                new AssetLoader(EAssetType.Pickaxe)
                {
                    ClassNames = ["AthenaPickaxeItemDefinition"],
                    HideNames = ["Dev_", "TBD_"],
                    IconHandler = asset =>
                    {
                        asset.TryGetValue(out UTexture2D? previewImage, "SmallPreviewImage", "LargePreviewImage");
                        if (asset.TryGetValue(out UObject weapon, "WeaponDefinition")) 
                            weapon.TryGetValue(out previewImage, "SmallPreviewImage", "LargePreviewImage");

                        return previewImage;
                    }
                },
                new AssetLoader(EAssetType.Glider)
                {
                    ClassNames = ["AthenaGliderItemDefinition"]
                },
                new AssetLoader(EAssetType.Pet)
                {
                    ClassNames = ["AthenaPetCarrierItemDefinition"]
                },
                new AssetLoader(EAssetType.Toy)
                {
                    ClassNames = ["AthenaToyItemDefinition"]
                },
                new AssetLoader(EAssetType.Emoticon)
                {
                    ClassNames = ["AthenaEmojiItemDefinition"],
                    HideNames = ["Emoji_100APlus"]
                },
                new AssetLoader(EAssetType.Spray)
                {
                    ClassNames = ["AthenaSprayItemDefinition"],
                    HideNames = ["SPID_000", "SPID_001"]
                },
                new AssetLoader(EAssetType.Banner)
                {
                    ClassNames = ["FortHomebaseBannerIconItemDefinition"],
                    HideRarity = true
                },
                new AssetLoader(EAssetType.LoadingScreen)
                {
                    ClassNames = ["AthenaLoadingScreenItemDefinition"]
                },
                new AssetLoader(EAssetType.Emote)
                {
                    ClassNames = ["AthenaDanceItemDefinition"],
                    HideNames = ["_CT", "_NPC"]
                }
            ]
        },
        new AssetLoaderCategory(EAssetCategory.Creative)
        {
            Loaders = 
            [
                new AssetLoader(EAssetType.Prop)
                {
                    ClassNames = ["FortPlaysetPropItemDefinition"],
                    HideRarity = true,
                    HidePredicate = (loader, asset, name) =>
                    {
                        // if prop has already been filtered, dont load it
                        // then compare names w/ the already loaded assets
                        // if they have the same name, they're a duplicate and should be filtered
                        // slow on first load, otherwise a LOT faster
                        // TODO put filtered assets into styles of main asset on click
                        var path = asset.GetPathName();
                        if (AppSettings.Current.FilteredProps.Contains(path)) return true;

                        if (loader.FilteredAssetBag.Contains(name))
                        {
                            AppSettings.Current.FilteredProps.Add(path);
                            return true;
                        }
                        
                        loader.FilteredAssetBag.Add(name);
                        return false;
                    } 
                },
                new AssetLoader(EAssetType.Prefab)
                {
                    ClassNames = ["FortPlaysetItemDefinition"],
                    HideNames = ["Device", "PID_Playset", "PID_MapIndicator", "SpikyStadium", "PID_StageLight", "PID_Temp_Island",
                                "PID_LimeEmptyPlot", "PID_Townscaper", "JunoPlotPlaysetItemDefintion"], // lol epic made a typo
                    HideRarity = true,
                    HidePredicate = (loader, asset, name) =>
                    {
                        var tagsHelper = asset.GetOrDefault<FStructFallback?>("CreativeTagsHelper");
                        if (tagsHelper is null) return false;

                        var tags = tagsHelper.GetOrDefault("CreativeTags", Array.Empty<FName>());
                        return tags.Any(tag => tag.Text.Contains("Device", StringComparison.OrdinalIgnoreCase));
                    } 
                }
            ]
        }

    ];
    
    [ObservableProperty] private ObservableCollection<NavigationViewItemBase> _navItems = [];
    [ObservableProperty] private NavigationViewItemBase _selectedNavItem;
    
    [ObservableProperty] private AssetLoader _activeLoader;
    [ObservableProperty] private ReadOnlyObservableCollection<AssetItem> _activeCollection;

    public AssetLoaderCollection()
    {
        TaskService.RunDispatcher(() =>
        {
            foreach (var category in Categories)
            {
                NavItems.Add(new NavigationViewItem
                {
                    Content = category.Category.GetDescription(),
                    SelectsOnInvoked = false,
                    IconSource = new ImageIconSource
                    {
                        Source = ImageExtensions.AvaresBitmap($"avares://FortnitePorting/Assets/FN/{category.Category.ToString()}.png")
                    },
                    MenuItemsSource = category.Loaders.Select(loader => new NavigationViewItem
                    {
                        Tag = loader.Type, 
                        Content = loader.Type.GetDescription(), 
                        IconSource = new ImageIconSource
                        {
                            Source = ImageExtensions.AvaresBitmap($"avares://FortnitePorting/Assets/FN/{loader.Type.ToString()}.png")
                        },
                    })
                });
            }
        });
    }
    
    public async Task Load(EAssetType type)
    {
        Set(type);
        await ActiveLoader.Load();
    }
    
    public void Set(EAssetType type)
    {
        ActiveLoader = Get(type);
        ActiveCollection = ActiveLoader.Filtered;
    }

    public AssetLoader Get(EAssetType type)
    {
        foreach (var category in Categories)
        {
            if (category.Loaders.FirstOrDefault(loader => loader.Type == type) is { } assetLoader)
            {
                return assetLoader;
            }
        }

        return null!; // if this happens it's bc im stupid
    }
}