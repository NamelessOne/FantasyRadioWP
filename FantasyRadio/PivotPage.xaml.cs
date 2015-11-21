﻿using FantasyRadio.Common;
using FantasyRadio.Data;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
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
            TimeSpan ts = new TimeSpan(500000);
            timer.Tick += new EventHandler<object>(timerAction);
            //timer.Tick += timerAction(this, null);
            //--------------------------------------BINDINGS-----------------------------
            RadioTitle.DataContext = Controller.getInstance().RadioManager;
            PlayPauseButton.DataContext = Controller.getInstance().RadioManager;
            RecButton.DataContext = Controller.getInstance().RadioManager;
            BitratePanel1.DataContext = Controller.getInstance().RadioManager;
            BitratePanel2.DataContext = Controller.getInstance().RadioManager;
            BitratePanel3.DataContext = Controller.getInstance().RadioManager;
            BitratePanel4.DataContext = Controller.getInstance().RadioManager;
            BitratePanel5.DataContext = Controller.getInstance().RadioManager;
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
            if (!Frame.Navigate(typeof(ItemPage), itemId))
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

        private void Play_Pause_Button_click(object sender, RoutedEventArgs e)
        {
            if (!Controller.getInstance().RadioManager.CurrentPlayStatus)
            {
                string streamUrl = Controller.getInstance().RadioManager.getCurrentBitrateUrl();
                Task.Run(() =>
                {
                    OpenURL(streamUrl);
                });
            }
            else
            {
                if (Controller.getInstance().RadioManager.CurrentRecStatus)
                    Controller.getInstance().RadioManager.CurrentRecStatus = false;
                //Task.Run(() =>
                //{
                //Bass.BASS.BASS_Stop();
                //Bass.BASS.BASS_StreamFree(Controller.getInstance().BassManager.Chan); //TODO здесь падает
                //Bass.BASS.BASS_Stop();
                //});
                //Bass.BASS.BASS_SetVolume((float)0.1);
                Bass.BASS.BASS_ChannelStop(Controller.getInstance().BassManager.Chan);
                Controller.getInstance().RadioManager.CurrentPlayStatus = false;
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
            int progress = (int)Bass.BASS.BASS_StreamGetFilePosition(Controller.getInstance().BassManager.Chan, Bass.BASS.BASS_FILEPOS_BUFFER);
            if (progress == -1)
            { // failed, eg. stream freed
                timer.Stop(); // stop monitoring
                return;
            }
            progress = progress * 100 / (int)Bass.BASS.BASS_StreamGetFilePosition(Controller.getInstance().BassManager.Chan, Bass.BASS.BASS_FILEPOS_END);
            if (progress > 75
                    || Bass.BASS.BASS_StreamGetFilePosition(Controller.getInstance().BassManager.Chan,
                    Bass.BASS.BASS_FILEPOS_CONNECTED) == 0)
            {
                string icy = Bass.BASS.BASS_ChannelGetTags(
                        Controller.getInstance().BassManager.Chan, Bass.BASS.BASS_TAG_ICY);
                if (icy == null)
                    icy = Bass.BASS.BASS_ChannelGetTags(
                            Controller.getInstance().BassManager.Chan, Bass.BASS.BASS_TAG_HTTP);
                DoMeta();
                Bass.BASS.BASS_ChannelSetSync(Controller.getInstance().BassManager.Chan,
                        Bass.BASS.BASS_SYNC_META, 0, MetaSync, 0);
                Bass.BASS.BASS_ChannelSetSync(Controller.getInstance().BassManager.Chan,
                        Bass.BASS.BASS_SYNC_OGG_CHANGE, 0, MetaSync, 0);
                Bass.BASS.BASS_ChannelSetSync(Controller.getInstance().BassManager.Chan,
                        Bass.BASS.BASS_SYNC_END, 0, EndSync, 0);
                // play it!
                Bass.BASS.BASS_ChannelPlay(Controller.getInstance().BassManager.Chan, false);
                timer.Stop();
            }
            else
            {
                Controller.getInstance().RadioManager.CurrentTitle = string.Format("buffering... {0}", progress);
            }
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
                Controller.getInstance().RadioManager.CurrentTitle = "";
            }
        };

        private void DoMeta()
        {
            string meta = Bass.BASS.BASS_ChannelGetTags(Controller.getInstance().BassManager.Chan,
                    Bass.BASS.BASS_TAG_META);
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
                        Controller.getInstance().RadioManager.CurrentTitle = title;
                    }
                }
                else
                {
                    string ogg = Bass.BASS.BASS_ChannelGetTags(
                            Controller.getInstance().BassManager.Chan, Bass.BASS.BASS_TAG_OGG);
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
                                Controller.getInstance().RadioManager.CurrentTitle = artist + " - " + title;
                            else
                                Controller.getInstance().RadioManager.CurrentTitle = title;
                        }
                    }
                }
            }
            else
            {
                Controller.getInstance().RadioManager.CurrentTitle = "";
            }
            Controller.getInstance().RadioManager.CurrentPlayStatus = true;
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
            Bass.BASS.BASS_StreamFree(Controller.getInstance().BassManager.Chan);
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { Controller.getInstance().RadioManager.CurrentTitle = LocalizedStrings.Instance.getString("Connecting"); });
            /*int c = Bass.BASS.BASS_StreamCreateURL(URL, 0, Bass.BASS.BASS_STREAM_BLOCK
                            | Bass.BASS.BASS_STREAM_STATUS | Bass.BASS.BASS_STREAM_AUTOFREE,
                    Controller.getInstance().BassManager.StatusProc, r);*/

            int c = Bass.BASS.BASS_StreamCreateURL(URL, 0, Bass.BASS.BASS_STREAM_BLOCK | Bass.BASS.BASS_STREAM_STATUS | Bass.BASS.BASS_STREAM_AUTOFREE, null /*Controller.getInstance().BassManager.StatusProc*/, r); // open URL

            lock (lockObject)
            {
                if (r != req)
                {
                    if (c != 0)
                        Bass.BASS.BASS_StreamFree(c);
                    return;
                }
                Controller.getInstance().BassManager.Chan = c;
            }

            if (Controller.getInstance().BassManager.Chan != 0)
            {
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
            Controller.getInstance().RadioManager.CurrentBitrate = (Bitrates)Enum.Parse(typeof(Bitrates), (sender as StackPanel).Tag.ToString());
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

        }
    }
}
