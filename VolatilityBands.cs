#region Using declarations
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Media;
using System.Xml.Serialization;
#endregion

namespace NinjaTrader.NinjaScript.Indicators
{
    public class VolatilityBands : Indicator
    {
        public const string GROUP_NAME_GENERAL = "1. General";
        public const string GROUP_NAME_VOLATILITY_BANDS = "2. Volatility Bands";
        public const string GROUP_NAME_PLOTS = "Plots";

        private EMA _ema;
        private ATR _atr;

        #region General Properties

        [NinjaScriptProperty]
        [Display(Name = "Version", Description = "Volatility Bands version.", Order = 0, GroupName = GROUP_NAME_GENERAL)]
        [ReadOnly(true)]
        public string Version => "1.0.0";

        #endregion

        #region Volatility Bands Properties

        [Range(1, int.MaxValue), NinjaScriptProperty]
        [Display(Name = "Period", GroupName = GROUP_NAME_VOLATILITY_BANDS, Order = 0)]
        public int Period
        { get; set; }

        [Range(0.01, double.MaxValue), NinjaScriptProperty]
        [Display(Name = "OffsetMultiplier", GroupName = GROUP_NAME_VOLATILITY_BANDS, Order = 1)]
        public double OffsetMultiplier
        { get; set; }

        [Range(0.01, double.MaxValue), NinjaScriptProperty]
        [Display(Name = "OffsetOuterMultiplier", GroupName = GROUP_NAME_VOLATILITY_BANDS, Order = 2)]
        public double OffsetOuterMultiplier
        { get; set; }


        #endregion

        #region Plots

        [Browsable(false)]
        [XmlIgnore]
        [Display(Name = "Midline", GroupName = GROUP_NAME_PLOTS, Order = 0)]
        public Series<double> Midline
        {
            get { return Values[0]; }
        }

        [Browsable(false)]
        [XmlIgnore]
        [Display(Name = "ATR Upper", GroupName = GROUP_NAME_PLOTS, Order = 1)]
        public Series<double> ATRUpper
        {
            get { return Values[1]; }
        }

        [Browsable(false)]
        [XmlIgnore]
        [Display(Name = "ATR Lower", GroupName = GROUP_NAME_PLOTS, Order = 2)]
        public Series<double> ATRLower
        {
            get { return Values[2]; }
        }

        [Browsable(false)]
        [XmlIgnore]
        [Display(Name = "Offset Upper", GroupName = GROUP_NAME_PLOTS, Order = 3)]
        public Series<double> OffsetUpper
        {
            get { return Values[3]; }
        }

        [Browsable(false)]
        [XmlIgnore]
        [Display(Name = "Offset Lower", GroupName = GROUP_NAME_PLOTS, Order = 4)]
        public Series<double> OffsetLower
        {
            get { return Values[4]; }
        }

        [Browsable(false)]
        [XmlIgnore]
        [Display(Name = "Offset Outer Upper", GroupName = GROUP_NAME_PLOTS, Order = 5)]
        public Series<double> OffsetOuterUpper
        {
            get { return Values[5]; }
        }

        [Browsable(false)]
        [XmlIgnore]
        [Display(Name = "Offset Outer Lower", GroupName = GROUP_NAME_PLOTS, Order = 6)]
        public Series<double> OffsetOuterLower
        {
            get { return Values[6]; }
        }

