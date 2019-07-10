using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using TaiwanPetroLibrary.Helpers;
using TaiwanPetroLibrary.Models;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
//using AngleSharp;

namespace TaiwanPetroLibrary.ViewModels
{
    public class ppViewModel : viewmodelBase
    {
        HttpClient httpClient;
        /*IConfiguration config = new Configuration().WithDefaultLoader();
        AngleSharp.Dom.IDocument tHtmlDoc;*/
        Uri URI = new Uri("https://www2.moeaboe.gov.tw/oil102/oil2017/A00/Oil_Price2.asp");
        List<internationalModel> lio = new List<internationalModel>();
        DateTime cd = DateTime.Now;
        double _pprice = double.NaN;
        DateTime _pstartdate = DateTime.MinValue;
        DateTime _penddate = DateTime.MinValue;
        public bool _loaded = false;
        bool _predictpause = true;
        weekpriceModel[] wp;
        bool connectivity;
        public bool predictpause
        {
            get
            {
                _predictpause = im.ppause;
                return _predictpause;
            }
            set
            {
                im.ppause = value;
                NotifyPropertyChanged();
            }
        }
        public double pprice
        {
            get
            {
                if (double.IsNaN(_pprice)) return double.NaN;
                _pprice = im.predictgasPrice;
                return _pprice;
            }
            set
            {
                _pprice = value;
                im.predictgasPrice = value;
                NotifyPropertyChanged();
            }
        }
        double _pdprice = double.NaN;
        public double pdprice
        {
            get
            {
                if (double.IsNaN(_pdprice)) return double.NaN;
                _pdprice = im.predictdieselPrice;
                return _pdprice;
            }
            set
            {
                _pdprice = value;
                im.predictdieselPrice = value;
                NotifyPropertyChanged();
            }
        }
        public DateTime pstartdate
        {
            get
            {
                if (_pstartdate == DateTime.MinValue) return _pstartdate;
                _pstartdate = im.pstartWeek;
                return _pstartdate;
            }
            set
            {
                _pstartdate = value;
                im.pstartWeek = value;
                NotifyPropertyChanged();
            }
        }
        DateTime _prunday = DateTime.MinValue;
        public DateTime prunday
        {
            get 
            {
                if (_prunday == DateTime.MinValue) return _prunday;
                _prunday = im.prunDay;
                return _prunday;
            }
            set
            {
                _prunday = value;
                im.prunDay = value;
                NotifyPropertyChanged();
            }
        }
        public DateTime penddate
        {
            get
            {
                if (_penddate == DateTime.MinValue) return _penddate;
                _penddate = im.pendWeek;
                return _penddate;
            }
            set
            {
                _penddate = value;
                im.pendWeek = value;
                NotifyPropertyChanged();
            }
        }
        public bool loaded
        {
            get
            {
                return _loaded;
            }
            set
            {
                _loaded = value;
                NotifyPropertyChanged();
            }
        }
        double _pastbrent = double.NaN;
        public double pastbrent
        {
            get { return _pastbrent; }
            set
            {
                _pastbrent = value;
                NotifyPropertyChanged();
            }
        }
        double _pastdubai = double.NaN;
        public double pastdubai
        {
            get { return _pastdubai; }
            set
            {
                _pastdubai = value;
                NotifyPropertyChanged();
            }
        }
        double _pastcurrency = double.NaN;
        public double pastcurrency
        {
            get { return _pastcurrency; }
            set
            {
                _pastcurrency = value;
                NotifyPropertyChanged();
            }
        }
        double _currentdubai = double.NaN;
        public double currentdubai
        {
            get { return _currentdubai; }
            set
            {
                _currentdubai = value;
                NotifyPropertyChanged();
            }
        }
        double _currentbrent = double.NaN;
        public double currentbrent
        {
            get { return _currentbrent; }
            set
            {
                _currentbrent = value;
                NotifyPropertyChanged();
            }
        }
        double _currentcurrency = double.NaN;
        public double currentcurrency
        {
            get { return _currentcurrency; }
            set
            {
                _currentcurrency = value;
                NotifyPropertyChanged();
            }
        }
        bool _runPredict = true;
        public bool runPredict
        {
            get
            {
                _runPredict = im.runPredict;
                return _runPredict;
            }
            set
            {
                im.runPredict = value;
                NotifyPropertyChanged();
            }
        }
        PlotModel _price95model;
        PlotModel _pricedieselmodel;
        public PlotModel pricedieselModel
        {
            get { return _pricedieselmodel; }
            set
            {
                _pricedieselmodel = value;
                NotifyPropertyChanged();
            }
        }
        public PlotModel price95Model
        {
            get { return _price95model; }
            set
            {
                _price95model = value;
                NotifyPropertyChanged();
            }
        }
        DateTimeAxis _dtx95;
        public DateTimeAxis dtx95
        {
            get { return _dtx95; }
            set
            {
                _dtx95 = value;
                NotifyPropertyChanged();
            }
        }
        DateTimeAxis _dtxdiesel;
        public DateTimeAxis dtxdiesel
        {
            get { return _dtxdiesel; }
            set
            {
                _dtxdiesel = value;
                NotifyPropertyChanged();
            }
        }
        LinearAxis _la95;
        public LinearAxis la95
        {
            get { return _la95; }
            set
            {
                _la95 = value;
                NotifyPropertyChanged();
            }
        }
        LinearAxis _ladiesel;
        public LinearAxis ladiesel
        {
            get { return _ladiesel; }
            set
            {
                _ladiesel = value;
                NotifyPropertyChanged();
            }
        }
        LineSeries _prices95;
        public LineSeries prices95
        {
            get { return _prices95; }
            set
            {
                _prices95 = value;
                NotifyPropertyChanged();
            }
        }
        LineSeries _pricesdiesel;
        public LineSeries pricesdiesel
        {
            get { return _pricesdiesel; }
            set
            {
                _pricesdiesel = value;
                NotifyPropertyChanged();
            }
        }
        LineSeries _avgsdiesel;
        public LineSeries avgsdiesel
        {
            get { return _avgsdiesel; }
            set
            {
                _avgsdiesel = value;
                NotifyPropertyChanged();
            }
        }
        LineSeries _avgs95;
        public LineSeries avgs95
        {
            get { return _avgs95; }
            set
            {
                _avgs95 = value;
                NotifyPropertyChanged();
            }
        }
        LineSeries _saveds95;
        public LineSeries saveds95
        {
            get { return _saveds95; }
            set
            {
                _saveds95 = value;
                NotifyPropertyChanged();
            }
        }
        LineSeries _savedsdiesel;
        public LineSeries savedsdiesel
        {
            get { return _savedsdiesel; }
            set
            {
                _savedsdiesel = value;
                NotifyPropertyChanged();
            }
        }

