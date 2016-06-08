using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Famoser.MassPass.View.Services.Interfaces;
using GalaSoft.MvvmLight.Views;

namespace Famoser.MassPass.Presentation.UniversalWindows.Services
{
    /// <summary>
    /// Windows 8 and Windows Phone Application 8.1 implementation of <see cref="T:GalaSoft.MvvmLight.Views.INavigationService"/>.
    ///  </summary>
    public class WindowsNavigationService : IHistoryNavigationService
    {
        private readonly Dictionary<string, Type> _pagesByKey = new Dictionary<string, Type>();

        public string RootPageKey => "-- ROOT--";
        public string UnknownPageKey => "-- UNKNOWN--";

        /// <summary>
        /// The key corresponding to the currently displayed page.
        /// </summary>
        public string CurrentPageKey
        {
            get
            {
                Dictionary<string, Type> dictionary = _pagesByKey;
                bool lockTaken = false;
                try
                {
                    Monitor.Enter(dictionary, ref lockTaken);
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
                finally
                {
                    if (lockTaken)
                        Monitor.Exit(dictionary);
                }
            }
        }

        /// <summary>
        /// If possible, discards the current page and displays the previous page on the navigation stack.
        /// </summary>
        public void GoBack()
        {
            Frame frame = (Frame)Window.Current.Content;
            if (!frame.CanGoBack)
                return;
            frame.GoBack();
        }

        /// <summary>
        /// Displays a new page corresponding to the given key,
        ///             and passes a parameter to the new page.
        ///             Make sure to call the <see cref="M:GalaSoft.MvvmLight.Views.NavigationService.Configure(System.String,System.Type)"/>
        ///             method first.
        /// 
        /// </summary>
        public virtual void NavigateTo(string pageKey, bool persist = true)
        {
            Dictionary<string, Type> dictionary = _pagesByKey;
            bool lockTaken = false;
            try
            {
                Monitor.Enter(dictionary, ref lockTaken);
                if (!_pagesByKey.ContainsKey(pageKey))
                    throw new ArgumentException(string.Format("No such page: {0}. Did you forget to call NavigationService.Configure?", pageKey), "pageKey");
                ((Frame)Window.Current.Content).Navigate(_pagesByKey[pageKey]);
            }
            finally
            {
                if (lockTaken)
                    Monitor.Exit(dictionary);
            }
        }

        /// <summary>
        /// Adds a key/page pair to the navigation service.
        /// </summary>
        public void Configure(string key, Type pageType)
        {
            Dictionary<string, Type> dictionary = _pagesByKey;
            bool lockTaken = false;
            try
            {
                Monitor.Enter(dictionary, ref lockTaken);
                if (_pagesByKey.ContainsKey(key))
                    throw new ArgumentException("This key is already used: " + key);
                if (_pagesByKey.Any(p => p.Value == pageType))
                    throw new ArgumentException("This type is already configured with key " + _pagesByKey.First(p => p.Value == pageType).Key);
                _pagesByKey.Add(key, pageType);
            }
            finally
            {
                if (lockTaken)
                    Monitor.Exit(dictionary);
            }
        }
    }
}
