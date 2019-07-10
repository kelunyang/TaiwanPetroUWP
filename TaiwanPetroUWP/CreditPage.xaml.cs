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
    public sealed partial class CreditPage : Page
    {
        infoViewModel ifvm;
        discountViewModel dtvm;
        string DB_PATH = Path.Combine(ApplicationData.Current.LocalFolder.Path, "price.sqlite");
        PropertyProgress<ProgressReport> progress;

        public CreditPage()
        {
            this.InitializeComponent();
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterType<infoModel>();
            builder.RegisterInstance<infoModel>(new infoModel());
            builder.RegisterType<infoViewModel>().PropertiesAutowired();
            builder.RegisterType<discountViewModel>().PropertiesAutowired();
            Autofac.IContainer container = builder.Build();
            gridRoot.DataContext = container.Resolve<discountViewModel>();
            pageRoot.DataContext = container.Resolve<infoViewModel>();
            ifvm = (infoViewModel)pageRoot.DataContext;
            dtvm = (discountViewModel)gridRoot.DataContext;
            progress = new PropertyProgress<ProgressReport>();
            progress.PropertyChanged += progress_PropertyChanged;
        }

        void progress_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            PropertyProgress<ProgressReport> obj = (PropertyProgress<ProgressReport>)sender;
            ProgressReport pr = obj.Progress;
            dtvm.progressVis = pr.display;
            dtvm.progressVal = pr.progress;
            dtvm.progressMsg = pr.progressMessage;
            /*systemtray.IsVisible = pr.display;
            systemtray.IsIndeterminate = pr.display;
            systemtray.Text = pr.progressMessage;
            systemtray.Value = pr.progress;*/
        }

        private void filterbutton_Click(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await dtvm.loadDB(DB_PATH);
            await dtvm.loadXML();
        }

        private async void refreshstabutton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            TimeSpan dupdate = new TimeSpan(dtvm.dDBcheckedDate.Ticks);
            TimeSpan now = new TimeSpan(DateTime.Now.Ticks);
            ContentDialog mailWindow = new ContentDialog()
            {
                Title = "更新折扣資料庫",
                Content = "折扣資料庫已經有"+ now.Subtract(dupdate).Days+"天沒更新了，建議每三個月更新一次",
                PrimaryButtonText = "更新",
                SecondaryButtonText = "不更新"
            };
            bool updatedtXML = false;
            ContentDialogResult result = await mailWindow.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                updatedtXML = await dtvm.updateXML(progress);
            }
            if (updatedtXML)
            {
                await dtvm.loadXML();
            }
        }

        private async void queryBtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await dtvm.queryDiscount(progress);
        }

        private async void email_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ContentDialog mailWindow = new ContentDialog()
            {
                Title = "提供折扣消息",
                Content = "如果你知道哪些信用卡的加油折扣，歡迎提供給開發者",
                PrimaryButtonText = "提供開發者",
                SecondaryButtonText = "不提供"
            };
            ContentDialogResult result = await mailWindow.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var emailMessage = new Windows.ApplicationModel.Email.EmailMessage();
                emailMessage.Body = "以下是範本，不一定要完整填寫\n加油站品牌：\n銀行名稱：\n信用卡卡種：\n折扣內容：\n折扣期間：\n";
                emailMessage.To.Add(new Windows.ApplicationModel.Email.EmailRecipient("kelunyang@outlook.com"));
                await Windows.ApplicationModel.Email.EmailManager.ShowComposeNewEmailAsync(emailMessage);
            }
        }
    }
}
