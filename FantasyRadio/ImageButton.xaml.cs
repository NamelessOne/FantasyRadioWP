using System;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// Шаблон элемента пользовательского элемента управления задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234236

namespace FantasyRadio.CustomControls
{
    public partial class ImageButton : UserControl
    {
        public ImageButton()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void SetValueDp(DependencyProperty property, object value, [System.Runtime.CompilerServices.CallerMemberName] String p = null)
        {
            SetValue(property, value);
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ImageSource), typeof(ImageButton), null);
        public ImageSource Source
        {

            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValueDp(SourceProperty, value); }
        }

        public static readonly DependencyProperty PressedSourceProperty = DependencyProperty.Register("PressedSource", typeof(ImageSource), typeof(ImageButton), null);
        public ImageSource PressedSource {
            get { return (ImageSource)GetValue(PressedSourceProperty); }
            set { SetValueDp(PressedSourceProperty, value); }
        }
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            Image.Source = PressedSource;
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);
            Image.Source = Source;
        }

        protected override void OnPointerCaptureLost(PointerRoutedEventArgs e)
        {
            base.OnPointerCaptureLost(e);
            Image.Source = Source;
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);
            Image.Source = Source;
            /*if (Click != null)
                Click(this, EventArgs.Empty);*/
        }
    }
}
