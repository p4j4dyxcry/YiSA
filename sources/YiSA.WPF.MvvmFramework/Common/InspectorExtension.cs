using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace YiSA.WPF.Common
{
    public class InspectorExtension
    {
        public string Label    { get; } = string.Empty;
        public double Min      { get; } = 0.0d;
        public double Max      { get; } = 256d;
        public double SnapUnit { get; } = 0d;
        public IEnumerable<string> Hints { get; } = Array.Empty<string>();
    }
}