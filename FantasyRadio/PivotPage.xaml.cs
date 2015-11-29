using FantasyRadio.Common;
using FantasyRadio.CustomControls;
using FantasyRadio.Data;
using FantasyRadio.DataModel;
using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону "Приложение с Pivot" см. по адресу http://go.microsoft.com/fwlink/?LinkID=391641

namespace FantasyRadio
{
    public sealed partial class PivotPage : Page
    {
        private const string FirstGroupName = "FirstGroup";
        private const string SecondGroupName = "SecondGroup";

        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");
        private static PivotPage pivotPage;
        private const long DURATION = 500000;
        DispatcherTimer timer = new DispatcherTimer();

        public PivotPage()
        {
            this.InitializeComponent();
            pivotPage = this;
            TimeSpan ts = new TimeSpan(DURATION);
            timer.Tick += new EventHandler<object>(timerAction);
            //timer.Tick += timerAction(this, null);
            //--------------------------------------BINDINGS-----------------------------
            Controller.getInstance().ResourceDict = Resources;
            RadioTitle.DataContext = Controller.getInstance().CurrentRadioManager;
            PlayPauseButton.DataContext = Controller.getInstance().CurrentRadioManager;
            RecButton.DataContext = Controller.getInstance().CurrentRadioManager;
            BitratePanel1.DataContext = Controller.getInstance().CurrentRadioManager;
            BitratePanel2.DataContext = Controller.getInstance().CurrentRadioManager;
            BitratePanel3.DataContext = Controller.getInstance().CurrentRadioManager;
            BitratePanel4.DataContext = Controller.getInstance().CurrentRadioManager;
            BitratePanel5.DataContext = Controller.getInstance().CurrentRadioManager;
            ScheduleListView.DataContext = Controller.getInstance().CurrentScheduleManager;
            ScheduleCollection.Source = Controller.getInstance().CurrentScheduleManager.Items;
            ArchiveListView.DataContext = Controller.getInstance().CurrentArchiveManager;
            ArchieveCollection.Source = Controller.getInstance().CurrentArchiveManager.Items;
            ArchieveProgressRing.DataContext = Controller.getInstance().CurrentArchiveManager;
            ScheduleProgressRing.DataContext = Controller.getInstance().CurrentScheduleManager;
            SavedListView.DataContext = Controller.getInstance().CurrentSavedManager;
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
            var sampleDataGroup = await SampleDataSource.GetGroupAsync("Group-1");
            this.DefaultViewModel[FirstGroupName] = sampleDataGroup;
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

        /// <summary>
        /// Добавляет элемент в список при нажатии кнопки на панели приложения.
        /// </summary>
        private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            string groupName = this.pivot.SelectedIndex == 0 ? FirstGroupName : SecondGroupName;
            var group = this.DefaultViewModel[groupName] as SampleDataGroup;
            var nextItemId = group.Items.Count + 1;
            var newItem = new SampleDataItem(
                string.Format(CultureInfo.InvariantCulture, "Group-{0}-Item-{1}", this.pivot.SelectedIndex + 1, nextItemId),
                string.Format(CultureInfo.CurrentCulture, this.resourceLoader.GetString("NewItemTitle"), nextItemId),
                string.Empty,
                string.Empty,
                this.resourceLoader.GetString("NewItemDescription"),
                string.Empty);

            group.Items.Add(newItem);

            // Прокручиваем, чтобы новый элемент оказался видимым.
            var container = this.pivot.ContainerFromIndex(this.pivot.SelectedIndex) as ContentControl;
            var listView = container.ContentTemplateRoot as ListView;
            listView.ScrollIntoView(newItem, ScrollIntoViewAlignment.Leading);
        }