        [NinjaScriptProperty]
        [Display(Name = "Midline Opacity", Description = "The opacity for the line. (0 to 255)", GroupName = GROUP_NAME_PLOTS, Order = 7)]
        public byte MidlineOpacity { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "ATR Upper Opacity", Description = "The opacity for the line. (0 to 255)", GroupName = GROUP_NAME_PLOTS, Order = 8)]
        public byte ATRUpperOpacity { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "ATR Lower Opacity", Description = "The opacity for the line. (0 to 255)", GroupName = GROUP_NAME_PLOTS, Order = 9)]
        public byte ATRLowerOpacity { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Offset Upper Opacity", Description = "The opacity for the line. (0 to 255)", GroupName = GROUP_NAME_PLOTS, Order = 10)]
        public byte OffsetUpperOpacity { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Offset Lower Opacity", Description = "The opacity for the line. (0 to 255)", GroupName = GROUP_NAME_PLOTS, Order = 11)]
        public byte OffsetLowerOpacity { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Offset Outer Upper Opacity", Description = "The opacity for the line. (0 to 255)", GroupName = GROUP_NAME_PLOTS, Order = 12)]
        public byte OffsetOuterUpperOpacity { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Offset Outer Lower Opacity", Description = "The opacity for the line. (0 to 255)", GroupName = GROUP_NAME_PLOTS, Order = 13)]
        public byte OffsetOuterLowerOpacity { get; set; }

        #endregion

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = @"Volatility bands based on ATR and EMA. Essentially, a Keltner Channel with more bands.";
                Name = "_VolatilityBands";
                Calculate = Calculate.OnBarClose;
                IsOverlay = true;
                DisplayInDataBox = true;
                DrawOnPricePanel = true;
                DrawHorizontalGridLines = true;
                DrawVerticalGridLines = true;
                PaintPriceMarkers = true;
                ScaleJustification = NinjaTrader.Gui.Chart.ScaleJustification.Right;
                IsSuspendedWhileInactive = true;

                Period = 10;
                OffsetMultiplier = 1.5;
                OffsetOuterMultiplier = 2.5;

                MidlineOpacity = 200;
                ATRUpperOpacity = 200;
                ATRLowerOpacity = 200;
                OffsetUpperOpacity = 200;
                OffsetLowerOpacity = 200;
                OffsetOuterUpperOpacity = 200;
                OffsetOuterLowerOpacity = 200;

                AddPlot(Brushes.DodgerBlue, "Midline");
                AddPlot(Brushes.RoyalBlue, "ATR Upper");
                AddPlot(Brushes.RoyalBlue, "ATR Lower");
                AddPlot(Brushes.CornflowerBlue, "Offset Upper");
                AddPlot(Brushes.CornflowerBlue, "Offset Lower");
                AddPlot(Brushes.LightSteelBlue, "Offset Outer Upper");
                AddPlot(Brushes.LightSteelBlue, "Offset Outer Lower");
            }
            else if (State == State.DataLoaded)
            {
                _ema = EMA(Period);
                _atr = ATR(Period);
            }
            else if (State == State.Configure)
            {
                Plots[0].Opacity = MidlineOpacity;
                Plots[1].Opacity = ATRUpperOpacity;
                Plots[2].Opacity = ATRLowerOpacity;
                Plots[3].Opacity = OffsetUpperOpacity;
                Plots[4].Opacity = OffsetLowerOpacity;
                Plots[5].Opacity = OffsetOuterUpperOpacity;
                Plots[6].Opacity = OffsetOuterLowerOpacity;
            }
        }

        protected override void OnBarUpdate()
        {
            if (CurrentBar < Period)
            {
                Midline.Reset();
                ATRUpper.Reset();
                ATRLower.Reset();
                OffsetUpper.Reset();
                OffsetLower.Reset();
                OffsetOuterUpper.Reset();
                OffsetOuterLower.Reset();

                return;
            }

            double mid = _ema[0];
            double rawAtr = _atr[0];
            double offset = rawAtr * OffsetMultiplier;
            double offsetOuter = rawAtr * OffsetOuterMultiplier;

            ATRUpper[0] = mid + rawAtr;
            ATRLower[0] = mid - rawAtr;
            OffsetUpper[0] = mid + offset;
            OffsetLower[0] = mid - offset;
            OffsetOuterUpper[0] = mid + offsetOuter;
            OffsetOuterLower[0] = mid - offsetOuter;
            Values[0][0] = mid;
        }
    }
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private VolatilityBands[] cacheVolatilityBands;
		public VolatilityBands VolatilityBands(int period, double offsetMultiplier, double offsetOuterMultiplier, byte midlineOpacity, byte aTRUpperOpacity, byte aTRLowerOpacity, byte offsetUpperOpacity, byte offsetLowerOpacity, byte offsetOuterUpperOpacity, byte offsetOuterLowerOpacity)
		{
			return VolatilityBands(Input, period, offsetMultiplier, offsetOuterMultiplier, midlineOpacity, aTRUpperOpacity, aTRLowerOpacity, offsetUpperOpacity, offsetLowerOpacity, offsetOuterUpperOpacity, offsetOuterLowerOpacity);
		}

		public VolatilityBands VolatilityBands(ISeries<double> input, int period, double offsetMultiplier, double offsetOuterMultiplier, byte midlineOpacity, byte aTRUpperOpacity, byte aTRLowerOpacity, byte offsetUpperOpacity, byte offsetLowerOpacity, byte offsetOuterUpperOpacity, byte offsetOuterLowerOpacity)
		{
			if (cacheVolatilityBands != null)
				for (int idx = 0; idx < cacheVolatilityBands.Length; idx++)
					if (cacheVolatilityBands[idx] != null && cacheVolatilityBands[idx].Period == period && cacheVolatilityBands[idx].OffsetMultiplier == offsetMultiplier && cacheVolatilityBands[idx].OffsetOuterMultiplier == offsetOuterMultiplier && cacheVolatilityBands[idx].MidlineOpacity == midlineOpacity && cacheVolatilityBands[idx].ATRUpperOpacity == aTRUpperOpacity && cacheVolatilityBands[idx].ATRLowerOpacity == aTRLowerOpacity && cacheVolatilityBands[idx].OffsetUpperOpacity == offsetUpperOpacity && cacheVolatilityBands[idx].OffsetLowerOpacity == offsetLowerOpacity && cacheVolatilityBands[idx].OffsetOuterUpperOpacity == offsetOuterUpperOpacity && cacheVolatilityBands[idx].OffsetOuterLowerOpacity == offsetOuterLowerOpacity && cacheVolatilityBands[idx].EqualsInput(input))
						return cacheVolatilityBands[idx];
			return CacheIndicator<VolatilityBands>(new VolatilityBands(){ Period = period, OffsetMultiplier = offsetMultiplier, OffsetOuterMultiplier = offsetOuterMultiplier, MidlineOpacity = midlineOpacity, ATRUpperOpacity = aTRUpperOpacity, ATRLowerOpacity = aTRLowerOpacity, OffsetUpperOpacity = offsetUpperOpacity, OffsetLowerOpacity = offsetLowerOpacity, OffsetOuterUpperOpacity = offsetOuterUpperOpacity, OffsetOuterLowerOpacity = offsetOuterLowerOpacity }, input, ref cacheVolatilityBands);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.VolatilityBands VolatilityBands(int period, double offsetMultiplier, double offsetOuterMultiplier, byte midlineOpacity, byte aTRUpperOpacity, byte aTRLowerOpacity, byte offsetUpperOpacity, byte offsetLowerOpacity, byte offsetOuterUpperOpacity, byte offsetOuterLowerOpacity)
		{
			return indicator.VolatilityBands(Input, period, offsetMultiplier, offsetOuterMultiplier, midlineOpacity, aTRUpperOpacity, aTRLowerOpacity, offsetUpperOpacity, offsetLowerOpacity, offsetOuterUpperOpacity, offsetOuterLowerOpacity);
		}

		public Indicators.VolatilityBands VolatilityBands(ISeries<double> input , int period, double offsetMultiplier, double offsetOuterMultiplier, byte midlineOpacity, byte aTRUpperOpacity, byte aTRLowerOpacity, byte offsetUpperOpacity, byte offsetLowerOpacity, byte offsetOuterUpperOpacity, byte offsetOuterLowerOpacity)
		{
			return indicator.VolatilityBands(input, period, offsetMultiplier, offsetOuterMultiplier, midlineOpacity, aTRUpperOpacity, aTRLowerOpacity, offsetUpperOpacity, offsetLowerOpacity, offsetOuterUpperOpacity, offsetOuterLowerOpacity);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.VolatilityBands VolatilityBands(int period, double offsetMultiplier, double offsetOuterMultiplier, byte midlineOpacity, byte aTRUpperOpacity, byte aTRLowerOpacity, byte offsetUpperOpacity, byte offsetLowerOpacity, byte offsetOuterUpperOpacity, byte offsetOuterLowerOpacity)
		{
			return indicator.VolatilityBands(Input, period, offsetMultiplier, offsetOuterMultiplier, midlineOpacity, aTRUpperOpacity, aTRLowerOpacity, offsetUpperOpacity, offsetLowerOpacity, offsetOuterUpperOpacity, offsetOuterLowerOpacity);
		}

		public Indicators.VolatilityBands VolatilityBands(ISeries<double> input , int period, double offsetMultiplier, double offsetOuterMultiplier, byte midlineOpacity, byte aTRUpperOpacity, byte aTRLowerOpacity, byte offsetUpperOpacity, byte offsetLowerOpacity, byte offsetOuterUpperOpacity, byte offsetOuterLowerOpacity)
		{
			return indicator.VolatilityBands(input, period, offsetMultiplier, offsetOuterMultiplier, midlineOpacity, aTRUpperOpacity, aTRLowerOpacity, offsetUpperOpacity, offsetLowerOpacity, offsetOuterUpperOpacity, offsetOuterLowerOpacity);
		}
	}
}

#endregion
