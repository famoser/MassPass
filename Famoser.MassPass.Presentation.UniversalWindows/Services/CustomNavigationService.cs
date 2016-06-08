using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Famoser.FrameworkEssentials.Singleton;
using GalaSoft.MvvmLight.Views;

namespace Famoser.MassPass.Presentation.UniversalWindows.Services
{
    public class CustomNavigationService : NavigationService
    {






        private int _backStack;

        public CustomNavigationService()
        {
            _backStack = 0;
        }

        public new void GoBack()
        {
            _backStack--;
            ConfigureButtons();

            base.GoBack();
        }

        public new void NavigateTo(string pageKey)
        {
            _backStack++;
            ConfigureButtons();

            base.NavigateTo(pageKey);
        }

        public new void NavigateTo(string pageKey, object parameter)
        {
            _backStack++;
            ConfigureButtons();
            base.NavigateTo(pageKey, parameter);
        }

        public 

        private void ConfigureButtons()
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = 
                _backStack == 0 ? AppViewBackButtonVisibility.Collapsed : AppViewBackButtonVisibility.Visible;
        }
    }
}
