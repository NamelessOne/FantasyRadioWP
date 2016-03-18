using BackgroundPlayer;
using FantasyRadio.Common;
using FantasyRadio.DataModel;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону "Приложение с Pivot" см. по адресу http://go.microsoft.com/fwlink/?LinkID=391641

namespace FantasyRadio
{
    public enum PlayerSource
    {
        Stream = 0,
        File = 1,
    }

    public sealed partial class PivotPage : Page
    {
        private const string FirstGroupName = "FirstGroup";
        private const string SecondGroupName = "SecondGroup";

        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");
        private static PivotPage pivotPage;
        //---------------------------------------------------
        private bool isMyBackgroundTaskRunning = false;
        private AutoResetEvent SererInitialized;

        public PivotPage()
        {
            this.InitializeComponent();
            SererInitialized = new AutoResetEvent(false);
            pivotPage = this;
            //--------------------------------------BINDINGS-----------------------------
            Controller.getInstance().ResourceDict = Resources;
            RadioTitle.DataContext = Controller.getInstance().CurrentRadioManager;
            PlayPauseButton.DataContext = Controller.getInstance().CurrentRadioManager;
            BitratePanel1.DataContext = Controller.getInstance().CurrentRadioManager;
            BitratePanel2.DataContext = Controller.getInstance().CurrentRadioManager;
            BitratePanel3.DataContext = Controller.getInstance().CurrentRadioManager;
            ScheduleListView.DataContext = Controller.getInstance().CurrentScheduleManager;
            ScheduleCollection.Source = Controller.getInstance().CurrentScheduleManager.Items;
            ArchiveListView.DataContext = Controller.getInstance().CurrentArchiveManager;
            ArchieveCollection.Source = Controller.getInstance().CurrentArchiveManager.Items;
            ArchieveProgressRing.DataContext = Controller.getInstance().CurrentArchiveManager;
            ScheduleProgressRing.DataContext = Controller.getInstance().CurrentScheduleManager;
            SavedListView.DataContext = Controller.getInstance().CurrentSavedManager;
            Controller.getInstance().CurrentSavedManager.ReloadItems();
            //--------------------------------------BINDINGS-----------------------------
            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Получает объект <see cref="NavigationHelper"/>, связанный с данным объектом <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Получает модель представлений для данного объекта <see cref="Page"/>.
        /// Эту настройку можно изменить на модель строго типизированных представлений.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Заполняет страницу содержимым, передаваемым в процессе навигации. Также предоставляется (при наличии) сохраненное состояние
        /// при повторном создании страницы из предыдущего сеанса.
        /// </summary>
        /// <param name="sender">
        /// Источник события; как правило, <see cref="NavigationHelper"/>.
        /// </param>
        /// <param name="e">Данные события, предоставляющие параметр навигации, который передается
        /// <see cref="Frame.Navigate(Type, Object)"/> при первоначальном запросе этой страницы и
        /// словарь состояний, сохраненных этой страницей в ходе предыдущего
        /// сеанса. Состояние будет равно значению NULL при первом посещении страницы.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: Создание соответствующей модели данных для области проблемы, чтобы заменить пример данных
        }

        /// <summary>
        /// Сохраняет состояние, связанное с данной страницей, в случае приостановки приложения или
        /// удаления страницы из кэша навигации. Значения должны соответствовать требованиям сериализации
        /// <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">Источник события; как правило, <see cref="NavigationHelper"/>.</param>
        /// <param name="e">Данные события, которые предоставляют пустой словарь для заполнения
        /// сериализуемым состоянием.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            // TODO: Сохраните здесь уникальное состояние страницы.
        }

        #region Регистрация NavigationHelper

