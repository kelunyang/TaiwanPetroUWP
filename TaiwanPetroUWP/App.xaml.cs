using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
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
using Plugin.Settings.Abstractions;
using Plugin.Settings;

namespace TaiwanPetroUWP
{
    /// <summary>
    /// 提供應用程式專屬行為以補充預設的應用程式類別。
    /// </summary>
    sealed partial class App : Application
    {
        TaiwanPetroLibrary.Models.infoModel im;
        bool upgraded = false;
        public bool applaunch { set; get; } //這是global變數，跨頁存取
        public bool firstload { set; get; }
        private static ISettings AppSettings => CrossSettings.Current;
        /// <summary>
        /// 初始化單一應用程式物件。這是第一行執行之撰寫程式碼，
        /// 而且其邏輯相當於 main() 或 WinMain()。
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            im = new TaiwanPetroLibrary.Models.infoModel();
        }

        /// <summary>
        /// 在應用程式由終端使用者正常啟動時叫用。當啟動應用
        /// 將在例如啟動應用程式時使用以開啟特定檔案。
        /// </summary>
        /// <param name="e">關於啟動要求和處理序的詳細資料。</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            bool copyDB = AppSettings.GetValueOrDefault("resetDB", false);
            bool copyConfig = AppSettings.GetValueOrDefault("copyConfig", false);
            List<TaiwanPetroLibrary.Models.configSet> updatedList = im.configList;
            if (new bool[] { copyDB, copyConfig }.Contains(true))
            {
                updatedList = im.configList;
            } else { 
                if (AppSettings.Contains("version"))
                {
                    if (AppSettings.GetValueOrDefault("version", DateTime.Now) != im.appVersion)
                    {
                        updatedList = (from cs in im.configList where cs.version != AppSettings.GetValueOrDefault("version", DateTime.Now) select cs).ToList();
                        copyConfig = true;
                    }
                } else
                {
                    copyConfig = true;
                }
                var item = await ApplicationData.Current.LocalFolder.TryGetItemAsync("price.sqlite");
                if (item == null)
                {
                    copyDB = true;
                }
            }
            if(copyConfig)
            {
                foreach (TaiwanPetroLibrary.Models.configSet cs in updatedList)
                {
                    cs.exeValue();
                }
                AppSettings.AddOrUpdateValue("copyConfig", false);
                copyDB = true;
            }
            if(copyDB)
            {
                await moveFile("price.sqlite");
                AppSettings.AddOrUpdateValue("resetDB", false);
            }
            /*applaunch = true;
            if (await configCheck("config.xml"))
            {
                moveXML("config.xml");
                //moveTextStream("config.xml"); //不需要這一段，XML還得修改其設定才能存檔
                moveFile("creditDiscount.xml");
            }*/
            Frame rootFrame = Window.Current.Content as Frame;

            // 當視窗中已有內容時，不重複應用程式初始化，
            // 只確定視窗是作用中
            if (rootFrame == null)
            {
                // 建立框架做為巡覽內容，並巡覽至第一頁
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: 從之前暫停的應用程式載入狀態
                }

