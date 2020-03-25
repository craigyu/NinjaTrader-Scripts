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
using NinjaTrader.NinjaScript.Indicators;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Strategies in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Strategies
{
	public class TimeTwoMACD : Strategy
	{
		private bool HasOrder;

		private TimeLimitedMACD MACD1;
		private TimeLimitedMACD MACD2;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"2 MACD cross over long";
				Name										= "TimeTwoMACD";
				Calculate									= Calculate.OnEachTick;
				EntriesPerDirection							= 1;
				EntryHandling								= EntryHandling.UniqueEntries;
				IsExitOnSessionCloseStrategy				= true;
				ExitOnSessionCloseSeconds					= 30;
				IsFillLimitOnTouch							= false;
				MaximumBarsLookBack							= MaximumBarsLookBack.TwoHundredFiftySix;
				OrderFillResolution							= OrderFillResolution.High;
				OrderFillResolutionType						= BarsPeriodType.Minute;
				OrderFillResolutionValue					= 1;
				Slippage									= 0;
				StartBehavior								= StartBehavior.WaitUntilFlat;
				TimeInForce									= TimeInForce.Gtc;
				TraceOrders									= true;
				RealtimeErrorHandling						= RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling							= StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade							= 20;
				// Disable this property for performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration	= true;
				OneFast					= 12;
				OneSlow					= 26;
				OneSmooth					= 9;
				TwoFast					= 12;
				TwoSlow					= 26;
				TwoSmooth					= 9;
				ProfitTarget					= 20;
				StartTime					= 93000;
				EndTime					= 100000;
				StartTimeString = "09:30";
				EndTimeString = "10:00";
				HasOrder					= false;
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{				

				
				
				
				MACD1				= TimeLimitedMACD(Close, Convert.ToInt32(OneFast), Convert.ToInt32(OneSlow), Convert.ToInt32(OneSmooth)
				, StartTimeString, EndTimeString);
				MACD2				= TimeLimitedMACD(Close, Convert.ToInt32(TwoFast), Convert.ToInt32(TwoSlow), Convert.ToInt32(TwoSmooth),
				StartTimeString, EndTimeString);
			}
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress != 0) 
				return;

			if (CurrentBars[0] < 1)
			return;
			
			
			if(Position.Quantity == 0){
				
				if ((CrossAbove(MACD2.Default, MACD2.Avg, 1))
					|| CrossAbove(MACD1.Default, MACD1.Avg, 1)
					)
				{
					EnterLong(Convert.ToInt32(DefaultQuantity), "");
					Draw.ArrowUp(this, @"BUY_IN Arrow up_MACD2", false, 0, 0, Brushes.Purple);
					
				}	
			}
			else{
				
				if(Position.GetUnrealizedProfitLoss(PerformanceUnit.Points, Close[0]) >= ProfitTarget){
					ExitLong(Convert.ToInt32(DefaultQuantity), "", "");
					Print("PROFIT SOLD");
					return;
				}
				
			    if((CrossBelow(MACD1.Default, MACD1.Avg, 1)) || (CrossBelow(MACD2.Default, MACD2.Avg, 1))){
					ExitLong(Convert.ToInt32(DefaultQuantity), "", "");
					Print("BELOW SOLD");
					return;
				}
				
				if(ToTime(Time[0]) > EndTime){
					ExitLong(Convert.ToInt32(DefaultQuantity), "", "");
					Print("TIME OUT SOLD");
					return;
				}
				
			}
			
		
			
		

//			 // Set 1
//			if ((CrossAbove(MACD1.Default, MACD1.Avg, 1))
//				 && (HasOrder == false))
//			{
//				EnterLong(Convert.ToInt32(DefaultQuantity), "");
//				HasOrder = true;
//				Draw.ArrowUp(this, @"BUY_IN Arrow up_MACD1", false, 0, 0, Brushes.DodgerBlue);
//			}
			
//			 // Set 2
//			if ((CrossAbove(MACD2.Default, MACD2.Avg, 1))
//				 && (HasOrder == false))
//			{
//				EnterLong(Convert.ToInt32(DefaultQuantity), "");
//				HasOrder = true;
//				Draw.ArrowUp(this, @"BUY_IN Arrow up_MACD2", false, 0, 0, Brushes.Purple);
//			}
			
//			 // Set 3
//			if ((HasOrder == true)
//				 // CrossBelow
//				 && ((CrossBelow(MACD1.Default, MACD1.Avg, 1))
//				 || (CrossBelow(MACD2.Default, MACD2.Avg, 1))))
//			{
//				ExitLong(Convert.ToInt32(DefaultQuantity), "", "");
//				HasOrder = false;
//				Draw.ArrowDown(this, @"FLAT Arrow down_1", false, 0, 0, Brushes.Red);
//			}
			
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="OneFast", Description="MACD One's Fast", Order=1, GroupName="Parameters")]
		public int OneFast
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="OneSlow", Description="MACD One's Slow", Order=2, GroupName="Parameters")]
		public int OneSlow
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="OneSmooth", Description="MACD One's Smooth", Order=3, GroupName="Parameters")]
		public int OneSmooth
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="TwoFast", Description="MACD Two's Fast", Order=4, GroupName="Parameters")]
		public int TwoFast
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="TwoSlow", Description="MACD Two's Slow", Order=5, GroupName="Parameters")]
		public int TwoSlow
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="TwoSmooth", Description="MACD Two's Smooth", Order=6, GroupName="Parameters")]
		public int TwoSmooth
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="ProfitTarget", Order=7, GroupName="Parameters")]
		public int ProfitTarget
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="EndTime", Order=8, GroupName="Parameters")]
		public int EndTime
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="StartTime", Order=9, GroupName="Parameters")]
		public int StartTime
		{ get; set; }
		
		
				[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "StartTime", GroupName = "NinjaScriptParameters", Order = 10)]
		public string StartTimeString
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "EndTime", GroupName = "NinjaScriptParameters", Order = 11)]
		public string EndTimeString
		{ get; set; }
		#endregion

	}
}
