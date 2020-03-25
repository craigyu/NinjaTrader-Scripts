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
	public class ThreeEMA : Strategy
	{
		private EMA EMA1;
		private CurrentDayOHL CurrentDayOHL1;
		private EMA EMA2;
		private EMA EMA3;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Strategy here.";
				Name										= "ThreeEMA";
				Calculate									= Calculate.OnEachTick;
				EntriesPerDirection							= 1;
				EntryHandling								= EntryHandling.AllEntries;
				IsExitOnSessionCloseStrategy				= true;
				ExitOnSessionCloseSeconds					= 30;
				IsFillLimitOnTouch							= false;
				MaximumBarsLookBack							= MaximumBarsLookBack.TwoHundredFiftySix;
				OrderFillResolution							= OrderFillResolution.Standard;
				Slippage									= 0;
				StartBehavior								= StartBehavior.WaitUntilFlat;
				TimeInForce									= TimeInForce.Gtc;
				TraceOrders									= false;
				RealtimeErrorHandling						= RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling							= StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade							= 20;
				// Disable this property for performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration	= true;
				EMA_Period1					= 5;
				EMA_Period2					= 30;
				EMA_Period3					= 60;
				Profit_Target					= 10;
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{				
				EMA1				= EMA(Close, Convert.ToInt32(EMA_Period3));
				CurrentDayOHL1				= CurrentDayOHL(Close);
				EMA2				= EMA(Close, Convert.ToInt32(EMA_Period1));
				EMA3				= EMA(Close, Convert.ToInt32(EMA_Period2));
			}
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress != 0) 
				return;

			if (CurrentBars[0] < 1)
			return;

			 // Set 1
			if (CrossAbove(EMA1, CurrentDayOHL1.CurrentOpen, 1) && Position.Quantity == 0)
			{
				EnterLong(Convert.ToInt32(DefaultQuantity), "");
			}
			
			 // Set 2
			if ((CrossBelow(EMA2, CurrentDayOHL1.CurrentOpen, 1)
				 || (CrossBelow(EMA3, CurrentDayOHL1.CurrentOpen, 1))) && Position.Quantity != 0)
			{
				ExitLong(Convert.ToInt32(DefaultQuantity), "", "");
			}
			
			if(Position.Quantity != 0 && Close[0] >= Position.AveragePrice + Profit_Target){
				ExitLong(Convert.ToInt32(DefaultQuantity), "", "");
			}
			
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="EMA_Period1", Order=1, GroupName="Parameters")]
		public int EMA_Period1
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="EMA_Period2", Order=2, GroupName="Parameters")]
		public int EMA_Period2
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="EMA_Period3", Order=3, GroupName="Parameters")]
		public int EMA_Period3
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Profit_Target", Order=4, GroupName="Parameters")]
		public int Profit_Target
		{ get; set; }
		#endregion

	}
}
