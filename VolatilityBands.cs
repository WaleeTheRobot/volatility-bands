#region Using declarations
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Media;
using System.Xml.Serialization;
#endregion

//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators
{
    public class VolatilityBands : Indicator
    {
        private EMA _ema;
        private ATR _atr;

        #region General Properties

        [NinjaScriptProperty]
        [Display(Name = "Version", Description = "Volatility Bands version.", Order = 0, GroupName = "General")]
        [ReadOnly(true)]
        public string Version
        {
            get { return "1.0.0"; }
            set { }
        }

        #endregion

        #region General Properties

        [Range(1, int.MaxValue), NinjaScriptProperty]
        [Display(Name = "Period", GroupName = "VolatilityBands", Order = 0)]
        public int Period
        { get; set; }

        [Range(0.01, double.MaxValue), NinjaScriptProperty]
        [Display(Name = "OffsetMultiplier", GroupName = "VolatilityBands", Order = 1)]
        public double OffsetMultiplier
        { get; set; }

        [Range(0.01, double.MaxValue), NinjaScriptProperty]
        [Display(Name = "OffsetOuterMultiplier", GroupName = "VolatilityBands", Order = 2)]
        public double OffsetOuterMultiplier
        { get; set; }

        #endregion

        #region Plots

        [Browsable(false)]
        [XmlIgnore]
        public Series<double> Midline
        {
            get { return Values[0]; }
        }

        [Browsable(false)]
        [XmlIgnore]
        public Series<double> ATRUpper
        {
            get { return Values[1]; }
        }

        [Browsable(false)]
        [XmlIgnore]
        public Series<double> ATRLower
        {
            get { return Values[2]; }
        }

        [Browsable(false)]
        [XmlIgnore]
        public Series<double> OffsetUpper
        {
            get { return Values[3]; }
        }

        [Browsable(false)]
        [XmlIgnore]
        public Series<double> OffsetLower
        {
            get { return Values[4]; }
        }

        [Browsable(false)]
        [XmlIgnore]
        public Series<double> OffsetOuterUpper
        {
            get { return Values[5]; }
        }

        [Browsable(false)]
        [XmlIgnore]
        public Series<double> OffsetOuterLower
        {
            get { return Values[6]; }
        }

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
		public VolatilityBands VolatilityBands(string version, int period, double offsetMultiplier, double offsetOuterMultiplier)
		{
			return VolatilityBands(Input, version, period, offsetMultiplier, offsetOuterMultiplier);
		}

		public VolatilityBands VolatilityBands(ISeries<double> input, string version, int period, double offsetMultiplier, double offsetOuterMultiplier)
		{
			if (cacheVolatilityBands != null)
				for (int idx = 0; idx < cacheVolatilityBands.Length; idx++)
					if (cacheVolatilityBands[idx] != null && cacheVolatilityBands[idx].Version == version && cacheVolatilityBands[idx].Period == period && cacheVolatilityBands[idx].OffsetMultiplier == offsetMultiplier && cacheVolatilityBands[idx].OffsetOuterMultiplier == offsetOuterMultiplier && cacheVolatilityBands[idx].EqualsInput(input))
						return cacheVolatilityBands[idx];
			return CacheIndicator<VolatilityBands>(new VolatilityBands(){ Version = version, Period = period, OffsetMultiplier = offsetMultiplier, OffsetOuterMultiplier = offsetOuterMultiplier }, input, ref cacheVolatilityBands);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.VolatilityBands VolatilityBands(string version, int period, double offsetMultiplier, double offsetOuterMultiplier)
		{
			return indicator.VolatilityBands(Input, version, period, offsetMultiplier, offsetOuterMultiplier);
		}

		public Indicators.VolatilityBands VolatilityBands(ISeries<double> input , string version, int period, double offsetMultiplier, double offsetOuterMultiplier)
		{
			return indicator.VolatilityBands(input, version, period, offsetMultiplier, offsetOuterMultiplier);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.VolatilityBands VolatilityBands(string version, int period, double offsetMultiplier, double offsetOuterMultiplier)
		{
			return indicator.VolatilityBands(Input, version, period, offsetMultiplier, offsetOuterMultiplier);
		}

		public Indicators.VolatilityBands VolatilityBands(ISeries<double> input , string version, int period, double offsetMultiplier, double offsetOuterMultiplier)
		{
			return indicator.VolatilityBands(input, version, period, offsetMultiplier, offsetOuterMultiplier);
		}
	}
}

#endregion
