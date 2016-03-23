using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


// Документацию по шаблону элемента пустой страницы см. по адресу http://go.microsoft.com/fwlink/?LinkID=390556

namespace FantasyRadio
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public const string LOGIN_KEY = "login";
        public const string PASSWORD_KEY = "password";
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Вызывается перед отображением этой страницы во фрейме.
        /// </summary>
        /// <param name="e">Данные события, описывающие, каким образом была достигнута эта страница.
        /// Этот параметр обычно используется для настройки страницы.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var login = ApplicationSettingsHelper.ReadSettingsValue(LOGIN_KEY);
            var password = ApplicationSettingsHelper.ReadSettingsValue(PASSWORD_KEY);
            if (login != null)
                LoginTextBox.Text = login.ToString();
            if (password != null)
                PasswordTextBox.Password = password.ToString();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            ApplicationSettingsHelper.SaveSettingsValue(LOGIN_KEY, LoginTextBox.Text);
            ApplicationSettingsHelper.SaveSettingsValue(PASSWORD_KEY, PasswordTextBox.Password);
            Frame.GoBack();
        }
    }
}
