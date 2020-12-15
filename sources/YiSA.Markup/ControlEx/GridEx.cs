using System;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace YiSA.Markup.ControlEx
{
    public static class GridEx
    {
        /// <summary>
        /// Define Colm using a string.
        /// You can optionally specify a minimum and maximum value.
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
        /// Define Row using a string.
        /// You can optionally specify a minimum and maximum value.
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
        /// Set the Grid area in the order of "Colm", "Row", "ColmSpan", "RowSpan".
        /// example
        ///   <Button ge:GridEx.Area="0,0,1,1"/>
        /// </summary>
        public static readonly DependencyProperty AreaProperty =
            DependencyProperty.RegisterAttached("Area", typeof(Thickness), typeof(GridEx), new PropertyMetadata(default(Thickness), OnAreaChanged));

        public static Thickness GetArea(DependencyObject obj)                  => (Thickness)obj.GetValue(AreaProperty);
        public static void      SetArea(DependencyObject obj, Thickness value) => obj.SetValue(AreaProperty, value);
        
        /// <summary>
        /// Give the split grid a name.
        /// You can specify the position of the named Grid with GridEx.AreaName.
        /// Use this parameter at the same time as GridEx.RowDef, GridEx.RowColmDef.
        /// example 1
        ///   <Grid ge:GridEx.ColmDef="*,*,*"
        ///         ge:GridEx.RowDef ="*,*,*"
        ///         ge:GridEx.AreaDef="      header    /
        ///                            menu content sub/
        ///                            menu con        /"/>
        ///                    |--------------------|
        ///                    |      header        |
        ///                    |--------------------|
        ///                    |    | content| sub  |
        ///                    |menu|---------------|
        ///                    |    |     con       |
        ///                    |--------------------|
        /// </summary>
        public static readonly DependencyProperty AreaDefProperty =
            DependencyProperty.RegisterAttached("AreaDef", typeof(string), typeof(GridEx), new PropertyMetadata(null, OnAreaDefChanged)); 
        public static string GetAreaDef(DependencyObject obj)               => (string)obj.GetValue(AreaDefProperty);
        public static void   SetAreaDef(DependencyObject obj, string value) => obj.SetValue(AreaDefProperty, value);
        
        /// <summary>
        /// Places in the area specified by AreaDef.
        /// example
        ///  <Button ge:GridEx.AreaName="header"/>
        /// </summary>
        public static readonly DependencyProperty AreaNameProperty =
            DependencyProperty.RegisterAttached("AreaName", typeof(string), typeof(GridEx), new PropertyMetadata(null, OnAreaNameChanged)); 
        public static string GetAreaName(DependencyObject obj)               => (string)obj.GetValue(AreaNameProperty);
        public static void   SetAreaName(DependencyObject obj, string value) => obj.SetValue(AreaNameProperty, value);

        
        //-------------------------------------------------------------------------------------
        //! internal functions
        
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
            if (d is FrameworkElement ctrl)
            {
                var thickness = (Thickness)e.NewValue;
                SetGridArea(ctrl, thickness);
            }
        }
        
        private static void OnAreaDefChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var areaDef = (string)e.NewValue;

            if (d is Grid grid)
            {
                string[,] areas = new string[grid.ColumnDefinitions.Count,grid.RowDefinitions.Count];
                var cells = areaDef.Split(new[] {'\n', '/'})
                    .Select(o => o.Trim())
                    .Where(o => !string.IsNullOrWhiteSpace(o))
                    .Select(o => o.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries))
                    .ToArray();

                foreach (var row in Enumerable.Range(0,cells.Length))
                {
                    areas.Fill(row , cells[row][0]);
                }

                for (int row = 0; row < cells.Length; ++row)
                {
                    if (row > grid.RowDefinitions.Count)
                        break;

                    for (int colm = 0; colm < cells[row].Length; ++colm)
                    {
                        if (colm > grid.ColumnDefinitions.Count)
                            break;
                        areas[colm, row] = cells[row][colm];
                    }
                }
                SetInternalTable(d,areas);
            }
        }

        private static void OnAreaNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var areaName = (string)e.NewValue;
            if (d is FrameworkElement control)
            {
                if (control.Parent is Grid parentGrid)
                {
                    string[,] area = GetInternalTable(parentGrid);
                    var rect = area.GetArea(areaName);
                    SetGridArea(control, rect);
                }
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
        
        private static void SetGridArea(FrameworkElement control, Thickness thickness)
        {
            var left   = thickness.Left;
            var top    = thickness.Top;
            var width  = thickness.Right;
            var height = thickness.Bottom;

            if (width <= 0)
                width = 1;
            if (height <= 0)
                height = 1;
            
            Grid.SetColumn    (control, (int)left);
            Grid.SetRow       (control, (int)top);
            Grid.SetColumnSpan(control, (int)width);                
            Grid.SetRowSpan   (control, (int)height);
        }
        
        //-------------------------------------------------------------------------------------
        //! internal Dependency Properties
        
        public static readonly DependencyProperty InternalTableProperty =
            DependencyProperty.RegisterAttached("InternalTable", typeof(string[,]), typeof(GridEx), new PropertyMetadata(null)); 
        private static string[,] GetInternalTable(DependencyObject obj)               => (string[,])obj.GetValue(InternalTableProperty);
        private static void   SetInternalTable(DependencyObject obj, string[,] value) => obj.SetValue(InternalTableProperty, value);
        
        
    }

    /// <summary>
    /// internal extension methods.
    /// </summary>
    internal static class InternalStringExtensions
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

        internal static void Fill(this string[,] table , int startRowIndex , string name)
        {
            for (int colm = 0 ; colm < table.GetLength(0) ; ++colm)
            {
                for (int row = startRowIndex; row < table.GetLength(1); ++row)
                {
                    table[colm, row] = name;
                }
            }
        }

        internal static Thickness GetArea(this string[,] table , string name)
        {
            int colmFirst =0, colmCount =0, rowFirst =0,rowCount = 0;
            int prevColmIndex = -1, prevRowIndex = -1;
            bool found = false;
            for (int colm = 0 ; colm < table.GetLength(0) ; ++colm)
            {
                for (int row = 0; row < table.GetLength(1); ++row)
                {
                    if (table[colm, row] == name)
                    {
                        if (found is false)
                        {
                            colmFirst = prevColmIndex = colm;
                            rowFirst  = prevRowIndex  = row;
                            colmCount++;
                            rowCount++;
                            found = true;                            
                        }
                        else if(prevColmIndex == colm-1)
                        {
                            prevColmIndex = colm;
                            colmCount++;
                        }
                        else if (prevRowIndex == row-1)
                        {
                            prevRowIndex = row;
                            rowCount++;
                        }
                    }
                }
            }
            return new Thickness(colmFirst,rowFirst,colmCount,rowCount);
        }
    }
}