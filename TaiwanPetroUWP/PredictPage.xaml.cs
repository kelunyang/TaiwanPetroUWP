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
using OxyPlot;
using TaiwanPetroLibrary.ViewModels;
using Windows.Storage;
using Nito.AsyncEx;
using TaiwanPetroLibrary.Helpers;
using Autofac;
using TaiwanPetroLibrary.Models;
using System.Threading.Tasks;
using TaiwanPetroUWP.Helpers;

// 空白頁項目範本已記錄在 https://go.microsoft.com/fwlink/?LinkId=234238

namespace TaiwanPetroUWP
{
    /// <summary>
    /// 可以在本身使用或巡覽至框架內的空白頁面。
    /// </summary>
    public sealed partial class PredictPage : Page
    {
        infoViewModel ifvm;
        ppViewModel ppvm;
        cpViewModel cpvm;
        string DB_PATH = Path.Combine(ApplicationData.Current.LocalFolder.Path, "price.sqlite");
        PropertyProgress<ProgressReport> progress;

        public PredictPage()
        {
            this.InitializeComponent();
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterType<infoModel>();
            builder.RegisterInstance<infoModel>(new infoModel());
            builder.RegisterType<infoViewModel>().PropertiesAutowired();
            builder.RegisterType<ppViewModel>().PropertiesAutowired();
            builder.RegisterType<cpViewModel>().PropertiesAutowired();
            Autofac.IContainer container = builder.Build();
            gridRoot.DataContext = container.Resolve<ppViewModel>();
            cpGrid.DataContext = container.Resolve<cpViewModel>();
            pageRoot.DataContext = container.Resolve<infoViewModel>();
            ifvm = (infoViewModel)pageRoot.DataContext;
            ppvm = (ppViewModel)gridRoot.DataContext;
            cpvm = (cpViewModel)cpGrid.DataContext;
            progress = new PropertyProgress<ProgressReport>();
            progress.PropertyChanged += progress_PropertyChanged;
        }
        void progress_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            PropertyProgress<ProgressReport> obj = (PropertyProgress<ProgressReport>)sender;
            ProgressReport pr = obj.Progress;
            ppvm.progressVis = pr.display;
            ppvm.progressVal = pr.progress;
            ppvm.progressMsg = pr.progressMessage;
            /*systemtray.IsVisible = pr.display;
            systemtray.IsIndeterminate = pr.display;
            systemtray.Text = pr.progressMessage;
            systemtray.Value = pr.progress;*/
        }

        private async void refreshBtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await update(true);
        }

        private async void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            await cpvm.loadDB(DB_PATH);
            await cpvm.buildDB();
            await ppvm.loadDB(DB_PATH);
            chartPlot cp = cpvm.setupHistorical();
            ppvm.la95 = cp.l;
            ppvm.dtx95 = cp.da;
            ppvm.prices95 = cp.pl;
            ppvm.avgs95 = cp.a;
            ppvm.saveds95 = cp.s;
            ppvm.price95Model = cp.pm;
            cp = cpvm.setupHistorical();
            ppvm.ladiesel = cp.l;
            ppvm.dtxdiesel = cp.da;
            ppvm.pricesdiesel = cp.pl;
            ppvm.avgsdiesel = cp.a;
            ppvm.savedsdiesel = cp.s;
            ppvm.pricedieselModel = cp.pm;
            await update(ppvm.runPredict);
        }

        private async Task update(bool runPredict)
        {
            if (runPredict)
            {
                await cpvm.fetchPrice(ifvm.connectivity, progress, 105);
                await ppvm.predictedPrice(ifvm.connectivity, true, progress);
                await cpvm.currentPrice(progress, false);
                List<string> oilchart = new List<string>() { "2", "4" };
                foreach(string oil in oilchart)
                {
                    await buildChart(oil);
                }
            }
        }

        private async Task buildChart(string kind)
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
            if (kind == "2")
            {
                chartPlot cp = new chartPlot();
                cp.l = ppvm.la95;
                cp.da = ppvm.dtx95;
                cp.pl = ppvm.prices95;
                cp.a = ppvm.avgs95;
                cp.s = ppvm.saveds95;
                cp.pm = ppvm.price95Model;
                cp = cpvm.historicalPrice(cp, pp, ppvm.predictpause, kind, progress);
                ppvm.la95 = cp.l;
                ppvm.dtx95 = cp.da;
                ppvm.prices95 = cp.pl;
                ppvm.avgs95 = cp.a;
                ppvm.saveds95 = cp.s;
                ppvm.price95Model = cp.pm;
                chart95.InvalidatePlot();
            }
            else if (kind == "4")
            {
                chartPlot cp = new chartPlot();
                cp.l = ppvm.ladiesel;
                cp.da = ppvm.dtxdiesel;
                cp.pl = ppvm.pricesdiesel;
                cp.a = ppvm.avgsdiesel;
                cp.s = ppvm.savedsdiesel;
                cp.pm = ppvm.pricedieselModel;
                cp = cpvm.historicalPrice(cp, pp, ppvm.predictpause, kind, progress);
                ppvm.ladiesel = cp.l;
                ppvm.dtxdiesel = cp.da;
                ppvm.pricesdiesel = cp.pl;
                ppvm.avgsdiesel = cp.a;
                ppvm.savedsdiesel = cp.s;
                ppvm.pricedieselModel = cp.pm;
                chartdiesel.InvalidatePlot();
            }
        }

        private void chartdiesel_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
    }
}
