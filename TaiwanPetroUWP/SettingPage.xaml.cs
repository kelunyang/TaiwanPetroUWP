using Autofac;
using Microsoft.Toolkit.Uwp.Notifications;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TaiwanPetroLibrary.Helpers;
using TaiwanPetroLibrary.Models;
using TaiwanPetroLibrary.ViewModels;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Notifications;
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
    public sealed partial class SettingPage : Page
    {
        infoViewModel ifvm;
        ppViewModel ppvm;
        stationViewModel stvm;
        cpViewModel cpvm;
        discountViewModel dtvm;
        string DB_PATH = Path.Combine(ApplicationData.Current.LocalFolder.Path, "price.sqlite");

        public SettingPage()
        {
            this.InitializeComponent();
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterType<infoModel>();
            builder.RegisterInstance<infoModel>(new infoModel());
            builder.RegisterType<infoViewModel>().PropertiesAutowired();
            builder.RegisterType<ppViewModel>().PropertiesAutowired();
            builder.RegisterType<cpViewModel>().PropertiesAutowired();
            builder.RegisterType<stationViewModel>().PropertiesAutowired();
            builder.RegisterType<discountViewModel>().PropertiesAutowired();
            Autofac.IContainer container = builder.Build();
            ppSetting.DataContext = container.Resolve<ppViewModel>();
            cpSetting.DataContext = container.Resolve<cpViewModel>();
            stationSetting.DataContext = container.Resolve<stationViewModel>();
            discountSetting.DataContext = container.Resolve<discountViewModel>();
            pageRoot.DataContext = container.Resolve<infoViewModel>();
            ifvm = (infoViewModel)pageRoot.DataContext;
            ppvm = (ppViewModel)ppSetting.DataContext;
            cpvm = (cpViewModel)cpSetting.DataContext;
            stvm = (stationViewModel)stationSetting.DataContext;
            dtvm = (discountViewModel)discountSetting.DataContext;
        }

        private async void revertBtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ContentDialog revertWindow = new ContentDialog()
            {
                Title = "回復設定（程式會自動關閉，請手動重啟）",
                Content = "回復原始設定通常可以解決一些設定錯誤的問題，確認嗎？如果確認會回復設定以及油價、加油站資料庫到程式初始狀態（如果還是發生問題請重裝App或聯絡開發者）",
                PrimaryButtonText = "回復",
                SecondaryButtonText = "不回復"
            };
            ContentDialogResult result = await revertWindow.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                ifvm.resetDB = true;
                ifvm.copyConfig = true;
                CoreApplication.Exit();
                //不思議事件之重新啟動後遇到Pending還捕捉不到錯誤= ="
                /*AppRestartFailureReason restart = await CoreApplication.RequestRestartAsync("");
                if (restart == AppRestartFailureReason.NotInForeground || restart == AppRestartFailureReason.Other || restart == AppRestartFailureReason.RestartPending)
                {
                    var toastContent = new ToastContent()
                    {
                        Visual = new ToastVisual()
                        {
                            BindingGeneric = new ToastBindingGeneric()
                            {
                                Children =
                                {
                                    new AdaptiveText()
                                    {
                                        Text = "請手動重新啟動"
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = "自動重新啟動失敗，請手動重啟App，下次開啟時會重新設定"
                                    }
                                }
                            }
                        }
                    };

                    // Create the toast notification
                    var toastNotif = new ToastNotification(toastContent.GetXml());

                    // And send the notification
                    ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
                }*/
            }
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

        private void aboutBtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.GetAttachedFlyout(gridRoot).ShowAt(ContentPanel);
        }

        private async void githubBtn_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/kelunyang/TaiwanPetroPriceUWP"));
        }
    }
}
