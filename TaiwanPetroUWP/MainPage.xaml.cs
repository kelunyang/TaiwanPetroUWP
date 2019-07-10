using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using FontAwesome.UWP;
using TaiwanPetroLibrary.ViewModels;
using Nito.AsyncEx;
using TaiwanPetroLibrary.Helpers;
using Autofac;
using TaiwanPetroLibrary.Models;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.ApplicationModel;
using System.Threading.Tasks;

// 空白頁項目範本已記錄在 https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x404

namespace TaiwanPetroUWP
{
    /// <summary>
    /// 可以在本身使用或巡覽至框架內的空白頁面。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        infoViewModel ifvm;
        PropertyProgress<ProgressReport> progress;

        public MainPage()
        {
            this.InitializeComponent();
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterType<infoModel>();
            builder.RegisterInstance<infoModel>(new infoModel());
            builder.RegisterType<infoViewModel>().PropertiesAutowired();
            Autofac.IContainer container = builder.Build();
            pageRoot.DataContext = container.Resolve<infoViewModel>();
            ifvm = (infoViewModel)pageRoot.DataContext;
            progress = new PropertyProgress<ProgressReport>();
            progress.PropertyChanged += progress_PropertyChanged;
        }

        void progress_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            PropertyProgress<ProgressReport> obj = (PropertyProgress<ProgressReport>)sender;
            ProgressReport pr = obj.Progress;
            ifvm.progressVis = pr.display;
            ifvm.progressVal = pr.progress;
            ifvm.progressMsg = pr.progressMessage;
            /*systemtray.IsVisible = pr.display;
            systemtray.IsIndeterminate = pr.display;
            systemtray.Text = pr.progressMessage;
            systemtray.Value = pr.progress;*/
        }

        private async void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            NavView.MenuItems.Add(new NavigationViewItem() { Content = "摘要資訊", Icon = new FontAwesome.UWP.FontAwesome() { Glyph = "\uf015" }, Tag = "digest" });
            NavView.MenuItems.Add(new NavigationViewItem() { Content = "油價預測", Icon = new FontAwesome.UWP.FontAwesome() { Glyph = "\uf201" }, Tag = "predict" });
            NavView.MenuItems.Add(new NavigationViewItem() { Content = "本周油價", Icon = new FontAwesome.UWP.FontAwesome() { Glyph = "\uf0ca" }, Tag = "current" });
            NavView.MenuItems.Add(new NavigationViewItem() { Content = "加油站位置", Icon = new FontAwesome.UWP.FontAwesome() { Glyph = "\uf041" }, Tag = "location" });
            NavView.MenuItems.Add(new NavigationViewItem() { Content = "信用卡優惠", Icon = new FontAwesome.UWP.FontAwesome() { Glyph = "\uf09d" }, Tag = "credit" });
            if (ifvm.firstLoad)
            {
                ContentFrame.Navigate(typeof(InitPage));
            }
            else
            {
                switch (ifvm.defaultPage)
                {
                    case 0:
                        ContentFrame.Navigate(typeof(DigestPage));
                        break;
                    case 1:
                        ContentFrame.Navigate(typeof(PredictPage));
                        break;
                    case 2:
                        ContentFrame.Navigate(typeof(CurrentPage));
                        break;
                    case 3:
                        ContentFrame.Navigate(typeof(LocationPage));
                        break;
                    case 4:
                        ContentFrame.Navigate(typeof(CreditPage));
                        break;
                }
            }
            if (ifvm.dailynotifyEnable)
            {
                var scheduleTask = new BackgroundTaskBuilder();
                scheduleTask.Name = "台灣油價查詢背景更新";
                scheduleTask.TaskEntryPoint = "TaiwanPetroUWPAgent.BackgroundAgent";
                scheduleTask.SetTrigger(new TimeTrigger(60, true));
                scheduleTask.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));
                var taskRegistered = false;
                var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
                if (backgroundAccessStatus == BackgroundAccessStatus.AllowedSubjectToSystemPolicy)
                {

                    foreach (var existedTask in BackgroundTaskRegistration.AllTasks)
                    {
                        if (existedTask.Value.Name == scheduleTask.Name)
                        {
                            taskRegistered = true;
                            break;
                        }
                    }
                    if (!taskRegistered)
                    {
                        BackgroundTaskRegistration task = scheduleTask.Register();
                    }
                }
            }
            /*foreach(var nmi in NavView.MenuItems)
            {
                if(nmi is NavigationViewItem)
                {
                    (nmi as NavigationViewItem).Tapped += MainPage_Tapped;
                }
            }*/
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItem is NavigationViewItem)
            {
                if(args.IsSettingsInvoked)
                {
                    ContentFrame.Navigate(typeof(SettingPage));
                }
            } else
            {
                switch (args.InvokedItem)
                {
                    case "油價預測":
                        ContentFrame.Navigate(typeof(PredictPage));
                        break;
                    case "摘要資訊":
                        ContentFrame.Navigate(typeof(DigestPage));
                        break;
                    case "本周油價":
                        ContentFrame.Navigate(typeof(CurrentPage));
                        break;
                    case "加油站位置":
                        ContentFrame.Navigate(typeof(LocationPage));
                        break;
                    case "信用卡優惠":
                        ContentFrame.Navigate(typeof(CreditPage));
                        break;
                }
            }
        }
    }
}
