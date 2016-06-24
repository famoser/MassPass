using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Famoser.FrameworkEssentials.Attributes;
using Famoser.FrameworkEssentials.Helpers;
using Famoser.FrameworkEssentials.Services;
using Famoser.MassPass.View.Enums;

namespace Famoser.MassPass.Presentation.UniversalWindows.Converters
{
    class ProgressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var pr = value as ProgressService;
            if (pr.IsIndeterminateProgressActive())
            {
                var pk = (ProgressKeys) pr.GetActiveIndeterminateProgresses().FirstOrDefault();
                return ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, ProgressKeys>(pk).Description;
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
