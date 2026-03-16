using System.ComponentModel;
using MahApps.Metro.Controls;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Minimal.Mvvm.Wpf;

/// <summary>
/// Manages tabbed documents within a MetroTabControl with Metro design language support.
/// Handles creation, lifecycle, and disposal of documents as metro-styled tabs.
/// This service extends DocumentServiceBase for asynchronous document management.
/// </summary>
public class MetroTabbedDocumentService : TabbedDocumentService
{
    #region Dependency Properties

    public static readonly DependencyProperty CloseButtonEnabledProperty = DependencyProperty.Register(
        nameof(CloseButtonEnabled), typeof(bool), typeof(MetroTabbedDocumentService),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

    #endregion

    public MetroTabbedDocumentService()
    {
    }

    #region Properties

    /// <summary>
    /// Gets or sets a value indicating whether the close button is enabled on tab items.
    /// This property affects the visual appearance and interactivity of tab close buttons.
    /// </summary>
    public bool CloseButtonEnabled
    {
        get => (bool)GetValue(CloseButtonEnabledProperty);
        set => SetValue(CloseButtonEnabledProperty, value);
    }

    #endregion

    #region Event Handlers

    private void OnTabControlTabItemClosingEvent(object? sender, BaseMetroTabControl.TabItemClosingEventArgs e)
    {
        Debug.Assert(Equals(sender, AssociatedObject));
        if (sender is not TabControl tabControl)
        {
            return;
        }

        if (e.Cancel || GetDocument(e.ClosingTabItem) is not TabbedDocument document || document.IsClosing || document.CancellationToken.IsCancellationRequested)
        {
            return;
        }

        e.Cancel = true;
        _ = tabControl.Dispatcher.InvokeAsync(async () => await CloseTabItemAsync(document, document.CancellationToken));
    }

    private static async ValueTask CloseTabItemAsync(TabbedDocument document, CancellationToken cancellationToken)
    {
        Debug.Assert(!document.IsClosing);

        if (document.HideInsteadOfClose)
        {
            if (await CanCloseAsync(document.TabItem, cancellationToken) == false)
            {
                return;
            }
            document.Hide();
            return;
        }

        await document.CloseAsync(false).ConfigureAwait(false);
    }

    #endregion

    #region Methods

    protected override TabItem CreateTabItem()
    {
        var tabItem = new MetroTabItem
        {
            Header = "Item",
        };
        BindingOperations.SetBinding(tabItem, MetroTabItem.CloseButtonEnabledProperty, new Binding()
        {
            Path = new PropertyPath(CloseButtonEnabledProperty),
            Source = this,
            Mode = BindingMode.OneWay
        });
        return tabItem;
    }

    protected override void ClearAllBindings(TabItem tabItem)
    {
        BindingOperations.ClearBinding(tabItem, FrameworkElement.DataContextProperty);
        BindingOperations.ClearBinding(tabItem, MetroTabItem.CloseButtonEnabledProperty);
    }

    protected override Lifetime? SubscribeTabControl(TabControl? tabControl)
    {
        var lifetime = base.SubscribeTabControl(tabControl);
        if (lifetime == null)
        {
            return null;
        }
        if (tabControl is BaseMetroTabControl metroTabControl)
        {
            lifetime.AddBracket(
                () => metroTabControl.TabItemClosingEvent += OnTabControlTabItemClosingEvent,
                () => metroTabControl.TabItemClosingEvent -= OnTabControlTabItemClosingEvent);
        }
        return lifetime;
    }

    #endregion
}
