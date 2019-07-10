using Autofac;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TaiwanPetroLibrary.Helpers;
using TaiwanPetroLibrary.Models;
using TaiwanPetroLibrary.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白頁項目範本已記錄在 https://go.microsoft.com/fwlink/?LinkId=234238

namespace TaiwanPetroUWP
{
    /// <summary>
    /// 可以在本身使用或巡覽至框架內的空白頁面。
    /// </summary>
    public sealed partial class InitPage : Page
    {
        infoViewModel ifvm;
        stationViewModel stvm;
        cpViewModel cpvm;
        dcViewModel dcvm;
        string DB_PATH = Path.Combine(ApplicationData.Current.LocalFolder.Path, "price.sqlite");
        PropertyProgress<ProgressReport> progress;

        public InitPage()
        {
            this.InitializeComponent();
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterType<infoModel>();
            builder.RegisterInstance<infoModel>(new infoModel());
            builder.RegisterType<infoViewModel>().PropertiesAutowired();
            builder.RegisterType<cpViewModel>().PropertiesAutowired();
            builder.RegisterType<dcViewModel>().PropertiesAutowired();
            builder.RegisterType<stationViewModel>().PropertiesAutowired();
            Autofac.IContainer container = builder.Build();
            soapcontrol.DataContext = container.Resolve<cpViewModel>();
            stationfilters.DataContext = container.Resolve<stationViewModel>();
            pageRoot.DataContext = container.Resolve<infoViewModel>();
            bulletin.DataContext = container.Resolve<dcViewModel>();
            ifvm = (infoViewModel)pageRoot.DataContext;
            cpvm = (cpViewModel)soapcontrol.DataContext;
            stvm = (stationViewModel)stationfilters.DataContext;
            dcvm = (dcViewModel)bulletin.DataContext;
            progress = new PropertyProgress<ProgressReport>();
            progress.PropertyChanged += progress_PropertyChanged;
        }

        void progress_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            PropertyProgress<ProgressReport> obj = (PropertyProgress<ProgressReport>)sender;
            ProgressReport pr = obj.Progress;
            cpvm.progressVis = pr.display;
            cpvm.progressVal = pr.progress;
            cpvm.progressMsg = pr.progressMessage;
            /*systemtray.IsVisible = pr.display;
            systemtray.IsIndeterminate = pr.display;
            systemtray.Text = pr.progressMessage;
            systemtray.Value = pr.progress;*/
        }

        private async void facebook_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://www.facebook.com/TaiwanPetrolPriceWinApp/"));
        }

        private async void email_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var emailMessage = new Windows.ApplicationModel.Email.EmailMessage();
            emailMessage.To.Add(new Windows.ApplicationModel.Email.EmailRecipient("kelunyang@outlook.com"));
            ContentDialog mailWindow = new ContentDialog()
            {
                Title = "聯絡開發者",
                Content = "要包含程式設定嗎？",
                PrimaryButtonText = "要包含",
                SecondaryButtonText = "不包含"
            };
            ContentDialogResult result = await mailWindow.ShowAsync();
            emailMessage.Body = result == ContentDialogResult.Primary ? "請在此填入你要給開發者的訊息 \n ---------------- \n " + ifvm.im.export() : "請在此填入你要給開發者的訊息";
            await Windows.ApplicationModel.Email.EmailManager.ShowComposeNewEmailAsync(emailMessage);
        }

        private async void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            await dcvm.load(ifvm.connectivity, progress);
            await dcvm.buildList(false, progress);
        }

        private void nextBtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ifvm.firstLoad = false;
            switch (ifvm.defaultPage)
            {
                case 0:
                    this.Frame.Navigate(typeof(DigestPage));
                    break;
                case 1:
                    this.Frame.Navigate(typeof(PredictPage));
                    break;
                case 2:
                    this.Frame.Navigate(typeof(CurrentPage));
                    break;
                case 3:
                    this.Frame.Navigate(typeof(LocationPage));
                    break;
                case 4:
                    this.Frame.Navigate(typeof(CreditPage));
                    break;
            }
        }
    }
}