        /// <summary>
        /// Вызывается при нажатии элемента внутри раздела.
        /// </summary>
        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Переход к соответствующей странице назначения и настройка новой страницы
            // путем передачи необходимой информации в виде параметра навигации
            var itemId = ((SampleDataItem)e.ClickedItem).UniqueId;
            if (!Frame.Navigate(typeof(ScheduleItemPage), itemId))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        /// <summary>
        /// Загружает содержимое для второго элемента Pivot, когда он становится видимым в результате прокрутки.
        /// </summary>
        private async void SecondPivot_Loaded(object sender, RoutedEventArgs e)
        {
            var sampleDataGroup = await SampleDataSource.GetGroupAsync("Group-2");
            this.DefaultViewModel[SecondGroupName] = sampleDataGroup;
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

        private async void Play_Pause_Button_click(object sender, RoutedEventArgs e)
        {
            if (!Controller.getInstance().CurrentRadioManager.CurrentPlayStatus)
            {
                string streamUrl = Controller.getInstance().CurrentRadioManager.getCurrentBitrateUrl();
                Task.Run(() =>
                {
                    OpenURL(streamUrl);
                });
            }
            else
            {
                Bass.BASS.BASS_StreamFree(Controller.getInstance().CurrentBassManager.Chan);
                Controller.getInstance().CurrentRadioManager.CurrentPlayStatus = false;
            }
        }

        //Application.Current.Dispatcher.Invoke((Action)(() => messageList.Add(read)));
        /*Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal,
        ref new Windows::UI::Core::DispatchedHandler([this]()
	{
		((TextBlock^)FindName(L"status2"))->Text = "connecting...";
		((TextBlock^)FindName(L"status1"))->Text = "";
		((TextBlock^)FindName(L"status3"))->Text = "";
	}));*/

        private void timerAction(object sender, object e)
        {

            // monitor prebuffering progress
            int progress = (int)Bass.BASS.BASS_StreamGetFilePosition(Controller.getInstance().CurrentBassManager.Chan, Bass.BASS.BASS_FILEPOS_BUFFER);
            if (progress == -1)
            { // failed, eg. stream freed
                timer.Stop(); // stop monitoring
                Controller.getInstance().CurrentRadioManager.CurrentPlayStatus = false;
                return;
            }
            Controller.getInstance().CurrentRadioManager.CurrentPlayStatus = true;
            progress = progress * 100 / (int)Bass.BASS.BASS_StreamGetFilePosition(Controller.getInstance().CurrentBassManager.Chan, Bass.BASS.BASS_FILEPOS_END);
            if (progress > 75
                    || Bass.BASS.BASS_StreamGetFilePosition(Controller.getInstance().CurrentBassManager.Chan,
                    Bass.BASS.BASS_FILEPOS_CONNECTED) == 0)
            {
                string icy = Marshal.PtrToStringAnsi(Bass.BASS.BASS_ChannelGetTags(
                        Controller.getInstance().CurrentBassManager.Chan, Bass.BASS.BASS_TAG_ICY)); //TODO Проблема здесь.
                if (icy == null)
                    icy = Marshal.PtrToStringAnsi(Bass.BASS.BASS_ChannelGetTags(
                            Controller.getInstance().CurrentBassManager.Chan, Bass.BASS.BASS_TAG_HTTP)); //TODO И здесь (иногда).
                DoMeta(); //TODO И здесь.
                Bass.BASS.BASS_ChannelSetSync(Controller.getInstance().CurrentBassManager.Chan,
                        Bass.BASS.BASS_SYNC_META, 0, MetaSync, 0); //TODO И здесь.*/
                Bass.BASS.BASS_ChannelSetSync(Controller.getInstance().CurrentBassManager.Chan,
                        Bass.BASS.BASS_SYNC_OGG_CHANGE, 0, MetaSync, 0);
                Bass.BASS.BASS_ChannelSetSync(Controller.getInstance().CurrentBassManager.Chan,
                        Bass.BASS.BASS_SYNC_END, 0, EndSync, 0);
                // play it!
                Bass.BASS.BASS_ChannelPlay(Controller.getInstance().CurrentBassManager.Chan, false);
                timer.Stop();
            }
            else
            {
                Controller.getInstance().CurrentRadioManager.CurrentTitle = string.Format("buffering... {0}", progress);
            }
            /*int progress = (int)Bass.BASS.BASS_StreamGetFilePosition(Controller.getInstance().BassManager.Chan, Bass.BASS.BASS_FILEPOS_BUFFER);
            progress = progress * 100 / (int)Bass.BASS.BASS_StreamGetFilePosition(Controller.getInstance().BassManager.Chan, Bass.BASS.BASS_FILEPOS_END);
            Bass.BASS.BASS_StreamGetFilePosition(Controller.getInstance().BassManager.Chan, Bass.BASS.BASS_FILEPOS_CONNECTED);
            Bass.BASS.BASS_ChannelSetSync(Controller.getInstance().BassManager.Chan,
                    Bass.BASS.BASS_SYNC_OGG_CHANGE, 0, MetaSync, 0);
            Bass.BASS.BASS_ChannelSetSync(Controller.getInstance().BassManager.Chan,
                    Bass.BASS.BASS_SYNC_END, 0, EndSync, 0);
            Bass.BASS.BASS_ChannelPlay(Controller.getInstance().BassManager.Chan, false);
            timer.Stop();*/
        }

        private Bass.BASS.SYNCPROC MetaSync = new MyMetaSync();

        private class MyMetaSync : Bass.BASS.SYNCPROC
        {
            public void SYNCPROC(int handle, int channel, int data, object user)
            {
                pivotPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => pivotPage.DoMeta());
            }
        }


