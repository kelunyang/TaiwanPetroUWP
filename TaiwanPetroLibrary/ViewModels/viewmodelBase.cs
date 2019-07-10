using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using TaiwanPetroLibrary.Helpers;
using TaiwanPetroLibrary.Models;
using System.Threading.Tasks;
using SQLitePCL;
using SQLite;
using System.IO;
using System.Windows.Input;
using System.Xml;
using System.Collections.Specialized;

namespace TaiwanPetroLibrary.ViewModels
{
    public abstract class viewmodelBase : INotifyPropertyChanged
    {
        protected SQLiteAsyncConnection dbConn;
        protected infoModel _im;
        public virtual event PropertyChangedEventHandler PropertyChanged = delegate { };
        public infoModel im {
            set
            {
                _im = value;
            }
            get
            {
                return _im;
            }
        }
        public virtual async Task loadDB(string dbPath)
        {
            try
            {
                //var connectionFactory = new Func<SQLiteConnectionWithLock>(() => new SQLiteConnectionWithLock(platform, new SQLiteConnectionString(dbPath, storeDateTimeAsTicks: false)));
                dbConn = new SQLiteAsyncConnection(dbPath);
            }
            catch
            {
                throw new dbException("開啟資料庫");
            }
        }
        protected viewmodelBase() {  }
        public bool _progressVis;
        public bool progressVis
        {
            set
            {
                _progressVis = value;
                NotifyPropertyChanged();
            }
            get { return _progressVis; }
        }
        public double _progressVal;
        public double progressVal {
            set
            {
                _progressVal = value;
                NotifyPropertyChanged();
            }
            get
            {
                return _progressVal;
            }
        }
        public string _progresMsg;
        public string progressMsg
        {
            set
            {
                _progresMsg = value;
                NotifyPropertyChanged();
            }
            get { return _progresMsg; }
        }
        /*public virtual void saveConfig(Stream sw)
        {
            im.save(sw);
            sw.Dispose();
        }*/
        protected virtual void NotifyPropertyChanged(string caller = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(caller));
            }
        }
    }
}
