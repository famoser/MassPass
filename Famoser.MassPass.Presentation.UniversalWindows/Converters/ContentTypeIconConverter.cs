using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Famoser.MassPass.Business.Enums;

namespace Famoser.MassPass.Presentation.UniversalWindows.Converters
{
    public class ContentTypeIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var res = value as ContentTypes?;
            if (res == ContentTypes.Root || res == ContentTypes.Folder)
                return Symbol.Folder;
            if (res == ContentTypes.Note)
                return Symbol.Caption;
            return Symbol.Placeholder;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
