using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Famoser.MassPass.View.Interfaces;
using Famoser.MassPass.View.Services.Interfaces;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;

namespace Famoser.MassPass.Presentation.UniversalWindows.Services
{
    public class HistoryNavigationService : IHistoryNavigationService
    {
        private readonly ConcurrentDictionary<string, Type> _pagesByKey = new ConcurrentDictionary<string, Type>();
        private readonly ConcurrentStack<Tuple<INavigationBackNotifier, object>> _notifiers = new ConcurrentStack<Tuple<INavigationBackNotifier, object>>();

        public string RootPageKey => "-- ROOT--";
        public string UnknownPageKey => "-- UNKNOWN--";

        public HistoryNavigationService(bool hideStatusBar = true)
        {
            SystemNavigationManager.GetForCurrentView().BackRequested += (s, ev) =>
            {
                if (!ev.Handled)
                {
                    GoBack();
                    ev.Handled = true;
                }
            };

            if (hideStatusBar)
                HideStatusBar();


            ConfigureBackButton();
        }

        private async void HideStatusBar()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var statusBar = StatusBar.GetForCurrentView();
                await statusBar.HideAsync();
            }
        }

        private void ConfigureBackButton()
        {
            if (_notifiers.Count == 0)
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            else
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
        }

        /// <summary>
        /// The key corresponding to the currently displayed page.
        /// </summary>
        public string CurrentPageKey
        {
            get
            {
                Frame frame = (Frame)Window.Current.Content;
                if (frame.BackStackDepth == 0)
                    return RootPageKey;
                if (frame.Content == null)
                    return UnknownPageKey;
                Type currentType = frame.Content.GetType();
                if (_pagesByKey.All(p => p.Value != currentType))
                    return UnknownPageKey;
                return _pagesByKey.FirstOrDefault(i => i.Value == currentType).Key;
            }
        }

        /// <summary>
        /// If possible, discards the current page and displays the previous page on the navigation stack.
        /// </summary>
        public void GoBack()
        {
            Frame frame = (Frame)Window.Current.Content;
            if (!frame.CanGoBack)
                Window.Current.Close();
            Tuple<INavigationBackNotifier, object> res;
            lock (this)
            {
                frame.GoBack();
                while (!_notifiers.TryPop(out res)) { }
                ConfigureBackButton();
            }
            res?.Item1?.HandleNavigationBack(res.Item2);
        }

        /// <summary>
        /// Displays a new page corresponding to the given key,
        ///             and passes a parameter to the new page.
        ///             Make sure to call the <see cref="M:GalaSoft.MvvmLight.Views.NavigationService.Configure(System.String,System.Type)"/>
        ///             method first.
        /// 
        /// </summary>
        public virtual void NavigateTo(string pageKey, INavigationBackNotifier navigationBackNotifier = null, object notifyObject = null)
        {
            if (!_pagesByKey.ContainsKey(pageKey))
                throw new ArgumentException(string.Format("No such page: {0}. Did you forget to call NavigationService.Configure?", pageKey), "pageKey");
            ((Frame)Window.Current.Content).Navigate(_pagesByKey[pageKey]);
            lock (this)
            {
                _notifiers.Push(new Tuple<INavigationBackNotifier, object>(navigationBackNotifier, notifyObject));
                ConfigureBackButton();
            }
        }

        /// <summary>
        /// Adds a key/page pair to the navigation service.
        /// </summary>
        public void Configure(string key, Type pageType)
        {
            if (_pagesByKey.ContainsKey(key))
                throw new ArgumentException("This key is already used: " + key);
            if (_pagesByKey.Any(p => p.Value == pageType))
                throw new ArgumentException("This type is already configured with key " + _pagesByKey.First(p => p.Value == pageType).Key);
            while (!_pagesByKey.TryAdd(key, pageType)) ;
        }

    }
}
