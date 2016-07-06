using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Famoser.MassPass.Presentation.UniversalWindows.UserControls.ContentPages
{
    public sealed partial class EditableTextBlock : UserControl
    {
        public EditableTextBlock()
        {
            this.InitializeComponent();
        }


        #region Label DP
        /// <summary>
        /// Gets or sets the Value which is being displayed
        /// </summary>
        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Identified the Label dependency property
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string),
              typeof(EditableTextBlock), new PropertyMetadata(null));
        #endregion

        #region TextBlockStyle DP
        /// <summary>
        /// Gets or sets the Value which is being displayed
        /// </summary>
        public Style TextBlockStyle
        {
            get { return (Style)GetValue(TextBlockStyleProperty); }
            set { SetValue(TextBlockStyleProperty, value); }
        }

        /// <summary>
        /// Identified the Label dependency property
        /// </summary>
        public static readonly DependencyProperty TextBlockStyleProperty =
            DependencyProperty.Register("TextBlockStyle", typeof(Style),
              typeof(EditableTextBlock), new PropertyMetadata(null));
        #endregion

        #region TextBoxStyle DP
        /// <summary>
        /// Gets or sets the Value which is being displayed
        /// </summary>
        public object TextBoxStyle
        {
            get { return (object)GetValue(TextBoxStyleProperty); }
            set { SetValue(TextBoxStyleProperty, value); }
        }

        /// <summary>
        /// Identified the Label dependency property
        /// </summary>
        public static readonly DependencyProperty TextBoxStyleProperty =
            DependencyProperty.Register("TextBoxStyle", typeof(Style),
              typeof(EditableTextBlock), new PropertyMetadata(null));
        #endregion

        private void SwitchEditModeButton(object sender, RoutedEventArgs e)
        {
            if (TextBlock.Visibility == Visibility.Visible)
            {
                TextBlock.Visibility = Visibility.Collapsed;
                TextBox.Visibility = Visibility.Visible;

                EditSymbolIcon.Style = Application.Current.Resources["FamoserSymbolIconSave"] as Style;
            }
            else
            {
                TextBlock.Visibility = Visibility.Visible;
                TextBox.Visibility = Visibility.Collapsed;

                EditSymbolIcon.Style = Application.Current.Resources["FamoserSymbolIconEdit"] as Style;
            }
        }

        private void CopyToClipboardButton(object sender, RoutedEventArgs e)
        {
            DataPackage dataPackage = new DataPackage { RequestedOperation = DataPackageOperation.Copy };
            dataPackage.SetText(Value);
            Clipboard.SetContent(dataPackage);
        }
    }
}
