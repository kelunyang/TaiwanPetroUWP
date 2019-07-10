using System;
using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Geolocation;
using TaiwanPetroLibrary.Helpers;
using TaiwanPetroLibrary.Models;
using TaiwanPetroLibrary.ViewModels;
using Windows.Storage;
using Autofac;
using Nito.AsyncEx;
using System.IO.IsolatedStorage;
using Windows.UI.Xaml.Controls.Maps;
using Windows.Foundation;
using System.Linq;
using System.Collections.Generic;

// 空白頁項目範本已記錄在 https://go.microsoft.com/fwlink/?LinkId=234238

namespace TaiwanPetroUWP
{
    /// <summary>
    /// 可以在本身使用或巡覽至框架內的空白頁面。
    /// </summary>
    public sealed partial class LocationPage : Page
    {
        string DB_PATH = Path.Combine(ApplicationData.Current.LocalFolder.Path, "price.sqlite");
        //string XML_PATH = "config.xml";
        infoViewModel ifvm;
        stationViewModel stvm;
        PropertyProgress<ProgressReport> progress;
        bool forcenavi = false;
        IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();
        int selectionlock = 0;
        Geopoint currentposition = null;

        public LocationPage()
        {
            this.InitializeComponent();
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterType<infoModel>();
            builder.RegisterInstance<infoModel>(new infoModel());
            builder.RegisterType<infoViewModel>().PropertiesAutowired();
            builder.RegisterType<stationViewModel>().PropertiesAutowired();
            builder.RegisterType<discountViewModel>().PropertiesAutowired();
            Autofac.IContainer container = builder.Build();
            gridRoot.DataContext = container.Resolve<stationViewModel>();
            pageRoot.DataContext = container.Resolve<infoViewModel>();
            ifvm = (infoViewModel)pageRoot.DataContext;
            stvm = (stationViewModel)gridRoot.DataContext;
            progress = new PropertyProgress<ProgressReport>();
            progress.PropertyChanged += progress_PropertyChanged;
        }
        void progress_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            PropertyProgress<ProgressReport> obj = (PropertyProgress<ProgressReport>)sender;
            ProgressReport pr = obj.Progress;
            stvm.progressVis = pr.display;
            stvm.progressVal = pr.progress;
            stvm.progressMsg = pr.progressMessage;
            /*systemtray.IsVisible = pr.display;
            systemtray.IsIndeterminate = pr.display;
            systemtray.Text = pr.progressMessage;
            systemtray.Value = pr.progress;*/
        }

        private void positionbutton_Click(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private void station_Tapped(object sender, TappedRoutedEventArgs e)
        {
            stationStorage s = (stationStorage)((FrameworkElement)sender).DataContext;
            if (stvm.stationBehavior)
            {
                stvm.centerloc = s.coordinance;
                stationmap.ZoomLevel = 16;
            }
            else
            {
                stationNavigate(s);
            }
        }

        private void station_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            stationStorage s = (stationStorage)((FrameworkElement)sender).DataContext;
            stationNavigate(s);
        }
        async void stationNavigate(stationStorage s)
        {
            //forcenavi = false;
            string brand = s.brand == 0 ? "中油" : "台塑";
            var uriNewYork = new Uri(@"ms-drive-to:?destination.latitude="+ s.latitude + @"&destination.longitude="+ s.longitude + @"&destination.name="+ brand + s.name);

            // Launch the Windows Maps app
            var launcherOptions = new Windows.System.LauncherOptions();
            launcherOptions.TargetApplicationPackageFamilyName = "Microsoft.WindowsMaps_8wekyb3d8bbwe";
            var success = await Windows.System.Launcher.LaunchUriAsync(uriNewYork, launcherOptions);
            /*MapsDirectionsTask mapsDirectionsTask = new MapsDirectionsTask();
            LabeledMapLocation spaceNeedleLML = new LabeledMapLocation(, new GeoCoordinate(, ));
            mapsDirectionsTask.End = spaceNeedleLML;
            //mapsDirectionsTask.Start = new LabeledMapLocation("aa", new GeoCoordinate(25.0170, 121.4500));  //test location
            mapsDirectionsTask.Show();*/
        }

