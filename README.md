# NuExt.Minimal.Mvvm.MahApps.Metro

`NuExt.Minimal.Mvvm.MahApps.Metro` is a NuGet package that provides extensions for integrating [MahApps.Metro](https://github.com/MahApps/MahApps.Metro), a popular Metro-style UI toolkit for WPF applications, with [NuExt.Minimal.Mvvm.Wpf](https://github.com/nu-ext/NuExt.Minimal.Mvvm.Wpf), a WPF extension for the lightweight MVVM framework [NuExt.Minimal.Mvvm](https://github.com/nu-ext/NuExt.Minimal.Mvvm). This package includes services and components to facilitate the creation of modern, responsive, and visually appealing user interfaces using the MVVM pattern.

[![NuGet](https://img.shields.io/nuget/v/NuExt.Minimal.Mvvm.MahApps.Metro.svg)](https://www.nuget.org/packages/NuExt.Minimal.Mvvm.MahApps.Metro)
[![Build](https://github.com/nu-ext/NuExt.Minimal.Mvvm.MahApps.Metro/actions/workflows/ci.yml/badge.svg)](https://github.com/nu-ext/NuExt.Minimal.Mvvm.MahApps.Metro/actions/workflows/ci.yml)
[![License](https://img.shields.io/github/license/nu-ext/NuExt.Minimal.Mvvm.MahApps.Metro?label=license)](https://github.com/nu-ext/NuExt.Minimal.Mvvm.MahApps.Metro/blob/main/LICENSE)
[![Downloads](https://img.shields.io/nuget/dt/NuExt.Minimal.Mvvm.MahApps.Metro.svg)](https://www.nuget.org/packages/NuExt.Minimal.Mvvm.MahApps.Metro)

## What you get

- **Async dialogs (MahApps)** — MVVM‑friendly `IAsyncDialogService` that shows Metro dialogs and returns `UICommand` / `MessageBoxResult` without UI thread blocking.
- **Tabbed documents (MetroTabControl)** — a document manager that hosts view + VM in Metro tabs with an explicit, async lifecycle and clean disposal.
- **Fits the Minimal MVVM WPF layer** — services are dispatcher‑safe and play well with NuExt.Minimal.Mvvm.Wpf window/document patterns.

> **Requires MahApps.Metro** (add resource dictionaries to `App.xaml` and use `MetroWindow`).

## Quick Start

### 1) Enable MahApps in your app (resources and window)
Add MahApps resource dictionaries to `App.xaml` and use `MetroWindow` as the base for your main window (per MahApps docs). [2](https://mahapps.com/docs/guides/quick-start)

```xml
<!-- App.xaml -->
<Application ...>
    <Application.Resources>
      <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
          <!-- MahApps resource dictionaries -->
          <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml" />
          <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml" />
          <!-- Pick a theme -->
          <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Themes/Light.Blue.xaml" />
        </ResourceDictionary.MergedDictionaries>
      </ResourceDictionary>
    </Application.Resources>
</Application>
```
```xml
<!-- MainWindow.xaml -->
<mah:MetroWindow
    x:Class="MyApp.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:mah="http://metro.mahapps.com/winfx/xaml/controls"
    Title="MyApp" Width="900" Height="600"
    WindowStartupLocation="CenterScreen">
    <!-- content -->
</mah:MetroWindow>
```

### 2) Async Metro dialog via IAsyncDialogService

```xml
<!-- attach the MahApps-backed dialog service on a window -->
<nx:Interaction.Behaviors xmlns:nx="http://schemas.nuext.minimal/xaml">
    <nx:DialogCoordinatorService x:Name="DialogCoordinatorService"/>
    <nx:MetroDialogService x:Name="Dialogs" 
        DialogCoordinator="{Binding Source={x:Reference DialogCoordinatorService}}" />
</nx:Interaction.Behaviors>
```
```csharp
// ViewModel (NuExt.Minimal.Mvvm.Wpf base)
private IAsyncDialogService Dialogs => GetService<IAsyncDialogService>("Dialogs");

public async Task AskAsync(CancellationToken ct)
{
	await using var viewModel = new ConfirmViewModel();
    var result = await Dialogs.ShowDialogAsync(
        MessageBoxButton.OKCancel,
        title: "Confirm",
        documentType: "MyDialogView",
        viewModel: viewModel,
        parentViewModel: this,
        parameter: null,
        cancellationToken: ct);

    if (result != MessageBoxResult.OK) return;
    // proceed...
}
```

### 3) Tabbed documents using MetroTabControl

```xml
    <mah:MetroTabControl>
        <nx:Interaction.Behaviors xmlns:nx="http://schemas.nuext.minimal/xaml">
            <nx:MetroTabbedDocumentService x:Name="Tabs" CloseButtonEnabled="True"
                                           ActiveDocument="{Binding ActiveDocument}"
                                           FallbackViewType="{x:Type views:ErrorView}">
            </nx:MetroTabbedDocumentService>
        </nx:Interaction.Behaviors>
    </mah:MetroTabControl>
```
```csharp
public IAsyncDocumentManagerService Tabs => GetService<IAsyncDocumentManagerService>("Tabs");

public async Task OpenCustomerAsync(CustomerModel model, CancellationToken ct)
{
    var vm = new CustomerViewModel(model);
    var doc = await Tabs.CreateDocumentAsync("CustomerView", vm, parentViewModel: this, parameter: null, ct);
    doc.DisposeOnClose = true;
    doc.Show();
}
```

See the repo [samples/MetroWpfApp](https://github.com/nu-ext/NuExt.Minimal.Mvvm.MahApps.Metro/tree/main/samples/MetroWpfApp) for a runnable example.

## Commonly Used Types

- **`Minimal.Mvvm.Wpf.DialogCoordinatorService`**: coordinates dialogs in MVVM scenarios.
- **`Minimal.Mvvm.Wpf.MetroDialogService`**: `IAsyncDialogService` backed by MahApps dialogs.
- **`Minimal.Mvvm.Wpf.MetroTabbedDocumentService`**: document manager for tabs (MetroTabControl).
- **`MahApps.Metro.Controls.Dialogs.MetroDialog`**: The class used for custom dialogs.

## Compatibility

- WPF on **.NET 8/9/10** and **.NET Framework 4.6.2+**
- Requires MahApps.Metro resources in App.xaml and MetroWindow as your window type.

## Installation

Via [NuGet](https://www.nuget.org/):

```sh
dotnet add package NuExt.Minimal.Mvvm.MahApps.Metro
```

Or via Visual Studio:

1. Go to `Tools -> NuGet Package Manager -> Manage NuGet Packages for Solution...`.
2. Search for `NuExt.Minimal.Mvvm.MahApps.Metro`.
3. Click "Install".

**Nice to have**: [NuExt.Minimal.Mvvm.SourceGenerator](https://www.nuget.org/packages/NuExt.Minimal.Mvvm.SourceGenerator) to remove boilerplate in view‑models.

## Contributing

Issues and PRs are welcome. Keep changes minimal and performance-conscious.

## License

MIT. See LICENSE.