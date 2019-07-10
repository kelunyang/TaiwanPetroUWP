using Autofac;
using Microsoft.Toolkit.Uwp.Notifications;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using TaiwanPetroLibrary.Helpers;
using TaiwanPetroLibrary.Models;
using TaiwanPetroLibrary.ViewModels;
using TaiwanPetroUWPAgent.Helpers;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.System;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

namespace TaiwanPetroUWPAgent
{
    public sealed class BackgroundAgent : IBackgroundTask
    {
        List<int> active = new List<int>();
        cpViewModel cpvm = new cpViewModel();
        infoViewModel ifvm = new infoViewModel();
        ppViewModel ppvm = new ppViewModel();
        stationViewModel stvm = new stationViewModel();
        discountViewModel dtvm = new discountViewModel();
        string DB_PATH = Path.Combine(ApplicationData.Current.LocalFolder.Path, "price.sqlite");
        bool connectivity = false;
        bool insequence = false;
        bool lastevent = true;
        uint seq = 1;
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
            await dbupdate();
            await tileupdate();
            deferral.Complete();
        }

        private async Task tileupdate()
        {
            Dictionary<priceStorage, bool> tileList = new Dictionary<priceStorage, bool>();
            var tiles = await SecondaryTile.FindAllAsync();
            foreach(SecondaryTile st in tiles)
            {
                IEnumerable<priceStorage> item = from p in cpvm.currentCollections where p.kind.ToString() == st.TileId select p; 
                if(item.Any())
                {
                    tileList.Add(item.First(), false);
                }
            }
            if(ifvm.defaultTile > 0)
            {
                IEnumerable<priceStorage> item = from p in cpvm.currentCollections where p.kind == ifvm.defaultTile select p;
                tileList.Add(item.First(), true);
            }
            foreach(KeyValuePair<priceStorage, bool> activeTile in tileList)
            {
                priceStorage ps = activeTile.Key;
                string price = Double.IsNaN(ppvm.pprice) ? "--.-" : ppvm.pprice.ToString();
                int itemid = ps.kind;
                string itemname = typeDB.productnameDB[ps.kind].name;
                string itemprice = "$" + ps.price;
                itemprice += ps.brand == 0 ? "(" + cpvm.CPCcurrentDate.ToString("MM/dd") + ")" : "(" + cpvm.FPCCcurrentDate.ToString("MM/dd") + ")";
                itemprice += "\r\n預計調整：" + price + "元(" + ppvm.penddate.ToString("MM/dd") + ")";
                string itemimg = ps.brand == 0 ? "CPC" : "FPCC";
                itemimg = "Assets/" + itemimg + ".png";
                tileUpdater.update(itemid, itemname, itemprice, itemimg, activeTile.Value);
            }
            if(tileList.Count > 0)
            {
                ifvm.tileupdateTime = DateTime.Now;
            }
        }