        /// <summary>
        /// Методы, предоставленные в этом разделе, используются исключительно для того, чтобы
        /// NavigationHelper для отклика на методы навигации страницы.
        /// <para>
        /// Логика страницы должна быть размещена в обработчиках событий для 
        /// <see cref="NavigationHelper.LoadState"/>
        /// и <see cref="NavigationHelper.SaveState"/>.
        /// Параметр навигации доступен в методе LoadState 
        /// в дополнение к состоянию страницы, сохраненному в ходе предыдущего сеанса.
        /// </para>
        /// </summary>
        /// <param name="e">Предоставляет данные для методов навигации и обработчики
        /// событий, которые не могут отменить запрос навигации.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void Play_Pause_Button_click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Play button pressed from App");
            if (IsMyBackgroundTaskRunning)
            {
                if (MediaPlayerState.Playing == BackgroundMediaPlayer.Current.CurrentState)
                {
                    BackgroundMediaPlayer.Current.Pause();
                }
                else if (MediaPlayerState.Paused == BackgroundMediaPlayer.Current.CurrentState || MediaPlayerState.Opening == BackgroundMediaPlayer.Current.CurrentState)
                {
                    var message = new ValueSet();
                    message.Add(Constants.PlayStream, Controller.getInstance().CurrentRadioManager.getCurrentBitrateUrl());
                    BackgroundMediaPlayer.SendMessageToBackground(message);
                }
                else if (MediaPlayerState.Closed == BackgroundMediaPlayer.Current.CurrentState)
                {
                    StartBackgroundAudioTaskStream();
                }
            }
            else
            {
                StartBackgroundAudioTaskStream();
            }
        }

        /// <summary>
        /// Initialize Background Media Player Handlers and starts playback
        /// </summary>
        private void StartBackgroundAudioTaskStream()
        {
            AddMediaPlayerEventHandlers();
            var backgroundtaskinitializationresult = this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                bool result = SererInitialized.WaitOne(5000);
                //Send message to initiate playback
                if (result == true)
                {
                    var message = new ValueSet();
                    message.Add(Constants.PlayStream, Controller.getInstance().CurrentRadioManager.getCurrentBitrateUrl());
                    BackgroundMediaPlayer.SendMessageToBackground(message);
                }
                else
                {
                    throw new Exception("Background Audio Task didn't start in expected time");
                }
            }
            );
            backgroundtaskinitializationresult.Completed = new AsyncActionCompletedHandler(BackgroundTaskInitializationCompleted);
        }


        private void bitrateClick(object sender, TappedRoutedEventArgs e)
        {
            //TODO меняем текущий битрейт
            Controller.getInstance().CurrentRadioManager.CurrentBitrate = (Bitrates)Enum.Parse(typeof(Bitrates), (sender as StackPanel).Tag.ToString());
        }

        private async void ScheduleRefreshButtonclick(object sender, TappedRoutedEventArgs e)
        {
            if (!Controller.getInstance().CurrentScheduleManager.IsParsingActive)
            {
                //TODO потенциально непотокобезопасно
                //TODO interlockedExchange
                Controller.getInstance().CurrentScheduleManager.IsParsingActive = true;
                Controller.getInstance().CurrentScheduleManager.Items.Clear();
                var items = await Controller.getInstance().CurrentScheduleManager.ParseScheduleAsync();
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    Controller.getInstance().CurrentScheduleManager.Items = items;
                });
                Controller.getInstance().CurrentScheduleManager.IsParsingActive = false;
            }
        }

        private void ScheduleItemTapped(object sender, TappedRoutedEventArgs e)
        {
            var panel = sender as Grid;
            var tag = panel.Tag as ScheduleEntity;
            Frame.Navigate(typeof(ScheduleItemPage), tag);
        }

        private async void ArchiveRefreshButtonclick(object sender, TappedRoutedEventArgs e)
        {
            if (!Controller.getInstance().CurrentArchiveManager.IsParsingActive)
            {
                //TODO потенциально непотокобезопасно
                //TODO interlockedExchange
                Controller.getInstance().CurrentArchiveManager.IsParsingActive = true;
                Controller.getInstance().CurrentArchiveManager.Items.Clear();
                var items = await Controller.getInstance().CurrentArchiveManager.ParseArchiveAsync();
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    foreach (var item in items)
                    {
                        Controller.getInstance().CurrentArchiveManager.Items.Add(item);
                    }
                });
                Controller.getInstance().CurrentArchiveManager.IsParsingActive = false;
            }
        }

        private async void DownloadTap(object sender, TappedRoutedEventArgs e)
        {
            string url = (sender as Button).Tag.ToString();
            if (!Controller.getInstance().CurrentArchiveManager.RunningDownloads.Contains(url))
            {
                //TODO сделать, чтобы одновременно скачивались только разнве файлы
                Controller.getInstance().CurrentArchiveManager.RunningDownloads.Add(url);
                bool b = await Controller.getInstance().CurrentArchiveManager.saveMp3Async(url);
                if (b)
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        Controller.getInstance().CurrentSavedManager.ReloadItems();
                    });
                }
                Controller.getInstance().CurrentArchiveManager.RunningDownloads.Remove(url);
            }
        }

        private void DeleteTap(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                var fileName = (sender as Control).Tag.ToString();
                var storageFolder = ApplicationData.Current.LocalFolder; //мб RoamingFolder?   
                var createStorageFolderTask = storageFolder.CreateFolderAsync(Constants.SAVED_FOLDER_NAME, CreationCollisionOption.OpenIfExists);
                createStorageFolderTask.AsTask().Wait();
                var folder = createStorageFolderTask.GetResults();
                var getFileTask = folder.GetFileAsync(fileName).AsTask();
                getFileTask.Wait();
                var deleteTask = getFileTask.Result.DeleteAsync().AsTask();
                deleteTask.Wait();
                Controller.getInstance().CurrentSavedManager.ReloadItems();
            }
            catch (Exception ex)
            {
                var message = ex.ToString();
            }
        }

        private void PlaySavedTap(Object sender, TappedRoutedEventArgs e)
        {
            Debug.WriteLine("Play button pressed from App");
            Controller.getInstance().CurrentSavedManager.CurrentMP3Entity = (sender as Control).Tag.ToString();
            if (IsMyBackgroundTaskRunning)
            {
                if (MediaPlayerState.Playing == BackgroundMediaPlayer.Current.CurrentState)
                {
                    BackgroundMediaPlayer.Current.Pause();
                }
                else if (MediaPlayerState.Paused == BackgroundMediaPlayer.Current.CurrentState || MediaPlayerState.Opening == BackgroundMediaPlayer.Current.CurrentState)
                {
                    var message = new ValueSet();
                    message.Add(Constants.PlayFileByName, (sender as Control).Tag);
                    BackgroundMediaPlayer.SendMessageToBackground(message);
                }
                else if (MediaPlayerState.Closed == BackgroundMediaPlayer.Current.CurrentState)
                {
                    StartBackgroundAudioTaskFile((sender as Control).Tag);
                }
            }
            else
            {
                StartBackgroundAudioTaskFile((sender as Control).Tag);
            }
        }

        private void StartBackgroundAudioTaskFile(object tag)
        {
            AddMediaPlayerEventHandlers();
            var backgroundtaskinitializationresult = this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                bool result = SererInitialized.WaitOne(5000);
                //Send message to initiate playback
                if (result == true)
                {
                    var message = new ValueSet();
                    message.Add(Constants.PlayFileByName, tag);
                    BackgroundMediaPlayer.SendMessageToBackground(message);
                }
                else
                {
                    throw new Exception("Background Audio Task didn't start in expected time");
                }
            }
            );
            backgroundtaskinitializationresult.Completed = new AsyncActionCompletedHandler(BackgroundTaskInitializationCompleted);
        }

        /// <summary>
        /// Subscribes to MediaPlayer events
        /// </summary>
        private void AddMediaPlayerEventHandlers()
        {
            //BackgroundMediaPlayer.Current.CurrentStateChanged += this.MediaPlayer_CurrentStateChanged; //Подписываться не здесь, а в PlaylistManager. И из него уже кидать сообщение сюда
            BackgroundMediaPlayer.MessageReceivedFromBackground += this.BackgroundMediaPlayer_MessageReceivedFromBackground;
        }

        private void BackgroundTaskInitializationCompleted(IAsyncAction action, AsyncStatus status)
        {
            if (status == AsyncStatus.Completed)
            {
                Debug.WriteLine("Background Audio Task initialized");
            }
            else if (status == AsyncStatus.Error)
            {
                Debug.WriteLine("Background Audio Task could not initialized due to an error ::" + action.ErrorCode.ToString());
            }
        }

        /// <summary>
        /// MediaPlayer state changed event handlers. 
        /// Note that we can subscribe to events even if Media Player is playing media in background
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        async void MediaPlayer_CurrentStateChanged(MediaPlayer sender, object args)
        {
            switch (sender.CurrentState)
            {
                case MediaPlayerState.Playing:
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        //TODO разделять статусы плеера радио и файлов

                        Controller.getInstance().CurrentRadioManager.CurrentPlayStatus = true;
                    }
                        );

                    break;
                case MediaPlayerState.Paused:
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        Controller.getInstance().CurrentRadioManager.CurrentPlayStatus = false;
                    }
                    );
                    break;
            }
        }

        /// <summary>
        /// This event fired when a message is recieved from Background Process
        /// </summary>
        async void BackgroundMediaPlayer_MessageReceivedFromBackground(object sender, MediaPlayerDataReceivedEventArgs e)
        {
            foreach (string key in e.Data.Keys)
            {
                string message = e.Data[key].ToString();
                var messages = message.Split('?');
                switch (key)
                {
                    case Constants.Trackchanged:
                        //When foreground app is active change track based on background message
                        await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (byte.Parse(messages[1]) == (byte)PlayerSource.Stream)
                            {
                                Controller.getInstance().CurrentRadioManager.CurrentTitle = messages[0];
                            }
                            else if (byte.Parse(messages[1]) == (byte)PlayerSource.File)
                            {
                                //TODO
                            }
                        }
                        );
                        break;
                    case Constants.BackgroundTaskStarted:
                        //Wait for Background Task to be initialized before starting playback
                        Debug.WriteLine("Background Task started");
                        SererInitialized.Set();
                        break;
                    case Constants.Statechanged:
                        Debug.WriteLine("State changed");
                        if (byte.Parse(messages[1]) == (byte)PlayerSource.Stream)
                        {
                            if (byte.Parse(messages[0]) == (byte)MediaPlayerState.Playing)
                            {
                                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                {
                                    //TODO разделять статусы плеера радио и файлов
                                    Controller.getInstance().CurrentRadioManager.CurrentPlayStatus = true;
                                });
                            }
                            else if (byte.Parse(messages[0]) == (byte)MediaPlayerState.Paused)
                            {
                                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                {
                                    Controller.getInstance().CurrentRadioManager.CurrentPlayStatus = false;
                                }
                                );
                            }
                        }
                        else if (byte.Parse(messages[1]) == (byte)PlayerSource.File)
                        {
                            if (byte.Parse(messages[0]) == (byte)MediaPlayerState.Playing)
                            {
                                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                    {
                                        //TODO ставим файлу картинку паузы
                                        Controller.getInstance().CurrentSavedManager.CurrentPlayStatus = SavedManager.PlayStatus.Play;
                                    }
                                        );

                            }
                            else if (byte.Parse(messages[0]) == (byte)MediaPlayerState.Paused)
                            {
                                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                {
                                    //TODO ставим файлу картинку плей
                                    Controller.getInstance().CurrentSavedManager.CurrentPlayStatus = SavedManager.PlayStatus.Stop;
                                }
                                );
                            }
                        }
                        break;
                }
            }
        }

        private void SettingsButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void AboutButtonClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutPage));
        }

        private bool IsMyBackgroundTaskRunning
        {
            get
            {
                if (isMyBackgroundTaskRunning)
                    return true;

                object value = ApplicationSettingsHelper.ReadResetSettingsValue(Constants.BackgroundTaskState);
                if (value == null)
                {
                    return false;
                }
                else
                {
                    isMyBackgroundTaskRunning = ((String)value).Equals(Constants.BackgroundTaskRunning);
                    return isMyBackgroundTaskRunning;
                }
            }
        }
    }
}
