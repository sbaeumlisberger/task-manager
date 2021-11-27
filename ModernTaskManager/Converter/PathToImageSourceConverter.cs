using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ModernTaskManager.Converter
{
    public class PathToImageSourceConverter : IValueConverter
    {

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        private static readonly Dictionary<string, Icon?> iconCache = new Dictionary<string, Icon?>();

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string path)
            {
                return null;
            }

            if (!iconCache.TryGetValue(path, out var icon))
            {
                icon = Icon.ExtractAssociatedIcon(path);
                iconCache.Add(path, icon);
            }    

            if (icon is null) 
            {
                return null;
            }

            Bitmap bitmap = icon.ToBitmap();
            IntPtr hBitmap = bitmap.GetHbitmap();

            ImageSource imageSource = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            if (!DeleteObject(hBitmap))
            {
                throw new Win32Exception();
            }

            return imageSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
