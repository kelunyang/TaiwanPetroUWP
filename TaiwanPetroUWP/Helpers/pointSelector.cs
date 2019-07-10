using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TaiwanPetroUWP.Helpers
{
    class pointSelector : DataTemplateSelector
    {
        public DataTemplate GreenTemplate { get; set; }
        public DataTemplate RedTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item != null)
            {
                if (item is GreenPOI)
                {
                    return GreenTemplate;
                }

                return RedTemplate;
            }

            return null;
        }
    }
    public class POI
    {
        public string Name { get; set; }

        public Geopoint Location { get; set; }
    }

    public class GreenPOI : POI { }

    public class RedPOI : POI { }
}
