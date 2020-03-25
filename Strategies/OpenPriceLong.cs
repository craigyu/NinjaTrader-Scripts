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
	public class OpenPriceLong : Strategy
	{
		private int OrderNum;
		//private bool WaitTillNext;
		private bool HasCrossedBelow;
		private int CurrentStage;
		private bool ProfitOneListed;
		private bool ProfitTwoListed;
		private bool ProfitThreeListed;

		private CurrentDayOHL CurrentDayOHL1;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Use open price as an indicator to long or short";
				Name										= "OpenPriceLong";
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
				ProfitTarget_1					= 2;
				ProfitTarget_2					= 5;
				ProfitTarget_3					= 12;
				ProfitOneListed					= true;
				ProfitTwoListed					= true;
				ProfitThreeListed			    = true;
				OrderNum					= 0;
				CurrentStage				= 0;
				//WaitTillNext					= false;
				HasCrossedBelow					= true;
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{				
				CurrentDayOHL1				= CurrentDayOHL(Close);
			}
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress != 0) 
				return;

			if (CurrentBars[0] < 1)
			return;
			
			int counter = 0;

			 // Set 1
			if (Close[0] > CurrentDayOHL1.CurrentOpen[0])
			{	
				if((Position.Quantity == 0) && HasCrossedBelow){
					EnterLong(Convert.ToInt32(3), "");
					Draw.ArrowUp(this, @"OpenPriceLong Arrow up_1", false, 0, 0, Brushes.Red);
					ProfitOneListed	= false;
					ProfitTwoListed	= false;
					ProfitThreeListed = false;
					counter = 0;
				}
				
				else if((Position.Quantity == 3) && (Close[0] >= CurrentDayOHL1.CurrentOpen[0] + ProfitTarget_1) && !ProfitOneListed){
					ProfitOneListed = true;
					ExitLong(Convert.ToInt32(1), "", "");
				}
				else if((Position.Quantity == 2) && (Close[0] >= CurrentDayOHL1.CurrentOpen[0] + ProfitTarget_2) && !ProfitTwoListed){
					ProfitTwoListed = true;
					ExitLong(Convert.ToInt32(1), "", "");
				}
				else if((Position.Quantity == 1) && (Close[0] >= CurrentDayOHL1.CurrentOpen[0] + ProfitTarget_3) && !ProfitThreeListed){
					ProfitThreeListed = true;
					ExitLong(Convert.ToInt32(1), "", "");
					HasCrossedBelow = false;
				}
			}
			
			
			
			// Set 6
			if (Close[0] < CurrentDayOHL1.CurrentOpen[0]) /*&& (CurrentStage == 4)*/
			{
				if(Position.Quantity > 0){
					Print("Bid Price < Open Price, Exit Long by Position size");
					ExitLong(Convert.ToInt32(Position.Quantity), "", "");
					Print("Remaining Position Size: " + Position.Quantity);
				}
				HasCrossedBelow = true;
			}
			
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="ProfitTarget_1", Order=1, GroupName="Parameters")]
		public int ProfitTarget_1
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="ProfitTarget_2", Order=2, GroupName="Parameters")]
		public int ProfitTarget_2
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="ProfitTarget_3", Order=3, GroupName="Parameters")]
		public int ProfitTarget_3
		{ get; set; }
		#endregion

	}
}