        private Bass.BASS.SYNCPROC EndSync = new MyEndSync();
        private class MyEndSync : Bass.BASS.SYNCPROC
        {
            public void SYNCPROC(int handle, int channel, int data, Object user)
            {
                Controller.getInstance().CurrentRadioManager.CurrentTitle = "";
            }
        };

        private void DoMeta()
        {
            string meta = Marshal.PtrToStringAnsi(Bass.BASS.BASS_ChannelGetTags(Controller.getInstance().CurrentBassManager.Chan,
                    Bass.BASS.BASS_TAG_META));
            if (meta != null)
            {
                int ti = meta.IndexOf("StreamTitle='");
                int te = meta.IndexOf(";");
                if (ti >= 0)
                {
                    string title = "No title";
                    try
                    {
                        title = meta.Substring(ti + 13, te - 13);
                    }
                    finally
                    {
                        Controller.getInstance().CurrentRadioManager.CurrentTitle = title;
                    }
                }
                else
                {
                    string ogg = Marshal.PtrToStringAnsi(Bass.BASS.BASS_ChannelGetTags(
                            Controller.getInstance().CurrentBassManager.Chan, Bass.BASS.BASS_TAG_OGG));
                    if (ogg != null)
                    { // got Icecast/OGG tags
                        string artist = null, title = null;
                        if (ogg.IndexOf("artist=", 0, ("artist=".Length)) == 0)
                        {
                            artist = ogg.Substring(7);
                        }
                        else if (ogg.IndexOf("title=", 0, ("title=".Length)) == 0)
                        {
                            title = ogg.Substring(6);
                        }
                        if (title != null)
                        {
                            if (artist != null)
                                Controller.getInstance().CurrentRadioManager.CurrentTitle = artist + " - " + title;
                            else
                                Controller.getInstance().CurrentRadioManager.CurrentTitle = title;
                        }
                    }
                }
            }
            else
            {
                Controller.getInstance().CurrentRadioManager.CurrentTitle = "";
            }
        }

        private object lockObject = new object();
        private int req;

        private void OpenURL(string URL)
        {
            int r;
            lock (lockObject)
            {
                r = ++req;
            }
            Bass.BASS.BASS_StreamFree(Controller.getInstance().CurrentBassManager.Chan);
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { Controller.getInstance().CurrentRadioManager.CurrentTitle = LocalizedStrings.Instance.getString("Connecting"); });
            /*int c = Bass.BASS.BASS_StreamCreateURL(URL, 0, Bass.BASS.BASS_STREAM_BLOCK
                            | Bass.BASS.BASS_STREAM_STATUS | Bass.BASS.BASS_STREAM_AUTOFREE,
                    Controller.getInstance().BassManager.StatusProc, r);*/

