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
        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Identified the Label dependency property
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object),
              typeof(EditableTextBlock), new PropertyMetadata(null));

        #endregion

        private void SwitchEditModeButton(object sender, RoutedEventArgs e)
        {
            if (TextBlock.Visibility == Visibility.Visible)
            {
                TextBlock.Visibility = Visibility.Collapsed;
                TextBox.Visibility = Visibility.Visible;

                SymbolIcon.Style = Application.Current.Resources["FamoserSymbolIconSave"] as Style;
            }
            else
            {
                TextBlock.Visibility = Visibility.Visible;
                TextBox.Visibility = Visibility.Collapsed;

                SymbolIcon.Style = Application.Current.Resources["FamoserSymbolIconEdit"] as Style;
            }
        }
    }
}
