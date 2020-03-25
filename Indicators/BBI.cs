#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators
{
	public class BBI : Indicator
	{
		private SMA sma1;
		private SMA sma2;
		private SMA sma3;
		private SMA sma4;
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"4 SMA / 4";
				Name										= "BBI";
				Calculate									= Calculate.OnEachTick;
				IsOverlay									= true;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= true;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= true;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive					= true;
				MA1					= 3;
				MA2					= 6;
				MA3					= 12;
				MA4					= 24;
				AddPlot(Brushes.DeepSkyBlue, "BullBear");
			}
			else if (State == State.Configure)
			{
			}
			
			else if (State == State.DataLoaded)
			{
				// initialize the SMA using the primary series and assign to sma1
				sma1 = SMA(Close,  Convert.ToInt32(MA1));

				sma2 = SMA(Close,  Convert.ToInt32(MA2));
				
				sma3 = SMA(Close,  Convert.ToInt32(MA3));
				
				sma4 = SMA(Close,  Convert.ToInt32(MA4));
			}
		}

		protected override void OnBarUpdate()
		{
			//Add your custom indicator logic here.
			BullBear[0] = (sma1[0] + sma2[0] + sma3[0] + sma4[0]) / 4;
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="MA1", Order=1, GroupName="Parameters")]
		public int MA1
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="MA2", Order=2, GroupName="Parameters")]
		public int MA2
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="MA3", Order=3, GroupName="Parameters")]
		public int MA3
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="MA4", Order=4, GroupName="Parameters")]
		public int MA4
		{ get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> BullBear
		{
			get { return Values[0]; }
		}
		#endregion

	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private BBI[] cacheBBI;
		public BBI BBI(int mA1, int mA2, int mA3, int mA4)
		{
			return BBI(Input, mA1, mA2, mA3, mA4);
		}

		public BBI BBI(ISeries<double> input, int mA1, int mA2, int mA3, int mA4)
		{
			if (cacheBBI != null)
				for (int idx = 0; idx < cacheBBI.Length; idx++)
					if (cacheBBI[idx] != null && cacheBBI[idx].MA1 == mA1 && cacheBBI[idx].MA2 == mA2 && cacheBBI[idx].MA3 == mA3 && cacheBBI[idx].MA4 == mA4 && cacheBBI[idx].EqualsInput(input))
						return cacheBBI[idx];
			return CacheIndicator<BBI>(new BBI(){ MA1 = mA1, MA2 = mA2, MA3 = mA3, MA4 = mA4 }, input, ref cacheBBI);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.BBI BBI(int mA1, int mA2, int mA3, int mA4)
		{
			return indicator.BBI(Input, mA1, mA2, mA3, mA4);
		}

		public Indicators.BBI BBI(ISeries<double> input , int mA1, int mA2, int mA3, int mA4)
		{
			return indicator.BBI(input, mA1, mA2, mA3, mA4);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.BBI BBI(int mA1, int mA2, int mA3, int mA4)
		{
			return indicator.BBI(Input, mA1, mA2, mA3, mA4);
		}

		public Indicators.BBI BBI(ISeries<double> input , int mA1, int mA2, int mA3, int mA4)
		{
			return indicator.BBI(input, mA1, mA2, mA3, mA4);
		}
	}
}

#endregion
