# NuExt.Minimal.Mvvm.MahApps.Metro

`NuExt.Minimal.Mvvm.MahApps.Metro` integrates MahApps.Metro with `NuExt.Minimal.Mvvm.Wpf`. It provides scoped MahApps dialog coordination, Metro dialogs through the common `IAsyncDialogService` contract, and document management for `MetroTabControl` using the existing NuExt ViewModel and document lifecycles.

[![NuGet](https://img.shields.io/nuget/v/NuExt.Minimal.Mvvm.MahApps.Metro.svg)](https://www.nuget.org/packages/NuExt.Minimal.Mvvm.MahApps.Metro)
[![Ask DeepWiki](https://img.shields.io/badge/Ask-DeepWiki-blue)](https://deepwiki.com/nu-ext/NuExt.Minimal.Mvvm.MahApps.Metro)
[![Build](https://github.com/nu-ext/NuExt.Minimal.Mvvm.MahApps.Metro/actions/workflows/ci.yml/badge.svg)](https://github.com/nu-ext/NuExt.Minimal.Mvvm.MahApps.Metro/actions/workflows/ci.yml)
[![License](https://img.shields.io/github/license/nu-ext/NuExt.Minimal.Mvvm.MahApps.Metro?label=license)](https://github.com/nu-ext/NuExt.Minimal.Mvvm.MahApps.Metro/blob/main/LICENSE)
[![Downloads](https://img.shields.io/nuget/dt/NuExt.Minimal.Mvvm.MahApps.Metro.svg)](https://www.nuget.org/packages/NuExt.Minimal.Mvvm.MahApps.Metro)

## Highlights

- **Scoped MahApps dialog coordination** — use the native `IDialogCoordinator` API through a Window-aware UI service.
- **Unified dialog contract** — show custom Metro dialogs through `IAsyncDialogService` with View resolution, ViewModel initialization, validation, cancellation, and deterministic cleanup.
- **Metro tabbed documents** — host Views and ViewModels in `MetroTabControl` through the common `IAsyncDocumentManagerService` contract.
- **Shared lifecycle** — reuse the same parent, parameter, initialization, closing, and disposal model as `NuExt.Minimal.Mvvm.Wpf`.
- **Native API access** — use MahApps-specific dialogs directly when the common NuExt abstraction is not sufficient.

> The package requires MahApps.Metro resources. Services that host MahApps dialogs must be attached to a `MetroWindow`.

## Quick Start

### 1) Enable MahApps in your app (resources and window)

Add the MahApps theme and the integration package resources to `App.xaml`, then use `MetroWindow` as the base for your main window. See the [MahApps.Metro Quick Start](https://mahapps.github.io/mahapps.com/docs/guides/quick-start).

```xml
<!-- App.xaml -->
<Application ...>
  <Application.Resources>
    <ResourceDictionary>
      <ResourceDictionary.MergedDictionaries>
        <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Themes/Light.Blue.xaml" />
        <ResourceDictionary Source="pack://application:,,,/NuExt.Minimal.Mvvm.MahApps.Metro;component/Themes/Generic.xaml" />
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
private IAsyncDialogService Dialogs => GetRequiredService<IAsyncDialogService>("Dialogs");

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

Use the native MahApps API when a built-in input, message, login, or progress dialog is more appropriate:

```csharp
private IDialogCoordinator DialogCoordinator => GetRequiredService<IDialogCoordinator>();

var result = await DialogCoordinator.ShowMessageAsync(
    this,
    "Confirmation",
    "Continue?",
    MessageDialogStyle.AffirmativeAndNegative);
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
private IAsyncDocumentManagerService Tabs => GetRequiredService<IAsyncDocumentManagerService>("Tabs");

public async Task OpenCustomerAsync(CustomerModel model, CancellationToken ct)
{
    var doc = await Tabs.FindDocumentByIdOrCreateAsync(
        id: model.Id,
        documentType: "CustomerView",
        viewModelFactory: () => new CustomerViewModel(model),
        parentViewModel: this,
        parameter: null,
        cancellationToken: ct);

    doc.Show();
}
```

See the repo [samples/MetroWpfApp](https://github.com/nu-ext/NuExt.Minimal.Mvvm.MahApps.Metro/tree/main/samples/MetroWpfApp) for a runnable example.

## Compatibility

- WPF on **.NET 8/9/10** and **.NET Framework 4.6.2+**
- MahApps.Metro 3
- `MetroWindow` for MahApps dialog hosting
- `MetroTabControl` for Metro tabbed documents

## Installation

Via [NuGet](https://www.nuget.org/):

```sh
dotnet add package NuExt.Minimal.Mvvm.MahApps.Metro
```

Or via Visual Studio:

1. Go to `Tools -> NuGet Package Manager -> Manage NuGet Packages for Solution...`.
2. Search for `NuExt.Minimal.Mvvm.MahApps.Metro`.
3. Click "Install".

### Recommended companions

- [`NuExt.Minimal.Mvvm.SourceGenerator`](https://www.nuget.org/packages/NuExt.Minimal.Mvvm.SourceGenerator) removes ViewModel boilerplate at compile time.

## Ecosystem

- [NuExt.Minimal.Mvvm](https://github.com/nu-ext/NuExt.Minimal.Mvvm)
- [NuExt.Minimal.Mvvm.SourceGenerator](https://github.com/nu-ext/NuExt.Minimal.Mvvm.SourceGenerator)
- [NuExt.Minimal.Behaviors.Wpf](https://github.com/nu-ext/NuExt.Minimal.Behaviors.Wpf)
- [NuExt.Presentation.Wpf](https://github.com/nu-ext/NuExt.Presentation.Wpf)
- [NuExt.Minimal.Mvvm.Wpf](https://github.com/nu-ext/NuExt.Minimal.Mvvm.Wpf)
- [NuExt.System](https://github.com/nu-ext/NuExt.System)
- [NuExt.System.Data](https://github.com/nu-ext/NuExt.System.Data)
- [NuExt.System.Data.SQLite](https://github.com/nu-ext/NuExt.System.Data.SQLite)

## Contributing

Issues and PRs are welcome. Keep changes minimal and performance-conscious.

## License

MIT. See LICENSE.