        public ppViewModel() {
            wp = new weekpriceModel[] { new weekpriceModel(), new weekpriceModel() };
        }
        public override async Task loadDB(string dbPath)
        {
 	        await base.loadDB(dbPath);
            if (im.pstartWeek != DateTime.MinValue)
            {
                pstartdate = im.pstartWeek;
                penddate = im.pendWeek;
                prunday = im.prunDay;
                pprice = im.predictgasPrice;
                pdprice = im.predictdieselPrice;
            }
        }

        public async Task predictedPrice(bool connectivity, bool predictenable, IProgress<ProgressReport> messenger)
        {
            if (predictenable)
            {
                this.connectivity = connectivity;
                if (connectivity)
                {
                    messenger.Report(new ProgressReport() { progress = 0, progressMessage = "計算預測油價", display = true });
                    lio = new List<internationalModel>();
                    try
                    {
                        //List<string> tAllNodes;
                        //List<string> tDateNodes;
                        var handler = new HttpClientHandler();
                        if (handler.SupportsAutomaticDecompression)
                        {
                            handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                        }
                        using (httpClient = new HttpClient(handler))
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                DateTime day = i == 0 ? cd : lio.First().date.AddDays(-1);
                                string body = "date_type=3&year1=2014&year2=2014&month2=3&year=" + day.Year + "&month=" + day.Month + "&date=" + day.Day + "&submit=About+submit+buttons&ttype=1";
                                StringContent theContent = new StringContent(body, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");
                                messenger.Report(new ProgressReport() { progress = 15 * i, progressMessage = "下載能源局油價資料中", display = true });
                                HttpResponseMessage aResponse = await httpClient.PostAsync(URI, theContent);
                                var byteData = await aResponse.Content.ReadAsByteArrayAsync();
                                string aContent = Portable.Text.Encoding.GetEncoding(950).GetString(byteData, 0, byteData.Length);
                                //string aContent = await aResponse.Content.ReadAsStringAsync();
                                //aResponse.Dispose();
                                RegexOptions opt = RegexOptions.None;
                                Regex datereg = new Regex(@"位.+?平", opt);
                                Match dateMatch = datereg.Match(aContent);
                                Regex detaildate = new Regex(@"\d+<br>\d+.\d+", opt);
                                MatchCollection datedetail = detaildate.Matches(dateMatch.Value);
                                Regex pricereg = new Regex(@"bgColor=#ffffff>.+?<tr\s{2}", opt);
                                MatchCollection priceMatch = pricereg.Matches(aContent);
                                List<MatchCollection> prices = new List<MatchCollection>();
                                foreach(Match m in priceMatch)
                                {
                                    Regex p = new Regex(@"\d{2}.\d{2}", opt);
                                    prices.Add(p.Matches(m.Value));
                                }
                                Regex curreg = new Regex(@"美元.+?<\/(tr)>", opt);
                                Match curMatch = curreg.Match(aContent);
                                Regex detailcur = new Regex(@"\d{2}.\d{3}", opt);
                                MatchCollection curdetail = detailcur.Matches(curMatch.Value);
                                /*AngleSharp.Parser.Html.HtmlParser parser = new AngleSharp.Parser.Html.HtmlParser();
                                tHtmlDoc = parser.Parse(aContent);
                                tDateNodes = tHtmlDoc.QuerySelectorAll("td[width='8%']").Select(m => m.TextContent).ToList();
                                tAllNodes = tHtmlDoc.QuerySelectorAll("td[align='right']").Select(m => m.TextContent).ToList();
                                tHtmlDoc.Dispose();*/
                                //GC.Collect();
                                for (int k = 0; k < 7; k++)
                                {
                                    lio.Add(new internationalModel(datedetail[k].Value, prices[2][k].Value, prices[1][k].Value, curdetail[k].Value));
                                }
                            }
                            //tDateNodes = null;
                            //tAllNodes = null;
                            //GC.Collect();
                            messenger.Report(new ProgressReport() { progress = 50.0, progressMessage = "分析數據中...", display = true });
                            IEnumerable<internationalModel> thisweek = from obj in lio where obj.tick > cd.AddDays((int)cd.DayOfWeek * -1).Ticks select obj;
                            IEnumerable<internationalModel> pastweek = from obj in lio where obj.tick > cd.AddDays(((int)cd.DayOfWeek + 7) * -1).Ticks && obj.tick < cd.AddDays((int)cd.DayOfWeek * -1).Ticks select obj;
                            if (thisweek.Any())
                            {
                                wp = new weekpriceModel[] { new weekpriceModel(), new weekpriceModel() };
                                foreach (internationalModel io in thisweek)
                                {
                                    wp[0].dubai += io.dubai;
                                    wp[0].brent += io.brent;
                                    wp[0].currency += io.currency;
                                    wp[0].cday++;
                                }
                                foreach (internationalModel io in pastweek)
                                {
                                    wp[1].dubai += io.dubai;
                                    wp[1].brent += io.brent;
                                    wp[1].currency += io.currency;
                                    wp[1].cday++;
                                }
                                pastbrent = wp[1].avgbrent;
                                pastdubai = wp[1].avgdubai;
                                pastcurrency = wp[1].avgcurrency;
                                currentbrent = wp[0].avgbrent;
                                currentdubai = wp[0].avgdubai;
                                currentcurrency = wp[0].avgcurrency;
                                this.penddate = thisweek.Last().date;
                                this.pstartdate = thisweek.First().date;
                                this.predictpause = false;
                            }
                            else
                            {
                                this.pprice = double.NaN;
                                this.penddate = DateTime.Now;
                                this.pstartdate = DateTime.Now;
                                this.predictpause = true;
                            }
                            loaded = true;
                            prunday = DateTime.Now;
                            messenger.Report(new ProgressReport() { progress = 100.0, progressMessage = "預測分析完成！", display = true });
                        }
                    }
                    catch
                    {
                        throw new htmlException("分析歷史油價");
                    }
                    messenger.Report(new ProgressReport() { progress = 100, progressMessage = "預測分析完成！", display = false });
                }
                else
                {
                    predictpause = (int)DateTime.Now.DayOfWeek == 0 || (int)DateTime.Now.DayOfWeek == 1;
                    loaded = true;
                }
            }
        }

        public void getPrice(double cpc95price, double cpcdieselprice)
        {
            if (connectivity)
            {
                double predictgasprice = Math.Round(cpc95price + cpc95price * (wp[0].price - wp[1].price) / wp[1].price * 0.8, 1);
                double predictdieselprice = Math.Round(cpcdieselprice + cpcdieselprice * (wp[0].price - wp[1].price) / wp[1].price * 0.8, 1);
                this.pdprice = Math.Round(predictdieselprice - cpcdieselprice, 1);
                this.pprice = Math.Round(predictgasprice - cpc95price, 1);
            }
        }
    }
}