            int c = Bass.BASS.BASS_StreamCreateURL(URL, 0, Bass.BASS.BASS_STREAM_BLOCK | Bass.BASS.BASS_STREAM_STATUS | Bass.BASS.BASS_STREAM_AUTOFREE, null/*Controller.getInstance().CurrentBassManager.StatusProc*/, r); // open URL
            //--------------------------------------------OLOLOLO--------------------------------------------
            //Bass.BASS.BASS_ChannelPlay(c, false);
            //Bass.BASS.BASS_StreamFree(c);
            //-----------------------------------------------------------------------------------------------
            lock (lockObject)
            {
                if (r != req)
                {
                    if (c != 0)
                        Bass.BASS.BASS_StreamFree(c);
                    return;
                }
                Controller.getInstance().CurrentBassManager.Chan = c;
            }

            if (Controller.getInstance().CurrentBassManager.Chan != 0)
            {
                //Bass.BASS.BASS_ChannelPlay(c, false);
                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { timer.Start(); });
                //handler.postDelayed(timer, 50); 
            }
            else
            {
                int x = Bass.BASS.BASS_ErrorGetCode();
            }
        }

        private void bitrateClick(object sender, TappedRoutedEventArgs e)
        {
            //TODO меняем текущий битрейт
            Controller.getInstance().CurrentRadioManager.CurrentBitrate = (Bitrates)Enum.Parse(typeof(Bitrates), (sender as StackPanel).Tag.ToString());
            /*PlayerState.getInstance().setCurrent_stream(Integer.parseInt(v.getTag().toString()));
            if (PlayerState.getInstance().getCurrentRadioEntity() != null)
            {
                ImageView b = (ImageView)mainFragmentView.findViewById(R.id.streamButton);
                b.performClick();
                b.performClick();
            }*/
        }

        private void Rec_Button_Click(object sender, TappedRoutedEventArgs e)
        {
            if (Controller.getInstance().CurrentRadioManager.CurrentRecStatus)
                Controller.getInstance().CurrentRadioManager.CurrentRecStatus = false;
            else if (Controller.getInstance().CurrentRadioManager.CurrentPlayStatus)
            {
                Controller.getInstance().CurrentRadioManager.CurrentRecStatus = true;
            }
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
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        Controller.getInstance().CurrentSavedManager.ReloadItems();
                    });
                }
                Controller.getInstance().CurrentArchiveManager.RunningDownloads.Remove(url);
            }
        }

        /*private const string FOLDER_PREFIX = "";
        private const int PADDING_FACTOR = 3;
        private const char SPACE = ' ';
        private static StringBuilder folderContents = new StringBuilder();

        // Continue recursive enumeration of files and folders.
        private static async Task ListFilesInFolder(StorageFolder folder, int indentationLevel)
        {
            string indentationPadding = String.Empty.PadRight(indentationLevel * PADDING_FACTOR, SPACE);

            // Get the subfolders in the current folder.
            var foldersInFolder = await folder.GetFoldersAsync();
            // Increase the indentation level of the output.
            int childIndentationLevel = indentationLevel + 1;
            // For each subfolder, call this method again recursively.
            foreach (StorageFolder currentChildFolder in foldersInFolder)
            {
                folderContents.AppendLine(indentationPadding + FOLDER_PREFIX + currentChildFolder.Name);
                await ListFilesInFolder(currentChildFolder, childIndentationLevel);
            }

            // Get the files in the current folder.
            var filesInFolder = await folder.GetFilesAsync();
            foreach (StorageFile currentFile in filesInFolder)
            {
                folderContents.AppendLine(indentationPadding + currentFile.Name);
            }
        }*/
    }
}
