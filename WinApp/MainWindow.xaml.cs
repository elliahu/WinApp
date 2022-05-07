using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WinApp.Model;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.ViewManagement;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinApp
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
            ("home",typeof(HomePage)),
            ("settings",typeof(SettingsPage)),
            ("document_detail",typeof(DocumentDetailPage)),
            ("event_planner",typeof(EventPlannerPage))
        };

        public MainWindow()
        {
            this.InitializeComponent();
            this.Title = "Document manager";

            HomePage.NavigationRequested += (s, e) => 
            {
                Navigate("document_detail", new EntranceNavigationTransitionInfo(), s);
            };
            DocumentDetailPage.GoBackRequested += (s, e) =>
            {
                Navigate("home", new EntranceNavigationTransitionInfo());
            };
        }

        // Called when NavView is loaded
        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            // handler for ContentFrame navigation
            ContentFrame.Navigated += ContentFrame_Navigated;

            // loading HomePage by default
            NavView.SelectedItem = NavView.MenuItems[0]; // home is index 0
            Navigate("home", new EntranceNavigationTransitionInfo());
        }

        private void Navigate(string navItemTag, NavigationTransitionInfo transitionInfo, object parametr = null)
        {
            Type _page = null;

            if (navItemTag == "settings")
            {
                _page = typeof(SettingsPage);
            }
            else
            {
                var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
                _page = item.Page;

            }

            var preNavPageType = ContentFrame.CurrentSourcePageType;

            // navige if the page is not already loaded
            if (!(_page is null) && !Type.Equals(preNavPageType, _page))
            {
                ContentFrame.Navigate(_page, parametr, transitionInfo);
            }
        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            NavView.IsBackEnabled = ContentFrame.CanGoBack;

            if (ContentFrame.SourcePageType == typeof(SettingsPage))
            {
                NavView.SelectedItem = (NavigationViewItem)NavView.SelectedItem;
                NavView.Header = CreateHeader("Settings");
            }
            else if (ContentFrame.SourcePageType == typeof(DocumentDetailPage))
            {
                NavView.SelectedItem = NavView.MenuItems.OfType<NavigationViewItem>().First(n => n.Tag.Equals("home"));
                NavView.Header = CreateHeader(((e.Parameter as Document).Name is null)? "Create new document" : "Edit document");
            }
            else if (ContentFrame.SourcePageType != null)
            {
                var item = _pages.FirstOrDefault(p => p.Page == e.SourcePageType);

                NavView.SelectedItem = NavView.MenuItems.OfType<NavigationViewItem>().First(n => n.Tag.Equals(item.Tag));

                NavView.Header = CreateHeader(((NavigationViewItem)NavView.SelectedItem)?.Content?.ToString());
            }
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                Navigate("settings", args.RecommendedNavigationTransitionInfo);
            }
            else if (args.InvokedItemContainer != null)
            {
                var navItemTag = args.InvokedItemContainer.Tag.ToString();
                Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void NavView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            TryGoBack();
        }

        private bool TryGoBack()
        {
            if (!ContentFrame.CanGoBack)
                return false;

            ContentFrame.GoBack();
            return true;
        }

        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private object CreateHeader(string text)
        {
            TextBlock header = new TextBlock();
            header.Text = text;
            header.FontSize = 34;
            header.Margin = new Thickness(0, 0, 0, 25);
            return header;
        }
    }
}