                // 將框架放在目前視窗中
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // 在巡覽堆疊未還原時，巡覽至第一頁，
                    // 設定新的頁面，方式是透過傳遞必要資訊做為巡覽
                    // 參數
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // 確定目前視窗是作用中
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// 在巡覽至某頁面失敗時叫用
        /// </summary>
        /// <param name="sender">導致巡覽失敗的框架</param>
        /// <param name="e">有關巡覽失敗的詳細資料</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// 在應用程式暫停執行時叫用。應用程式狀態會儲存起來，
        /// 但不知道應用程式即將結束或繼續，而且仍將記憶體
        /// 的內容保持不變。
        /// </summary>
        /// <param name="sender">暫停之要求的來源。</param>
        /// <param name="e">有關暫停之要求的詳細資料。</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: 儲存應用程式狀態，並停止任何背景活動
            deferral.Complete();
        }

        private async Task<bool> configCheck(string filename)
        {
            //await storfile.CopyAsync(ApplicationData.Current.LocalFolder);
            if (await ApplicationData.Current.LocalFolder.TryGetItemAsync(filename) == null)
            {
                firstload = true;
                return true;
            } else
            {
                XDocument isoxd;
                XDocument assxd;
                assxd = XDocument.Load(await (await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/" + filename))).OpenStreamForReadAsync());
                isoxd = XDocument.Load(await (await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appdata:///local/" + filename))).OpenStreamForReadAsync());
                firstload = isoxd.Element("appconfig").Element("info").Element("sysinfo").Attribute("firstload").Value == "1" ? true : false;
                if (isoxd.Element("appconfig").Attribute("version") == null)
                {
                    upgraded = true;
                    return true;
                }
                else
                {
                    long oldversion = Convert.ToInt64(isoxd.Element("appconfig").Attribute("version").Value);
                    long newversion = Convert.ToInt64(assxd.Element("appconfig").Attribute("version").Value);
                    if (oldversion != newversion)
                    {
                        upgraded = true;
                        return true;
                    }
                }
                return false;
            }
            //var testfile = (await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appdata:///Assets/"+filename))).IsAvailable;
            //if((await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appdata:///Assets/" + filename))).IsAvailable)
            //if(await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/"+filename)))
            /*if (!IsolatedStorageFile.GetUserStoreForApplication().FileExists(filename))
            {
                firstload = true;
                return true;
            }
            else
            {
                IsolatedStorageFile isFile = IsolatedStorageFile.GetUserStoreForApplication();
                string assetfilename = @"Assets\" + filename;
                XDocument isoxd;
                XDocument assxd;
                using (IsolatedStorageFileStream isost = new IsolatedStorageFileStream(filename, FileMode.Open, isFile))
                {
                    isoxd = XDocument.Load(isost);
                    isost.Dispose();
                }
                firstload = isoxd.Element("appconfig").Element("info").Element("sysinfo").Attribute("firstload").Value == "1" ? true : false;
                StreamResourceInfo streamInfo = Application.GetResourceStream(new Uri(assetfilename, UriKind.Relative));
                assxd = XDocument.Load(streamInfo.Stream);
                streamInfo.Stream.Close();
                if (isoxd.Element("appconfig").Attribute("version") == null)
                {
                    upgraded = true;
                    return true;
                }
                else
                {
                    long oldversion = Convert.ToInt64(isoxd.Element("appconfig").Attribute("version").Value);
                    long newversion = Convert.ToInt64(assxd.Element("appconfig").Attribute("version").Value);
                    if (oldversion != newversion)
                    {
                        upgraded = true;
                        return true;
                    }
                }
            }
            return false;*/
        }

        private async Task moveFile(string filename)
        {
            var assfile = await(await(await Package.Current.InstalledLocation.GetFolderAsync("Assets")).GetFileAsync(filename)).OpenStreamForReadAsync();
            var newfile = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            using (Stream writeStream = await newfile.OpenStreamForWriteAsync())
            {
                await assfile.CopyToAsync(writeStream);
            }
        }
        /*private void moveTextStream(string filename)
        {
            string assetfilename = @"Assets\" + filename;
            StreamResourceInfo streamInfo = Application.GetResourceStream(new Uri(assetfilename, UriKind.Relative));
            using (IsolatedStorageFile isFile = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream fileStream = new IsolatedStorageFileStream(filename, FileMode.Create, isFile))
                {
                    using (StreamWriter writer = new StreamWriter(fileStream))
                    {
                        using (StreamReader reader = new StreamReader(streamInfo.Stream))
                        {
                            writer.WriteLine(reader.ReadToEnd());
                            writer.Dispose();
                        }
                    }
                }
            }
        }*/

        /*private async void moveXML(string filename)
        {
            /*string assetfilename = @"Assets\" + filename;
            StreamResourceInfo streamInfo = Application.GetResourceStream(new Uri(assetfilename, UriKind.Relative));
            XDocument assxd = XDocument.Load(await (await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/" + filename))).OpenStreamForReadAsync());
            //streamInfo.Stream.Close();
            assxd.Element("appconfig").Element("info").Element("sysinfo").Attribute("upgrade").Value = upgraded ? "1" : "0";
            assxd.Element("appconfig").Element("info").Element("sysinfo").Attribute("firstload").Value = !upgraded ? "1" : "0";
            var isofile = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename);
            assxd.Save(await isofile.OpenStreamForWriteAsync());
        }*/

    }
}
