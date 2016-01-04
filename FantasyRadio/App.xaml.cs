using FantasyRadio.Common;
using System;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;


// Документацию по шаблону "Приложение с Pivot" см. по адресу http://go.microsoft.com/fwlink/?LinkID=391641

namespace FantasyRadio
{
    /// <summary>
    /// Обеспечивает зависящее от конкретного приложения поведение, дополняющее класс Application по умолчанию.
    /// </summary>
    public sealed partial class App : Application
    {
        private TransitionCollection transitions;

        /// <summary>
        /// Инициализирует одноэлементный объект приложения. Это первая выполняемая строка разрабатываемого
        /// кода; поэтому она является логическим эквивалентом main() или WinMain().
        /// </summary>
        ///         

        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
            this.UnhandledException += this.Application_UnhandledException;
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame != null && rootFrame.CanGoBack)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        /// <summary>
        /// Вызывается при обычном запуске приложения пользователем.  Будут использоваться другие точки входа,
        /// если приложение запускается для открытия конкретного файла, отображения
        /// результатов поиска и т. д.
        /// </summary>
        /// <param name="e">Сведения о запросе и обработке запуска.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            Frame rootFrame = Window.Current.Content as Frame;

            // Не повторяйте инициализацию приложения, если в окне уже имеется содержимое,
            // только обеспечьте активность окна.
            if (rootFrame == null)
            {
                // Создание фрейма, который станет контекстом навигации, и переход к первой странице.
                rootFrame = new Frame();

                // Связывание фрейма с ключом SuspensionManager.
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                // TODO: Измените это значение на размер кэша, подходящий для вашего приложения.
                rootFrame.CacheSize = 1;

                // Задайте язык по умолчанию
                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Восстановление сохраненного состояния сеанса только тогда, когда это необходимо.
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        // Возникли ошибки при восстановлении состояния.
                        // Предполагаем, что состояние отсутствует, и продолжаем.
                    }
                }

                // Размещение фрейма в текущем окне.
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // Удаляет турникетную навигацию для запуска.
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;

                // Если стек навигации не восстанавливается для перехода к первой странице,
                // настройка новой страницы путем передачи необходимой информации в качестве параметра
                // навигации.
                if (!rootFrame.Navigate(typeof(PivotPage), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }      
            }
            //----------------------------------------------

            if(!Bass.BASS.BASS_Init(-1, 44100, 0/*, IntPtr.Zero, IntPtr.Zero*/))
            {
                throw new Exception("fail init");
            }
            Bass.BASS.BASS_SetConfig(Bass.BASS.BASS_CONFIG_NET_PLAYLIST, 1); // enable playlist processing
            Bass.BASS.BASS_SetConfig(Bass.BASS.BASS_CONFIG_NET_PREBUF, 0); // minimize automatic pre-buffering, so we can do it (and display it) instead
                                                                           //Bass.BASS.BASS_SetConfigPtr(Bass.BASS.BASS_CONFIG_NET_PROXY, IntPtr.Zero);
                                                                           //Bass.BASS.BASS_SetVolume((float)0.5);
            //Bass.BASS.BASS_SetConfigPtr(Bass.BASS.BASS_CONFIG_NET_AGENT | Bass.BASS.BASS_UNICODE, "My App");

            /*Bass.BASS.BASS_Free();
            Bass.BASS.BASS_Init(-1, 44100, 0);
            Bass.BASS.BASS_SetConfig(Bass.BASS.BASS_CONFIG_NET_PLAYLIST, 1);
            Bass.BASS.BASS_SetConfig(Bass.BASS.BASS_CONFIG_NET_PREBUF, 0);
            Bass.BASS.BASS_SetVolume((float)0.5);*/
            //----------------------------------------------
            // Обеспечение активности текущего окна.
            Window.Current.Activate();
        }

        /// <summary>
        /// Восстанавливает переходы содержимого после запуска приложения.
        /// </summary>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }

        /// <summary>
        /// Вызывается при приостановке выполнения приложения.  Состояние приложения сохраняется
        /// без учета информации о том, будет ли оно завершено или возобновлено с неизменным
        /// содержимым памяти.
        /// </summary>
        /// <param name="sender">Источник запроса приостановки.</param>
        /// <param name="e">Сведения о запросе приостановки.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }
        private void Application_UnhandledException(object sender, object e)
        {
            Debug.WriteLine(e.ToString());
        }
    }
}
