using System;
using System.Collections.Generic;
using System.Text;

namespace Scryber.OpenType.SubTables
{
    public class HorizontalMetrics : TTFTable
    {

        public HorizontalMetrics(long offset)
            : base(offset)
        {
        }

        private List<HMetric> _hmetrics;

        public List<HMetric> HMetrics
        {
            get { return _hmetrics; }
            set { _hmetrics = value; }
        }

        //TODO:Load the left side bearings for monospace fonts.
    }
}
