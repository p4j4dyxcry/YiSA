using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace YiSA.WPF.ControlEx
{
    public static class GridEx
    {
        /// <summary>
        /// example 1
        ///   <Grid ge:GridEx.RowDef="*,*,*"/>
        /// example 2 (range base definition)
        ///   <Grid ge:GridEx.RowDef="(*,10,30),(30,20,100)"/>
        /// </summary>
        public static readonly DependencyProperty RowDefProperty =
            DependencyProperty.RegisterAttached("RowDef", typeof(string), typeof(GridEx), new PropertyMetadata(null, OnRowDefinitionChanged));
        public static string GetRowDef(DependencyObject obj)               => (string)obj.GetValue(RowDefProperty);
        public static void   SetRowDef(DependencyObject obj, string value) => obj.SetValue(RowDefProperty, value);

        /// <summary>
        /// example 1
        ///   <Grid ge:GridEx.ColmDef="*,*,*"/>
        /// example 2 (range base definition)
        ///   <Grid ge:GridEx.ColmDef="(*,10,30),(30,20,100)"/>
        /// </summary>
        public static readonly DependencyProperty ColmDefProperty =
            DependencyProperty.RegisterAttached("ColmDef", typeof(string), typeof(GridEx), new PropertyMetadata(null, OnColmDefinitionChanged)); 
        public static string GetColmDef(DependencyObject obj)               => (string)obj.GetValue(ColmDefProperty);
        public static void   SetColmDef(DependencyObject obj, string value) => obj.SetValue(ColmDefProperty, value);

        /// <summary>
        /// example 1
        ///   <Button ge:GridEx.Area="0,0,1,1"/>
        /// </summary>
        public static readonly DependencyProperty AreaProperty =
            DependencyProperty.RegisterAttached("Area", typeof(Thickness), typeof(GridEx), new PropertyMetadata(default(Thickness), OnAreaChanged));

        public static Thickness GetArea(DependencyObject obj)                  => (Thickness)obj.GetValue(AreaProperty);
        public static void      SetArea(DependencyObject obj, Thickness value) => obj.SetValue(AreaProperty, value);

        private static void OnRowDefinitionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = (Grid)d;
            var param = (string)e.NewValue;
            
            SetupRowDef(grid,param);
        }
        
        private static void OnColmDefinitionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = (Grid)d;
            var param = (string)e.NewValue;
            
            SetupColmDef(grid,param);
        }
        
        private static void OnAreaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thickness = (Thickness)e.NewValue;

            var left   = thickness.Left;
            var top    = thickness.Top;
            var width  = thickness.Right;
            var height = thickness.Bottom;

            if (width <= 0)
                width = 1;
            if (height <= 0)
                height = 1;
            
            if (d is FrameworkElement ctrl)
            {
                Grid.SetColumn    (ctrl, (int)left);
                Grid.SetRow       (ctrl, (int)top);
                Grid.SetColumnSpan(ctrl, (int)width);                
                Grid.SetRowSpan   (ctrl, (int)height);
            }
        }
        
        private static void SetupRowDef(Grid grid, string param)
        {
            grid.RowDefinitions.Clear();

            var list = param.Split(',')
                .Select(o => o.Trim());

            foreach (var item in list)
            {
                var def = item.ToGridLengthDefinition();
                var rowDefinition = new RowDefinition();
                rowDefinition.Height = def.GridLength;
                if (def.Min != null) rowDefinition.MinHeight = def.Min.Value;
                if (def.Max != null) rowDefinition.MinHeight = def.Max.Value;
                grid.RowDefinitions.Add(rowDefinition);
            }
        }
        
        private static void SetupColmDef(Grid grid, string param)
        {
            grid.ColumnDefinitions.Clear();

            var list = param.Split(',')
                .Select(o => o.Trim());

            foreach (var item in list)
            {
                var def = item.ToGridLengthDefinition();
                var rowDefinition = new ColumnDefinition();
                rowDefinition.Width = def.GridLength;
                if (def.Min != null) rowDefinition.MinWidth = def.Min.Value;
                if (def.Max != null) rowDefinition.MinWidth = def.Max.Value;
                grid.ColumnDefinitions.Add(rowDefinition);
            }
        }
    }

    /// <summary>
    /// 内部利用、文字列からデータを取得します。
    /// </summary>
    internal static class StringExtensions
    {
        private static double? ToDouble(this string source)
        {
            if( double.TryParse(source, out var result))
                return result;
            return null;
        }

        private static GridLength ToGridLength(this string source , GridLength fallback = default)
        {
            try
            {
                return (GridLength) TypeDescriptor
                    .GetConverter(typeof(GridLength))
                    .ConvertFromString(source);
            }
            catch
            {
                return fallback;
            }
        }
        
        internal static Thickness ToThickness(this string source , Thickness fallback = default)
        {
            try
            {
                return (Thickness) TypeDescriptor
                    .GetConverter(typeof(GridLength))
                    .ConvertFromString(source);
            }
            catch
            {
                return fallback;
            }
        }
        
        internal class GridLengthTuple
        {
            public GridLength GridLength { get; set; }
            public double? Min { get; set; }
            public double? Max { get; set; }
        }
        private static readonly Regex GridLengthRegex = new Regex(@"(^[^\(\)]+)(?:\((.*)-(.*)\))?");
        internal static GridLengthTuple ToGridLengthDefinition(this string source)
        {
            // (val,min,max)の形式
            var m = GridLengthRegex.Match(source);

            var length = m.Groups[1].Value;
            var min = m.Groups[2].Value;
            var max = m.Groups[3].Value;

            return new GridLengthTuple()
            {
                GridLength = length.ToGridLength(),
                Min = min.ToDouble(),
                Max = max.ToDouble()
            };
        }        
    }
}