        private async Task dbupdate()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterType<infoModel>();
            builder.RegisterInstance<infoModel>(new infoModel());
            builder.RegisterType<infoViewModel>().PropertiesAutowired();
            builder.RegisterType<stationViewModel>().PropertiesAutowired();
            builder.RegisterType<cpViewModel>().PropertiesAutowired();
            builder.RegisterType<ppViewModel>().PropertiesAutowired();
            builder.RegisterType<discountViewModel>().PropertiesAutowired();
            builder.RegisterType<stationViewModel>().PropertiesAutowired();
            IContainer container = builder.Build();
            ppvm = container.Resolve<ppViewModel>();
            cpvm = container.Resolve<cpViewModel>();
            ifvm = container.Resolve<infoViewModel>();
            dtvm = container.Resolve<discountViewModel>();
            stvm = container.Resolve<stationViewModel>();
            connectivity = NetworkInterface.GetIsNetworkAvailable();
            //System.Diagnostics.Debug.WriteLine("MemoryUsage0:"+(Windows.Phone.System.Memory.MemoryManager.ProcessCommittedBytes/1024f/1024f)+"/"+ (Windows.Phone.System.Memory.MemoryManager.ProcessCommittedLimit/1024f/1024f));
            try
            {
                var progress = new PropertyProgress<ProgressReport>();
                var disprogress = new PropertyProgress<ProgressReport>();
                disprogress.PropertyChanged += Disprogress_PropertyChanged;
                var statprogress = new PropertyProgress<ProgressReport>();
                statprogress.PropertyChanged += Statprogress_PropertyChanged;
                long originalDate = cpvm.moeaboeDBdate.Ticks;
                await cpvm.loadDB(DB_PATH);
                await cpvm.buildDB();
                await ppvm.loadDB(DB_PATH);
                if ((int)DateTime.Today.DayOfWeek == 0)
                {
                    if (!ifvm.CPCnotified || !ifvm.FPCCnotified)
                    {
                        await cpvm.fetchPrice(connectivity, progress, 6);
                        await cpvm.currentPrice(progress, true);
                        ifvm.notifycheckedHour = DateTime.Now;
                        if (originalDate - cpvm.moeaboeDBdate.Ticks != 0)
                        {
                            insequence = true;
                            IEnumerable<priceStorage> product95 = from p in cpvm.currentCollections where p.kind == typeDB.CPC95.key select p;
                            IEnumerable<priceStorage> productdiesel = from p in cpvm.currentCollections where p.kind == typeDB.CPCdiesel.key select p;
                            //System.Diagnostics.Debug.WriteLine("MemoryUsage1:" + (Windows.Phone.System.Memory.MemoryManager.ProcessCommittedBytes / 1024f / 1024f) + "/" + (Windows.Phone.System.Memory.MemoryManager.ProcessCommittedLimit / 1024f / 1024f));
                            if (product95.Any())
                            {
                                await ppvm.predictedPrice(ifvm.connectivity, true, progress);
                                ppvm.getPrice(product95.First().price, productdiesel.First().price);
                            }
                            //System.Diagnostics.Debug.WriteLine("MemoryUsage2:" + (Windows.Phone.System.Memory.MemoryManager.ProcessCommittedBytes / 1024f / 1024f) + "/" + (Windows.Phone.System.Memory.MemoryManager.ProcessCommittedLimit / 1024f / 1024f));
                            IEnumerable<int> brands = cpvm.save.Select(p => p.brand).Distinct();
                            foreach (int b in brands)
                            {
                                string bname = b == 0 ? "中油" : "台塑";
                                string c95s;
                                string cdiesels;
                                switch (b)
                                {
                                    case 0:
                                        c95s = cpvm.CPC95Change > 0 ? "+" + cpvm.CPC95Change.ToString() : cpvm.CPC95Change.ToString();
                                        cdiesels = cpvm.CPCdieselChange > 0 ? "+" + cpvm.CPCdieselChange.ToString() : cpvm.CPCdieselChange.ToString();
                                        if (!ifvm.CPCnotified) sendNotification(bname + "公告，汽油" + c95s + "元，柴油" + cdiesels + "元", "油價公告", "MainPage");
                                        ifvm.CPCnotified = true;
                                        break;
                                    case 1:
                                        c95s = cpvm.FPCC95Change > 0 ? "+" + cpvm.FPCC95Change.ToString() : cpvm.FPCC95Change.ToString();
                                        cdiesels = cpvm.FPCCdieselChange > 0 ? "+" + cpvm.FPCCdieselChange.ToString() : cpvm.FPCCdieselChange.ToString();
                                        if (!ifvm.FPCCnotified) sendNotification(bname + "公告，汽油" + c95s + "元，柴油" + cdiesels + "元", "油價公告", "MainPage");
                                        ifvm.FPCCnotified = true;
                                        break;
                                }
                            }
                            GC.Collect();
                        }
                    }
                }
                else if ((int)DateTime.Now.DayOfWeek != 1)
                {
                    ifvm.notifycheckedHour = DateTime.Now;
                    ifvm.CPCnotified = false;
                    ifvm.FPCCnotified = false;
                    if (DateTime.Now.Day != ifvm.dailynotify.Day)
                    {
                        if (DateTime.Now.Hour >= ifvm.dailynotifytime.Hours)
                        {
                            if (!ifvm.dailynotified)
                            {
                                insequence = true;
                                ifvm.dailynotified = true;
                                ifvm.dailycheckHour = DateTime.Now;
                                connectivity = ifvm.dbcheckedDate.AddMinutes(5) < DateTime.Now ? connectivity : false;
                                await cpvm.fetchPrice(connectivity, progress, 6);
                                await cpvm.currentPrice(progress, true);
                                IEnumerable<priceStorage> product95 = from p in cpvm.currentCollections where p.kind == typeDB.CPC95.key select p;
                                IEnumerable<priceStorage> productdiesel = from p in cpvm.currentCollections where p.kind == typeDB.CPCdiesel.key select p;
                                //System.Diagnostics.Debug.WriteLine("MemoryUsage3:" + (Windows.Phone.System.Memory.MemoryManager.ProcessCommittedBytes / 1024f / 1024f) + "/" + (Windows.Phone.System.Memory.MemoryManager.ProcessCommittedLimit / 1024f / 1024f));
                                if (product95.Any())
                                {
                                    await ppvm.predictedPrice(ifvm.connectivity, true, progress);
                                    ppvm.getPrice(product95.First().price, productdiesel.First().price);
                                    //System.Diagnostics.Debug.WriteLine("MemoryUsage4:" + (Windows.Phone.System.Memory.MemoryManager.ProcessCommittedBytes / 1024f / 1024f) + "/" + (Windows.Phone.System.Memory.MemoryManager.ProcessCommittedLimit / 1024f / 1024f));
                                }
                                if (ifvm.dbcheckedDate.AddMinutes(5) < DateTime.Now) ifvm.dbcheckedDate = DateTime.Now;
                                if (ifvm.dailynotifyEnable)
                                {
                                    string pprice = ppvm.pprice > 0 ? "+" + ppvm.pprice.ToString() : ppvm.pprice.ToString();
                                    sendNotification(ppvm.predictpause ? "能源局尚未更新國際油價" : "下周預測將調整" + pprice + "元", "油價預測", "MainPage");
                                }
                                ifvm.dailynotify = DateTime.Now;
                                GC.Collect();
                            }
                        }
                    }
                }
                TimeSpan now = new TimeSpan(DateTime.Now.Ticks);
                TimeSpan dupdate = new TimeSpan(dtvm.dDBcheckedDate.Ticks);
                if (now.Subtract(dupdate).Days > 15)
                {
                    if (await dtvm.updateXML(progress))
                    {
                        sendProgress("更新加油折扣資料庫", "加油折扣資料庫更新", "台灣油價查詢");
                    }
                }
                if (!insequence)
                {
                    if (!lastevent)
                    {
                        if (Math.Abs(DateTime.Now.Subtract(ifvm.tileupdateTime).Hours) > 6)
                        {
                            await cpvm.currentPrice(progress, true);
                        }
                    }
                }
                TimeSpan supdate = new TimeSpan(stvm.stationDBnotifyDate.Ticks);
                ConnectionProfile InternetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();
                bool isWLANConnection = (InternetConnectionProfile == null) ? false : InternetConnectionProfile.IsWlanConnectionProfile;
                if (isWLANConnection)
                {
                    if (now.Subtract(dupdate).Days > 30)
                    {
                        await stvm.updateCPC(progress);
                        await stvm.updateFPCC(progress);
                        sendProgress("更新加油站資料庫", "加油站資料庫更新", "台灣油價查詢");
                    }
                }
                if (!insequence)
                {
                    if (!lastevent)
                    {
                        if (Math.Abs(DateTime.Now.Subtract(ifvm.tileupdateTime).Hours) > 6)
                        {
                            await cpvm.currentPrice(progress, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ifvm.scheduledTaskErrorcode = ex.Message;
                ifvm.scheduledTaskErrortime = DateTime.Now;
                ifvm.scheduledTaskErrortrace = "記憶體用量：" + (MemoryManager.AppMemoryUsage / 1024f / 1024f) + "/" + (MemoryManager.AppMemoryUsageLimit / 1024f / 1024f) + "\r" + ex.StackTrace;
                sendNotification("背景更新油價程式執行失敗，請進入主程式回報錯誤給作者", "錯誤通知", "MainPage");
            }
        }

        private void Statprogress_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            PropertyProgress<ProgressReport> obj = (PropertyProgress<ProgressReport>)sender;
            ProgressReport pr = obj.Progress;
            UpdateProgress("加油站資料庫更新", "台灣油價查詢", pr.progress.ToString(), pr.progressMessage, seq++);
        }

        private void Disprogress_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            PropertyProgress<ProgressReport> obj = (PropertyProgress<ProgressReport>)sender;
            ProgressReport pr = obj.Progress;
            UpdateProgress("更新加油折扣資料庫", "台灣油價查詢", pr.progress.ToString(), pr.progressMessage, seq++);
        }

        private void sendNotification(string msg, string type, string link)
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
                    Text = "台灣油價查詢"
                },
                new AdaptiveText()
                {
                    Text = msg
                }
            },
                        Attribution = new ToastGenericAttributionText()
                        {
                            Text = type
                        }
                    }
                },
                Launch = "action=/"+link
            };

            // Create the toast notification
            var toastNotif = new ToastNotification(toastContent.GetXml());

            // And send the notification
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
        }

        private void sendProgress(string msg, string tag, string group)
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
                    Text = msg
                },
                new AdaptiveProgressBar()
                {
                    Value = new BindableProgressBarValue("progressValue"),
                    ValueStringOverride = new BindableString("progressValueString"),
                    Title = new BindableString("progressTitle"),
                    Status = new BindableString("progressStatus")
                }
            }
                    }
                },
                Launch = "action=MainPage"
            };

            // Create the toast notification
            var toastNotif = new ToastNotification(toastContent.GetXml());
            toastNotif.Tag = tag;
            toastNotif.Group = group;

            // And send the notification
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
        }

        public void UpdateProgress(string tag, string group, string precent, string msg, uint seq)
        {
            // Construct a NotificationData object;
            string toastTag = tag;
            string toastGroup = group;

            // Create NotificationData with new values;
            // Make sure that sequence number is incremented since last update, or assign with value 0 for updating regardless of order;
            var data = new NotificationData { SequenceNumber = seq };
            data.Values["progressValue"] = precent;
            data.Values["progressStatus"] = msg;

            // Updating a previously sent toast with tag, group, and new data;
            NotificationUpdateResult updateResult = ToastNotificationManager.CreateToastNotifier().Update(data, tag, group);
        }
    }
}
