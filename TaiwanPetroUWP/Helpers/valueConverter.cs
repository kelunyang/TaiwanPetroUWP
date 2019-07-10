using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Text.RegularExpressions;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.Devices.Geolocation;
using TaiwanPetroLibrary.Helpers;

namespace TaiwanPetroUWP.Helpers
{
    public class durationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)   //參數一下二上零不動
        {
            switch ((string)parameter)
            {
                case "0":   //沒營業
                    if ((long)value == 0) return Visibility.Visible;
                    break;
                case "1":   //二十四小時
                    if ((long)value == 864000000000) return Visibility.Visible;
                    break;
                case "3":
                    if ((long)value == 1) return Visibility.Visible;
                    break;
                case "2":   //一般值
                    if ((long)value != 0)
                    {
                        if ((long)value != 864000000000)
                        {
                            if ((long)value != 1)
                            {
                                return Visibility.Visible;
                            }
                        }
                    }
                    break;
            }
            return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class sourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Match mc = Regex.Match((string)value, @"https?:\/\/.*tw\/");
            return mc.Value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class exceldateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            DateTime dt = new DateTime(1900, 1, 1);
            return string.Format("{0:d}", dt.AddDays(((long)value) - 2));
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class stationbehaviortitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? "單點顯示位置，雙擊導航，更多功能請長壓" : "單點進入導航，更多功能請長壓";
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class connectivitystringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? "" : "(離線)";
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class filterwidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (int)value > 0 ? 256 : 286;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class filterheightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (int)value > 0 ? 36 : double.NaN;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    /*public class rectclipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (int)value > 0 ? new RectangleGeometry() { Rect = new Rect(0, 0, 256, 36) } : new RectangleGeometry() { Rect = new Rect(0,0,286,500) };
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }*/
    public class infopointVisConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch((string)parameter)
            {
                case "1":
                    return ((GeoPoint)value).remark == "currentloc" ? Visibility.Visible : Visibility.Collapsed;
                case "0":
                    return ((GeoPoint)value).remark == "queryloc" ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class geoPointConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return new Geopoint(new BasicGeoposition() { Latitude = ((GeoPoint)value).Latitude, Longitude = ((GeoPoint)value).Longitude});
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new GeoPoint(((Geopoint)value).Position.Latitude, ((Geopoint)value).Position.Longitude);
        }
    }
    public class predictpausetextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return !(bool)value ? "週日為中油公告油價時間，請直接看油價公告" : "能源局每週二更新國際油價，目前暫無資料";
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class productnameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return typeDB.productnameDB[(int)value].name;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class stringchangeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return double.IsNaN((double)value) ? "--.-" : (double)value > 0 ? "+"+(double)value : value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class puredateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return String.Format("{0:HH:mm}", new DateTime((long)value));
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class distancefontsizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (double)value >= 10 ? (double)value >= 100 ? 27 : 40 : 40;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class favoritetextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? "取消儲存該站" : "儲存該站";
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class favoriteiconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? FontAwesome.UWP.FontAwesomeIcon.Minus : FontAwesome.UWP.FontAwesomeIcon.Plus;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class observetextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? "移出觀察清單" : "加入觀察清單";
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class observeiconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? FontAwesome.UWP.FontAwesomeIcon.Minus : FontAwesome.UWP.FontAwesomeIcon.Plus;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class colorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (System.Convert.ToDouble(value) > 0)
            {
                return new SolidColorBrush(Colors.Red);
            }
            else if (System.Convert.ToDouble(value) < 0)
            {
                return new SolidColorBrush(Color.FromArgb(255, 70, 222, 70));    //light green
            }
            return new SolidColorBrush(Colors.LightGray);
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class stationopencolorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class datecolorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((DateTime)value == DateTime.MinValue) return new SolidColorBrush(Colors.LightGray);
            return new SolidColorBrush(Colors.Black);
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class signConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)     //參數一下二上零不動
        {
            double v = (double)value;
            if (Double.IsNaN(v)) return Visibility.Collapsed;
            switch ((string)parameter)
            {
                case "0":   //不動
                    if (v == 0) return Visibility.Visible;
                    break;
                case "1":   //下跌
                    if (v < 0) return Visibility.Visible;
                    break;
                case "2":   //上漲
                    if (v > 0) return Visibility.Visible;
                    break;
            }
            return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class bulletincolorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (int)value == 0 ? new SolidColorBrush(Color.FromArgb(255, 19, 78, 139)) : new SolidColorBrush(Color.FromArgb(255, 27, 130, 139));
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class brandvisconvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch (System.Convert.ToInt32(parameter))
            {
                case 0:
                    return System.Convert.ToInt32(value) == 0 ? Visibility.Visible : Visibility.Collapsed;
                case 1:
                    return System.Convert.ToInt32(value) == 1 ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class DBcountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return "資料庫共" + (int)value + "筆加油站";
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class foundcountCounter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return "找到" + (int)value + "筆加油站";
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class selfvisConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (int)value == 0 ? Visibility.Collapsed : Visibility.Visible;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class selftypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch ((int)value)
            {
                case 1:
                    return "汽";
                case 2:
                    return "柴";
                case 3:
                    return "汽/柴";
            }
            return "";
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class stationpromptConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return "找到了" + (int)value + "家加油站";
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class discountcountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return "找到了" + (int)value + "項優惠方案";
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class savetimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? "自動更新時儲存" : "手動更新時儲存";
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class updatefreqConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? "啟動時更新" : "每週六日更新";
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class filterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? "是" : "否";
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class distanceswitchfilterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? "搜尋特定縣市" : "依照距離遠近搜尋";
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class nanvisConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool control = (string)parameter == "0" ? !double.IsNaN((double)value) : double.IsNaN((double)value);
            return control ? Visibility.Collapsed : Visibility.Visible;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class boolvisConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool control = System.Convert.ToInt32(parameter) == 0 ? !(bool)value : (bool)value;
            switch (control)
            {
                case true:
                    return Visibility.Visible;
                case false:
                    return Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class countvisConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool control = System.Convert.ToInt32(parameter) == 0 ? !((int)value > 0) : (int)value > 0;
            switch (control)
            {
                case true:
                    return Visibility.Visible;
                case false:
                    return Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class priceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return double.IsNaN((double)value) ? "--.-" : (double)value < 0 ? string.Format("{0:N1}", Math.Abs((double)value)) : value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class dateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (DateTime)value == DateTime.MinValue ? "----/--/--" : string.Format("{0:d}",value);
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class tickConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (long)value == 0 ? "----/--/--" : (object)new DateTime((long)value);
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class kilometertermConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return "搜尋範圍（"+(double)value+"公里）：";
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
