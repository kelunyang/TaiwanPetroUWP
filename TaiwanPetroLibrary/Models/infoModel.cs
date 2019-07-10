using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Xml;
//using System.Xml.Linq;
using TaiwanPetroLibrary.Helpers;
using PCLWebUtility;
using Plugin.Settings.Abstractions;
using Plugin.Settings;
using Newtonsoft.Json;
using System.IO.Compression;

namespace TaiwanPetroLibrary.Models
{
    public class infoModel : INotifyPropertyChanged
    {
        private static ISettings AppSettings => CrossSettings.Current;
        private object xmllock = new object();
        private long appDate = 636807798000000000;
        public DateTime appVersion ;
        public List<configSet> configList;
        //XDocument config = new XDocument();
        public infoModel() 
        {
            appVersion = DateTime.SpecifyKind(new DateTime(appDate).AddHours(-8), DateTimeKind.Utc);
            configList = new List<configSet>() {
                new configSet("version", new DateTime(appDate)),
                new configSet("firstLoad", new DateTime(appDate)),
                new configSet("upgrade", new DateTime(appDate)),
                new configSet("scheduledTaskErrorcode", new DateTime(appDate)),
                new configSet("scheduledTaskErrortime", new DateTime(appDate)),
                new configSet("scheduledTaskErrortrace", new DateTime(appDate)),
                new configSet("ppause", new DateTime(appDate)),
                new configSet("pstartWeek", new DateTime(appDate)),
                new configSet("pendWeek", new DateTime(appDate)),
                new configSet("prunDay", new DateTime(appDate)),
                new configSet("predictgasPrice", new DateTime(appDate)),
                new configSet("predictdieselPrice", new DateTime(appDate)),
                new configSet("priceDBdate", new DateTime(appDate)),
                new configSet("stationDBdate", new DateTime(appDate)),
                new configSet("stationDBnotifydate", new DateTime(appDate)),
                new configSet("savetime", new DateTime(appDate)),
                new configSet("autoupdate", new DateTime(appDate)),
                new configSet("soapupdate", new DateTime(appDate)),
                new configSet("runPredict", new DateTime(appDate)),
                new configSet("stationBehavior", new DateTime(appDate)),
                new configSet("updatefreq", new DateTime(appDate)),
                new configSet("dailynotifytime", new DateTime(appDate)),
                new configSet("cpcdefaultProduct", new DateTime(appDate)),
                new configSet("fpccdefaultProduct", new DateTime(appDate)),
                new configSet("sfilterFPCC", new DateTime(appDate)),
                new configSet("sfilterInservice", new DateTime(appDate)),
                new configSet("sfiltercountryEnable", new DateTime(appDate)),
                new configSet("sfiltercountry", new DateTime(appDate)),
                new configSet("sfilterCPC", new DateTime(appDate)),
                new configSet("sfilterDirect", new DateTime(appDate)),
                new configSet("sfilterSelf", new DateTime(appDate)),
                new configSet("sfilterFavoirte", new DateTime(appDate)),
                new configSet("sfilterKilonmeter", new DateTime(appDate)),
                new configSet("sfilterp92", new DateTime(appDate)),
                new configSet("sfilterp95", new DateTime(appDate)),
                new configSet("sfilterp98", new DateTime(appDate)),
                new configSet("sfilterpdiesel", new DateTime(appDate)),
                new configSet("sfilterpgasohol", new DateTime(appDate)),
                new configSet("defaultTile", new DateTime(appDate)),
                new configSet("tileupdateTime", new DateTime(appDate)),
                new configSet("dailynotify", new DateTime(appDate)),
                new configSet("CPCnotified", new DateTime(appDate)),
                new configSet("FPCCnotified", new DateTime(appDate)),
                new configSet("dailynotified", new DateTime(appDate)),
                new configSet("dailynotifyEnable", new DateTime(appDate)),
                new configSet("notifycheckedHour", new DateTime(appDate)),
                new configSet("dailynotifiedHour", new DateTime(appDate)),
                new configSet("defaultPage", new DateTime(appDate)),
                new configSet("DBcheckedDate", new DateTime(appDate)),
                new configSet("sfilterrangeLocation", new DateTime(appDate)),
                new configSet("sfilterrangeLocationname", new DateTime(appDate)),
                new configSet("sfiltercustomLocation", new DateTime(appDate)),
                new configSet("productMonitor", new DateTime(appDate)),
                new configSet("sfilterSubBrands", new DateTime(appDate)),
                new configSet("moeaboeDBdate", new DateTime(appDate)),
                new configSet("discountRev", new DateTime(appDate)),
                new configSet("dDBupdateDate", new DateTime(appDate)),
                new configSet("creditXML", new DateTime(appDate)),
                new configSet("resetDB", new DateTime(appDate)),
                new configSet("copyConfig", new DateTime(appDate))
            };
        }
        /*public void load(Stream str)
        {
            try
            {
                lock (xmllock)
                {
                    config = XDocument.Load(str);
                }
            }
            catch
            {
                throw new xmlException();
            }
        }*/
        /*public void save(Stream str)
        {
            try
            {
                lock (xmllock)
                {
                    config.Save(str);
                }
            }
            catch
            {
                throw new xmlException();
            }
        }*/
        public string export()
        {
            string config = "";
            foreach(configSet cs in configList)
            {
                config += cs.key + ":"+ AppSettings.GetValueOrDefault(cs.key,string.Empty).ToString();
            }
            return config;
        }
        public bool resetDB
        {
            get => AppSettings.GetValueOrDefault(nameof(resetDB), true);
            set => AppSettings.AddOrUpdateValue(nameof(resetDB), value);
            /*get
            {
                return config.Element("appconfig").Element("info").Element("sysinfo").Attribute("firstload").Value == "1" ? true : false;
            }
            set
            {
                config.Element("appconfig").Element("info").Element("sysinfo").Attribute("firstload").Value = value ? "1" : "0";
            }*/
        }
        public bool copyConfig
        {
            get => AppSettings.GetValueOrDefault(nameof(copyConfig), true);
            set => AppSettings.AddOrUpdateValue(nameof(copyConfig), value);
            /*get
            {
                return config.Element("appconfig").Element("info").Element("sysinfo").Attribute("firstload").Value == "1" ? true : false;
            }
            set
            {
                config.Element("appconfig").Element("info").Element("sysinfo").Attribute("firstload").Value = value ? "1" : "0";
            }*/
        }
        public bool firstLoad {
            get => AppSettings.GetValueOrDefault(nameof(firstLoad), true);
            set => AppSettings.AddOrUpdateValue(nameof(firstLoad), value);
            /*get
            {
                return config.Element("appconfig").Element("info").Element("sysinfo").Attribute("firstload").Value == "1" ? true : false;
            }
            set
            {
                config.Element("appconfig").Element("info").Element("sysinfo").Attribute("firstload").Value = value ? "1" : "0";
            }*/
        }
        public bool upgrade
        {
            get => AppSettings.GetValueOrDefault(nameof(upgrade), false);
            set => AppSettings.AddOrUpdateValue(nameof(upgrade), value);
            /*get
            {
                return config.Element("appconfig").Element("info").Element("sysinfo").Attribute("upgrade").Value == "1" ? true : false;
            }
            set
            {
                config.Element("appconfig").Element("info").Element("sysinfo").Attribute("upgrade").Value = value ? "1" : "0";
            }*/
        }
        public string scheduledTaskErrorcode
        {
            get => AppSettings.GetValueOrDefault(nameof(scheduledTaskErrorcode), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(scheduledTaskErrorcode), value);
            /*get
            {
                return WebUtility.UrlDecode(config.Element("appconfig").Element("info").Element("scheduledTaskError").Attribute("lasterror").Value);
            }
            set
            {
                config.Element("appconfig").Element("info").Element("scheduledTaskError").Attribute("lasterror").Value = WebUtility.UrlEncode(value);
            }*/
        }
        public DateTime scheduledTaskErrortime
        {
            get => AppSettings.GetValueOrDefault(nameof(scheduledTaskErrortime), DateTime.MinValue);
            set => AppSettings.AddOrUpdateValue(nameof(scheduledTaskErrortime), value);
            /*get
            {
                return new DateTime(Convert.ToInt64(config.Element("appconfig").Element("info").Element("scheduledTaskError").Attribute("timestamp").Value));
            }
            set
            {
                config.Element("appconfig").Element("info").Element("scheduledTaskError").Attribute("timestamp").Value = value.Ticks.ToString();
            }*/
        }
        public string scheduledTaskErrortrace   //encoded by urlencode
        {
            get => AppSettings.GetValueOrDefault(nameof(scheduledTaskErrortrace), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(scheduledTaskErrortrace), value);
            /*get
            {
                return WebUtility.UrlDecode(config.Element("appconfig").Element("info").Element("scheduledTaskError").Value);
            }
            set
            {
                config.Element("appconfig").Element("info").Element("scheduledTaskError").Value = WebUtility.UrlEncode(value);
            }*/
        }
        public bool ppause
        {
            get => AppSettings.GetValueOrDefault(nameof(ppause), false);
            set => AppSettings.AddOrUpdateValue(nameof(ppause), value);
            /*get
            {
                return config.Element("appconfig").Element("predictPrice").Attribute("pause").Value == "1" ? true : false;
            }
            set
            {
                config.Element("appconfig").Element("predictPrice").Attribute("pause").Value = value ? "1" : "0";
            }*/
        }
        public DateTime pstartWeek
        {
            get => AppSettings.GetValueOrDefault(nameof(pstartWeek), DateTime.MinValue);
            set => AppSettings.AddOrUpdateValue(nameof(pstartWeek), value);
            /*get
            {
                return new DateTime(Convert.ToInt64(config.Element("appconfig").Element("predictPrice").Element("duration").Attribute("start").Value));
            }
            set
            {
                config.Element("appconfig").Element("predictPrice").Element("duration").Attribute("start").Value = value.Ticks.ToString();
            }*/
        }
        public DateTime pendWeek
        {
            get => AppSettings.GetValueOrDefault(nameof(pendWeek), DateTime.Now);
            set => AppSettings.AddOrUpdateValue(nameof(pendWeek), value);
            /*get
            {
                return new DateTime(Convert.ToInt64(config.Element("appconfig").Element("predictPrice").Element("duration").Attribute("end").Value));
            }
            set
            {
                config.Element("appconfig").Element("predictPrice").Element("duration").Attribute("end").Value = value.Ticks.ToString();
            }*/
        }
        public DateTime prunDay
        {
            get => AppSettings.GetValueOrDefault(nameof(prunDay), DateTime.Now);
            set => AppSettings.AddOrUpdateValue(nameof(prunDay), value);
            /*get
            {
                return new DateTime(Convert.ToInt64(config.Element("appconfig").Element("predictPrice").Element("duration").Attribute("runday").Value));
            }
            set
            {
                config.Element("appconfig").Element("predictPrice").Element("duration").Attribute("runday").Value = value.Ticks.ToString();
            }*/
        }
        public double predictgasPrice
        {
            get => AppSettings.GetValueOrDefault(nameof(predictgasPrice), 0.0);
            set => AppSettings.AddOrUpdateValue(nameof(predictgasPrice), value);
            /*get
            {
                double r = config.Element("appconfig").Element("predictPrice").Element("gasprice").Value == "" ? double.NaN : Convert.ToDouble(config.Element("appconfig").Element("predictPrice").Element("gasprice").Value);
                return r;
            }
            set
            {
                string v = double.IsNaN(value) ? "" : value.ToString();
                config.Element("appconfig").Element("predictPrice").Element("gasprice").Value = v;
            }*/
        }
        public double predictdieselPrice
        {
            get => AppSettings.GetValueOrDefault(nameof(predictdieselPrice), 0.0);
            set => AppSettings.AddOrUpdateValue(nameof(predictdieselPrice), value);
            /*get
            {
                double r = config.Element("appconfig").Element("predictPrice").Element("dieselprice").Value == "" ? double.NaN : Convert.ToDouble(config.Element("appconfig").Element("predictPrice").Element("dieselprice").Value);
                return r;
            }
            set
            {
                string v = double.IsNaN(value) ? "" : value.ToString();
                config.Element("appconfig").Element("predictPrice").Element("dieselprice").Value = v;
            }*/
        }
        public DateTime priceDBdate {
            get => AppSettings.GetValueOrDefault(nameof(priceDBdate), DateTime.MinValue);
            set => AppSettings.AddOrUpdateValue(nameof(priceDBdate), value);
            /*get
            {
                return new DateTime(Convert.ToInt64(config.Element("appconfig").Element("info").Element("updateDate").Attribute("priceDB").Value));
            }
            set
            {
                config.Element("appconfig").Element("info").Element("updateDate").Attribute("priceDB").Value = value.Ticks.ToString();
            }*/
        }
        public DateTime stationDBdate
        {
            get => AppSettings.GetValueOrDefault(nameof(stationDBdate), DateTime.Now);
            set => AppSettings.AddOrUpdateValue(nameof(stationDBdate), value);
            /*get
            {
                return new DateTime(Convert.ToInt64(config.Element("appconfig").Element("info").Element("updateDate").Attribute("stationDB").Value));
            }
            set
            {
                config.Element("appconfig").Element("info").Element("updateDate").Attribute("stationDB").Value = value.Ticks.ToString();
            }*/
        }
        public DateTime stationDBnotifydate
        {
            get => AppSettings.GetValueOrDefault(nameof(stationDBnotifydate), DateTime.Now);
            set => AppSettings.AddOrUpdateValue(nameof(stationDBnotifydate), value);
            /*get
            {
                return new DateTime(Convert.ToInt64(config.Element("appconfig").Element("info").Element("updateDate").Attribute("sDBnotifyDate").Value));
            }
            set
            {
                config.Element("appconfig").Element("info").Element("updateDate").Attribute("sDBnotifyDate").Value = value.Ticks.ToString();
            }*/
        }
        public bool savetime {
            get => AppSettings.GetValueOrDefault(nameof(savetime), false);
            set => AppSettings.AddOrUpdateValue(nameof(savetime), value);
            /*get
            {
                return config.Element("appconfig").Element("settings").Element("timing").Attribute("saveTime").Value == "1" ? true : false;
            }
            set
            {
                config.Element("appconfig").Element("settings").Element("timing").Attribute("saveTime").Value = value ? "1" : "0";
            }*/
        }
        public bool autoupdate
        {
            get => AppSettings.GetValueOrDefault(nameof(autoupdate), false);
            set => AppSettings.AddOrUpdateValue(nameof(autoupdate), value);
            /*get
            {
                return config.Element("appconfig").Element("settings").Element("autoUpdate").Attribute("enable").Value == "1" ? true : false;
            }
            set
            {
                config.Element("appconfig").Element("settings").Element("autoUpdate").Attribute("enable").Value = value ? "1" : "0";
            }*/
        }
        public bool soapupdate
        {
            get => AppSettings.GetValueOrDefault(nameof(soapupdate), false);
            set => AppSettings.AddOrUpdateValue(nameof(soapupdate), value);
            /*get
            {
                return config.Element("appconfig").Element("settings").Element("soapUpdate").Attribute("enable").Value == "1" ? true : false;
            }
            set
            {
                config.Element("appconfig").Element("settings").Element("soapUpdate").Attribute("enable").Value = value ? "1" : "0";
            }*/
        }
        public bool runPredict
        {
            get => AppSettings.GetValueOrDefault(nameof(runPredict), true);
            set => AppSettings.AddOrUpdateValue(nameof(runPredict), value);
            /*get
            {
                return config.Element("appconfig").Element("settings").Element("runPredict").Attribute("enable").Value == "1" ? true : false;
            }
            set
            {
                config.Element("appconfig").Element("settings").Element("runPredict").Attribute("enable").Value = value ? "1" : "0";
            }*/
        }
        public bool stationBehavior
        {
            get => AppSettings.GetValueOrDefault(nameof(stationBehavior), false);
            set => AppSettings.AddOrUpdateValue(nameof(stationBehavior), value);
            /*get
            {
                return config.Element("appconfig").Element("settings").Element("stationBehavior").Attribute("behavior").Value == "1" ? true : false;
            }
            set
            {
                config.Element("appconfig").Element("settings").Element("stationBehavior").Attribute("behavior").Value = value ? "1" : "0";
            }*/
        }
        public bool updatefreq {
            get => AppSettings.GetValueOrDefault(nameof(updatefreq), false);
            set => AppSettings.AddOrUpdateValue(nameof(updatefreq), value);
            /*get
            {
                return config.Element("appconfig").Element("settings").Element("timing").Attribute("updateFreq").Value == "1" ? true : false;
            }
            set
            {
                config.Element("appconfig").Element("settings").Element("timing").Attribute("updateFreq").Value = value ? "1" : "0";
            }*/
        }
        public long dailynotifytime
        {
            get => AppSettings.GetValueOrDefault(nameof(dailynotifytime), 360000000000);
            set => AppSettings.AddOrUpdateValue(nameof(dailynotifytime), value);
            /*get
            {
                return new DateTime(Convert.ToInt64(config.Element("appconfig").Element("settings").Element("timing").Attribute("dailynotifytime").Value));
            }
            set
            {
                config.Element("appconfig").Element("settings").Element("timing").Attribute("dailynotifytime").Value = value.Ticks.ToString();
            }*/
        }
        public int cpcdefaultProduct
        {
            get => AppSettings.GetValueOrDefault(nameof(cpcdefaultProduct), 0);
            set => AppSettings.AddOrUpdateValue(nameof(cpcdefaultProduct), value);
            /*get
            {
                return Convert.ToInt32(config.Element("appconfig").Element("settings").Element("defaultProduct").Attribute("CPC").Value);
            }
            set
            {
                config.Element("appconfig").Element("settings").Element("defaultProduct").Attribute("CPC").Value = value.ToString();
            }*/
        }
        public int fpccdefaultProduct
        {
            get => AppSettings.GetValueOrDefault(nameof(fpccdefaultProduct), 0);
            set => AppSettings.AddOrUpdateValue(nameof(fpccdefaultProduct), value);
            /*get
            {
                return Convert.ToInt32(config.Element("appconfig").Element("settings").Element("defaultProduct").Attribute("FPCC").Value);
            }
            set
            {
                config.Element("appconfig").Element("settings").Element("defaultProduct").Attribute("FPCC").Value = value.ToString();
            }*/
        }
        public bool sfilterFPCC
        {
            get => AppSettings.GetValueOrDefault(nameof(sfilterFPCC), false);
            set => AppSettings.AddOrUpdateValue(nameof(sfilterFPCC), value);
            /*get
            {
                return config.Element("appconfig").Element("settings").Element("stationFilters").Element("FPCC").Attribute("filter").Value == "1" ? true : false;
            }
            set
            {
                config.Element("appconfig").Element("settings").Element("stationFilters").Element("FPCC").Attribute("filter").Value = value ? "1" : "0";
            }*/
        }
        public bool sfilterInservice
        {
            get => AppSettings.GetValueOrDefault(nameof(sfilterInservice), false);
            set => AppSettings.AddOrUpdateValue(nameof(sfilterInservice), value);
            /*get
            {
                return config.Element("appconfig").Element("settings").Element("stationFilters").Element("Inservice").Attribute("filter").Value == "1" ? true : false;
            }
            set
            {
                config.Element("appconfig").Element("settings").Element("stationFilters").Element("Inservice").Attribute("filter").Value = value ? "1" : "0";
            }*/
        }
        public bool sfiltercountryEnable
        {
            get => AppSettings.GetValueOrDefault(nameof(sfiltercountryEnable), false);
            set => AppSettings.AddOrUpdateValue(nameof(sfiltercountryEnable), value);
            /*get
            {
                return config.Element("appconfig").Element("settings").Element("stationFilters").Element("Country").Attribute("filter").Value == "1" ? true : false;
            }
            set
            {
                config.Element("appconfig").Element("settings").Element("stationFilters").Element("Country").Attribute("filter").Value = value ? "1" : "0";
            }*/
        }
        public int sfiltercountry
        {
            get => AppSettings.GetValueOrDefault(nameof(sfiltercountry), 0);
            set => AppSettings.AddOrUpdateValue(nameof(sfiltercountry), value);
            /*get
            {
                return Convert.ToInt32(config.Element("appconfig").Element("settings").Element("stationFilters").Element("Country").Value);
            }
            set
            {
                config.Element("appconfig").Element("settings").Element("stationFilters").Element("Country").Value = value.ToString();
            }*/
        }
        public bool sfilterCPC
        {
            get => AppSettings.GetValueOrDefault(nameof(sfilterCPC), false);
            set => AppSettings.AddOrUpdateValue(nameof(sfilterCPC), value);
            /*get
            {
                return config.Element("appconfig").Element("settings").Element("stationFilters").Element("CPC").Attribute("filter").Value == "1" ? true : false;
            }
            set
            {
                config.Element("appconfig").Element("settings").Element("stationFilters").Element("CPC").Attribute("filter").Value = value ? "1" : "0";
            }*/
        }
        public bool sfilterDirect
        {
            get => AppSettings.GetValueOrDefault(nameof(sfilterDirect), false);
            set => AppSettings.AddOrUpdateValue(nameof(sfilterDirect), value);
            /*get
            {
                return config.Element("appconfig").Element("settings").Element("stationFilters").Element("Direct").Attribute("filter").Value == "1" ? true : false;
            }
            set
            {
                config.Element("appconfig").Element("settings").Element("stationFilters").Element("Direct").Attribute("filter").Value = value ? "1" : "0";
            }*/
        }
        public bool sfilterSelf
        {
            get => AppSettings.GetValueOrDefault(nameof(sfilterSelf), false);
            set => AppSettings.AddOrUpdateValue(nameof(sfilterSelf), value);
            /*get
            {
                return config.Element("appconfig").Element("settings").Element("stationFilters").Element("Self").Attribute("filter").Value == "1" ? true : false;
            }
            set
            {
                config.Element("appconfig").Element("settings").Element("stationFilters").Element("Self").Attribute("filter").Value = value ? "1" : "0";
            }*/
        }
        public bool sfilterFavoirte
        {
            get => AppSettings.GetValueOrDefault(nameof(sfilterFavoirte), false);
            set => AppSettings.AddOrUpdateValue(nameof(sfilterFavoirte), value);
            /*get
            {
                return config.Element("appconfig").Element("settings").Element("stationFilters").Element("Favorite").Attribute("filter").Value == "1" ? true : false;
            }
            set
            {
                config.Element("appconfig").Element("settings").Element("stationFilters").Element("Favorite").Attribute("filter").Value = value ? "1" : "0";
            }*/
        }
        public double sfilterKilonmeter
        {
            get => AppSettings.GetValueOrDefault(nameof(sfilterKilonmeter), 5.0);
            set => AppSettings.AddOrUpdateValue(nameof(sfilterKilonmeter), value);
            /*get
            {
                return Convert.ToDouble(config.Element("appconfig").Element("settings").Element("stationFilters").Element("Kilometer").Value);
            }
            set
            {
                config.Element("appconfig").Element("settings").Element("stationFilters").Element("Kilometer").Value = value.ToString();
            }*/
        }
        public bool sfilterp92
        {
            get => AppSettings.GetValueOrDefault(nameof(sfilterp92), false);
            set => AppSettings.AddOrUpdateValue(nameof(sfilterp92), value);
            /*get
            {
                return config.Element("appconfig").Element("settings").Element("stationFilters").Element("Product").Attribute("p92").Value == "1" ? true : false;
            }
            set
            {
                config.Element("appconfig").Element("settings").Element("stationFilters").Element("Product").Attribute("p92").Value = value ? "1" : "0";
            }*/
        }
        public bool sfilterp95
        {
            get => AppSettings.GetValueOrDefault(nameof(sfilterp95), false);
            set => AppSettings.AddOrUpdateValue(nameof(sfilterp95), value);
            /*get
            {
                return config.Element("appconfig").Element("settings").Element("stationFilters").Element("Product").Attribute("p95").Value == "1" ? true : false;
            }
            set
            {
                config.Element("appconfig").Element("settings").Element("stationFilters").Element("Product").Attribute("p95").Value = value ? "1" : "0";
            }*/
        }
        public bool sfilterp98
        {
            get => AppSettings.GetValueOrDefault(nameof(sfilterp98), false);
            set => AppSettings.AddOrUpdateValue(nameof(sfilterp98), value);
            /*get
            {
                return config.Element("appconfig").Element("settings").Element("stationFilters").Element("Product").Attribute("p98").Value == "1" ? true : false;
            }
            set
            {
                config.Element("appconfig").Element("settings").Element("stationFilters").Element("Product").Attribute("p98").Value = value ? "1" : "0";
            }*/
        }
        public bool sfilterpdiesel
        {
            get => AppSettings.GetValueOrDefault(nameof(sfilterpdiesel), false);
            set => AppSettings.AddOrUpdateValue(nameof(sfilterpdiesel), value);
            /*get
            {
                return config.Element("appconfig").Element("settings").Element("stationFilters").Element("Product").Attribute("pdiesel").Value == "1" ? true : false;
            }
            set
            {
                config.Element("appconfig").Element("settings").Element("stationFilters").Element("Product").Attribute("pdiesel").Value = value ? "1" : "0";
            }*/
        }
        public bool sfilterpgasohol
        {
            get => AppSettings.GetValueOrDefault(nameof(sfilterpgasohol), false);
            set => AppSettings.AddOrUpdateValue(nameof(sfilterpgasohol), value);
            /*get
            {
                return config.Element("appconfig").Element("settings").Element("stationFilters").Element("Product").Attribute("pgasohol").Value == "1" ? true : false;
            }
            set
            {
                config.Element("appconfig").Element("settings").Element("stationFilters").Element("Product").Attribute("pgasohol").Value = value ? "1" : "0";
            }*/
        }
        public int defaultTile {
            get => AppSettings.GetValueOrDefault(nameof(defaultTile), -1);
            set => AppSettings.AddOrUpdateValue(nameof(defaultTile), value);
            /*get
            {
                return Convert.ToInt32(config.Element("appconfig").Element("tiles").Attribute("default").Value);
            }
            set
            {
                config.Element("appconfig").Element("tiles").Attribute("default").Value = value.ToString();
            }*/
        }
        public DateTime tileupdateTime
        {
            get => AppSettings.GetValueOrDefault(nameof(tileupdateTime), DateTime.Now);
            set => AppSettings.AddOrUpdateValue(nameof(tileupdateTime), value);
            /*get
            {
                return new DateTime(Convert.ToInt64(config.Element("appconfig").Element("tiles").Attribute("updateDate").Value));
            }
            set
            {
                config.Element("appconfig").Element("tiles").Attribute("updateDate").Value = value.Ticks.ToString();
            }*/
        }
        public DateTime dailynotify
        {
            get => AppSettings.GetValueOrDefault(nameof(dailynotify), DateTime.Now);
            set => AppSettings.AddOrUpdateValue(nameof(dailynotify), value);
            /*get
            {
                return new DateTime(Convert.ToInt64(config.Element("appconfig").Element("settings").Element("dailynotify").Value));
            }
            set
            {
                config.Element("appconfig").Element("settings").Element("dailynotify").Value = value.Ticks.ToString();
            }*/
        }
        public bool CPCnotified
        {
            get => AppSettings.GetValueOrDefault(nameof(CPCnotified), false);
            set => AppSettings.AddOrUpdateValue(nameof(CPCnotified), value);
            /*get
            {
                return config.Element("appconfig").Element("info").Element("notified").Attribute("CPC").Value == "1" ? true : false;
            }
            set
            {
                config.Element("appconfig").Element("info").Element("notified").Attribute("CPC").Value = value ? "1" : "0";
            }*/
        }
        public bool FPCCnotified
        {
            get => AppSettings.GetValueOrDefault(nameof(FPCCnotified), false);
            set => AppSettings.AddOrUpdateValue(nameof(FPCCnotified), value);
            /*get
            {
                return config.Element("appconfig").Element("info").Element("notified").Attribute("FPCC").Value == "1" ? true : false;
            }
            set
            {
                config.Element("appconfig").Element("info").Element("notified").Attribute("FPCC").Value = value ? "1" : "0";
            }*/
        }
        public bool dailynotified
        {
            get => AppSettings.GetValueOrDefault(nameof(dailynotified), false);
            set => AppSettings.AddOrUpdateValue(nameof(dailynotified), value);
            /*get
            {
                return config.Element("appconfig").Element("settings").Element("dailynotify").Attribute("notified").Value == "1" ? true : false;
            }
            set
            {
                config.Element("appconfig").Element("settings").Element("dailynotify").Attribute("notified").Value = value ? "1" : "0";
            }*/
        }
        public bool dailynotifyEnable
        {
            get => AppSettings.GetValueOrDefault(nameof(dailynotifyEnable), false);
            set => AppSettings.AddOrUpdateValue(nameof(dailynotifyEnable), value);
            /*get
            {
                return config.Element("appconfig").Element("settings").Element("dailynotify").Attribute("enable").Value == "1" ? true : false;
            }
            set
            {
                config.Element("appconfig").Element("settings").Element("dailynotify").Attribute("enable").Value = value ? "1" : "0";
            }*/
        }
        public DateTime notifycheckedHour
        {
            get => AppSettings.GetValueOrDefault(nameof(notifycheckedHour), DateTime.Now);
            set => AppSettings.AddOrUpdateValue(nameof(notifycheckedHour), value);
            /*get
            {
                return new DateTime(Convert.ToInt64(config.Element("appconfig").Element("info").Element("notified").Attribute("checkHour").Value));
            }
            set
            {
                config.Element("appconfig").Element("info").Element("notified").Attribute("checkHour").Value = value.Ticks.ToString();
            }*/
        }
        public DateTime dailynotifiedHour
        {
            get => AppSettings.GetValueOrDefault(nameof(dailynotifiedHour), DateTime.Now);
            set => AppSettings.AddOrUpdateValue(nameof(dailynotifiedHour), value);
            /*get
            {
                return new DateTime(Convert.ToInt64(config.Element("appconfig").Element("settings").Element("dailynotify").Attribute("checkHour").Value));
            }
            set
            {
                config.Element("appconfig").Element("settings").Element("dailynotify").Attribute("checkHour").Value = value.Ticks.ToString();
            }*/
        }
        public int defaultPage
        {
            get => AppSettings.GetValueOrDefault(nameof(defaultPage), 0);
            set => AppSettings.AddOrUpdateValue(nameof(defaultPage), value);
            /*get
            {
                return Convert.ToInt32(config.Element("appconfig").Element("settings").Element("defaultPage").Attribute("page").Value);
            }
            set
            {
                config.Element("appconfig").Element("settings").Element("defaultPage").Attribute("page").Value = value.ToString();
            }*/
        }
        public DateTime version
        {
            get => AppSettings.GetValueOrDefault(nameof(version), DateTime.Now);
            set => AppSettings.AddOrUpdateValue(nameof(version), value);
            /*get
            {
                return new DateTime(Convert.ToInt64(config.Element("appconfig").Attribute("version").Value));
            }
            set
            {
                config.Element("appconfig").Attribute("version").Value = value.Ticks.ToString();
            }*/
        }
        public DateTime DBcheckedDate
        {
            get => AppSettings.GetValueOrDefault(nameof(DBcheckedDate), DateTime.Now);
            set => AppSettings.AddOrUpdateValue(nameof(DBcheckedDate), value);
            /*get
            {
                return new DateTime(Convert.ToInt64(config.Element("appconfig").Element("info").Element("updateDate").Attribute("DBcheckedDate").Value));
            }
            set
            {
                config.Element("appconfig").Element("info").Element("updateDate").Attribute("DBcheckedDate").Value = value.Ticks.ToString();
            }*/
        }
        public GeoPoint sfilterrangeLocation
        {
            get
            {
                string[] geostring = AppSettings.GetValueOrDefault(nameof(sfilterrangeLocation), "25.021918,121.535285").Split(',');
                return new GeoPoint(Convert.ToDouble(geostring[0]), Convert.ToDouble(geostring[1]));
            }
            set => AppSettings.AddOrUpdateValue(nameof(sfilterrangeLocation), value.Latitude.ToString()+","+value.Longitude.ToString());
            /*get
            {
                return new GeoPoint(Convert.ToDouble(config.Element("appconfig").Element("settings").Element("stationFilters").Element("Kilometer").Attribute("lat").Value), Convert.ToDouble(config.Element("appconfig").Element("settings").Element("stationFilters").Element("Kilometer").Attribute("long").Value));
            }
            set
            {
                config.Element("appconfig").Element("settings").Element("stationFilters").Element("Kilometer").Attribute("lat").Value = value.Latitude.ToString();
                config.Element("appconfig").Element("settings").Element("stationFilters").Element("Kilometer").Attribute("long").Value = value.Longitude.ToString();
            }*/
        }
        public string sfilterrangeLocationname
        {
            get => AppSettings.GetValueOrDefault(nameof(sfilterrangeLocationname), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(sfilterrangeLocationname), value);
            /*get
            {
                return config.Element("appconfig").Element("settings").Element("stationFilters").Element("Kilometer").Attribute("name").Value;
            }
            set
            {
                config.Element("appconfig").Element("settings").Element("stationFilters").Element("Kilometer").Attribute("name").Value = value;
            }*/
        }
        public bool sfiltercustomLocation
        {
            get => AppSettings.GetValueOrDefault(nameof(sfiltercustomLocation), false);
            set => AppSettings.AddOrUpdateValue(nameof(sfiltercustomLocation), value);
            /*get
            {
                return config.Element("appconfig").Element("settings").Element("stationFilters").Element("Kilometer").Attribute("filter").Value == "0" ? false : true;
            }
            set
            {
                config.Element("appconfig").Element("settings").Element("stationFilters").Element("Kilometer").Attribute("filter").Value = value ? "1" : "0";
            }*/
        }
        public List<int> productMonitor
        {
            get
            {
                return JsonConvert.DeserializeObject<List<int>>(AppSettings.GetValueOrDefault(nameof(productMonitor), JsonConvert.SerializeObject(new List<int>())));
            }
            set => AppSettings.AddOrUpdateValue(nameof(productMonitor), JsonConvert.SerializeObject(value));
            /*get
            {
                return (from node in config.Element("appconfig").Element("settings").Element("productMonitor").Descendants("product") select Convert.ToInt32(node.Attribute("id").Value)).ToList();
            }
            set
            {
                IEnumerable<XElement> nlist = from v in value select new XElement("product", new XAttribute("id", v));
                config.Element("appconfig").Element("settings").Element("productMonitor").RemoveAll();
                foreach (XElement n in nlist)
                {
                    config.Element("appconfig").Element("settings").Element("productMonitor").Add(n);
                }
            }*/
        }
        public List<string> sfilterSubBrands
        {
            get
            {
                return (List<string>)JsonConvert.DeserializeObject(AppSettings.GetValueOrDefault(nameof(sfilterSubBrands), JsonConvert.SerializeObject(new List<string>())));
            }
            set => AppSettings.AddOrUpdateValue(nameof(sfilterSubBrands), JsonConvert.SerializeObject(value));
            /*get
            {
                return (from node in config.Element("appconfig").Element("settings").Element("stationFilters").Element("Subbrand").Descendants("term") select node.Attribute("id").Value).ToList();
            }
            set
            {
                IEnumerable<XElement> nlist = from v in value select new XElement("term", new XCData(v));
                config.Element("appconfig").Element("settings").Element("stationFilters").Element("Subbrand").RemoveAll();
                foreach (XElement n in nlist)
                {
                    config.Element("appconfig").Element("settings").Element("stationFilters").Element("Subbrand").Add(n);
                }
            }*/
        }
        public DateTime moeaboeDBdate
        {
            get => AppSettings.GetValueOrDefault(nameof(moeaboeDBdate), DateTime.Now);
            set => AppSettings.AddOrUpdateValue(nameof(moeaboeDBdate), value);
            /*get { return new DateTime(Convert.ToInt64(config.Element("appconfig").Element("info").Element("updateDate").Attribute("moeaboeDBdate").Value)); }
            set
            {
                config.Element("appconfig").Element("info").Element("updateDate").Attribute("moeaboeDBdate").Value = value.Ticks.ToString();
            }*/
        }
        public string discountRev
        {
            get => AppSettings.GetValueOrDefault(nameof(discountRev), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(discountRev), value);
            /*get { return config.Element("appconfig").Element("info").Element("updateDate").Attribute("discountRev").Value; }
            set
            {
                config.Element("appconfig").Element("info").Element("updateDate").Attribute("discountRev").Value = value;
            }*/
        }
        public DateTime dDBupdateDate
        {
            get => AppSettings.GetValueOrDefault(nameof(dDBupdateDate), DateTime.Now);
            set => AppSettings.AddOrUpdateValue(nameof(dDBupdateDate), value);
            /*get { return new DateTime(Convert.ToInt64(config.Element("appconfig").Element("info").Element("updateDate").Attribute("dDBcheckedDate").Value)); }
            set
            {
                config.Element("appconfig").Element("info").Element("updateDate").Attribute("dDBcheckedDate").Value = value.Ticks.ToString();
            }*/
        }
        public string creditXML
        {
            get
            {
                byte[] gzBuffer = Convert.FromBase64String(AppSettings.GetValueOrDefault(nameof(creditXML), string.Empty));
                using (MemoryStream ms = new MemoryStream())
                {
                    int msgLength = BitConverter.ToInt32(gzBuffer, 0);
                    ms.Write(gzBuffer, 4, gzBuffer.Length - 4);

                    byte[] buffer = new byte[msgLength];

                    ms.Position = 0;
                    using (GZipStream zip = new GZipStream(ms, CompressionMode.Decompress))
                    {
                        zip.Read(buffer, 0, buffer.Length);
                    }
                    return Portable.Text.Encoding.UTF8.GetString(buffer);
                }
            }
            set
            {
                byte[] buffer = Encoding.UTF8.GetBytes(value);
                MemoryStream ms = new MemoryStream();
                using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    zip.Write(buffer, 0, buffer.Length);
                }

                ms.Position = 0;
                MemoryStream outStream = new MemoryStream();

                byte[] compressed = new byte[ms.Length];
                ms.Read(compressed, 0, compressed.Length);

                byte[] gzBuffer = new byte[compressed.Length + 4];
                System.Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
                System.Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
                //System.Diagnostics.Debug.WriteLine(Convert.ToBase64String(gzBuffer));
                AppSettings.AddOrUpdateValue(nameof(creditXML), Convert.ToBase64String(gzBuffer));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string caller = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(caller));
            }
        }
    }
    public class configSet
    {
        private static ISettings AppSettings => CrossSettings.Current;
        public string key;
        public DateTime version;
        private long appDate = 636807798000000000;
        public configSet(string k, DateTime d)
        {
            key = k;
            version = d;
        }
        public string getValue()
        {
            switch (key)
            {
                case "version":
                    return AppSettings.GetValueOrDefault(key, new DateTime(appDate)).ToString();
                case "firstLoad":
                    return AppSettings.GetValueOrDefault(key, true).ToString();
                case "upgrade":
                    return AppSettings.GetValueOrDefault(key, false).ToString();
                case "scheduledTaskErrorcode":
                    return AppSettings.GetValueOrDefault(key, string.Empty).ToString();
                case "scheduledTaskErrortime":
                    return AppSettings.GetValueOrDefault(key, DateTime.MinValue).ToString();
                case "scheduledTaskErrortrace":
                    return AppSettings.GetValueOrDefault(key, string.Empty).ToString();
                case "ppause":
                    return AppSettings.GetValueOrDefault(key, false).ToString();
                case "pstartWeek":
                    return AppSettings.GetValueOrDefault(key, DateTime.MinValue).ToString();
                case "pendWeek":
                    return AppSettings.GetValueOrDefault(key, DateTime.MinValue).ToString();
                case "prunDay":
                    return AppSettings.GetValueOrDefault(key, DateTime.MinValue).ToString();
                case "predictgasPrice":
                    return AppSettings.GetValueOrDefault(key, 0.0).ToString();
                case "predictdieselPrice":
                    return AppSettings.GetValueOrDefault(key, 0.0).ToString();
                case "priceDBdate":
                    return AppSettings.GetValueOrDefault(key, DateTime.MinValue).ToString();
                case "stationDBdate":
                    return AppSettings.GetValueOrDefault(key, new DateTime(636807798000000000)).ToString();
                case "stationDBnotifydate":
                    return AppSettings.GetValueOrDefault(key, DateTime.MinValue).ToString();
                case "savetime":
                    return AppSettings.GetValueOrDefault(key, false).ToString();
                case "autoupdate":
                    return AppSettings.GetValueOrDefault(key, false).ToString();
                case "soapupdate":
                    return AppSettings.GetValueOrDefault(key, false).ToString();
                case "runPredict":
                    return AppSettings.GetValueOrDefault(key, true).ToString();
                case "stationBehavior":
                    return AppSettings.GetValueOrDefault(key, false).ToString();
                case "updatefreq":
                    return AppSettings.GetValueOrDefault(key, false).ToString();
                case "dailynotifytime":
                    return AppSettings.GetValueOrDefault(key, 360000000000).ToString();
                case "cpcdefaultProduct":
                    return AppSettings.GetValueOrDefault(key, 0).ToString();
                case "fpccdefaultProduct":
                    return AppSettings.GetValueOrDefault(key, 0).ToString();
                case "sfilterFPCC":
                    return AppSettings.GetValueOrDefault(key, false).ToString();
                case "sfilterInservice":
                    return AppSettings.GetValueOrDefault(key, false).ToString();
                case "sfiltercountryEnable":
                    return AppSettings.GetValueOrDefault(key, false).ToString();
                case "sfiltercountry":
                    return AppSettings.GetValueOrDefault(key, 0).ToString();
                case "sfilterCPC":
                    return AppSettings.GetValueOrDefault(key, false).ToString();
                case "sfilterDirect":
                    return AppSettings.GetValueOrDefault(key, false).ToString();
                case "sfilterSelf":
                    return AppSettings.GetValueOrDefault(key, false).ToString();
                case "sfilterFavoirte":
                    return AppSettings.GetValueOrDefault(key, false).ToString();
                case "sfilterKilonmeter":
                    return AppSettings.GetValueOrDefault(key, 5.0).ToString();
                case "sfilterp92":
                    return AppSettings.GetValueOrDefault(key, false).ToString();
                case "sfilterp95":
                    return AppSettings.GetValueOrDefault(key, false).ToString();
                case "sfilterp98":
                    return AppSettings.GetValueOrDefault(key, false).ToString();
                case "sfilterpdiesel":
                    return AppSettings.GetValueOrDefault(key, false).ToString();
                case "sfilterpgasohol":
                    return AppSettings.GetValueOrDefault(key, false).ToString();
                case "defaultTile":
                    return AppSettings.GetValueOrDefault(key, -1).ToString();
                case "tileupdateTime":
                    return AppSettings.GetValueOrDefault(key, DateTime.MinValue).ToString();
                case "dailynotify":
                    return AppSettings.GetValueOrDefault(key, DateTime.MinValue).ToString();
                case "CPCnotified":
                    return AppSettings.GetValueOrDefault(key, false).ToString();
                case "FPCCnotified":
                    return AppSettings.GetValueOrDefault(key, false).ToString();
                case "dailynotified":
                    return AppSettings.GetValueOrDefault(key, false).ToString();
                case "dailynotifyEnable":
                    return AppSettings.GetValueOrDefault(key, false).ToString();
                case "notifycheckedHour":
                    return AppSettings.GetValueOrDefault(key, DateTime.MinValue).ToString();
                case "dailynotifiedHour":
                    return AppSettings.GetValueOrDefault(key, DateTime.MinValue).ToString();
                case "defaultPage":
                    return AppSettings.GetValueOrDefault(key, 0).ToString();
                case "DBcheckedDate":
                    return AppSettings.GetValueOrDefault(key, DateTime.MinValue).ToString();
                case "sfilterrangeLocation":
                    return AppSettings.GetValueOrDefault(key, "25.021918,121.535285").ToString();
                case "sfilterrangeLocationname":
                    return AppSettings.GetValueOrDefault(key, string.Empty).ToString();
                case "sfiltercustomLocation":
                    return AppSettings.GetValueOrDefault(key, false).ToString();
                case "productMonitor":
                    return AppSettings.GetValueOrDefault(key, Newtonsoft.Json.JsonConvert.SerializeObject(new List<int>())).ToString();
                case "sfilterSubBrands":
                    return AppSettings.GetValueOrDefault(key, Newtonsoft.Json.JsonConvert.SerializeObject(new List<int>())).ToString();
                case "moeaboeDBdate":
                    return AppSettings.GetValueOrDefault(key, DateTime.MinValue).ToString();
                case "discountRev":
                    return AppSettings.GetValueOrDefault(key, string.Empty).ToString();
                case "dDBupdateDate":
                    return AppSettings.GetValueOrDefault(key, DateTime.MinValue).ToString();
                case "resetDB":
                    return AppSettings.GetValueOrDefault(key, true).ToString();
                case "copyConfig":
                    return AppSettings.GetValueOrDefault(key, true).ToString();
                case "creditXML":
                    return AppSettings.GetValueOrDefault(key, "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").ToString();
            }
            return string.Empty;
        }
        public void exeValue()
        {
            switch (key)
            {
                case "version":
                    AppSettings.AddOrUpdateValue(key, new DateTime(appDate));
                    break;
                case "firstLoad":
                    AppSettings.AddOrUpdateValue(key, true);
                    break;
                case "upgrade":
                    AppSettings.AddOrUpdateValue(key, false);
                    break;
                case "scheduledTaskErrorcode":
                    AppSettings.AddOrUpdateValue(key, string.Empty);
                    break;
                case "scheduledTaskErrortime":
                    AppSettings.AddOrUpdateValue(key, DateTime.MinValue);
                    break;
                case "scheduledTaskErrortrace":
                    AppSettings.AddOrUpdateValue(key, string.Empty);
                    break;
                case "ppause":
                    AppSettings.AddOrUpdateValue(key, false);
                    break;
                case "pstartWeek":
                    AppSettings.AddOrUpdateValue(key, DateTime.MinValue);
                    break;
                case "pendWeek":
                    AppSettings.AddOrUpdateValue(key, DateTime.MinValue);
                    break;
                case "prunDay":
                    AppSettings.AddOrUpdateValue(key, DateTime.MinValue);
                    break;
                case "predictgasPrice":
                    AppSettings.AddOrUpdateValue(key, 0.0);
                    break;
                case "predictdieselPrice":
                    AppSettings.AddOrUpdateValue(key, 0.0);
                    break;
                case "priceDBdate":
                    AppSettings.AddOrUpdateValue(key, DateTime.MinValue);
                    break;
                case "stationDBdate":
                    AppSettings.AddOrUpdateValue(key, new DateTime(636807798000000000));
                    break;
                case "stationDBnotifydate":
                    AppSettings.AddOrUpdateValue(key, DateTime.MinValue);
                    break;
                case "savetime":
                    AppSettings.AddOrUpdateValue(key, false);
                    break;
                case "autoupdate":
                    AppSettings.AddOrUpdateValue(key, false);
                    break;
                case "soapupdate":
                    AppSettings.AddOrUpdateValue(key, false);
                    break;
                case "runPredict":
                    AppSettings.AddOrUpdateValue(key, false);
                    break;
                case "stationBehavior":
                    AppSettings.AddOrUpdateValue(key, false);
                    break;
                case "updatefreq":
                    AppSettings.AddOrUpdateValue(key, false);
                    break;
                case "dailynotifytime":
                    AppSettings.AddOrUpdateValue(key, 360000000000);
                    break;
                case "cpcdefaultProduct":
                    AppSettings.AddOrUpdateValue(key, 0);
                    break;
                case "fpccdefaultProduct":
                    AppSettings.AddOrUpdateValue(key, 0);
                    break;
                case "sfilterFPCC":
                    AppSettings.AddOrUpdateValue(key, false);
                    break;
                case "sfilterInservice":
                    AppSettings.AddOrUpdateValue(key, false);
                    break;
                case "sfiltercountryEnable":
                    AppSettings.AddOrUpdateValue(key, false);
                    break;
                case "sfiltercountry":
                    AppSettings.AddOrUpdateValue(key, 0);
                    break;
                case "sfilterCPC":
                    AppSettings.AddOrUpdateValue(key, false);
                    break;
                case "sfilterDirect":
                    AppSettings.AddOrUpdateValue(key, false);
                    break;
                case "sfilterSelf":
                    AppSettings.AddOrUpdateValue(key, false);
                    break;
                case "sfilterFavoirte":
                    AppSettings.AddOrUpdateValue(key, false);
                    break;
                case "sfilterKilonmeter":
                    AppSettings.AddOrUpdateValue(key, 5.0);
                    break;
                case "sfilterp92":
                    AppSettings.AddOrUpdateValue(key, false);
                    break;
                case "sfilterp95":
                    AppSettings.AddOrUpdateValue(key, false);
                    break;
                case "sfilterp98":
                    AppSettings.AddOrUpdateValue(key, false);
                    break;
                case "sfilterpdiesel":
                    AppSettings.AddOrUpdateValue(key, false);
                    break;
                case "sfilterpgasohol":
                    AppSettings.AddOrUpdateValue(key, false);
                    break;
                case "defaultTile":
                    AppSettings.AddOrUpdateValue(key, -1);
                    break;
                case "tileupdateTime":
                    AppSettings.AddOrUpdateValue(key, DateTime.MinValue);
                    break;
                case "dailynotify":
                    AppSettings.AddOrUpdateValue(key, DateTime.MinValue);
                    break;
                case "CPCnotified":
                    AppSettings.AddOrUpdateValue(key, false);
                    break;
                case "FPCCnotified":
                    AppSettings.AddOrUpdateValue(key, false);
                    break;
                case "dailynotified":
                    AppSettings.AddOrUpdateValue(key, false);
                    break;
                case "dailynotifyEnable":
                    AppSettings.AddOrUpdateValue(key, false);
                    break;
                case "notifycheckedHour":
                    AppSettings.AddOrUpdateValue(key, DateTime.MinValue);
                    break;
                case "dailynotifiedHour":
                    AppSettings.AddOrUpdateValue(key, DateTime.MinValue);
                    break;
                case "defaultPage":
                    AppSettings.AddOrUpdateValue(key, 0);
                    break;
                case "DBcheckedDate":
                    AppSettings.AddOrUpdateValue(key, DateTime.MinValue);
                    break;
                case "sfilterrangeLocation":
                    AppSettings.AddOrUpdateValue(key, "25.021918,121.535285");
                    break;
                case "sfilterrangeLocationname":
                    AppSettings.AddOrUpdateValue(key, string.Empty);
                    break;
                case "sfiltercustomLocation":
                    AppSettings.AddOrUpdateValue(key, false);
                    break;
                case "productMonitor":
                    AppSettings.AddOrUpdateValue(key, Newtonsoft.Json.JsonConvert.SerializeObject(new List<int>()));
                    break;
                case "sfilterSubBrands":
                    AppSettings.AddOrUpdateValue(key, Newtonsoft.Json.JsonConvert.SerializeObject(new List<int>()));
                    break;
                case "moeaboeDBdate":
                    AppSettings.AddOrUpdateValue(key, DateTime.MinValue);
                    break;
                case "discountRev":
                    AppSettings.AddOrUpdateValue(key, string.Empty);
                    break;
                case "dDBupdateDate":
                    AppSettings.AddOrUpdateValue(key, DateTime.MinValue);
                    break;
                case "resetDB":
                    AppSettings.AddOrUpdateValue(key, true);
                    break;
                case "copyConfig":
                    AppSettings.AddOrUpdateValue(key, true);
                    break;
                case "creditXML":
                    AppSettings.AddOrUpdateValue(key, "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");
                    break;
            }
        }
    }

}