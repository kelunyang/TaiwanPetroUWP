using Autofac;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class DigestPage : Page
    {
        infoViewModel ifvm;
        cpViewModel cpvm;
        ppViewModel ppvm;
        dcViewModel dcvm;
        string DB_PATH = Path.Combine(ApplicationData.Current.LocalFolder.Path, "price.sqlite");
        PropertyProgress<ProgressReport> progress;

        public DigestPage()
        {
            this.InitializeComponent();
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterType<infoModel>();
            builder.RegisterInstance<infoModel>(new infoModel());
            builder.RegisterType<infoViewModel>().PropertiesAutowired();
            builder.RegisterType<cpViewModel>().PropertiesAutowired();
            builder.RegisterType<ppViewModel>().PropertiesAutowired();
            builder.RegisterType<dcViewModel>().PropertiesAutowired();
            Autofac.IContainer container = builder.Build();
            gridRoot.DataContext = container.Resolve<cpViewModel>();
            pageRoot.DataContext = container.Resolve<infoViewModel>();
            ppStack.DataContext = container.Resolve<ppViewModel>();
            DCBulletin.DataContext = container.Resolve<dcViewModel>();
            ifvm = (infoViewModel)pageRoot.DataContext;
            cpvm = (cpViewModel)gridRoot.DataContext;
            ppvm = (ppViewModel)ppStack.DataContext;
            dcvm = (dcViewModel)DCBulletin.DataContext;
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

        private async void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            await cpvm.loadDB(DB_PATH);
            await cpvm.buildDB();
            await ppvm.loadDB(DB_PATH);
            await update();
        }

        private async Task update()
        {
            await cpvm.fetchPrice(ifvm.connectivity, progress, 6);
            await ppvm.predictedPrice(ifvm.connectivity, true, progress);
            await cpvm.currentPrice(progress, false);
            await dcvm.load(ifvm.connectivity, progress);
            await dcvm.buildList(true, progress);
        }

        private async void refreshBtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await update();
        }

        private async void historical_Tapped(object sender, TappedRoutedEventArgs e)
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

        private async void countryCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await updaterange(cpvm.kind);
        }

        private async void monitor_Click(object sender, RoutedEventArgs e)
        {
            await cpvm.monitorProduct((priceStorage)((FrameworkElement)sender).DataContext, progress, false);
        }
    }
}
