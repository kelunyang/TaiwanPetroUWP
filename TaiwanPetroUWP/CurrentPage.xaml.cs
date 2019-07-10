using Autofac;
using Nito.AsyncEx;
using OxyPlot.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using TaiwanPetroLibrary.Helpers;
using TaiwanPetroLibrary.Models;
using TaiwanPetroLibrary.ViewModels;
using TaiwanPetroUWPAgent.Helpers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
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
    public sealed partial class CurrentPage : Page
    {
        infoViewModel ifvm;
        cpViewModel cpvm;
        ppViewModel ppvm;
        string DB_PATH = Path.Combine(ApplicationData.Current.LocalFolder.Path, "price.sqlite");
        PropertyProgress<ProgressReport> progress;

        public CurrentPage()
        {
            this.InitializeComponent();
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterType<infoModel>();
            builder.RegisterInstance<infoModel>(new infoModel());
            builder.RegisterType<infoViewModel>().PropertiesAutowired();
            builder.RegisterType<cpViewModel>().PropertiesAutowired();
            builder.RegisterType<ppViewModel>().PropertiesAutowired();
            Autofac.IContainer container = builder.Build();
            gridRoot.DataContext = container.Resolve<cpViewModel>();
            pageRoot.DataContext = container.Resolve<infoViewModel>();
            ppGrid.DataContext = container.Resolve<ppViewModel>();
            ifvm = (infoViewModel)pageRoot.DataContext;
            cpvm = (cpViewModel)gridRoot.DataContext;
            ppvm = (ppViewModel)ppGrid.DataContext;
            progress = new PropertyProgress<ProgressReport>();
            progress.PropertyChanged += progress_PropertyChanged;
            //cpvm.setupHistorical();
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

        private async void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            await cpvm.loadDB(DB_PATH);
            await cpvm.buildDB();
            await ppvm.loadDB(DB_PATH);
            await update();
            chartPlot cp = cpvm.setupHistorical();
            cpvm.la = cp.l;
            cpvm.dtx = cp.da;
            cpvm.prices = cp.pl;
            cpvm.avgs = cp.a;
            cpvm.saveds = cp.s;
            cpvm.historicalModel = cp.pm;
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

        private async void refreshBtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await update();
        }

        private async void historal_Click(object sender, RoutedEventArgs e)
        {
            priceStorage ps = ((priceStorage)(((FrameworkElement)sender).DataContext));
            await updaterange(ps.kind.ToString());
            var style = new Style(typeof(FlyoutPresenter));
            style.Setters.Add(new Setter(FlyoutPresenter.MinWidthProperty, gridRoot.ActualWidth));
            FlyoutBase.GetAttachedFlyout(gridRoot).SetValue(Flyout.FlyoutPresenterStyleProperty, style);
            FlyoutBase.GetAttachedFlyout(gridRoot).Placement = ps.brand == 0 ? FlyoutPlacementMode.Bottom : FlyoutPlacementMode.Top;
            FlyoutBase.GetAttachedFlyout(gridRoot).ShowAt(gridRoot);
        }

        private async Task updaterange(string kind)
        {
            cpvm.kind = kind;
            await cpvm.fetchPrice(ifvm.connectivity, progress, 105);
            IEnumerable<double> p95 = from item in cpvm.currentCollections where item.kind == typeDB.CPC95.key select item.price;
            IEnumerable<double> pdiesel = from item in cpvm.currentCollections where item.kind == typeDB.CPCdiesel.key select item.price;
            if (p95.Any())
            {
                if (pdiesel.Any())
                {
                    ppvm.getPrice(p95.First(), pdiesel.First());
                }
            }
            double pp = kind == "4" || kind == "8" ? ppvm.pdprice : ppvm.pprice;
            chartPlot cp = new chartPlot();
            cp.l = cpvm.la;
            cp.da = cpvm.dtx;
            cp.pl = cpvm.prices;
            cp.a = cpvm.avgs;
            cp.s = cpvm.saveds;
            cp.pm = cpvm.historicalModel;
            cp = cpvm.historicalPrice(cp, pp, ppvm.predictpause, kind, progress);
            cpvm.la = cp.l;
            cpvm.dtx = cp.da;
            cpvm.prices = cp.pl;
            cpvm.avgs = cp.a;
            cpvm.saveds = cp.s;
            cpvm.historicalModel = cp.pm;
            chart.InvalidatePlot();
        }

        private async Task update()
        {
            try
            {
                await cpvm.fetchPrice(ifvm.connectivity, progress, 6);
                await ppvm.predictedPrice(ifvm.connectivity, true, progress);
                await cpvm.currentPrice(progress, false);
            } catch(Exception ex)
            {
                string errormsg = ex.Message;
                DateTime errordate = DateTime.Now;
                string errortrace = "記憶體用量：" + (MemoryManager.AppMemoryUsage / 1024f / 1024f) + "/" + (MemoryManager.AppMemoryUsageLimit / 1024f / 1024f) + "\r" + ex.StackTrace;
                ContentDialog mailWindow = new ContentDialog()
                {
                    Title = "程式發生錯誤",
                    Content = "程式發生錯誤，請問您是否想把錯誤回報給開發者？",
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

        private async void countryCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await updaterange(cpvm.kind);
        }

        private async void nozzleBtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await cpvm.savePrice(progress);
        }

        private async void monitor_Click(object sender, RoutedEventArgs e)
        {
            await cpvm.monitorProduct((priceStorage)((FrameworkElement)sender).DataContext, progress, false);
        }

        private async void tile_Click(object sender, RoutedEventArgs e)
        {
            string price = Double.IsNaN(ppvm.pprice) ? "--.-" : ppvm.pprice.ToString();
            int itemid = ((priceStorage)((FrameworkElement)sender).DataContext).kind;
            string itemname = typeDB.productnameDB[((priceStorage)((FrameworkElement)sender).DataContext).kind].name;
            string itemprice = "$" + ((priceStorage)((FrameworkElement)sender).DataContext).price;
            itemprice += ((priceStorage)((FrameworkElement)sender).DataContext).brand == 0 ? "(" + cpvm.CPCcurrentDate.ToString("MM/dd") + ")" : "(" + cpvm.FPCCcurrentDate.ToString("MM/dd") + ")";
            itemprice += "\r\n預計調整：" + price + "元(" + ppvm.penddate.ToString("MM/dd") + ")";
            string itemimg = ((priceStorage)((FrameworkElement)sender).DataContext).brand == 0 ? "CPC" : "FPCC";
            itemimg = "Assets/" + itemimg + ".png";
            bool type = ((MenuFlyoutItem)sender).Text == "設定為預設動態磚" ? true : false;
            if (type) ifvm.defaultTile = itemid;
            tileUpdater.update(itemid, itemname, itemprice, itemimg, type);
        }
    }
}
