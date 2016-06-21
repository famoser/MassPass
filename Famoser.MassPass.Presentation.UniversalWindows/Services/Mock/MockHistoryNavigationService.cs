using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.FrameworkEssentials.View.Interfaces;

namespace Famoser.MassPass.Presentation.UniversalWindows.Services.Mock
{
    public class MockHistoryNavigationService : IHistoryNavigationService
    {
        public void GoBack()
        {
            throw new NotImplementedException();
        }

        public void NavigateTo(string pageKey, INavigationBackNotifier navigationBackNotifier = null, object notifyObject = null)
        {
            throw new NotImplementedException();
        }

        public void Configure(string key, Type pageType)
        {
            throw new NotImplementedException();
        }

        public string RootPageKey { get; }
        public string UnknownPageKey { get; }
        public string CurrentPageKey { get; }
    }
}
