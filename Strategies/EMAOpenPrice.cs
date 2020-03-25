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
	public class EMAOpenPrice : Strategy
	{
		private int OrderNum;
		//private bool WaitTillNext;
		private bool HasCrossedBelow;
		private int CurrentStage;
		private bool ProfitOneListed;
		private bool ProfitTwoListed;
		private bool ProfitThreeListed;
		private CurrentDayOHL CurrentDayOHL1;
		private EMA EMA1;
		private MACD MACD1;
		private bool BoughtWithOpen;
		private bool BoughtWithOpen2;
		private bool BoughtWithOpen3;
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Strategy here.";
				Name										= "EMAOpenPrice";
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
				ProfitTarget_1					= 3;
				ProfitTarget_2					= 100;
				ProfitTarget_3					= 12;
				ProfitOneListed					= true;
				ProfitTwoListed					= true;
				ProfitThreeListed			    = true;
				EMAPeriod					= 2;
				OrderNum					= 0;
				CurrentStage				= 0;
				//WaitTillNext					= false;
				HasCrossedBelow					= true;
				BoughtWithOpen				= false;
				OneFast					= 2;
				OneSlow					= 10;
				OneSmooth					= 10;
				OpenPriceOffset				= 7;
				OpenPriceOffset2				= 4;
				BoughtWithOpen2 			= false;
				BoughtWithOpen3 			= false;
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{				
				CurrentDayOHL1				= CurrentDayOHL(Close);
				EMA1				= EMA(Close, Convert.ToInt32(EMAPeriod));
				MACD1				= MACD(Close, Convert.ToInt32(OneFast), Convert.ToInt32(OneSlow), Convert.ToInt32(OneSmooth));

			}
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress != 0) 
				return;

			if (CurrentBars[0] < 1)
			return;

			 // Set 1
			if ((EMA1[0] > CurrentDayOHL1.CurrentOpen[0] 
				|| EMA1[0] > CurrentDayOHL1.CurrentOpen[0] + OpenPriceOffset
				|| EMA1[0] > CurrentDayOHL1.CurrentOpen[0] + OpenPriceOffset2
				) 
				&& (MACD1.Default[0] > MACD1.Avg[0]))
			{	
				if((Position.Quantity == 0) && HasCrossedBelow){
					EnterLong(Convert.ToInt32(3), "");
					ProfitOneListed	= false;
					ProfitTwoListed	= false;
					ProfitThreeListed = false;
					
					if(EMA1[0] < CurrentDayOHL1.CurrentOpen[0] + OpenPriceOffset){
						BoughtWithOpen = true;
					}
					else BoughtWithOpen = false;
					
					if(EMA1[0] >= CurrentDayOHL1.CurrentOpen[0] + OpenPriceOffset  && EMA1[0] < CurrentDayOHL1.CurrentOpen[0] + OpenPriceOffset2){
						BoughtWithOpen2 = true;
					}
					else BoughtWithOpen2 = false;
					
					if(EMA1[0] >= CurrentDayOHL1.CurrentOpen[0] + OpenPriceOffset  && EMA1[0] < CurrentDayOHL1.CurrentOpen[0] + OpenPriceOffset2){
						BoughtWithOpen3 = true;
					}
					else BoughtWithOpen3 = false;
					
				}
				
				else if((Position.Quantity == 3) && (Close[0] >= Position.AveragePrice + ProfitTarget_1) && !ProfitOneListed){
					ProfitOneListed = true;
					ExitLong(Convert.ToInt32(1), "", "");
				}
				else if((Position.Quantity == 2) && (Close[0] >= Position.AveragePrice + ProfitTarget_2) && !ProfitTwoListed){
					ProfitTwoListed = true;
					ExitLong(Convert.ToInt32(1), "", "");
				}
				else if((Position.Quantity == 1) && (Close[0] >= Position.AveragePrice + ProfitTarget_3) && !ProfitThreeListed){
					ProfitThreeListed = true;
					ExitLong(Convert.ToInt32(1), "", "");
					HasCrossedBelow = false;
				}
			}
			
			
			if(BoughtWithOpen && EMA1[0] >= CurrentDayOHL1.CurrentOpen[0] + OpenPriceOffset){
				BoughtWithOpen = false;
			}
			
			if(BoughtWithOpen2 && EMA1[0] >= CurrentDayOHL1.CurrentOpen[0] + OpenPriceOffset2){
				BoughtWithOpen2 = false;
			}
			
			
			// Set 6
			if ((EMA1[0] < CurrentDayOHL1.CurrentOpen[0]) && (MACD1.Default[0] < MACD1.Avg[0])) /*&& (CurrentStage == 4)*/
			{
				if(Position.Quantity > 0){
					Print("Bid Price < Open Price, Exit Long by Position size");
					ExitLong(Convert.ToInt32(Position.Quantity), "", "");
					Print("Remaining Position Size: " + Position.Quantity);
				}
				HasCrossedBelow = true;
			}
			
			if(EMA1[0] < CurrentDayOHL1.CurrentOpen[0] + OpenPriceOffset && (MACD1.Default[0] < MACD1.Avg[0]) && !BoughtWithOpen){
					if(Position.Quantity > 0){
					Print("Bid Price < Open Price, Exit Long by Position size");
					ExitLong(Convert.ToInt32(Position.Quantity), "", "");
					Print("Remaining Position Size: " + Position.Quantity);
				}
					HasCrossedBelow = true;
			}
			
			if(EMA1[0] < CurrentDayOHL1.CurrentOpen[0] + OpenPriceOffset2 && (MACD1.Default[0] < MACD1.Avg[0]) && !BoughtWithOpen2){
					if(Position.Quantity > 0){
					Print("Bid Price < Open Price, Exit Long by Position size");
					ExitLong(Convert.ToInt32(Position.Quantity), "", "");
					Print("Remaining Position Size: " + Position.Quantity);
				}
					HasCrossedBelow = true;
			}
			
//				if(EMA1[0] < CurrentDayOHL1.CurrentOpen[0] + OpenPriceOffset3 && (MACD1.Default[0] < MACD1.Avg[0]) && !BoughtWithOpen3){
//					if(Position.Quantity > 0){
//					Print("Bid Price < Open Price, Exit Long by Position size");
//					ExitLong(Convert.ToInt32(Position.Quantity), "", "");
//					Print("Remaining Position Size: " + Position.Quantity);
//				}
//					HasCrossedBelow = true;
//			}
			
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
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="EMAPeriod", Order=4, GroupName="Parameters")]
		public int EMAPeriod
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="OneFast", Description="MACD One's Fast", Order=5, GroupName="Parameters")]
		public int OneFast
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="OneSlow", Description="MACD One's Slow", Order=6, GroupName="Parameters")]
		public int OneSlow
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="OneSmooth", Description="MACD One's Smooth", Order=7, GroupName="Parameters")]
		public int OneSmooth
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="OpenPriceOffset", Description="MACD One's Slow", Order=9, GroupName="Parameters")]
		public int OpenPriceOffset
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="OpenPriceOffset", Description="MACD One's Slow", Order=9, GroupName="Parameters")]
		public int OpenPriceOffset2
		{ get; set; }
		#endregion
	}
}