        private async void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            /*var isofile = await ApplicationData.Current.LocalFolder.GetFileAsync(XML_PATH);
            ifvm.loadConfig(await isofile.OpenStreamForReadAsync());
            stvm.loadConfig(await isofile.OpenStreamForReadAsync());*/
            await stvm.loadDB(DB_PATH);
            if (ifvm.scheduledTaskErrortime != DateTime.MinValue)
            {
                string errormsg = new string(ifvm.scheduledTaskErrorcode.ToCharArray());
                DateTime errordate = new DateTime(ifvm.scheduledTaskErrortime.Ticks);
                string errortrace = new string(ifvm.scheduledTaskErrortrace.ToCharArray());
                ifvm.scheduledTaskErrorcode = "0";
                ifvm.scheduledTaskErrortime = DateTime.MinValue;
                ifvm.scheduledTaskErrortrace = "0";
                ContentDialog mailWindow = new ContentDialog()
                {
                    Title = "背景程式發生錯誤",
                    Content = "背景執行程式似乎發生過錯誤，請問您是否想把錯誤回報給開發者？（回報與否都會自動消除這次的錯誤紀錄）",
                    PrimaryButtonText = "聯絡開發者",
                    SecondaryButtonText = "消除這次的錯誤訊息"
                };
                ContentDialogResult result = await mailWindow.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    var emailMessage = new Windows.ApplicationModel.Email.EmailMessage();
                    emailMessage.Body = "背景執行時發生錯誤\r\n錯誤日期：" + errordate.ToString("yyyy/MM/dd HH:mm:ss") + "\r\n錯誤訊息如下：\r\n" + errormsg + "\r\n錯誤追蹤：\r\n" + errortrace + "\r\n設定檔：\r\n" + ifvm.im.export();
                    emailMessage.To.Add(new Windows.ApplicationModel.Email.EmailRecipient("kelunyang@outlook.com"));
                    await Windows.ApplicationModel.Email.EmailManager.ShowComposeNewEmailAsync(emailMessage);
                }
            }
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            /*var isofile = await ApplicationData.Current.LocalFolder.GetFileAsync(XML_PATH);
            stvm.saveConfig(await isofile.OpenStreamForWriteAsync());*/
            base.OnNavigatedFrom(e);
        }

        private void countryCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).SelectedIndex == -1)
            {
                selectionlock += 1;
            }
            else if (selectionlock == 1)
            {
                selectionlock += 1;
            }
            stvm.sfiltercountry = selectionlock == 0 ? ((ComboBox)sender).SelectedIndex : stvm.sfiltercountry;
            if (selectionlock == 2) selectionlock = 0;
        }

        private async void stationmap_Loaded(object sender, RoutedEventArgs e)
        {
            // Specify a known location.
            currentposition = new Geopoint(new BasicGeoposition() { Latitude = 25.021918, Longitude = 121.535285 });
            var accessStatus = await Geolocator.RequestAccessAsync();
            if (accessStatus == GeolocationAccessStatus.Allowed)
            {
                Geolocator geolocator = new Geolocator();
                Geoposition pos = await geolocator.GetGeopositionAsync();
                currentposition = pos.Coordinate.Point;
            }
            stvm.centerloc = new GeoPoint(currentposition.Position.Latitude, currentposition.Position.Longitude);
            stvm.currentloc = new GeoPoint(currentposition.Position.Latitude, currentposition.Position.Longitude);
            // Add XAML to the map.
            /*UIElement currentPinsta = (UIElement)((DataTemplate)gridRoot.Resources["currentPin"]).LoadContent();
            UIElement currentPinpos = (UIElement)((DataTemplate)gridRoot.Resources["currentPin"]).LoadContent();
            stationmap.Children.Add(currentPinsta);
            positionmap.Children.Add(currentPinpos);
            positionmap.Children.Add((UIElement)((DataTemplate)gridRoot.Resources["queryPin"]).LoadContent());
            MapControl.SetLocation(currentPinsta, currentposition);
            MapControl.SetNormalizedAnchorPoint(currentPinsta, new Point(0.5, 0.5));
            MapControl.SetLocation(currentPinpos, currentposition);
            MapControl.SetNormalizedAnchorPoint(currentPinpos, new Point(0.5, 0.5));*/

            // Set the map location.
            /*stationmap.Center = currentposition;
            stationmap.ZoomLevel = 15;
            stationmap.LandmarksVisible = false;
            positionmap.Center = currentposition;
            positionmap.ZoomLevel = 15;
            positionmap.LandmarksVisible = false;*/

        }

        private async void customPosBtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                await stvm.queryCustomLocation(progress, customlocation.Text);
            }
            catch (Exception ex)
            {
                emailWindow("發生錯誤：" + ex.Message, ex.StackTrace);
            }
        }

        private async void refreshstabutton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            TimeSpan sdb = new TimeSpan(stvm.stationDBnotifyDate.Ticks);
            TimeSpan now = new TimeSpan(DateTime.Now.Ticks);
            ContentDialog msgWindow = new ContentDialog()
            {
                Title = "更新加油站資料庫",
                Content = now.Subtract(sdb).Days > 30 ? "加油站資料庫已經有" + now.Subtract(sdb).Days + "日未更新，請確定網路穩定下按是進行更新，按否將展延一個月更新（若太久不更新，會因為Google API數量而限制無法查詢加油站經緯度）" : "資料庫還在"+ now.Subtract(sdb).Days + "天前才更新過，還要更新嗎？（上次更新日期為："+ stvm.stationDBnotifyDate.ToString("yyyy-MM-dd HH:mm") +"）",
                PrimaryButtonText = "更新",
                SecondaryButtonText = "不更新"
            };
            ContentDialogResult result = await msgWindow.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                await stvm.updateCPC(progress);
                await stvm.updateFPCC(progress);
            }
        }

        private async void emailWindow(string msg,string stack = "無錯誤追蹤")
        {
            ContentDialog mailWindow = new ContentDialog()
            {
                Title = "發生錯誤",
                Content = msg+"\n 您是否願意將App設定檔一並傳回給開發者分析？",
                PrimaryButtonText = "通知開發者",
                SecondaryButtonText = "不通知"
            };
            ContentDialogResult result = await mailWindow.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var emailMessage = new Windows.ApplicationModel.Email.EmailMessage();
                emailMessage.Body = stack == "無錯誤追蹤" ? "" : "\n config.xml內容： \n" +stvm.im.export()+"\n 錯誤名稱："+msg+"\n 錯誤追蹤："+stack;
                emailMessage.To.Add(new Windows.ApplicationModel.Email.EmailRecipient("kelunyang@outlook.com"));
                await Windows.ApplicationModel.Email.EmailManager.ShowComposeNewEmailAsync(emailMessage);
            }
        }

        private async void searchBtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ContentDialog notfoundWindow = new ContentDialog()
            {
                Title = "找不到加油站",
                Content = "指定的範圍找不到加油站！",
                PrimaryButtonText = "OK"
            };
            try
            {
                GeoPoint gp = new GeoPoint(0, 0);
                if (!stvm.sfiltercountryEnable)
                {
                    if (!stvm.sfiltercustomLocation)
                    {
                        currentposition = new Geopoint(new BasicGeoposition() { Latitude = 25.021918, Longitude = 121.535285 });
                        var accessStatus = await Geolocator.RequestAccessAsync();
                        if (accessStatus == GeolocationAccessStatus.Allowed)
                        {
                            Geolocator geolocator = new Geolocator();
                            Geoposition pos = await geolocator.GetGeopositionAsync();
                            currentposition = pos.Coordinate.Point;
                        }
                        gp = new GeoPoint(currentposition.Position.Latitude, currentposition.Position.Longitude);
                        stvm.currentloc = gp;
                    }
                    else
                    {
                        gp = stvm.sfilterlocation;
                    }
                    await stvm.queryStation(gp, progress);
                }
                else
                {
                    await stvm.queryStation(gp, progress);
                }
                if (forcenavi)
                {
                    if (stvm.queryStations.Any())   //第一個是我的最愛的時候會失敗，所以要在longselection裡面能判斷favorite
                    {
                        IEnumerable<longlistCollection<stationStorage>> nearlist = (from col in stvm.queryStations where col.favorite == false orderby col.distance select col).Take(1);
                        if (nearlist.Any())
                        {
                            if (nearlist.First().Any())
                            {
                                stationNavigate(nearlist.First().First());
                            }
                            else
                            {
                                await notfoundWindow.ShowAsync();
                            }
                        }
                        else
                        {
                            await notfoundWindow.ShowAsync();
                        }
                    }
                    else
                    {
                        await notfoundWindow.ShowAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message.Contains("timeout") ? "GPS定位逾時，請再次嘗試" : ex.Message.Contains("turned on location") ? "請開啟GPS再行使用" : ex.Message;
                emailWindow("發生錯誤：" + message, ex.StackTrace);
            }
        }

        private void navistation_Click(object sender, RoutedEventArgs e)
        {
            stationStorage s = (stationStorage)((FrameworkElement)sender).DataContext;
            stationNavigate(s);
        }

        private async void savestation_Click(object sender, RoutedEventArgs e)
        {
            await stvm.saveStation((stationStorage)((FrameworkElement)sender).DataContext, progress);
        }

        private async void querynearstation_Click(object sender, RoutedEventArgs e)
        {
            stationStorage s = (stationStorage)((FrameworkElement)sender).DataContext;
            await stvm.queryStation(s.coordinance, progress);
            stationmap.ZoomLevel = 13.5;
        }
    }
}
