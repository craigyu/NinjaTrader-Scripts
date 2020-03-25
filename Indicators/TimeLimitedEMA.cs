//
// Copyright (C) 2019, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//
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
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

// This namespace holds indicators in this folder and is required. Do not change it.
namespace NinjaTrader.NinjaScript.Indicators
{
	/// <summary>
	/// Exponential Moving Average. The Exponential Moving Average is an indicator that
	/// shows the average value of a security's price over a period of time. When calculating
	/// a moving average. The EMA applies more weight to recent prices than the SMA.
	/// </summary>
	public class TimeLimitedEMA : Indicator
	{
		private double constant1;
		private double constant2;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description					= NinjaTrader.Custom.Resource.NinjaScriptIndicatorDescriptionEMA;
				Name						= "TimeLimitedEMA";
				IsOverlay					= true;
				IsSuspendedWhileInactive	= true;
				Period						= 14;
				StartTime = "09:30";
				EndTime = "10:00";

				AddPlot(Brushes.Goldenrod, NinjaTrader.Custom.Resource.NinjaScriptIndicatorNameEMA);
			}
			else if (State == State.Configure)
			{
				constant1 = 2.0 / (1 + Period);
				constant2 = 1 - (2.0 / (1 + Period));
			}
		}

		protected override void OnBarUpdate()
		{
			DateTime StartSession = DateTime.Parse(StartTime, System.Globalization.CultureInfo.InvariantCulture);
			DateTime EndSession = DateTime.Parse(EndTime, System.Globalization.CultureInfo.InvariantCulture);
			if ( Times[0][0].TimeOfDay < StartSession.TimeOfDay || Times[0][0].TimeOfDay > EndSession.TimeOfDay)
			{
			return;
			}
			Value[0] = (CurrentBar == 0 ? Input[0] : Input[0] * constant1 + constant2 * Value[1]);
		}

		#region Properties
		[Range(1, int.MaxValue), NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Period", GroupName = "NinjaScriptParameters", Order = 0)]
		public int Period
		{ get; set; }
		
				
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "StartTime", GroupName = "NinjaScriptParameters", Order = 1)]
		public string StartTime
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "EndTime", GroupName = "NinjaScriptParameters", Order = 2)]
		public string EndTime
		{ get; set; }
		#endregion
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private TimeLimitedEMA[] cacheTimeLimitedEMA;
		public TimeLimitedEMA TimeLimitedEMA(int period, string startTime, string endTime)
		{
			return TimeLimitedEMA(Input, period, startTime, endTime);
		}

		public TimeLimitedEMA TimeLimitedEMA(ISeries<double> input, int period, string startTime, string endTime)
		{
			if (cacheTimeLimitedEMA != null)
				for (int idx = 0; idx < cacheTimeLimitedEMA.Length; idx++)
					if (cacheTimeLimitedEMA[idx] != null && cacheTimeLimitedEMA[idx].Period == period && cacheTimeLimitedEMA[idx].StartTime == startTime && cacheTimeLimitedEMA[idx].EndTime == endTime && cacheTimeLimitedEMA[idx].EqualsInput(input))
						return cacheTimeLimitedEMA[idx];
			return CacheIndicator<TimeLimitedEMA>(new TimeLimitedEMA(){ Period = period, StartTime = startTime, EndTime = endTime }, input, ref cacheTimeLimitedEMA);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.TimeLimitedEMA TimeLimitedEMA(int period, string startTime, string endTime)
		{
			return indicator.TimeLimitedEMA(Input, period, startTime, endTime);
		}

		public Indicators.TimeLimitedEMA TimeLimitedEMA(ISeries<double> input , int period, string startTime, string endTime)
		{
			return indicator.TimeLimitedEMA(input, period, startTime, endTime);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.TimeLimitedEMA TimeLimitedEMA(int period, string startTime, string endTime)
		{
			return indicator.TimeLimitedEMA(Input, period, startTime, endTime);
		}

		public Indicators.TimeLimitedEMA TimeLimitedEMA(ISeries<double> input , int period, string startTime, string endTime)
		{
			return indicator.TimeLimitedEMA(input, period, startTime, endTime);
		}
	}
}

#endregion
