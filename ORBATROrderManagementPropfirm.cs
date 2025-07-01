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
	public class ORBATROrderManagement2 : Strategy
	{
		private double StoreProfit1;
		private double StoreProfit2;
		private double StoreStop;
		private bool InitialEntry;
		private double StoreTrailTarget;
		private double StoreTrailStop;
		private bool TriggerTrailStop;
		private bool TrailStopUpdate;
		private double OrbHigh;
		private double OrbLow;

		private SMA SMA1;
		private ADX ADX1;
		private EMA EMA1;
		private EMA EMA2;
		private EMA EMA3;
		private ADX ADX2;
		private RSI RSI1;
		private ATR ATR1;
		private NinjaTrader.NinjaScript.Indicators.TradeSaber.ORB_TradeSaber ORB_TradeSaber1;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Strategy here.";
				Name										= "ORBATROrderManagement2";
				Calculate									= Calculate.OnEachTick;
				EntriesPerDirection							= 2;
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
				ATRPeriod					= 14;
				QTY1					= 1;
				QTY2					= 2;
				ProfitMultiplier1					= 1;
				ProfitMultiplier2					= 5;
				StopMultiplier					= 2;
				TrailTargetStart					= 0.7;
				TrailStopMultiplier					= 2.5;
				StartTime						= DateTime.Parse("08:45", System.Globalization.CultureInfo.InvariantCulture);
				StopTime						= DateTime.Parse("10:00", System.Globalization.CultureInfo.InvariantCulture);
				ADXPeriod					= 23;
				TickOffsetOrbHigh					= 2;
				TicjOffsetOrbLow					= -2;
				StoreProfit1					= 1;
				StoreProfit2					= 1;
				StoreStop					= 1;
				InitialEntry					= false;
				StoreTrailTarget					= 1;
				StoreTrailStop					= 1;
				TriggerTrailStop					= false;
				TrailStopUpdate					= false;
				OrbHigh					= 1;
				OrbLow					= 1;
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{				
				SMA1				= SMA(Close, 50);
				ADX1				= ADX(Close, 14);
				EMA1				= EMA(Close, 8);
				EMA2				= EMA(Close, 10);
				EMA3				= EMA(Close, 20);
				ADX2				= ADX(Close, Convert.ToInt32(ATRPeriod));
				RSI1				= RSI(Close, 14, 3);
				ATR1				= ATR(Close, Convert.ToInt32(ATRPeriod));
				ORB_TradeSaber1				= ORB_TradeSaber(Close, true, 0.2, DateTime.Parse("8:30 AM"), DateTime.Parse("8:45 AM"), @"Local", @"Recommended that PC and 
NinjaTrader clocks match", true, @"AboveUpper", @"BelowLower", Brushes.SteelBlue, Brushes.DarkOrange, false, @"AboveLower", @"BelowUpper", Brushes.LightGreen, Brushes.Pink, false, @"AboveMidline", @"BelowMidline", Brushes.Aqua, Brushes.Fuchsia, false, @"AboveUpperQuarter", @"BelowUpperQuarter", Brushes.Aqua, Brushes.Crimson, false, @"AboveLowerQuarter", @"BelowLowerQuarter", Brushes.Lime, Brushes.OrangeRed, false, @"AboveTarget1Upper", @"BelowTarget1Upper", Brushes.DodgerBlue, Brushes.Tomato, false, @"BelowTarget1Lower", @"AboveTarget1Lower", Brushes.Tomato, Brushes.DodgerBlue, false, @"AboveTarget2Upper", @"BelowTarget2Upper", Brushes.DeepSkyBlue, Brushes.DarkRed, false, @"AboveTarget2Lower", @"BelowTarget2Lower", Brushes.DeepSkyBlue, Brushes.DarkRed, false, false, @"SessionStartUp", @"SessionStartDown", Brushes.LightGreen, Brushes.Pink, false, 0.5, false, 0.25, false, 1, false, 2, false, @"TradeSaber - Built With Grok", @"Version 1.3 // April 2025", @"https://tradesaber.com/predator-guide/", @"https://Discord.gg/2YU9GDme8j", @"https://youtu.be/jUYT-Erzc_8");
				SMA1.Plots[0].Brush = Brushes.Goldenrod;
				ATR1.Plots[0].Brush = Brushes.DarkCyan;
				ORB_TradeSaber1.Plots[0].Brush = Brushes.SteelBlue;
				ORB_TradeSaber1.Plots[1].Brush = Brushes.DarkOrange;
				ORB_TradeSaber1.Plots[2].Brush = Brushes.Transparent;
				ORB_TradeSaber1.Plots[3].Brush = Brushes.Gray;
				ORB_TradeSaber1.Plots[4].Brush = Brushes.LightBlue;
				ORB_TradeSaber1.Plots[5].Brush = Brushes.Aqua;
				ORB_TradeSaber1.Plots[6].Brush = Brushes.LightSalmon;
				ORB_TradeSaber1.Plots[7].Brush = Brushes.Salmon;
				ORB_TradeSaber1.Plots[8].Brush = Brushes.DimGray;
				ORB_TradeSaber1.Plots[9].Brush = Brushes.DimGray;
				AddChartIndicator(SMA1);
				AddChartIndicator(ATR1);
				AddChartIndicator(ORB_TradeSaber1);
			}
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress != 0) 
				return;

			if (CurrentBars[0] < 1)
				return;

			 // Set 1
			if ((Position.MarketPosition == MarketPosition.Flat)
				 // Long Entry Condition
				 && ((Close[0] > SMA1[0])
				 && (ADX1[0] >= ADXPeriod)
				 && (IsRising(EMA1) == true)
				 && (CrossAbove(Typical, OrbHigh, 1))
				 && (EMA2[0] > EMA3[0])
				 && (EMA2[0] > SMA1[0]))
				 && (InitialEntry == false)
				 && (TriggerTrailStop == false)
				 // Time
				 && ((Times[0][0].TimeOfDay >= StartTime.TimeOfDay)
				 && (Times[0][0].TimeOfDay < StopTime.TimeOfDay))
				 // Confluence
				 && ((IsRising(ADX2) == true)
				 && (IsRising(RSI1.Default) == true)
				 && (RSI1.Avg[0] >= 30)))
			{
				EnterLong(Convert.ToInt32(QTY1), @"LongEntry1");
				EnterLong(Convert.ToInt32(QTY2), @"LongEntry2");
				StoreProfit1 = (Close[0] + ((ATR1[0] * ProfitMultiplier1) )) ;
				StoreProfit2 = (Close[0] + ((ATR1[0] * ProfitMultiplier2) )) ;
				StoreStop = (Close[0] - ((ATR1[0] * StopMultiplier) )) ;
				InitialEntry = true;
				StoreTrailTarget = (Close[0] + ((ATR1[0] * TrailTargetStart) )) ;
				StoreTrailStop = StoreStop;
				TriggerTrailStop = false;
				TrailStopUpdate = false;
				Print(@"Set 1:  " + Convert.ToString(Times[0][0].TimeOfDay));
			}
			
			 // Set 2
			if ((Position.MarketPosition == MarketPosition.Long)
				 // Condition group 1
				 && ((IsFirstTickOfBar == true)
				 || (InitialEntry == true))
				 && (TriggerTrailStop == false))
			{
				ExitLongLimit(Convert.ToInt32(QTY1), StoreProfit1, @"LongProfit1", @"LongEntry1");
				ExitLongLimit(Convert.ToInt32(QTY2), StoreProfit2, @"LongProfit2", @"LongEntry2");
				ExitLongStopMarket(Convert.ToInt32(QTY1), StoreStop, @"LongStop1", @"LongEntry1");
				ExitLongStopMarket(Convert.ToInt32(QTY2), StoreStop, @"LongStop2", @"LongEntry2");
				InitialEntry = false;
				Print(@"Set 2:  " + Convert.ToString(Times[0][0].TimeOfDay));
			}
			
			 // Set 3
			if ((Position.MarketPosition == MarketPosition.Long)
				 && (Close[0] >= StoreTrailTarget)
				 && (InitialEntry == false)
				 // Condition group 1
				 && ((IsFirstTickOfBar == true)
				 || (TrailStopUpdate == false))
				 && ((Close[0] - ((ATR1[0] * TrailStopMultiplier) ))  > StoreTrailStop))
			{
				StoreTrailStop = (Close[0] - ((ATR1[0] * TrailStopMultiplier) )) ;
				TriggerTrailStop = true;
				Print(@"Set 3:  " + Convert.ToString(Times[0][0].TimeOfDay));
			}
			
			 // Set 4
			if ((Position.MarketPosition == MarketPosition.Long)
				 && (InitialEntry == false)
				 && (TriggerTrailStop == true)
				 // Condition group 1
				 && ((IsFirstTickOfBar == true)
				 || (TrailStopUpdate == false)))
			{
				ExitLongLimit(Convert.ToInt32(QTY1), StoreProfit1, @"LongProfit1", @"LongEntry1");
				ExitLongLimit(Convert.ToInt32(QTY2), StoreProfit2, @"LongProfit2", @"LongEntry2");
				ExitLongStopMarket(Convert.ToInt32(QTY1), StoreTrailStop, @"LongStop1", @"LongEntry1");
				ExitLongStopMarket(Convert.ToInt32(QTY2), StoreTrailStop, @"LongStop2", @"LongEntry2");
				TrailStopUpdate = true;
				Print(@"Set 4:  " + Convert.ToString(Times[0][0].TimeOfDay));
			}
			
			 // Set 5
			if ((Position.MarketPosition == MarketPosition.Flat)
				 // Short Entry Condition
				 && ((Close[0] < SMA1[0])
				 && (ADX1[0] >= ADXPeriod)
				 && (IsFalling(EMA1) == true)
				 && (CrossBelow(Typical, OrbLow, 1))
				 && (EMA2[0] < EMA3[0])
				 && (EMA2[0] < SMA1[0]))
				 && (InitialEntry == false)
				 && (TriggerTrailStop == false)
				 // Time
				 && ((Times[0][0].TimeOfDay >= StartTime.TimeOfDay)
				 && (Times[0][0].TimeOfDay < StopTime.TimeOfDay))
				 // Confluence
				 && ((IsRising(ADX2) == true)
				 && (IsFalling(RSI1.Default) == true)
				 && (RSI1.Avg[0] <= 70)))
			{
				EnterShort(Convert.ToInt32(QTY1), @"ShortEntry1");
				EnterShort(Convert.ToInt32(QTY2), @"ShortEntry2");
				StoreProfit1 = (Close[0] - ((ATR1[0] * ProfitMultiplier1) )) ;
				StoreProfit2 = (Close[0] - ((ATR1[0] * ProfitMultiplier2) )) ;
				StoreStop = (Close[0] + ((ATR1[0] * StopMultiplier) )) ;
				InitialEntry = true;
				StoreTrailTarget = (Close[0] - ((ATR1[0] * TrailTargetStart) )) ;
				StoreTrailStop = StoreStop;
				TriggerTrailStop = false;
				TrailStopUpdate = false;
				Print(@"Set 5:  " + Convert.ToString(Times[0][0].TimeOfDay));
			}
			
			 // Set 6
			if ((Position.MarketPosition == MarketPosition.Short)
				 // Condition group 1
				 && ((IsFirstTickOfBar == true)
				 || (InitialEntry == true))
				 && (TriggerTrailStop == false))
			{
				ExitShortLimit(Convert.ToInt32(QTY1), StoreProfit1, @"ShortProfit1", @"ShortEntry1");
				ExitShortLimit(Convert.ToInt32(QTY2), StoreProfit2, @"ShortProfit2", @"ShortEntry2");
				ExitShortStopMarket(Convert.ToInt32(QTY1), StoreStop, @"ShortStop1", @"ShortEntry1");
				ExitShortStopMarket(Convert.ToInt32(QTY2), StoreStop, @"ShortStop2", @"ShortEntry2");
				InitialEntry = false;
				Print(@"Set 6:  " + Convert.ToString(Times[0][0].TimeOfDay));
			}
			
			 // Set 7
			if ((Position.MarketPosition == MarketPosition.Short)
				 && (Close[0] <= StoreTrailTarget)
				 && (InitialEntry == false)
				 // Condition group 1
				 && ((IsFirstTickOfBar == true)
				 || (TrailStopUpdate == false))
				 && ((Close[0] + ((ATR1[0] * TrailStopMultiplier) ))  < StoreTrailStop))
			{
				StoreTrailStop = (Close[0] + ((ATR1[0] * TrailStopMultiplier) )) ;
				TriggerTrailStop = true;
				Print(@"Set 7:  " + Convert.ToString(Times[0][0].TimeOfDay));
			}
			
			 // Set 8
			if ((Position.MarketPosition == MarketPosition.Short)
				 && (InitialEntry == false)
				 && (TriggerTrailStop == true)
				 // Condition group 1
				 && ((IsFirstTickOfBar == true)
				 || (TrailStopUpdate == false)))
			{
				TrailStopUpdate = true;
				ExitShortLimit(Convert.ToInt32(QTY1), StoreProfit1, @"ShortProfit1", @"ShortEntry1");
				ExitShortLimit(Convert.ToInt32(QTY2), StoreProfit2, @"ShortProfit2", @"ShortEntry2");
				ExitShortStopMarket(Convert.ToInt32(QTY1), StoreTrailStop, @"ShortStop1", @"ShortEntry1");
				ExitShortStopMarket(Convert.ToInt32(QTY2), StoreTrailStop, @"ShortStop2", @"ShortEntry2");
				Print(@"Set 8:  " + Convert.ToString(Times[0][0].TimeOfDay));
			}
			
			 // Set 9
			if ((Position.MarketPosition == MarketPosition.Flat)
				 // Condition group 1
				 && ((InitialEntry == true)
				 || (TriggerTrailStop == true))
				 && (IsFirstTickOfBar == true))
			{
				InitialEntry = false;
				TriggerTrailStop = false;
				TrailStopUpdate = false;
				Print(@"Set 9:  " + Convert.ToString(Times[0][0].TimeOfDay));
			}
			
			 // Set 10
			if ((ORB_TradeSaber1.Signal[0] == 1)
				 || (ORB_TradeSaber1.Signal[0] == -1))
			{
				OrbHigh = (ORB_TradeSaber1.ORHigh[0] + (TickOffsetOrbHigh * TickSize)) ;
				OrbLow = (ORB_TradeSaber1.ORLow[0] + (TicjOffsetOrbLow * TickSize)) ;
			}
			
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="ATRPeriod", Order=1, GroupName="Parameters")]
		public int ATRPeriod
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="QTY1", Order=2, GroupName="Parameters")]
		public int QTY1
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="QTY2", Order=3, GroupName="Parameters")]
		public int QTY2
		{ get; set; }

		[NinjaScriptProperty]
		[Range(0.1, double.MaxValue)]
		[Display(Name="ProfitMultiplier1", Order=4, GroupName="Parameters")]
		public double ProfitMultiplier1
		{ get; set; }

		[NinjaScriptProperty]
		[Range(0.1, double.MaxValue)]
		[Display(Name="ProfitMultiplier2", Order=5, GroupName="Parameters")]
		public double ProfitMultiplier2
		{ get; set; }

		[NinjaScriptProperty]
		[Range(0.1, double.MaxValue)]
		[Display(Name="StopMultiplier", Order=6, GroupName="Parameters")]
		public double StopMultiplier
		{ get; set; }

		[NinjaScriptProperty]
		[Range(0.1, double.MaxValue)]
		[Display(Name="TrailTargetStart", Order=7, GroupName="Parameters")]
		public double TrailTargetStart
		{ get; set; }

		[NinjaScriptProperty]
		[Range(0.1, double.MaxValue)]
		[Display(Name="TrailStopMultiplier", Order=8, GroupName="Parameters")]
		public double TrailStopMultiplier
		{ get; set; }

		[NinjaScriptProperty]
		[PropertyEditor("NinjaTrader.Gui.Tools.TimeEditorKey")]
		[Display(Name="StartTime", Order=9, GroupName="Parameters")]
		public DateTime StartTime
		{ get; set; }

		[NinjaScriptProperty]
		[PropertyEditor("NinjaTrader.Gui.Tools.TimeEditorKey")]
		[Display(Name="StopTime", Order=10, GroupName="Parameters")]
		public DateTime StopTime
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="ADXPeriod", Order=11, GroupName="Parameters")]
		public int ADXPeriod
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="TickOffsetOrbHigh", Order=12, GroupName="Parameters")]
		public int TickOffsetOrbHigh
		{ get; set; }

		[NinjaScriptProperty]
		[Range(-100, int.MaxValue)]
		[Display(Name="TicjOffsetOrbLow", Order=13, GroupName="Parameters")]
		public int TicjOffsetOrbLow
		{ get; set; }
		#endregion

	}
}

#region Wizard settings, neither change nor remove
/*@
<?xml version="1.0"?>
<ScriptProperties xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <Calculate>OnEachTick</Calculate>
  <ConditionalActions>
    <ConditionalAction>
      <Actions>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Enter long position</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">QTY1</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>QTY1</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>QTY1</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:07:22.4742571</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">QTY1</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>LongEntry1</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <Tag>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set Enter long position</StringValue>
                </NinjaScriptString>
              </Strings>
            </Tag>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T10:07:12.7872073</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>Enter</ActionType>
          <Command>
            <Command>EnterLong</Command>
            <Parameters>
              <string>quantity</string>
              <string>signalName</string>
            </Parameters>
          </Command>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Enter long position</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">QTY2</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>QTY2</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>QTY2</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:07:53.7627238</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">QTY2</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>LongEntry2</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <Tag>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set Enter long position</StringValue>
                </NinjaScriptString>
              </Strings>
            </Tag>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T10:07:49.3608204</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>Enter</ActionType>
          <Command>
            <Command>EnterLong</Command>
            <Parameters>
              <string>quantity</string>
              <string>signalName</string>
            </Parameters>
          </Command>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set StoreProfit1</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:08:52.5006111</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T10:08:52.5006111</VariableDateTime>
            <VariableBool>false</VariableBool>
            <VariableDouble>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">(Default input[0] + ((ATR(Close, Convert.ToInt32(ATRPeriod))[0] * ProfitMultiplier1) )) </BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Close</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:09:08.3590208</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <OffsetBuilder>
                  <ConditionOffset>
                    <IsSetEnabled>false</IsSetEnabled>
                    <OffsetValue>0</OffsetValue>
                    <OffsetOperator>Add</OffsetOperator>
                    <OffsetType>Arithmetic</OffsetType>
                  </ConditionOffset>
                  <Offset>
                    <DefaultValue>0</DefaultValue>
                    <IsInt>false</IsInt>
                    <BindingValue xsi:type="xsd:string">(ATR(ATRPeriod)[0] * ProfitMultiplier1) </BindingValue>
                    <DynamicValue>
                      <Children />
                      <IsExpanded>false</IsExpanded>
                      <IsSelected>true</IsSelected>
                      <Name>ATR</Name>
                      <OffsetType>Arithmetic</OffsetType>
                      <AssignedCommand>
                        <Command>ATR</Command>
                        <Parameters>
                          <string>AssociatedIndicator</string>
                          <string>BarsAgo</string>
                          <string>OffsetBuilder</string>
                        </Parameters>
                      </AssignedCommand>
                      <AssociatedIndicator>
                        <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                        <CustomProperties>
                          <item>
                            <key>
                              <string>Period</string>
                            </key>
                            <value>
                              <anyType xsi:type="NumberBuilder">
                                <DefaultValue>0</DefaultValue>
                                <IsInt>true</IsInt>
                                <BindingValue xsi:type="xsd:string">ATRPeriod</BindingValue>
                                <DynamicValue>
                                  <Children />
                                  <IsExpanded>false</IsExpanded>
                                  <IsSelected>true</IsSelected>
                                  <Name>ATRPeriod</Name>
                                  <OffsetType>Arithmetic</OffsetType>
                                  <AssignedCommand>
                                    <Command>ATRPeriod</Command>
                                    <Parameters />
                                  </AssignedCommand>
                                  <BarsAgo>0</BarsAgo>
                                  <CurrencyType>Currency</CurrencyType>
                                  <Date>2024-09-29T10:10:23.6974823</Date>
                                  <DayOfWeek>Sunday</DayOfWeek>
                                  <EndBar>0</EndBar>
                                  <ForceSeriesIndex>false</ForceSeriesIndex>
                                  <LookBackPeriod>0</LookBackPeriod>
                                  <MarketPosition>Long</MarketPosition>
                                  <Period>0</Period>
                                  <ReturnType>Number</ReturnType>
                                  <StartBar>0</StartBar>
                                  <State>Undefined</State>
                                  <Time>0001-01-01T00:00:00</Time>
                                </DynamicValue>
                                <IsLiteral>false</IsLiteral>
                                <LiveValue xsi:type="xsd:string">ATRPeriod</LiveValue>
                              </anyType>
                            </value>
                          </item>
                        </CustomProperties>
                        <IndicatorHolder>
                          <IndicatorName>ATR</IndicatorName>
                          <Plots>
                            <Plot>
                              <IsOpacityVisible>false</IsOpacityVisible>
                              <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008B8B&lt;/SolidColorBrush&gt;</BrushSerialize>
                              <DashStyleHelper>Solid</DashStyleHelper>
                              <Opacity>100</Opacity>
                              <Width>1</Width>
                              <AutoWidth>false</AutoWidth>
                              <Max>1.7976931348623157E+308</Max>
                              <Min>-1.7976931348623157E+308</Min>
                              <Name>ATR</Name>
                              <PlotStyle>Line</PlotStyle>
                            </Plot>
                          </Plots>
                        </IndicatorHolder>
                        <IsExplicitlyNamed>false</IsExplicitlyNamed>
                        <IsPriceTypeLocked>false</IsPriceTypeLocked>
                        <PlotOnChart>true</PlotOnChart>
                        <PriceType>Close</PriceType>
                        <SeriesType>Indicator</SeriesType>
                      </AssociatedIndicator>
                      <BarsAgo>0</BarsAgo>
                      <CurrencyType>Currency</CurrencyType>
                      <Date>2024-09-29T10:09:41.8636016</Date>
                      <DayOfWeek>Sunday</DayOfWeek>
                      <EndBar>0</EndBar>
                      <ForceSeriesIndex>false</ForceSeriesIndex>
                      <LookBackPeriod>0</LookBackPeriod>
                      <MarketPosition>Long</MarketPosition>
                      <OffsetBuilder>
                        <ConditionOffset>
                          <IsSetEnabled>false</IsSetEnabled>
                          <OffsetValue>0</OffsetValue>
                          <OffsetOperator>Multiply</OffsetOperator>
                          <OffsetType>Arithmetic</OffsetType>
                        </ConditionOffset>
                        <Offset>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">ProfitMultiplier1</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>ProfitMultiplier1</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>ProfitMultiplier1</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-09-29T10:10:53.1776917</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">ProfitMultiplier1</LiveValue>
                        </Offset>
                      </OffsetBuilder>
                      <Period>0</Period>
                      <ReturnType>Series</ReturnType>
                      <StartBar>0</StartBar>
                      <State>Undefined</State>
                      <Time>0001-01-01T00:00:00</Time>
                    </DynamicValue>
                    <IsLiteral>false</IsLiteral>
                    <LiveValue xsi:type="xsd:string">(ATR(Close, Convert.ToInt32(ATRPeriod))[0] * ProfitMultiplier1) </LiveValue>
                  </Offset>
                </OffsetBuilder>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">(Close[0] + ((ATR(Close, Convert.ToInt32(ATRPeriod))[0] * ProfitMultiplier1) )) </LiveValue>
            </VariableDouble>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>double</UserVariableType>
          <VariableName>StoreProfit1</VariableName>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set StoreProfit2</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:12:43.8935217</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T10:12:43.8935217</VariableDateTime>
            <VariableBool>false</VariableBool>
            <VariableDouble>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">(Default input[0] + ((ATR(Close, Convert.ToInt32(ATRPeriod))[0] * ProfitMultiplier2) )) </BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Close</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:12:56.0211987</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <OffsetBuilder>
                  <ConditionOffset>
                    <IsSetEnabled>false</IsSetEnabled>
                    <OffsetValue>0</OffsetValue>
                    <OffsetOperator>Add</OffsetOperator>
                    <OffsetType>Arithmetic</OffsetType>
                  </ConditionOffset>
                  <Offset>
                    <DefaultValue>0</DefaultValue>
                    <IsInt>false</IsInt>
                    <BindingValue xsi:type="xsd:string">(ATR(ATRPeriod)[0] * ProfitMultiplier2) </BindingValue>
                    <DynamicValue>
                      <Children />
                      <IsExpanded>false</IsExpanded>
                      <IsSelected>true</IsSelected>
                      <Name>ATR</Name>
                      <OffsetType>Arithmetic</OffsetType>
                      <AssignedCommand>
                        <Command>ATR</Command>
                        <Parameters>
                          <string>AssociatedIndicator</string>
                          <string>BarsAgo</string>
                          <string>OffsetBuilder</string>
                        </Parameters>
                      </AssignedCommand>
                      <AssociatedIndicator>
                        <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                        <CustomProperties>
                          <item>
                            <key>
                              <string>Period</string>
                            </key>
                            <value>
                              <anyType xsi:type="NumberBuilder">
                                <DefaultValue>0</DefaultValue>
                                <IsInt>true</IsInt>
                                <BindingValue xsi:type="xsd:string">ATRPeriod</BindingValue>
                                <DynamicValue>
                                  <Children />
                                  <IsExpanded>false</IsExpanded>
                                  <IsSelected>true</IsSelected>
                                  <Name>ATRPeriod</Name>
                                  <OffsetType>Arithmetic</OffsetType>
                                  <AssignedCommand>
                                    <Command>ATRPeriod</Command>
                                    <Parameters />
                                  </AssignedCommand>
                                  <BarsAgo>0</BarsAgo>
                                  <CurrencyType>Currency</CurrencyType>
                                  <Date>2024-09-29T10:13:15.3471292</Date>
                                  <DayOfWeek>Sunday</DayOfWeek>
                                  <EndBar>0</EndBar>
                                  <ForceSeriesIndex>false</ForceSeriesIndex>
                                  <LookBackPeriod>0</LookBackPeriod>
                                  <MarketPosition>Long</MarketPosition>
                                  <Period>0</Period>
                                  <ReturnType>Number</ReturnType>
                                  <StartBar>0</StartBar>
                                  <State>Undefined</State>
                                  <Time>0001-01-01T00:00:00</Time>
                                </DynamicValue>
                                <IsLiteral>false</IsLiteral>
                                <LiveValue xsi:type="xsd:string">ATRPeriod</LiveValue>
                              </anyType>
                            </value>
                          </item>
                        </CustomProperties>
                        <IndicatorHolder>
                          <IndicatorName>ATR</IndicatorName>
                          <Plots>
                            <Plot>
                              <IsOpacityVisible>false</IsOpacityVisible>
                              <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008B8B&lt;/SolidColorBrush&gt;</BrushSerialize>
                              <DashStyleHelper>Solid</DashStyleHelper>
                              <Opacity>100</Opacity>
                              <Width>1</Width>
                              <AutoWidth>false</AutoWidth>
                              <Max>1.7976931348623157E+308</Max>
                              <Min>-1.7976931348623157E+308</Min>
                              <Name>ATR</Name>
                              <PlotStyle>Line</PlotStyle>
                            </Plot>
                          </Plots>
                        </IndicatorHolder>
                        <IsExplicitlyNamed>false</IsExplicitlyNamed>
                        <IsPriceTypeLocked>false</IsPriceTypeLocked>
                        <PlotOnChart>true</PlotOnChart>
                        <PriceType>Close</PriceType>
                        <SeriesType>Indicator</SeriesType>
                      </AssociatedIndicator>
                      <BarsAgo>0</BarsAgo>
                      <CurrencyType>Currency</CurrencyType>
                      <Date>2024-09-29T10:13:05.2449651</Date>
                      <DayOfWeek>Sunday</DayOfWeek>
                      <EndBar>0</EndBar>
                      <ForceSeriesIndex>false</ForceSeriesIndex>
                      <LookBackPeriod>0</LookBackPeriod>
                      <MarketPosition>Long</MarketPosition>
                      <OffsetBuilder>
                        <ConditionOffset>
                          <IsSetEnabled>false</IsSetEnabled>
                          <OffsetValue>0</OffsetValue>
                          <OffsetOperator>Multiply</OffsetOperator>
                          <OffsetType>Arithmetic</OffsetType>
                        </ConditionOffset>
                        <Offset>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">ProfitMultiplier2</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>ProfitMultiplier2</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>ProfitMultiplier2</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-09-29T10:13:23.9438433</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">ProfitMultiplier2</LiveValue>
                        </Offset>
                      </OffsetBuilder>
                      <Period>0</Period>
                      <ReturnType>Series</ReturnType>
                      <StartBar>0</StartBar>
                      <State>Undefined</State>
                      <Time>0001-01-01T00:00:00</Time>
                    </DynamicValue>
                    <IsLiteral>false</IsLiteral>
                    <LiveValue xsi:type="xsd:string">(ATR(Close, Convert.ToInt32(ATRPeriod))[0] * ProfitMultiplier2) </LiveValue>
                  </Offset>
                </OffsetBuilder>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">(Close[0] + ((ATR(Close, Convert.ToInt32(ATRPeriod))[0] * ProfitMultiplier2) )) </LiveValue>
            </VariableDouble>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>double</UserVariableType>
          <VariableName>StoreProfit2</VariableName>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set StoreStop</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:13:40.9315494</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T10:13:40.9315494</VariableDateTime>
            <VariableBool>false</VariableBool>
            <VariableDouble>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">(Default input[0] - ((ATR(Close, Convert.ToInt32(ATRPeriod))[0] * StopMultiplier) )) </BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Close</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:13:47.5864972</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <OffsetBuilder>
                  <ConditionOffset>
                    <IsSetEnabled>false</IsSetEnabled>
                    <OffsetValue>0</OffsetValue>
                    <OffsetOperator>Subtract</OffsetOperator>
                    <OffsetType>Arithmetic</OffsetType>
                  </ConditionOffset>
                  <Offset>
                    <DefaultValue>0</DefaultValue>
                    <IsInt>false</IsInt>
                    <BindingValue xsi:type="xsd:string">(ATR(ATRPeriod)[0] * StopMultiplier) </BindingValue>
                    <DynamicValue>
                      <Children />
                      <IsExpanded>false</IsExpanded>
                      <IsSelected>true</IsSelected>
                      <Name>ATR</Name>
                      <OffsetType>Arithmetic</OffsetType>
                      <AssignedCommand>
                        <Command>ATR</Command>
                        <Parameters>
                          <string>AssociatedIndicator</string>
                          <string>BarsAgo</string>
                          <string>OffsetBuilder</string>
                        </Parameters>
                      </AssignedCommand>
                      <AssociatedIndicator>
                        <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                        <CustomProperties>
                          <item>
                            <key>
                              <string>Period</string>
                            </key>
                            <value>
                              <anyType xsi:type="NumberBuilder">
                                <DefaultValue>0</DefaultValue>
                                <IsInt>true</IsInt>
                                <BindingValue xsi:type="xsd:string">ATRPeriod</BindingValue>
                                <DynamicValue>
                                  <Children />
                                  <IsExpanded>false</IsExpanded>
                                  <IsSelected>true</IsSelected>
                                  <Name>ATRPeriod</Name>
                                  <OffsetType>Arithmetic</OffsetType>
                                  <AssignedCommand>
                                    <Command>ATRPeriod</Command>
                                    <Parameters />
                                  </AssignedCommand>
                                  <BarsAgo>0</BarsAgo>
                                  <CurrencyType>Currency</CurrencyType>
                                  <Date>2024-09-29T10:14:17.3923925</Date>
                                  <DayOfWeek>Sunday</DayOfWeek>
                                  <EndBar>0</EndBar>
                                  <ForceSeriesIndex>false</ForceSeriesIndex>
                                  <LookBackPeriod>0</LookBackPeriod>
                                  <MarketPosition>Long</MarketPosition>
                                  <Period>0</Period>
                                  <ReturnType>Number</ReturnType>
                                  <StartBar>0</StartBar>
                                  <State>Undefined</State>
                                  <Time>0001-01-01T00:00:00</Time>
                                </DynamicValue>
                                <IsLiteral>false</IsLiteral>
                                <LiveValue xsi:type="xsd:string">ATRPeriod</LiveValue>
                              </anyType>
                            </value>
                          </item>
                        </CustomProperties>
                        <IndicatorHolder>
                          <IndicatorName>ATR</IndicatorName>
                          <Plots>
                            <Plot>
                              <IsOpacityVisible>false</IsOpacityVisible>
                              <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008B8B&lt;/SolidColorBrush&gt;</BrushSerialize>
                              <DashStyleHelper>Solid</DashStyleHelper>
                              <Opacity>100</Opacity>
                              <Width>1</Width>
                              <AutoWidth>false</AutoWidth>
                              <Max>1.7976931348623157E+308</Max>
                              <Min>-1.7976931348623157E+308</Min>
                              <Name>ATR</Name>
                              <PlotStyle>Line</PlotStyle>
                            </Plot>
                          </Plots>
                        </IndicatorHolder>
                        <IsExplicitlyNamed>false</IsExplicitlyNamed>
                        <IsPriceTypeLocked>false</IsPriceTypeLocked>
                        <PlotOnChart>true</PlotOnChart>
                        <PriceType>Close</PriceType>
                        <SeriesType>Indicator</SeriesType>
                      </AssociatedIndicator>
                      <BarsAgo>0</BarsAgo>
                      <CurrencyType>Currency</CurrencyType>
                      <Date>2024-09-29T10:14:09.8858217</Date>
                      <DayOfWeek>Sunday</DayOfWeek>
                      <EndBar>0</EndBar>
                      <ForceSeriesIndex>false</ForceSeriesIndex>
                      <LookBackPeriod>0</LookBackPeriod>
                      <MarketPosition>Long</MarketPosition>
                      <OffsetBuilder>
                        <ConditionOffset>
                          <IsSetEnabled>false</IsSetEnabled>
                          <OffsetValue>0</OffsetValue>
                          <OffsetOperator>Multiply</OffsetOperator>
                          <OffsetType>Arithmetic</OffsetType>
                        </ConditionOffset>
                        <Offset>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">StopMultiplier</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>StopMultiplier</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>StopMultiplier</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-09-29T10:14:25.9027247</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">StopMultiplier</LiveValue>
                        </Offset>
                      </OffsetBuilder>
                      <Period>0</Period>
                      <ReturnType>Series</ReturnType>
                      <StartBar>0</StartBar>
                      <State>Undefined</State>
                      <Time>0001-01-01T00:00:00</Time>
                    </DynamicValue>
                    <IsLiteral>false</IsLiteral>
                    <LiveValue xsi:type="xsd:string">(ATR(Close, Convert.ToInt32(ATRPeriod))[0] * StopMultiplier) </LiveValue>
                  </Offset>
                </OffsetBuilder>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">(Close[0] - ((ATR(Close, Convert.ToInt32(ATRPeriod))[0] * StopMultiplier) )) </LiveValue>
            </VariableDouble>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>double</UserVariableType>
          <VariableName>StoreStop</VariableName>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set InitialEntry</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:42:02.6053323</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T10:42:02.6053323</VariableDateTime>
            <VariableBool>true</VariableBool>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>bool</UserVariableType>
          <VariableName>InitialEntry</VariableName>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set StoreTrailTarget</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:12:24.8363462</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T11:12:24.8363462</VariableDateTime>
            <VariableBool>false</VariableBool>
            <VariableDouble>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">(Default input[0] + ((ATR(Close, Convert.ToInt32(ATRPeriod))[0] * TrailTargetStart) )) </BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Close</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:12:44.1295756</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <OffsetBuilder>
                  <ConditionOffset>
                    <IsSetEnabled>false</IsSetEnabled>
                    <OffsetValue>0</OffsetValue>
                    <OffsetOperator>Add</OffsetOperator>
                    <OffsetType>Arithmetic</OffsetType>
                  </ConditionOffset>
                  <Offset>
                    <DefaultValue>0</DefaultValue>
                    <IsInt>false</IsInt>
                    <BindingValue xsi:type="xsd:string">(ATR(ATRPeriod)[0] * TrailTargetStart) </BindingValue>
                    <DynamicValue>
                      <Children />
                      <IsExpanded>false</IsExpanded>
                      <IsSelected>true</IsSelected>
                      <Name>ATR</Name>
                      <OffsetType>Arithmetic</OffsetType>
                      <AssignedCommand>
                        <Command>ATR</Command>
                        <Parameters>
                          <string>AssociatedIndicator</string>
                          <string>BarsAgo</string>
                          <string>OffsetBuilder</string>
                        </Parameters>
                      </AssignedCommand>
                      <AssociatedIndicator>
                        <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                        <CustomProperties>
                          <item>
                            <key>
                              <string>Period</string>
                            </key>
                            <value>
                              <anyType xsi:type="NumberBuilder">
                                <DefaultValue>0</DefaultValue>
                                <IsInt>true</IsInt>
                                <BindingValue xsi:type="xsd:string">ATRPeriod</BindingValue>
                                <DynamicValue>
                                  <Children />
                                  <IsExpanded>false</IsExpanded>
                                  <IsSelected>true</IsSelected>
                                  <Name>ATRPeriod</Name>
                                  <OffsetType>Arithmetic</OffsetType>
                                  <AssignedCommand>
                                    <Command>ATRPeriod</Command>
                                    <Parameters />
                                  </AssignedCommand>
                                  <BarsAgo>0</BarsAgo>
                                  <CurrencyType>Currency</CurrencyType>
                                  <Date>2024-09-29T11:13:07.9176008</Date>
                                  <DayOfWeek>Sunday</DayOfWeek>
                                  <EndBar>0</EndBar>
                                  <ForceSeriesIndex>false</ForceSeriesIndex>
                                  <LookBackPeriod>0</LookBackPeriod>
                                  <MarketPosition>Long</MarketPosition>
                                  <Period>0</Period>
                                  <ReturnType>Number</ReturnType>
                                  <StartBar>0</StartBar>
                                  <State>Undefined</State>
                                  <Time>0001-01-01T00:00:00</Time>
                                </DynamicValue>
                                <IsLiteral>false</IsLiteral>
                                <LiveValue xsi:type="xsd:string">ATRPeriod</LiveValue>
                              </anyType>
                            </value>
                          </item>
                        </CustomProperties>
                        <IndicatorHolder>
                          <IndicatorName>ATR</IndicatorName>
                          <Plots>
                            <Plot>
                              <IsOpacityVisible>false</IsOpacityVisible>
                              <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008B8B&lt;/SolidColorBrush&gt;</BrushSerialize>
                              <DashStyleHelper>Solid</DashStyleHelper>
                              <Opacity>100</Opacity>
                              <Width>1</Width>
                              <AutoWidth>false</AutoWidth>
                              <Max>1.7976931348623157E+308</Max>
                              <Min>-1.7976931348623157E+308</Min>
                              <Name>ATR</Name>
                              <PlotStyle>Line</PlotStyle>
                            </Plot>
                          </Plots>
                        </IndicatorHolder>
                        <IsExplicitlyNamed>false</IsExplicitlyNamed>
                        <IsPriceTypeLocked>false</IsPriceTypeLocked>
                        <PlotOnChart>true</PlotOnChart>
                        <PriceType>Close</PriceType>
                        <SeriesType>Indicator</SeriesType>
                      </AssociatedIndicator>
                      <BarsAgo>0</BarsAgo>
                      <CurrencyType>Currency</CurrencyType>
                      <Date>2024-09-29T11:12:55.7160932</Date>
                      <DayOfWeek>Sunday</DayOfWeek>
                      <EndBar>0</EndBar>
                      <ForceSeriesIndex>false</ForceSeriesIndex>
                      <LookBackPeriod>0</LookBackPeriod>
                      <MarketPosition>Long</MarketPosition>
                      <OffsetBuilder>
                        <ConditionOffset>
                          <IsSetEnabled>false</IsSetEnabled>
                          <OffsetValue>0</OffsetValue>
                          <OffsetOperator>Multiply</OffsetOperator>
                          <OffsetType>Arithmetic</OffsetType>
                        </ConditionOffset>
                        <Offset>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">TrailTargetStart</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>TrailTargetStart</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>TrailTargetStart</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-09-29T11:13:15.7448574</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">TrailTargetStart</LiveValue>
                        </Offset>
                      </OffsetBuilder>
                      <Period>0</Period>
                      <ReturnType>Series</ReturnType>
                      <StartBar>0</StartBar>
                      <State>Undefined</State>
                      <Time>0001-01-01T00:00:00</Time>
                    </DynamicValue>
                    <IsLiteral>false</IsLiteral>
                    <LiveValue xsi:type="xsd:string">(ATR(Close, Convert.ToInt32(ATRPeriod))[0] * TrailTargetStart) </LiveValue>
                  </Offset>
                </OffsetBuilder>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">(Close[0] + ((ATR(Close, Convert.ToInt32(ATRPeriod))[0] * TrailTargetStart) )) </LiveValue>
            </VariableDouble>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>double</UserVariableType>
          <VariableName>StoreTrailTarget</VariableName>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set StoreTrailStop</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:24:58.0946777</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T11:24:58.0946777</VariableDateTime>
            <VariableBool>false</VariableBool>
            <VariableDouble>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">StoreStop</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>StoreStop</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>StoreStop</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:25:10.55461</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">StoreStop</LiveValue>
            </VariableDouble>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>double</UserVariableType>
          <VariableName>StoreTrailStop</VariableName>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set TriggerTrailStop</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:58:14.0307941</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T11:58:14.0307941</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>bool</UserVariableType>
          <VariableName>TriggerTrailStop</VariableName>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set TrailStopUpdate</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:58:25.5717792</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T11:58:25.5717792</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>bool</UserVariableType>
          <VariableName>TrailStopUpdate</VariableName>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Print</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <MessageValue>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set 1: </StringValue>
                </NinjaScriptString>
                <NinjaScriptString>
                  <Action>
                    <Children />
                    <IsExpanded>false</IsExpanded>
                    <IsSelected>true</IsSelected>
                    <Name>Time series</Name>
                    <OffsetType>Arithmetic</OffsetType>
                    <AssignedCommand>
                      <Command>Times[{0}][{1}].TimeOfDay</Command>
                      <Parameters>
                        <string>Series1</string>
                        <string>BarsAgo</string>
                      </Parameters>
                    </AssignedCommand>
                    <BarsAgo>0</BarsAgo>
                    <CurrencyType>Currency</CurrencyType>
                    <Date>2024-09-29T12:16:36.5951082</Date>
                    <DayOfWeek>Sunday</DayOfWeek>
                    <EndBar>0</EndBar>
                    <ForceSeriesIndex>true</ForceSeriesIndex>
                    <LookBackPeriod>0</LookBackPeriod>
                    <MarketPosition>Long</MarketPosition>
                    <Period>0</Period>
                    <ReturnType>Time</ReturnType>
                    <StartBar>0</StartBar>
                    <State>Undefined</State>
                    <Time>0001-01-01T00:00:00</Time>
                  </Action>
                  <Index>1</Index>
                  <StringValue>Times[0][0].TimeOfDay</StringValue>
                </NinjaScriptString>
              </Strings>
            </MessageValue>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T12:16:09.408723</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <Tag>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set Print</StringValue>
                </NinjaScriptString>
              </Strings>
            </Tag>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T12:16:09.408723</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>Misc</ActionType>
          <Command>
            <Command>Print</Command>
            <Parameters>
              <string>MessageValue</string>
            </Parameters>
          </Command>
        </WizardAction>
      </Actions>
      <ActiveAction>
        <Children />
        <IsExpanded>false</IsExpanded>
        <IsSelected>true</IsSelected>
        <Name>Enter long position</Name>
        <OffsetType>Arithmetic</OffsetType>
        <ActionProperties>
          <DashStyle>Solid</DashStyle>
          <DivideTimePrice>false</DivideTimePrice>
          <Id />
          <File />
          <IsAutoScale>false</IsAutoScale>
          <IsSimulatedStop>false</IsSimulatedStop>
          <IsStop>false</IsStop>
          <LogLevel>Information</LogLevel>
          <Mode>Currency</Mode>
          <OffsetType>Currency</OffsetType>
          <Priority>Medium</Priority>
          <Quantity>
            <DefaultValue>0</DefaultValue>
            <IsInt>true</IsInt>
            <BindingValue xsi:type="xsd:string">QTY1</BindingValue>
            <DynamicValue>
              <Children />
              <IsExpanded>false</IsExpanded>
              <IsSelected>true</IsSelected>
              <Name>QTY1</Name>
              <OffsetType>Arithmetic</OffsetType>
              <AssignedCommand>
                <Command>QTY1</Command>
                <Parameters />
              </AssignedCommand>
              <BarsAgo>0</BarsAgo>
              <CurrencyType>Currency</CurrencyType>
              <Date>2024-09-29T10:07:22.4742571</Date>
              <DayOfWeek>Sunday</DayOfWeek>
              <EndBar>0</EndBar>
              <ForceSeriesIndex>false</ForceSeriesIndex>
              <LookBackPeriod>0</LookBackPeriod>
              <MarketPosition>Long</MarketPosition>
              <Period>0</Period>
              <ReturnType>Number</ReturnType>
              <StartBar>0</StartBar>
              <State>Undefined</State>
              <Time>0001-01-01T00:00:00</Time>
            </DynamicValue>
            <IsLiteral>false</IsLiteral>
            <LiveValue xsi:type="xsd:string">QTY1</LiveValue>
          </Quantity>
          <ServiceName />
          <ScreenshotPath />
          <SignalName>
            <SeparatorCharacter> </SeparatorCharacter>
            <Strings>
              <NinjaScriptString>
                <Index>0</Index>
                <StringValue>LongEntry1</StringValue>
              </NinjaScriptString>
            </Strings>
          </SignalName>
          <SoundLocation />
          <Tag>
            <SeparatorCharacter> </SeparatorCharacter>
            <Strings>
              <NinjaScriptString>
                <Index>0</Index>
                <StringValue>Set Enter long position</StringValue>
              </NinjaScriptString>
            </Strings>
          </Tag>
          <TextPosition>BottomLeft</TextPosition>
          <VariableDateTime>2024-09-29T10:07:12.7872073</VariableDateTime>
          <VariableBool>false</VariableBool>
        </ActionProperties>
        <ActionType>Enter</ActionType>
        <Command>
          <Command>EnterLong</Command>
          <Parameters>
            <string>quantity</string>
            <string>signalName</string>
          </Parameters>
        </Command>
      </ActiveAction>
      <AnyOrAll>All</AnyOrAll>
      <Conditions>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Current market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.MarketPosition</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:01:42.8325241</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>MarketPosition.{0}</Command>
                  <Parameters>
                    <string>MarketPosition</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:01:42.8792814</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Flat</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>Position.MarketPosition = MarketPosition.Flat</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>All</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Close</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:05:51.9020976</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Greater</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>SMA</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>SMA</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Period</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">50</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">50</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>SMA</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFDAA520&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>SMA</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>true</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                </AssociatedIndicator>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:05:51.9151127</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>ADX</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>ADX</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Period</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">14</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">14</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>ADX</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008B8B&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ADX</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                </AssociatedIndicator>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-05-12T23:24:49.1434385</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>GreaterEqual</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>ADXPeriod</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>ADXPeriod</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-05-12T23:24:49.1739383</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Rising</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>IsRising({0})</Command>
                  <Parameters>
                    <string>Series1</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-05-12T23:47:58.2508528</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <Series1>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Period</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">8</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">8</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>EMA</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFDAA520&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>EMA</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                </Series1>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>True</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>true</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-05-12T23:47:58.2898517</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Typical</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-06-30T13:49:53.8816116</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <Series1>
                  <AcceptableSeries>DataSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties />
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>true</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Typical</PriceType>
                  <SeriesType>DefaultSeries</SeriesType>
                </Series1>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>CrossAbove</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>OrbHigh</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>OrbHigh</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-05-17T23:35:13.7099106</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>EMA</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>EMA</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Period</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">10</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">10</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>EMA</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFDAA520&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>EMA</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                </AssociatedIndicator>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-05-18T00:37:02.158462</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Greater</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>EMA</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>EMA</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Period</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">20</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">20</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>EMA</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFDAA520&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>EMA</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                </AssociatedIndicator>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-05-18T00:37:02.2009583</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>EMA</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>EMA</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Period</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">10</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">10</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>EMA</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFDAA520&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>EMA</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                </AssociatedIndicator>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-05-18T10:32:06.3766918</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Greater</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>SMA</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>SMA</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Period</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">50</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">50</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>SMA</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFDAA520&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>SMA</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                </AssociatedIndicator>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-05-18T10:32:06.4071958</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>true</IsGroup>
          <DisplayName>Long Entry Condition</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>InitialEntry</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>InitialEntry</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:41:37.8072032</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>False</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>false</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:41:37.8331983</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>InitialEntry = false</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>TriggerTrailStop</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>TriggerTrailStop</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:48:05.4996371</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>False</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>false</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:48:05.5118311</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>TriggerTrailStop = false</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>All</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Time series</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Times[{0}][{1}].TimeOfDay</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>BarsAgo</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-05-12T23:19:41.9921754</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>true</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Time</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>GreaterEqual</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>StartTime</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>StartTime.TimeOfDay</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-05-12T23:19:42.0161752</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Time</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Time series</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Times[{0}][{1}].TimeOfDay</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>BarsAgo</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-05-12T23:20:00.4746159</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>true</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Time</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Less</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>StopTime</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>StopTime.TimeOfDay</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-05-12T23:20:00.5106157</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Time</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>true</IsGroup>
          <DisplayName>Time</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>All</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Rising</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>IsRising({0})</Command>
                  <Parameters>
                    <string>Series1</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-06-29T13:10:01.8057959</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <Series1>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Period</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">ATRPeriod</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>ATRPeriod</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>ATRPeriod</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2025-06-29T13:10:33.4613064</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">ATRPeriod</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>ADX</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008B8B&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ADX</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                </Series1>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>True</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>true</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-06-29T13:10:01.8278097</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Rising</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>IsRising({0})</Command>
                  <Parameters>
                    <string>Series1</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-06-29T14:08:35.7957186</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <Series1>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Period</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">14</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">14</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Smooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">3</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">3</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>RSI</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF1E90FF&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>RSI</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFDAA520&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>Avg</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                  <SelectedPlot>RSI</SelectedPlot>
                </Series1>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>True</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>true</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-06-29T14:08:35.8137344</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>RSI</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>RSI</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Period</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">14</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">14</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Smooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">3</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">3</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>RSI</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF1E90FF&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>RSI</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFDAA520&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>Avg</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                  <SelectedPlot>Avg</SelectedPlot>
                </AssociatedIndicator>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-06-30T13:46:19.9877085</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>GreaterEqual</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Numeric value</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>NumericValue</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-06-30T13:46:20.0157103</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <NumericValue>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>false</IsInt>
                  <BindingValue xsi:type="xsd:string">30</BindingValue>
                  <IsLiteral>true</IsLiteral>
                  <LiveValue xsi:type="xsd:string">30</LiveValue>
                </NumericValue>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>true</IsGroup>
          <DisplayName>Confluence</DisplayName>
        </WizardConditionGroup>
      </Conditions>
      <SetName>Set 1</SetName>
      <SetNumber>1</SetNumber>
    </ConditionalAction>
    <ConditionalAction>
      <Actions>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Exit long position by a limit order</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <FromEntrySignal>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>LongEntry1</StringValue>
                </NinjaScriptString>
              </Strings>
            </FromEntrySignal>
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LimitPrice>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">StoreProfit1</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>StoreProfit1</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>StoreProfit1</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:20:22.7148277</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">StoreProfit1</LiveValue>
            </LimitPrice>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">QTY1</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>QTY1</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>QTY1</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:20:36.5608201</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">QTY1</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>LongProfit1</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <Tag>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set Exit long position by a limit order</StringValue>
                </NinjaScriptString>
              </Strings>
            </Tag>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T10:19:47.9947262</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>ExitLimit</ActionType>
          <Command>
            <Command>ExitLongLimit</Command>
            <Parameters>
              <string>quantity</string>
              <string>limitPrice</string>
              <string>signalName</string>
              <string>fromEntrySignal</string>
            </Parameters>
          </Command>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Exit long position by a limit order</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <FromEntrySignal>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>LongEntry2</StringValue>
                </NinjaScriptString>
              </Strings>
            </FromEntrySignal>
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LimitPrice>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">StoreProfit2</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>StoreProfit2</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>StoreProfit2</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:21:21.212071</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">StoreProfit2</LiveValue>
            </LimitPrice>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">QTY2</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>QTY2</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>QTY2</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:21:27.1958621</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">QTY2</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>LongProfit2</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <Tag>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set Exit long position by a limit order</StringValue>
                </NinjaScriptString>
              </Strings>
            </Tag>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T10:21:00.1745909</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>ExitLimit</ActionType>
          <Command>
            <Command>ExitLongLimit</Command>
            <Parameters>
              <string>quantity</string>
              <string>limitPrice</string>
              <string>signalName</string>
              <string>fromEntrySignal</string>
            </Parameters>
          </Command>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Exit long position by a stop order</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <FromEntrySignal>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>LongEntry1</StringValue>
                </NinjaScriptString>
              </Strings>
            </FromEntrySignal>
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">QTY1</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>QTY1</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>QTY1</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:22:21.4838454</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">QTY1</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>LongStop1</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <StopPrice>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">StoreStop</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>StoreStop</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>StoreStop</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:22:31.6945153</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">StoreStop</LiveValue>
            </StopPrice>
            <Tag>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set Exit long position by a stop order</StringValue>
                </NinjaScriptString>
              </Strings>
            </Tag>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T10:22:07.1799001</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>ExitStop</ActionType>
          <Command>
            <Command>ExitLongStopMarket</Command>
            <Parameters>
              <string>quantity</string>
              <string>stopPrice</string>
              <string>signalName</string>
              <string>fromEntrySignal</string>
            </Parameters>
          </Command>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Exit long position by a stop order</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <FromEntrySignal>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>LongEntry2</StringValue>
                </NinjaScriptString>
              </Strings>
            </FromEntrySignal>
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">QTY2</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>QTY2</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>QTY2</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:23:11.6329999</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">QTY2</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>LongStop2</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <StopPrice>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">StoreStop</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>StoreStop</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>StoreStop</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:23:22.3255077</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">StoreStop</LiveValue>
            </StopPrice>
            <Tag>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set Exit long position by a stop order</StringValue>
                </NinjaScriptString>
              </Strings>
            </Tag>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T10:22:48.5675719</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>ExitStop</ActionType>
          <Command>
            <Command>ExitLongStopMarket</Command>
            <Parameters>
              <string>quantity</string>
              <string>stopPrice</string>
              <string>signalName</string>
              <string>fromEntrySignal</string>
            </Parameters>
          </Command>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set InitialEntry</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:43:24.2179834</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T10:43:24.2179834</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>bool</UserVariableType>
          <VariableName>InitialEntry</VariableName>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Print</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <MessageValue>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set 2: </StringValue>
                </NinjaScriptString>
                <NinjaScriptString>
                  <Action>
                    <Children />
                    <IsExpanded>false</IsExpanded>
                    <IsSelected>true</IsSelected>
                    <Name>Time series</Name>
                    <OffsetType>Arithmetic</OffsetType>
                    <AssignedCommand>
                      <Command>Times[{0}][{1}].TimeOfDay</Command>
                      <Parameters>
                        <string>Series1</string>
                        <string>BarsAgo</string>
                      </Parameters>
                    </AssignedCommand>
                    <BarsAgo>0</BarsAgo>
                    <CurrencyType>Currency</CurrencyType>
                    <Date>2024-09-29T12:16:36.5951082</Date>
                    <DayOfWeek>Sunday</DayOfWeek>
                    <EndBar>0</EndBar>
                    <ForceSeriesIndex>true</ForceSeriesIndex>
                    <LookBackPeriod>0</LookBackPeriod>
                    <MarketPosition>Long</MarketPosition>
                    <Period>0</Period>
                    <ReturnType>Time</ReturnType>
                    <StartBar>0</StartBar>
                    <State>Undefined</State>
                    <Time>0001-01-01T00:00:00</Time>
                  </Action>
                  <Index>1</Index>
                  <StringValue>Times[0][0].TimeOfDay</StringValue>
                </NinjaScriptString>
              </Strings>
            </MessageValue>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T12:16:48.840888</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T12:16:48.840888</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>Misc</ActionType>
          <Command>
            <Command>Print</Command>
            <Parameters>
              <string>MessageValue</string>
            </Parameters>
          </Command>
        </WizardAction>
      </Actions>
      <ActiveAction>
        <Children />
        <IsExpanded>false</IsExpanded>
        <IsSelected>true</IsSelected>
        <Name>Print</Name>
        <OffsetType>Arithmetic</OffsetType>
        <ActionProperties>
          <DashStyle>Solid</DashStyle>
          <DivideTimePrice>false</DivideTimePrice>
          <Id />
          <File />
          <IsAutoScale>false</IsAutoScale>
          <IsSimulatedStop>false</IsSimulatedStop>
          <IsStop>false</IsStop>
          <LogLevel>Information</LogLevel>
          <MessageValue>
            <SeparatorCharacter> </SeparatorCharacter>
            <Strings>
              <NinjaScriptString>
                <Index>0</Index>
                <StringValue>Set 2: </StringValue>
              </NinjaScriptString>
              <NinjaScriptString>
                <Action>
                  <Children />
                  <IsExpanded>false</IsExpanded>
                  <IsSelected>true</IsSelected>
                  <Name>Time series</Name>
                  <OffsetType>Arithmetic</OffsetType>
                  <AssignedCommand>
                    <Command>Times[{0}][{1}].TimeOfDay</Command>
                    <Parameters>
                      <string>Series1</string>
                      <string>BarsAgo</string>
                    </Parameters>
                  </AssignedCommand>
                  <BarsAgo>0</BarsAgo>
                  <CurrencyType>Currency</CurrencyType>
                  <Date>2024-09-29T12:16:36.5951082</Date>
                  <DayOfWeek>Sunday</DayOfWeek>
                  <EndBar>0</EndBar>
                  <ForceSeriesIndex>true</ForceSeriesIndex>
                  <LookBackPeriod>0</LookBackPeriod>
                  <MarketPosition>Long</MarketPosition>
                  <Period>0</Period>
                  <ReturnType>Time</ReturnType>
                  <StartBar>0</StartBar>
                  <State>Undefined</State>
                  <Time>0001-01-01T00:00:00</Time>
                </Action>
                <Index>1</Index>
                <StringValue>Times[0][0].TimeOfDay</StringValue>
              </NinjaScriptString>
            </Strings>
          </MessageValue>
          <Mode>Currency</Mode>
          <OffsetType>Currency</OffsetType>
          <Priority>Medium</Priority>
          <Quantity>
            <DefaultValue>0</DefaultValue>
            <IsInt>true</IsInt>
            <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
            <DynamicValue>
              <Children />
              <IsExpanded>false</IsExpanded>
              <IsSelected>false</IsSelected>
              <Name>Default order quantity</Name>
              <OffsetType>Arithmetic</OffsetType>
              <AssignedCommand>
                <Command>DefaultQuantity</Command>
                <Parameters />
              </AssignedCommand>
              <BarsAgo>0</BarsAgo>
              <CurrencyType>Currency</CurrencyType>
              <Date>2024-09-29T12:16:48.840888</Date>
              <DayOfWeek>Sunday</DayOfWeek>
              <EndBar>0</EndBar>
              <ForceSeriesIndex>false</ForceSeriesIndex>
              <LookBackPeriod>0</LookBackPeriod>
              <MarketPosition>Long</MarketPosition>
              <Period>0</Period>
              <ReturnType>Number</ReturnType>
              <StartBar>0</StartBar>
              <State>Undefined</State>
              <Time>0001-01-01T00:00:00</Time>
            </DynamicValue>
            <IsLiteral>false</IsLiteral>
            <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
          </Quantity>
          <ServiceName />
          <ScreenshotPath />
          <SoundLocation />
          <TextPosition>BottomLeft</TextPosition>
          <VariableDateTime>2024-09-29T12:16:48.840888</VariableDateTime>
          <VariableBool>false</VariableBool>
        </ActionProperties>
        <ActionType>Misc</ActionType>
        <Command>
          <Command>Print</Command>
          <Parameters>
            <string>MessageValue</string>
          </Parameters>
        </Command>
      </ActiveAction>
      <AnyOrAll>All</AnyOrAll>
      <Conditions>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Current market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.MarketPosition</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:17:12.5688017</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>MarketPosition.{0}</Command>
                  <Parameters>
                    <string>MarketPosition</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:17:12.5976349</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>Position.MarketPosition = MarketPosition.Long</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>IsFirstTickOfBar</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>IsFirstTickOfBar</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:38:55.0154843</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>True</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>true</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:38:55.0427548</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>InitialEntry</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>InitialEntry</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:42:40.3323622</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>True</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>true</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:42:40.3604866</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>true</IsGroup>
          <DisplayName>Condition group 1</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>TriggerTrailStop</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>TriggerTrailStop</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:48:05.4996371</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>False</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>false</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:48:05.5118311</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>TriggerTrailStop = false</DisplayName>
        </WizardConditionGroup>
      </Conditions>
      <SetName>Set 2</SetName>
      <SetNumber>2</SetNumber>
    </ConditionalAction>
    <ConditionalAction>
      <Actions>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set StoreTrailStop</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:18:27.3681777</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T11:18:27.3681777</VariableDateTime>
            <VariableBool>false</VariableBool>
            <VariableDouble>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">(Default input[0] - ((ATR(Close, Convert.ToInt32(ATRPeriod))[0] * TrailStopMultiplier) )) </BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Close</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:18:46.3104769</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <OffsetBuilder>
                  <ConditionOffset>
                    <IsSetEnabled>false</IsSetEnabled>
                    <OffsetValue>0</OffsetValue>
                    <OffsetOperator>Subtract</OffsetOperator>
                    <OffsetType>Arithmetic</OffsetType>
                  </ConditionOffset>
                  <Offset>
                    <DefaultValue>0</DefaultValue>
                    <IsInt>false</IsInt>
                    <BindingValue xsi:type="xsd:string">(ATR(ATRPeriod)[0] * TrailStopMultiplier) </BindingValue>
                    <DynamicValue>
                      <Children />
                      <IsExpanded>false</IsExpanded>
                      <IsSelected>true</IsSelected>
                      <Name>ATR</Name>
                      <OffsetType>Arithmetic</OffsetType>
                      <AssignedCommand>
                        <Command>ATR</Command>
                        <Parameters>
                          <string>AssociatedIndicator</string>
                          <string>BarsAgo</string>
                          <string>OffsetBuilder</string>
                        </Parameters>
                      </AssignedCommand>
                      <AssociatedIndicator>
                        <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                        <CustomProperties>
                          <item>
                            <key>
                              <string>Period</string>
                            </key>
                            <value>
                              <anyType xsi:type="NumberBuilder">
                                <DefaultValue>0</DefaultValue>
                                <IsInt>true</IsInt>
                                <BindingValue xsi:type="xsd:string">ATRPeriod</BindingValue>
                                <DynamicValue>
                                  <Children />
                                  <IsExpanded>false</IsExpanded>
                                  <IsSelected>true</IsSelected>
                                  <Name>ATRPeriod</Name>
                                  <OffsetType>Arithmetic</OffsetType>
                                  <AssignedCommand>
                                    <Command>ATRPeriod</Command>
                                    <Parameters />
                                  </AssignedCommand>
                                  <BarsAgo>0</BarsAgo>
                                  <CurrencyType>Currency</CurrencyType>
                                  <Date>2024-09-29T11:19:09.3078493</Date>
                                  <DayOfWeek>Sunday</DayOfWeek>
                                  <EndBar>0</EndBar>
                                  <ForceSeriesIndex>false</ForceSeriesIndex>
                                  <LookBackPeriod>0</LookBackPeriod>
                                  <MarketPosition>Long</MarketPosition>
                                  <Period>0</Period>
                                  <ReturnType>Number</ReturnType>
                                  <StartBar>0</StartBar>
                                  <State>Undefined</State>
                                  <Time>0001-01-01T00:00:00</Time>
                                </DynamicValue>
                                <IsLiteral>false</IsLiteral>
                                <LiveValue xsi:type="xsd:string">ATRPeriod</LiveValue>
                              </anyType>
                            </value>
                          </item>
                        </CustomProperties>
                        <IndicatorHolder>
                          <IndicatorName>ATR</IndicatorName>
                          <Plots>
                            <Plot>
                              <IsOpacityVisible>false</IsOpacityVisible>
                              <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008B8B&lt;/SolidColorBrush&gt;</BrushSerialize>
                              <DashStyleHelper>Solid</DashStyleHelper>
                              <Opacity>100</Opacity>
                              <Width>1</Width>
                              <AutoWidth>false</AutoWidth>
                              <Max>1.7976931348623157E+308</Max>
                              <Min>-1.7976931348623157E+308</Min>
                              <Name>ATR</Name>
                              <PlotStyle>Line</PlotStyle>
                            </Plot>
                          </Plots>
                        </IndicatorHolder>
                        <IsExplicitlyNamed>false</IsExplicitlyNamed>
                        <IsPriceTypeLocked>false</IsPriceTypeLocked>
                        <PlotOnChart>true</PlotOnChart>
                        <PriceType>Close</PriceType>
                        <SeriesType>Indicator</SeriesType>
                      </AssociatedIndicator>
                      <BarsAgo>0</BarsAgo>
                      <CurrencyType>Currency</CurrencyType>
                      <Date>2024-09-29T11:19:02.4143372</Date>
                      <DayOfWeek>Sunday</DayOfWeek>
                      <EndBar>0</EndBar>
                      <ForceSeriesIndex>false</ForceSeriesIndex>
                      <LookBackPeriod>0</LookBackPeriod>
                      <MarketPosition>Long</MarketPosition>
                      <OffsetBuilder>
                        <ConditionOffset>
                          <IsSetEnabled>false</IsSetEnabled>
                          <OffsetValue>0</OffsetValue>
                          <OffsetOperator>Multiply</OffsetOperator>
                          <OffsetType>Arithmetic</OffsetType>
                        </ConditionOffset>
                        <Offset>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">TrailStopMultiplier</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>TrailStopMultiplier</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>TrailStopMultiplier</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-09-29T11:19:14.9730054</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">TrailStopMultiplier</LiveValue>
                        </Offset>
                      </OffsetBuilder>
                      <Period>0</Period>
                      <ReturnType>Series</ReturnType>
                      <StartBar>0</StartBar>
                      <State>Undefined</State>
                      <Time>0001-01-01T00:00:00</Time>
                    </DynamicValue>
                    <IsLiteral>false</IsLiteral>
                    <LiveValue xsi:type="xsd:string">(ATR(Close, Convert.ToInt32(ATRPeriod))[0] * TrailStopMultiplier) </LiveValue>
                  </Offset>
                </OffsetBuilder>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">(Close[0] - ((ATR(Close, Convert.ToInt32(ATRPeriod))[0] * TrailStopMultiplier) )) </LiveValue>
            </VariableDouble>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>double</UserVariableType>
          <VariableName>StoreTrailStop</VariableName>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set TriggerTrailStop</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:19:49.1787588</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T11:19:49.1787588</VariableDateTime>
            <VariableBool>true</VariableBool>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>bool</UserVariableType>
          <VariableName>TriggerTrailStop</VariableName>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Print</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <MessageValue>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set 3: </StringValue>
                </NinjaScriptString>
                <NinjaScriptString>
                  <Action>
                    <Children />
                    <IsExpanded>false</IsExpanded>
                    <IsSelected>true</IsSelected>
                    <Name>Time series</Name>
                    <OffsetType>Arithmetic</OffsetType>
                    <AssignedCommand>
                      <Command>Times[{0}][{1}].TimeOfDay</Command>
                      <Parameters>
                        <string>Series1</string>
                        <string>BarsAgo</string>
                      </Parameters>
                    </AssignedCommand>
                    <BarsAgo>0</BarsAgo>
                    <CurrencyType>Currency</CurrencyType>
                    <Date>2024-09-29T12:16:36.5951082</Date>
                    <DayOfWeek>Sunday</DayOfWeek>
                    <EndBar>0</EndBar>
                    <ForceSeriesIndex>true</ForceSeriesIndex>
                    <LookBackPeriod>0</LookBackPeriod>
                    <MarketPosition>Long</MarketPosition>
                    <Period>0</Period>
                    <ReturnType>Time</ReturnType>
                    <StartBar>0</StartBar>
                    <State>Undefined</State>
                    <Time>0001-01-01T00:00:00</Time>
                  </Action>
                  <Index>1</Index>
                  <StringValue>Times[0][0].TimeOfDay</StringValue>
                </NinjaScriptString>
              </Strings>
            </MessageValue>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T12:16:59.911165</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T12:16:59.911165</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>Misc</ActionType>
          <Command>
            <Command>Print</Command>
            <Parameters>
              <string>MessageValue</string>
            </Parameters>
          </Command>
        </WizardAction>
      </Actions>
      <ActiveAction>
        <Children />
        <IsExpanded>false</IsExpanded>
        <IsSelected>true</IsSelected>
        <Name>Print</Name>
        <OffsetType>Arithmetic</OffsetType>
        <ActionProperties>
          <DashStyle>Solid</DashStyle>
          <DivideTimePrice>false</DivideTimePrice>
          <Id />
          <File />
          <IsAutoScale>false</IsAutoScale>
          <IsSimulatedStop>false</IsSimulatedStop>
          <IsStop>false</IsStop>
          <LogLevel>Information</LogLevel>
          <MessageValue>
            <SeparatorCharacter> </SeparatorCharacter>
            <Strings>
              <NinjaScriptString>
                <Index>0</Index>
                <StringValue>Set 3: </StringValue>
              </NinjaScriptString>
              <NinjaScriptString>
                <Action>
                  <Children />
                  <IsExpanded>false</IsExpanded>
                  <IsSelected>true</IsSelected>
                  <Name>Time series</Name>
                  <OffsetType>Arithmetic</OffsetType>
                  <AssignedCommand>
                    <Command>Times[{0}][{1}].TimeOfDay</Command>
                    <Parameters>
                      <string>Series1</string>
                      <string>BarsAgo</string>
                    </Parameters>
                  </AssignedCommand>
                  <BarsAgo>0</BarsAgo>
                  <CurrencyType>Currency</CurrencyType>
                  <Date>2024-09-29T12:16:36.5951082</Date>
                  <DayOfWeek>Sunday</DayOfWeek>
                  <EndBar>0</EndBar>
                  <ForceSeriesIndex>true</ForceSeriesIndex>
                  <LookBackPeriod>0</LookBackPeriod>
                  <MarketPosition>Long</MarketPosition>
                  <Period>0</Period>
                  <ReturnType>Time</ReturnType>
                  <StartBar>0</StartBar>
                  <State>Undefined</State>
                  <Time>0001-01-01T00:00:00</Time>
                </Action>
                <Index>1</Index>
                <StringValue>Times[0][0].TimeOfDay</StringValue>
              </NinjaScriptString>
            </Strings>
          </MessageValue>
          <Mode>Currency</Mode>
          <OffsetType>Currency</OffsetType>
          <Priority>Medium</Priority>
          <Quantity>
            <DefaultValue>0</DefaultValue>
            <IsInt>true</IsInt>
            <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
            <DynamicValue>
              <Children />
              <IsExpanded>false</IsExpanded>
              <IsSelected>false</IsSelected>
              <Name>Default order quantity</Name>
              <OffsetType>Arithmetic</OffsetType>
              <AssignedCommand>
                <Command>DefaultQuantity</Command>
                <Parameters />
              </AssignedCommand>
              <BarsAgo>0</BarsAgo>
              <CurrencyType>Currency</CurrencyType>
              <Date>2024-09-29T12:16:59.911165</Date>
              <DayOfWeek>Sunday</DayOfWeek>
              <EndBar>0</EndBar>
              <ForceSeriesIndex>false</ForceSeriesIndex>
              <LookBackPeriod>0</LookBackPeriod>
              <MarketPosition>Long</MarketPosition>
              <Period>0</Period>
              <ReturnType>Number</ReturnType>
              <StartBar>0</StartBar>
              <State>Undefined</State>
              <Time>0001-01-01T00:00:00</Time>
            </DynamicValue>
            <IsLiteral>false</IsLiteral>
            <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
          </Quantity>
          <ServiceName />
          <ScreenshotPath />
          <SoundLocation />
          <TextPosition>BottomLeft</TextPosition>
          <VariableDateTime>2024-09-29T12:16:59.911165</VariableDateTime>
          <VariableBool>false</VariableBool>
        </ActionProperties>
        <ActionType>Misc</ActionType>
        <Command>
          <Command>Print</Command>
          <Parameters>
            <string>MessageValue</string>
          </Parameters>
        </Command>
      </ActiveAction>
      <AnyOrAll>All</AnyOrAll>
      <Conditions>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Current market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.MarketPosition</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:17:12.5688017</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>MarketPosition.{0}</Command>
                  <Parameters>
                    <string>MarketPosition</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:17:12.5976349</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>Position.MarketPosition = MarketPosition.Long</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Close</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:11:38.5447508</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>GreaterEqual</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>StoreTrailTarget</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>StoreTrailTarget</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:11:38.5590272</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>Default input[0] &gt;= StoreTrailTarget</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>InitialEntry</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>InitialEntry</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:14:36.7088201</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>False</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>false</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:14:36.721998</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>InitialEntry = false</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>IsFirstTickOfBar</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>IsFirstTickOfBar</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:16:47.2012527</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>True</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>true</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:16:47.2139792</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>TrailStopUpdate</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>TrailStopUpdate</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:17:17.8891793</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>False</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>false</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:17:17.9027306</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>true</IsGroup>
          <DisplayName>Condition group 1</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Close</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:22:26.1137909</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <OffsetBuilder>
                  <ConditionOffset>
                    <IsSetEnabled>false</IsSetEnabled>
                    <OffsetValue>0</OffsetValue>
                    <OffsetOperator>Subtract</OffsetOperator>
                    <OffsetType>Arithmetic</OffsetType>
                  </ConditionOffset>
                  <Offset>
                    <DefaultValue>0</DefaultValue>
                    <IsInt>false</IsInt>
                    <BindingValue xsi:type="xsd:string">(ATR(ATRPeriod)[0] * TrailStopMultiplier) </BindingValue>
                    <DynamicValue>
                      <Children />
                      <IsExpanded>false</IsExpanded>
                      <IsSelected>true</IsSelected>
                      <Name>ATR</Name>
                      <OffsetType>Arithmetic</OffsetType>
                      <AssignedCommand>
                        <Command>ATR</Command>
                        <Parameters>
                          <string>AssociatedIndicator</string>
                          <string>BarsAgo</string>
                          <string>OffsetBuilder</string>
                        </Parameters>
                      </AssignedCommand>
                      <AssociatedIndicator>
                        <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                        <CustomProperties>
                          <item>
                            <key>
                              <string>Period</string>
                            </key>
                            <value>
                              <anyType xsi:type="NumberBuilder">
                                <DefaultValue>0</DefaultValue>
                                <IsInt>true</IsInt>
                                <BindingValue xsi:type="xsd:string">ATRPeriod</BindingValue>
                                <DynamicValue>
                                  <Children />
                                  <IsExpanded>false</IsExpanded>
                                  <IsSelected>true</IsSelected>
                                  <Name>ATRPeriod</Name>
                                  <OffsetType>Arithmetic</OffsetType>
                                  <AssignedCommand>
                                    <Command>ATRPeriod</Command>
                                    <Parameters />
                                  </AssignedCommand>
                                  <BarsAgo>0</BarsAgo>
                                  <CurrencyType>Currency</CurrencyType>
                                  <Date>2024-09-29T11:23:34.3954502</Date>
                                  <DayOfWeek>Sunday</DayOfWeek>
                                  <EndBar>0</EndBar>
                                  <ForceSeriesIndex>false</ForceSeriesIndex>
                                  <LookBackPeriod>0</LookBackPeriod>
                                  <MarketPosition>Long</MarketPosition>
                                  <Period>0</Period>
                                  <ReturnType>Number</ReturnType>
                                  <StartBar>0</StartBar>
                                  <State>Undefined</State>
                                  <Time>0001-01-01T00:00:00</Time>
                                </DynamicValue>
                                <IsLiteral>false</IsLiteral>
                                <LiveValue xsi:type="xsd:string">ATRPeriod</LiveValue>
                              </anyType>
                            </value>
                          </item>
                        </CustomProperties>
                        <IndicatorHolder>
                          <IndicatorName>ATR</IndicatorName>
                          <Plots>
                            <Plot>
                              <IsOpacityVisible>false</IsOpacityVisible>
                              <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008B8B&lt;/SolidColorBrush&gt;</BrushSerialize>
                              <DashStyleHelper>Solid</DashStyleHelper>
                              <Opacity>100</Opacity>
                              <Width>1</Width>
                              <AutoWidth>false</AutoWidth>
                              <Max>1.7976931348623157E+308</Max>
                              <Min>-1.7976931348623157E+308</Min>
                              <Name>ATR</Name>
                              <PlotStyle>Line</PlotStyle>
                            </Plot>
                          </Plots>
                        </IndicatorHolder>
                        <IsExplicitlyNamed>false</IsExplicitlyNamed>
                        <IsPriceTypeLocked>false</IsPriceTypeLocked>
                        <PlotOnChart>true</PlotOnChart>
                        <PriceType>Close</PriceType>
                        <SeriesType>Indicator</SeriesType>
                      </AssociatedIndicator>
                      <BarsAgo>0</BarsAgo>
                      <CurrencyType>Currency</CurrencyType>
                      <Date>2024-09-29T11:23:27.5065309</Date>
                      <DayOfWeek>Sunday</DayOfWeek>
                      <EndBar>0</EndBar>
                      <ForceSeriesIndex>false</ForceSeriesIndex>
                      <LookBackPeriod>0</LookBackPeriod>
                      <MarketPosition>Long</MarketPosition>
                      <OffsetBuilder>
                        <ConditionOffset>
                          <IsSetEnabled>false</IsSetEnabled>
                          <OffsetValue>0</OffsetValue>
                          <OffsetOperator>Multiply</OffsetOperator>
                          <OffsetType>Arithmetic</OffsetType>
                        </ConditionOffset>
                        <Offset>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">TrailStopMultiplier</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>TrailStopMultiplier</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>TrailStopMultiplier</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-09-29T11:23:41.2745038</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">TrailStopMultiplier</LiveValue>
                        </Offset>
                      </OffsetBuilder>
                      <Period>0</Period>
                      <ReturnType>Series</ReturnType>
                      <StartBar>0</StartBar>
                      <State>Undefined</State>
                      <Time>0001-01-01T00:00:00</Time>
                    </DynamicValue>
                    <IsLiteral>false</IsLiteral>
                    <LiveValue xsi:type="xsd:string">(ATR(Close, Convert.ToInt32(ATRPeriod))[0] * TrailStopMultiplier) </LiveValue>
                  </Offset>
                </OffsetBuilder>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Greater</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>StoreTrailStop</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>StoreTrailStop</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:22:26.1406118</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>(Default input[0] - ((ATR(Close, Convert.ToInt32(ATRPeriod))[0] * TrailStopMultiplier) ))  &gt; StoreTrailStop</DisplayName>
        </WizardConditionGroup>
      </Conditions>
      <SetName>Set 3</SetName>
      <SetNumber>3</SetNumber>
    </ConditionalAction>
    <ConditionalAction>
      <Actions>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Exit long position by a limit order</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <FromEntrySignal>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>LongEntry1</StringValue>
                </NinjaScriptString>
              </Strings>
            </FromEntrySignal>
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LimitPrice>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">StoreProfit1</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>StoreProfit1</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>StoreProfit1</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:20:22.7148277</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">StoreProfit1</LiveValue>
            </LimitPrice>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">QTY1</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>QTY1</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>QTY1</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:20:36.5608201</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">QTY1</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>LongProfit1</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <Tag>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set Exit long position by a limit order</StringValue>
                </NinjaScriptString>
              </Strings>
            </Tag>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T10:19:47.9947262</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>ExitLimit</ActionType>
          <Command>
            <Command>ExitLongLimit</Command>
            <Parameters>
              <string>quantity</string>
              <string>limitPrice</string>
              <string>signalName</string>
              <string>fromEntrySignal</string>
            </Parameters>
          </Command>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Exit long position by a limit order</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <FromEntrySignal>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>LongEntry2</StringValue>
                </NinjaScriptString>
              </Strings>
            </FromEntrySignal>
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LimitPrice>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">StoreProfit2</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>StoreProfit2</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>StoreProfit2</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:21:21.212071</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">StoreProfit2</LiveValue>
            </LimitPrice>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">QTY2</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>QTY2</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>QTY2</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:21:27.1958621</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">QTY2</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>LongProfit2</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <Tag>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set Exit long position by a limit order</StringValue>
                </NinjaScriptString>
              </Strings>
            </Tag>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T10:21:00.1745909</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>ExitLimit</ActionType>
          <Command>
            <Command>ExitLongLimit</Command>
            <Parameters>
              <string>quantity</string>
              <string>limitPrice</string>
              <string>signalName</string>
              <string>fromEntrySignal</string>
            </Parameters>
          </Command>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Exit long position by a stop order</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <FromEntrySignal>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>LongEntry1</StringValue>
                </NinjaScriptString>
              </Strings>
            </FromEntrySignal>
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">QTY1</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>QTY1</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>QTY1</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:22:21.4838454</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">QTY1</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>LongStop1</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <StopPrice>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">StoreTrailStop</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>StoreTrailStop</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>StoreTrailStop</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:55:09.2057949</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">StoreTrailStop</LiveValue>
            </StopPrice>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T11:55:03.8538899</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>ExitStop</ActionType>
          <Command>
            <Command>ExitLongStopMarket</Command>
            <Parameters>
              <string>quantity</string>
              <string>stopPrice</string>
              <string>signalName</string>
              <string>fromEntrySignal</string>
            </Parameters>
          </Command>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Exit long position by a stop order</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <FromEntrySignal>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>LongEntry2</StringValue>
                </NinjaScriptString>
              </Strings>
            </FromEntrySignal>
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">QTY2</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>QTY2</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>QTY2</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:23:11.6329999</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">QTY2</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>LongStop2</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <StopPrice>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">StoreTrailStop</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>StoreTrailStop</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>StoreTrailStop</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:55:32.8296194</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">StoreTrailStop</LiveValue>
            </StopPrice>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T11:55:26.3604967</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>ExitStop</ActionType>
          <Command>
            <Command>ExitLongStopMarket</Command>
            <Parameters>
              <string>quantity</string>
              <string>stopPrice</string>
              <string>signalName</string>
              <string>fromEntrySignal</string>
            </Parameters>
          </Command>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set TrailStopUpdate</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:55:57.4271709</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T11:55:57.4271709</VariableDateTime>
            <VariableBool>true</VariableBool>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>bool</UserVariableType>
          <VariableName>TrailStopUpdate</VariableName>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Print</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <MessageValue>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set 4: </StringValue>
                </NinjaScriptString>
                <NinjaScriptString>
                  <Action>
                    <Children />
                    <IsExpanded>false</IsExpanded>
                    <IsSelected>true</IsSelected>
                    <Name>Time series</Name>
                    <OffsetType>Arithmetic</OffsetType>
                    <AssignedCommand>
                      <Command>Times[{0}][{1}].TimeOfDay</Command>
                      <Parameters>
                        <string>Series1</string>
                        <string>BarsAgo</string>
                      </Parameters>
                    </AssignedCommand>
                    <BarsAgo>0</BarsAgo>
                    <CurrencyType>Currency</CurrencyType>
                    <Date>2024-09-29T12:16:36.5951082</Date>
                    <DayOfWeek>Sunday</DayOfWeek>
                    <EndBar>0</EndBar>
                    <ForceSeriesIndex>true</ForceSeriesIndex>
                    <LookBackPeriod>0</LookBackPeriod>
                    <MarketPosition>Long</MarketPosition>
                    <Period>0</Period>
                    <ReturnType>Time</ReturnType>
                    <StartBar>0</StartBar>
                    <State>Undefined</State>
                    <Time>0001-01-01T00:00:00</Time>
                  </Action>
                  <Index>1</Index>
                  <StringValue>Times[0][0].TimeOfDay</StringValue>
                </NinjaScriptString>
              </Strings>
            </MessageValue>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T12:17:12.4070704</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T12:17:12.4080357</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>Misc</ActionType>
          <Command>
            <Command>Print</Command>
            <Parameters>
              <string>MessageValue</string>
            </Parameters>
          </Command>
        </WizardAction>
      </Actions>
      <AnyOrAll>All</AnyOrAll>
      <Conditions>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Current market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.MarketPosition</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:17:12.5688017</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>MarketPosition.{0}</Command>
                  <Parameters>
                    <string>MarketPosition</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:17:12.5976349</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>Position.MarketPosition = MarketPosition.Long</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>InitialEntry</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>InitialEntry</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:41:37.8072032</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>False</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>false</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:41:37.8331983</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>InitialEntry = false</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>TriggerTrailStop</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>TriggerTrailStop</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:50:41.0421594</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>True</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>true</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:50:41.0711653</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>TriggerTrailStop = true</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>IsFirstTickOfBar</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>IsFirstTickOfBar</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:51:40.9522529</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>True</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>true</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:51:40.9757658</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>TrailStopUpdate</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>TrailStopUpdate</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:51:55.8887945</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>False</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>false</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:51:55.902795</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>true</IsGroup>
          <DisplayName>Condition group 1</DisplayName>
        </WizardConditionGroup>
      </Conditions>
      <SetName>Set 4</SetName>
      <SetNumber>4</SetNumber>
    </ConditionalAction>
    <ConditionalAction>
      <Actions>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Enter short position</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">QTY1</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>QTY1</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>QTY1</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T12:07:40.3391126</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">QTY1</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>ShortEntry1</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <Tag>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set Enter short position</StringValue>
                </NinjaScriptString>
              </Strings>
            </Tag>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T12:07:32.7043971</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>Enter</ActionType>
          <Command>
            <Command>EnterShort</Command>
            <Parameters>
              <string>quantity</string>
              <string>signalName</string>
            </Parameters>
          </Command>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Enter short position</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">QTY2</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>QTY2</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>QTY2</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T12:08:07.5655341</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">QTY2</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>ShortEntry2</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <Tag>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set Enter short position</StringValue>
                </NinjaScriptString>
              </Strings>
            </Tag>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T12:08:02.4707016</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>Enter</ActionType>
          <Command>
            <Command>EnterShort</Command>
            <Parameters>
              <string>quantity</string>
              <string>signalName</string>
            </Parameters>
          </Command>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set StoreProfit1</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T12:08:19.4417739</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T12:08:19.4417739</VariableDateTime>
            <VariableBool>false</VariableBool>
            <VariableDouble>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">(Default input[0] - ((ATR(Close, Convert.ToInt32(ATRPeriod))[0] * ProfitMultiplier1) )) </BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Close</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:09:08.3590208</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <OffsetBuilder>
                  <ConditionOffset>
                    <IsSetEnabled>false</IsSetEnabled>
                    <OffsetValue>0</OffsetValue>
                    <OffsetOperator>Subtract</OffsetOperator>
                    <OffsetType>Arithmetic</OffsetType>
                  </ConditionOffset>
                  <Offset>
                    <DefaultValue>0</DefaultValue>
                    <IsInt>false</IsInt>
                    <BindingValue xsi:type="xsd:string">(ATR(ATRPeriod)[0] * ProfitMultiplier1) </BindingValue>
                    <DynamicValue>
                      <Children />
                      <IsExpanded>false</IsExpanded>
                      <IsSelected>true</IsSelected>
                      <Name>ATR</Name>
                      <OffsetType>Arithmetic</OffsetType>
                      <AssignedCommand>
                        <Command>ATR</Command>
                        <Parameters>
                          <string>AssociatedIndicator</string>
                          <string>BarsAgo</string>
                          <string>OffsetBuilder</string>
                        </Parameters>
                      </AssignedCommand>
                      <AssociatedIndicator>
                        <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                        <CustomProperties>
                          <item>
                            <key>
                              <string>Period</string>
                            </key>
                            <value>
                              <anyType xsi:type="NumberBuilder">
                                <DefaultValue>0</DefaultValue>
                                <IsInt>true</IsInt>
                                <BindingValue xsi:type="xsd:string">ATRPeriod</BindingValue>
                                <DynamicValue>
                                  <Children />
                                  <IsExpanded>false</IsExpanded>
                                  <IsSelected>true</IsSelected>
                                  <Name>ATRPeriod</Name>
                                  <OffsetType>Arithmetic</OffsetType>
                                  <AssignedCommand>
                                    <Command>ATRPeriod</Command>
                                    <Parameters />
                                  </AssignedCommand>
                                  <BarsAgo>0</BarsAgo>
                                  <CurrencyType>Currency</CurrencyType>
                                  <Date>2024-09-29T10:10:23.6974823</Date>
                                  <DayOfWeek>Sunday</DayOfWeek>
                                  <EndBar>0</EndBar>
                                  <ForceSeriesIndex>false</ForceSeriesIndex>
                                  <LookBackPeriod>0</LookBackPeriod>
                                  <MarketPosition>Long</MarketPosition>
                                  <Period>0</Period>
                                  <ReturnType>Number</ReturnType>
                                  <StartBar>0</StartBar>
                                  <State>Undefined</State>
                                  <Time>0001-01-01T00:00:00</Time>
                                </DynamicValue>
                                <IsLiteral>false</IsLiteral>
                                <LiveValue xsi:type="xsd:string">ATRPeriod</LiveValue>
                              </anyType>
                            </value>
                          </item>
                        </CustomProperties>
                        <IndicatorHolder>
                          <IndicatorName>ATR</IndicatorName>
                          <Plots>
                            <Plot>
                              <IsOpacityVisible>false</IsOpacityVisible>
                              <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008B8B&lt;/SolidColorBrush&gt;</BrushSerialize>
                              <DashStyleHelper>Solid</DashStyleHelper>
                              <Opacity>100</Opacity>
                              <Width>1</Width>
                              <AutoWidth>false</AutoWidth>
                              <Max>1.7976931348623157E+308</Max>
                              <Min>-1.7976931348623157E+308</Min>
                              <Name>ATR</Name>
                              <PlotStyle>Line</PlotStyle>
                            </Plot>
                          </Plots>
                        </IndicatorHolder>
                        <IsExplicitlyNamed>false</IsExplicitlyNamed>
                        <IsPriceTypeLocked>false</IsPriceTypeLocked>
                        <PlotOnChart>true</PlotOnChart>
                        <PriceType>Close</PriceType>
                        <SeriesType>Indicator</SeriesType>
                      </AssociatedIndicator>
                      <BarsAgo>0</BarsAgo>
                      <CurrencyType>Currency</CurrencyType>
                      <Date>2024-09-29T10:09:41.8636016</Date>
                      <DayOfWeek>Sunday</DayOfWeek>
                      <EndBar>0</EndBar>
                      <ForceSeriesIndex>false</ForceSeriesIndex>
                      <LookBackPeriod>0</LookBackPeriod>
                      <MarketPosition>Long</MarketPosition>
                      <OffsetBuilder>
                        <ConditionOffset>
                          <IsSetEnabled>false</IsSetEnabled>
                          <OffsetValue>0</OffsetValue>
                          <OffsetOperator>Multiply</OffsetOperator>
                          <OffsetType>Arithmetic</OffsetType>
                        </ConditionOffset>
                        <Offset>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">ProfitMultiplier1</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>ProfitMultiplier1</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>ProfitMultiplier1</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-09-29T10:10:53.1776917</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">ProfitMultiplier1</LiveValue>
                        </Offset>
                      </OffsetBuilder>
                      <Period>0</Period>
                      <ReturnType>Series</ReturnType>
                      <StartBar>0</StartBar>
                      <State>Undefined</State>
                      <Time>0001-01-01T00:00:00</Time>
                    </DynamicValue>
                    <IsLiteral>false</IsLiteral>
                    <LiveValue xsi:type="xsd:string">(ATR(Close, Convert.ToInt32(ATRPeriod))[0] * ProfitMultiplier1) </LiveValue>
                  </Offset>
                </OffsetBuilder>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">(Close[0] - ((ATR(Close, Convert.ToInt32(ATRPeriod))[0] * ProfitMultiplier1) )) </LiveValue>
            </VariableDouble>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>double</UserVariableType>
          <VariableName>StoreProfit1</VariableName>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set StoreProfit2</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T12:08:44.8151743</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T12:08:44.8151743</VariableDateTime>
            <VariableBool>false</VariableBool>
            <VariableDouble>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">(Default input[0] - ((ATR(Close, Convert.ToInt32(ATRPeriod))[0] * ProfitMultiplier2) )) </BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Close</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:12:56.0211987</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <OffsetBuilder>
                  <ConditionOffset>
                    <IsSetEnabled>false</IsSetEnabled>
                    <OffsetValue>0</OffsetValue>
                    <OffsetOperator>Subtract</OffsetOperator>
                    <OffsetType>Arithmetic</OffsetType>
                  </ConditionOffset>
                  <Offset>
                    <DefaultValue>0</DefaultValue>
                    <IsInt>false</IsInt>
                    <BindingValue xsi:type="xsd:string">(ATR(ATRPeriod)[0] * ProfitMultiplier2) </BindingValue>
                    <DynamicValue>
                      <Children />
                      <IsExpanded>false</IsExpanded>
                      <IsSelected>true</IsSelected>
                      <Name>ATR</Name>
                      <OffsetType>Arithmetic</OffsetType>
                      <AssignedCommand>
                        <Command>ATR</Command>
                        <Parameters>
                          <string>AssociatedIndicator</string>
                          <string>BarsAgo</string>
                          <string>OffsetBuilder</string>
                        </Parameters>
                      </AssignedCommand>
                      <AssociatedIndicator>
                        <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                        <CustomProperties>
                          <item>
                            <key>
                              <string>Period</string>
                            </key>
                            <value>
                              <anyType xsi:type="NumberBuilder">
                                <DefaultValue>0</DefaultValue>
                                <IsInt>true</IsInt>
                                <BindingValue xsi:type="xsd:string">ATRPeriod</BindingValue>
                                <DynamicValue>
                                  <Children />
                                  <IsExpanded>false</IsExpanded>
                                  <IsSelected>true</IsSelected>
                                  <Name>ATRPeriod</Name>
                                  <OffsetType>Arithmetic</OffsetType>
                                  <AssignedCommand>
                                    <Command>ATRPeriod</Command>
                                    <Parameters />
                                  </AssignedCommand>
                                  <BarsAgo>0</BarsAgo>
                                  <CurrencyType>Currency</CurrencyType>
                                  <Date>2024-09-29T10:13:15.3471292</Date>
                                  <DayOfWeek>Sunday</DayOfWeek>
                                  <EndBar>0</EndBar>
                                  <ForceSeriesIndex>false</ForceSeriesIndex>
                                  <LookBackPeriod>0</LookBackPeriod>
                                  <MarketPosition>Long</MarketPosition>
                                  <Period>0</Period>
                                  <ReturnType>Number</ReturnType>
                                  <StartBar>0</StartBar>
                                  <State>Undefined</State>
                                  <Time>0001-01-01T00:00:00</Time>
                                </DynamicValue>
                                <IsLiteral>false</IsLiteral>
                                <LiveValue xsi:type="xsd:string">ATRPeriod</LiveValue>
                              </anyType>
                            </value>
                          </item>
                        </CustomProperties>
                        <IndicatorHolder>
                          <IndicatorName>ATR</IndicatorName>
                          <Plots>
                            <Plot>
                              <IsOpacityVisible>false</IsOpacityVisible>
                              <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008B8B&lt;/SolidColorBrush&gt;</BrushSerialize>
                              <DashStyleHelper>Solid</DashStyleHelper>
                              <Opacity>100</Opacity>
                              <Width>1</Width>
                              <AutoWidth>false</AutoWidth>
                              <Max>1.7976931348623157E+308</Max>
                              <Min>-1.7976931348623157E+308</Min>
                              <Name>ATR</Name>
                              <PlotStyle>Line</PlotStyle>
                            </Plot>
                          </Plots>
                        </IndicatorHolder>
                        <IsExplicitlyNamed>false</IsExplicitlyNamed>
                        <IsPriceTypeLocked>false</IsPriceTypeLocked>
                        <PlotOnChart>true</PlotOnChart>
                        <PriceType>Close</PriceType>
                        <SeriesType>Indicator</SeriesType>
                      </AssociatedIndicator>
                      <BarsAgo>0</BarsAgo>
                      <CurrencyType>Currency</CurrencyType>
                      <Date>2024-09-29T10:13:05.2449651</Date>
                      <DayOfWeek>Sunday</DayOfWeek>
                      <EndBar>0</EndBar>
                      <ForceSeriesIndex>false</ForceSeriesIndex>
                      <LookBackPeriod>0</LookBackPeriod>
                      <MarketPosition>Long</MarketPosition>
                      <OffsetBuilder>
                        <ConditionOffset>
                          <IsSetEnabled>false</IsSetEnabled>
                          <OffsetValue>0</OffsetValue>
                          <OffsetOperator>Multiply</OffsetOperator>
                          <OffsetType>Arithmetic</OffsetType>
                        </ConditionOffset>
                        <Offset>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">ProfitMultiplier2</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>ProfitMultiplier2</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>ProfitMultiplier2</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-09-29T10:13:23.9438433</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">ProfitMultiplier2</LiveValue>
                        </Offset>
                      </OffsetBuilder>
                      <Period>0</Period>
                      <ReturnType>Series</ReturnType>
                      <StartBar>0</StartBar>
                      <State>Undefined</State>
                      <Time>0001-01-01T00:00:00</Time>
                    </DynamicValue>
                    <IsLiteral>false</IsLiteral>
                    <LiveValue xsi:type="xsd:string">(ATR(Close, Convert.ToInt32(ATRPeriod))[0] * ProfitMultiplier2) </LiveValue>
                  </Offset>
                </OffsetBuilder>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">(Close[0] - ((ATR(Close, Convert.ToInt32(ATRPeriod))[0] * ProfitMultiplier2) )) </LiveValue>
            </VariableDouble>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>double</UserVariableType>
          <VariableName>StoreProfit2</VariableName>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set StoreStop</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T12:08:54.1597798</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T12:08:54.1597798</VariableDateTime>
            <VariableBool>false</VariableBool>
            <VariableDouble>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">(Default input[0] + ((ATR(Close, Convert.ToInt32(ATRPeriod))[0] * StopMultiplier) )) </BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Close</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:13:47.5864972</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <OffsetBuilder>
                  <ConditionOffset>
                    <IsSetEnabled>false</IsSetEnabled>
                    <OffsetValue>0</OffsetValue>
                    <OffsetOperator>Add</OffsetOperator>
                    <OffsetType>Arithmetic</OffsetType>
                  </ConditionOffset>
                  <Offset>
                    <DefaultValue>0</DefaultValue>
                    <IsInt>false</IsInt>
                    <BindingValue xsi:type="xsd:string">(ATR(ATRPeriod)[0] * StopMultiplier) </BindingValue>
                    <DynamicValue>
                      <Children />
                      <IsExpanded>false</IsExpanded>
                      <IsSelected>true</IsSelected>
                      <Name>ATR</Name>
                      <OffsetType>Arithmetic</OffsetType>
                      <AssignedCommand>
                        <Command>ATR</Command>
                        <Parameters>
                          <string>AssociatedIndicator</string>
                          <string>BarsAgo</string>
                          <string>OffsetBuilder</string>
                        </Parameters>
                      </AssignedCommand>
                      <AssociatedIndicator>
                        <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                        <CustomProperties>
                          <item>
                            <key>
                              <string>Period</string>
                            </key>
                            <value>
                              <anyType xsi:type="NumberBuilder">
                                <DefaultValue>0</DefaultValue>
                                <IsInt>true</IsInt>
                                <BindingValue xsi:type="xsd:string">ATRPeriod</BindingValue>
                                <DynamicValue>
                                  <Children />
                                  <IsExpanded>false</IsExpanded>
                                  <IsSelected>true</IsSelected>
                                  <Name>ATRPeriod</Name>
                                  <OffsetType>Arithmetic</OffsetType>
                                  <AssignedCommand>
                                    <Command>ATRPeriod</Command>
                                    <Parameters />
                                  </AssignedCommand>
                                  <BarsAgo>0</BarsAgo>
                                  <CurrencyType>Currency</CurrencyType>
                                  <Date>2024-09-29T10:14:17.3923925</Date>
                                  <DayOfWeek>Sunday</DayOfWeek>
                                  <EndBar>0</EndBar>
                                  <ForceSeriesIndex>false</ForceSeriesIndex>
                                  <LookBackPeriod>0</LookBackPeriod>
                                  <MarketPosition>Long</MarketPosition>
                                  <Period>0</Period>
                                  <ReturnType>Number</ReturnType>
                                  <StartBar>0</StartBar>
                                  <State>Undefined</State>
                                  <Time>0001-01-01T00:00:00</Time>
                                </DynamicValue>
                                <IsLiteral>false</IsLiteral>
                                <LiveValue xsi:type="xsd:string">ATRPeriod</LiveValue>
                              </anyType>
                            </value>
                          </item>
                        </CustomProperties>
                        <IndicatorHolder>
                          <IndicatorName>ATR</IndicatorName>
                          <Plots>
                            <Plot>
                              <IsOpacityVisible>false</IsOpacityVisible>
                              <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008B8B&lt;/SolidColorBrush&gt;</BrushSerialize>
                              <DashStyleHelper>Solid</DashStyleHelper>
                              <Opacity>100</Opacity>
                              <Width>1</Width>
                              <AutoWidth>false</AutoWidth>
                              <Max>1.7976931348623157E+308</Max>
                              <Min>-1.7976931348623157E+308</Min>
                              <Name>ATR</Name>
                              <PlotStyle>Line</PlotStyle>
                            </Plot>
                          </Plots>
                        </IndicatorHolder>
                        <IsExplicitlyNamed>false</IsExplicitlyNamed>
                        <IsPriceTypeLocked>false</IsPriceTypeLocked>
                        <PlotOnChart>true</PlotOnChart>
                        <PriceType>Close</PriceType>
                        <SeriesType>Indicator</SeriesType>
                      </AssociatedIndicator>
                      <BarsAgo>0</BarsAgo>
                      <CurrencyType>Currency</CurrencyType>
                      <Date>2024-09-29T10:14:09.8858217</Date>
                      <DayOfWeek>Sunday</DayOfWeek>
                      <EndBar>0</EndBar>
                      <ForceSeriesIndex>false</ForceSeriesIndex>
                      <LookBackPeriod>0</LookBackPeriod>
                      <MarketPosition>Long</MarketPosition>
                      <OffsetBuilder>
                        <ConditionOffset>
                          <IsSetEnabled>false</IsSetEnabled>
                          <OffsetValue>0</OffsetValue>
                          <OffsetOperator>Multiply</OffsetOperator>
                          <OffsetType>Arithmetic</OffsetType>
                        </ConditionOffset>
                        <Offset>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">StopMultiplier</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>StopMultiplier</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>StopMultiplier</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-09-29T10:14:25.9027247</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">StopMultiplier</LiveValue>
                        </Offset>
                      </OffsetBuilder>
                      <Period>0</Period>
                      <ReturnType>Series</ReturnType>
                      <StartBar>0</StartBar>
                      <State>Undefined</State>
                      <Time>0001-01-01T00:00:00</Time>
                    </DynamicValue>
                    <IsLiteral>false</IsLiteral>
                    <LiveValue xsi:type="xsd:string">(ATR(Close, Convert.ToInt32(ATRPeriod))[0] * StopMultiplier) </LiveValue>
                  </Offset>
                </OffsetBuilder>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">(Close[0] + ((ATR(Close, Convert.ToInt32(ATRPeriod))[0] * StopMultiplier) )) </LiveValue>
            </VariableDouble>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>double</UserVariableType>
          <VariableName>StoreStop</VariableName>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set InitialEntry</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:42:02.6053323</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T10:42:02.6053323</VariableDateTime>
            <VariableBool>true</VariableBool>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>bool</UserVariableType>
          <VariableName>InitialEntry</VariableName>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set StoreTrailTarget</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T12:09:06.479377</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T12:09:06.479377</VariableDateTime>
            <VariableBool>false</VariableBool>
            <VariableDouble>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">(Default input[0] - ((ATR(Close, Convert.ToInt32(ATRPeriod))[0] * TrailTargetStart) )) </BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Close</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:12:44.1295756</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <OffsetBuilder>
                  <ConditionOffset>
                    <IsSetEnabled>false</IsSetEnabled>
                    <OffsetValue>0</OffsetValue>
                    <OffsetOperator>Subtract</OffsetOperator>
                    <OffsetType>Arithmetic</OffsetType>
                  </ConditionOffset>
                  <Offset>
                    <DefaultValue>0</DefaultValue>
                    <IsInt>false</IsInt>
                    <BindingValue xsi:type="xsd:string">(ATR(ATRPeriod)[0] * TrailTargetStart) </BindingValue>
                    <DynamicValue>
                      <Children />
                      <IsExpanded>false</IsExpanded>
                      <IsSelected>true</IsSelected>
                      <Name>ATR</Name>
                      <OffsetType>Arithmetic</OffsetType>
                      <AssignedCommand>
                        <Command>ATR</Command>
                        <Parameters>
                          <string>AssociatedIndicator</string>
                          <string>BarsAgo</string>
                          <string>OffsetBuilder</string>
                        </Parameters>
                      </AssignedCommand>
                      <AssociatedIndicator>
                        <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                        <CustomProperties>
                          <item>
                            <key>
                              <string>Period</string>
                            </key>
                            <value>
                              <anyType xsi:type="NumberBuilder">
                                <DefaultValue>0</DefaultValue>
                                <IsInt>true</IsInt>
                                <BindingValue xsi:type="xsd:string">ATRPeriod</BindingValue>
                                <DynamicValue>
                                  <Children />
                                  <IsExpanded>false</IsExpanded>
                                  <IsSelected>true</IsSelected>
                                  <Name>ATRPeriod</Name>
                                  <OffsetType>Arithmetic</OffsetType>
                                  <AssignedCommand>
                                    <Command>ATRPeriod</Command>
                                    <Parameters />
                                  </AssignedCommand>
                                  <BarsAgo>0</BarsAgo>
                                  <CurrencyType>Currency</CurrencyType>
                                  <Date>2024-09-29T11:13:07.9176008</Date>
                                  <DayOfWeek>Sunday</DayOfWeek>
                                  <EndBar>0</EndBar>
                                  <ForceSeriesIndex>false</ForceSeriesIndex>
                                  <LookBackPeriod>0</LookBackPeriod>
                                  <MarketPosition>Long</MarketPosition>
                                  <Period>0</Period>
                                  <ReturnType>Number</ReturnType>
                                  <StartBar>0</StartBar>
                                  <State>Undefined</State>
                                  <Time>0001-01-01T00:00:00</Time>
                                </DynamicValue>
                                <IsLiteral>false</IsLiteral>
                                <LiveValue xsi:type="xsd:string">ATRPeriod</LiveValue>
                              </anyType>
                            </value>
                          </item>
                        </CustomProperties>
                        <IndicatorHolder>
                          <IndicatorName>ATR</IndicatorName>
                          <Plots>
                            <Plot>
                              <IsOpacityVisible>false</IsOpacityVisible>
                              <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008B8B&lt;/SolidColorBrush&gt;</BrushSerialize>
                              <DashStyleHelper>Solid</DashStyleHelper>
                              <Opacity>100</Opacity>
                              <Width>1</Width>
                              <AutoWidth>false</AutoWidth>
                              <Max>1.7976931348623157E+308</Max>
                              <Min>-1.7976931348623157E+308</Min>
                              <Name>ATR</Name>
                              <PlotStyle>Line</PlotStyle>
                            </Plot>
                          </Plots>
                        </IndicatorHolder>
                        <IsExplicitlyNamed>false</IsExplicitlyNamed>
                        <IsPriceTypeLocked>false</IsPriceTypeLocked>
                        <PlotOnChart>true</PlotOnChart>
                        <PriceType>Close</PriceType>
                        <SeriesType>Indicator</SeriesType>
                      </AssociatedIndicator>
                      <BarsAgo>0</BarsAgo>
                      <CurrencyType>Currency</CurrencyType>
                      <Date>2024-09-29T11:12:55.7160932</Date>
                      <DayOfWeek>Sunday</DayOfWeek>
                      <EndBar>0</EndBar>
                      <ForceSeriesIndex>false</ForceSeriesIndex>
                      <LookBackPeriod>0</LookBackPeriod>
                      <MarketPosition>Long</MarketPosition>
                      <OffsetBuilder>
                        <ConditionOffset>
                          <IsSetEnabled>false</IsSetEnabled>
                          <OffsetValue>0</OffsetValue>
                          <OffsetOperator>Multiply</OffsetOperator>
                          <OffsetType>Arithmetic</OffsetType>
                        </ConditionOffset>
                        <Offset>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">TrailTargetStart</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>TrailTargetStart</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>TrailTargetStart</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-09-29T11:13:15.7448574</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">TrailTargetStart</LiveValue>
                        </Offset>
                      </OffsetBuilder>
                      <Period>0</Period>
                      <ReturnType>Series</ReturnType>
                      <StartBar>0</StartBar>
                      <State>Undefined</State>
                      <Time>0001-01-01T00:00:00</Time>
                    </DynamicValue>
                    <IsLiteral>false</IsLiteral>
                    <LiveValue xsi:type="xsd:string">(ATR(Close, Convert.ToInt32(ATRPeriod))[0] * TrailTargetStart) </LiveValue>
                  </Offset>
                </OffsetBuilder>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">(Close[0] - ((ATR(Close, Convert.ToInt32(ATRPeriod))[0] * TrailTargetStart) )) </LiveValue>
            </VariableDouble>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>double</UserVariableType>
          <VariableName>StoreTrailTarget</VariableName>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set StoreTrailStop</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:24:58.0946777</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T11:24:58.0946777</VariableDateTime>
            <VariableBool>false</VariableBool>
            <VariableDouble>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">StoreStop</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>StoreStop</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>StoreStop</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:25:10.55461</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">StoreStop</LiveValue>
            </VariableDouble>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>double</UserVariableType>
          <VariableName>StoreTrailStop</VariableName>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set TriggerTrailStop</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:58:14.0307941</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T11:58:14.0307941</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>bool</UserVariableType>
          <VariableName>TriggerTrailStop</VariableName>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set TrailStopUpdate</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:58:25.5717792</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T11:58:25.5717792</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>bool</UserVariableType>
          <VariableName>TrailStopUpdate</VariableName>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Print</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <MessageValue>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set 5: </StringValue>
                </NinjaScriptString>
                <NinjaScriptString>
                  <Action>
                    <Children />
                    <IsExpanded>false</IsExpanded>
                    <IsSelected>true</IsSelected>
                    <Name>Time series</Name>
                    <OffsetType>Arithmetic</OffsetType>
                    <AssignedCommand>
                      <Command>Times[{0}][{1}].TimeOfDay</Command>
                      <Parameters>
                        <string>Series1</string>
                        <string>BarsAgo</string>
                      </Parameters>
                    </AssignedCommand>
                    <BarsAgo>0</BarsAgo>
                    <CurrencyType>Currency</CurrencyType>
                    <Date>2024-09-29T12:16:36.5951082</Date>
                    <DayOfWeek>Sunday</DayOfWeek>
                    <EndBar>0</EndBar>
                    <ForceSeriesIndex>true</ForceSeriesIndex>
                    <LookBackPeriod>0</LookBackPeriod>
                    <MarketPosition>Long</MarketPosition>
                    <Period>0</Period>
                    <ReturnType>Time</ReturnType>
                    <StartBar>0</StartBar>
                    <State>Undefined</State>
                    <Time>0001-01-01T00:00:00</Time>
                  </Action>
                  <Index>1</Index>
                  <StringValue>Times[0][0].TimeOfDay</StringValue>
                </NinjaScriptString>
              </Strings>
            </MessageValue>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T12:18:17.2205588</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T12:18:17.2205588</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>Misc</ActionType>
          <Command>
            <Command>Print</Command>
            <Parameters>
              <string>MessageValue</string>
            </Parameters>
          </Command>
        </WizardAction>
      </Actions>
      <AnyOrAll>All</AnyOrAll>
      <Conditions>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Current market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.MarketPosition</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:01:42.8325241</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>MarketPosition.{0}</Command>
                  <Parameters>
                    <string>MarketPosition</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:01:42.8792814</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Flat</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>Position.MarketPosition = MarketPosition.Flat</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>All</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Close</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:05:51.9020976</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Less</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>SMA</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>SMA</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Period</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">50</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">50</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>SMA</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFDAA520&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>SMA</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>true</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                </AssociatedIndicator>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:05:51.9151127</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>ADX</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>ADX</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Period</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">14</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">14</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>ADX</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008B8B&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ADX</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                </AssociatedIndicator>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-05-12T23:25:13.9008633</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>GreaterEqual</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>ADXPeriod</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>ADXPeriod</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-05-12T23:25:13.9388619</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Falling</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>IsFalling({0})</Command>
                  <Parameters>
                    <string>Series1</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-05-12T23:48:41.1753527</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <Series1>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Period</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">8</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">8</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>EMA</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFDAA520&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>EMA</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                </Series1>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>True</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>true</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-05-12T23:48:41.2148511</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Typical</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-06-30T13:49:37.7822517</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <Series1>
                  <AcceptableSeries>DataSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties />
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>true</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Typical</PriceType>
                  <SeriesType>DefaultSeries</SeriesType>
                </Series1>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>CrossBelow</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>OrbLow</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>OrbLow</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-05-17T23:35:49.1614234</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>EMA</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>EMA</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Period</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">10</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">10</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>EMA</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFDAA520&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>EMA</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                </AssociatedIndicator>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-05-18T00:37:54.0099609</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Less</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>EMA</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>EMA</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Period</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">20</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">20</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>EMA</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFDAA520&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>EMA</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                </AssociatedIndicator>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-05-18T00:37:54.0444601</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>EMA</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>EMA</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Period</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">10</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">10</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>EMA</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFDAA520&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>EMA</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                </AssociatedIndicator>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-05-18T10:31:22.7276904</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Less</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>SMA</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>SMA</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Period</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">50</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">50</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>SMA</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFDAA520&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>SMA</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                </AssociatedIndicator>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-05-18T10:31:22.7571931</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>true</IsGroup>
          <DisplayName>Short Entry Condition</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>InitialEntry</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>InitialEntry</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:41:37.8072032</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>False</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>false</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:41:37.8331983</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>InitialEntry = false</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>TriggerTrailStop</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>TriggerTrailStop</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:48:05.4996371</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>False</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>false</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:48:05.5118311</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>TriggerTrailStop = false</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>All</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Time series</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Times[{0}][{1}].TimeOfDay</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>BarsAgo</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-05-12T23:19:41.9921754</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>true</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Time</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>GreaterEqual</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>StartTime</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>StartTime.TimeOfDay</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-05-12T23:19:42.0161752</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Time</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Time series</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Times[{0}][{1}].TimeOfDay</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>BarsAgo</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-05-12T23:20:00.4746159</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>true</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Time</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Less</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>StopTime</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>StopTime.TimeOfDay</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-05-12T23:20:00.5106157</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Time</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>true</IsGroup>
          <DisplayName>Time</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>All</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Rising</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>IsRising({0})</Command>
                  <Parameters>
                    <string>Series1</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-06-29T13:10:01.8057959</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <Series1>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Period</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">ATRPeriod</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>ATRPeriod</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>ATRPeriod</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2025-06-29T13:10:33.4613064</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">ATRPeriod</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>ADX</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008B8B&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ADX</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                </Series1>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>True</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>true</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-06-29T13:10:01.8278097</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Falling</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>IsFalling({0})</Command>
                  <Parameters>
                    <string>Series1</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-06-29T14:07:47.1156601</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <Series1>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Period</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">14</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">14</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Smooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">3</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">3</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>RSI</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF1E90FF&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>RSI</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFDAA520&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>Avg</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                  <SelectedPlot>RSI</SelectedPlot>
                </Series1>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>True</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>true</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-06-29T14:07:47.135659</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>RSI</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>RSI</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>Period</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">14</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">14</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Smooth</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>true</IsInt>
                          <BindingValue xsi:type="xsd:string">3</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">3</LiveValue>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>RSI</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF1E90FF&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>RSI</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFDAA520&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>Avg</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                  <SelectedPlot>Avg</SelectedPlot>
                </AssociatedIndicator>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-06-30T13:46:57.3831236</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>LessEqual</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Numeric value</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>NumericValue</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-06-30T13:46:57.403732</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <NumericValue>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>false</IsInt>
                  <BindingValue xsi:type="xsd:string">70</BindingValue>
                  <IsLiteral>true</IsLiteral>
                  <LiveValue xsi:type="xsd:string">70</LiveValue>
                </NumericValue>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>true</IsGroup>
          <DisplayName>Confluence</DisplayName>
        </WizardConditionGroup>
      </Conditions>
      <SetName>Set 5</SetName>
      <SetNumber>5</SetNumber>
    </ConditionalAction>
    <ConditionalAction>
      <Actions>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Exit short position by a limit order</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <FromEntrySignal>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>ShortEntry1</StringValue>
                </NinjaScriptString>
              </Strings>
            </FromEntrySignal>
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LimitPrice>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">StoreProfit1</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>StoreProfit1</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>StoreProfit1</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T12:10:14.194136</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">StoreProfit1</LiveValue>
            </LimitPrice>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">QTY1</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>QTY1</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>QTY1</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T12:10:19.9495209</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">QTY1</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>ShortProfit1</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <Tag>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set Exit short position by a limit order</StringValue>
                </NinjaScriptString>
              </Strings>
            </Tag>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T12:09:53.7712622</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>ExitLimit</ActionType>
          <Command>
            <Command>ExitShortLimit</Command>
            <Parameters>
              <string>quantity</string>
              <string>limitPrice</string>
              <string>signalName</string>
              <string>fromEntrySignal</string>
            </Parameters>
          </Command>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Exit short position by a limit order</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <FromEntrySignal>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>ShortEntry2</StringValue>
                </NinjaScriptString>
              </Strings>
            </FromEntrySignal>
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LimitPrice>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">StoreProfit2</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>StoreProfit2</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>StoreProfit2</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T12:10:50.7028417</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">StoreProfit2</LiveValue>
            </LimitPrice>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">QTY2</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>QTY2</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>QTY2</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T12:10:55.6986651</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">QTY2</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>ShortProfit2</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <Tag>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set Exit short position by a limit order</StringValue>
                </NinjaScriptString>
              </Strings>
            </Tag>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T12:10:38.8011913</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>ExitLimit</ActionType>
          <Command>
            <Command>ExitShortLimit</Command>
            <Parameters>
              <string>quantity</string>
              <string>limitPrice</string>
              <string>signalName</string>
              <string>fromEntrySignal</string>
            </Parameters>
          </Command>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Exit short position by a stop order</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <FromEntrySignal>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>ShortEntry1</StringValue>
                </NinjaScriptString>
              </Strings>
            </FromEntrySignal>
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">QTY1</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>QTY1</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>QTY1</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T12:11:23.2655973</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">QTY1</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>ShortStop1</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <StopPrice>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">StoreStop</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>StoreStop</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>StoreStop</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T12:11:33.618845</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">StoreStop</LiveValue>
            </StopPrice>
            <Tag>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set Exit short position by a stop order</StringValue>
                </NinjaScriptString>
              </Strings>
            </Tag>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T12:11:09.5857942</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>ExitStop</ActionType>
          <Command>
            <Command>ExitShortStopMarket</Command>
            <Parameters>
              <string>quantity</string>
              <string>stopPrice</string>
              <string>signalName</string>
              <string>fromEntrySignal</string>
            </Parameters>
          </Command>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Exit short position by a stop order</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <FromEntrySignal>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>ShortEntry2</StringValue>
                </NinjaScriptString>
              </Strings>
            </FromEntrySignal>
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">QTY2</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>QTY2</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>QTY2</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T12:11:57.0013734</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">QTY2</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>ShortStop2</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <StopPrice>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">StoreStop</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>StoreStop</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>StoreStop</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T12:12:05.7313009</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">StoreStop</LiveValue>
            </StopPrice>
            <Tag>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set Exit short position by a stop order</StringValue>
                </NinjaScriptString>
              </Strings>
            </Tag>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T12:11:44.1819986</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>ExitStop</ActionType>
          <Command>
            <Command>ExitShortStopMarket</Command>
            <Parameters>
              <string>quantity</string>
              <string>stopPrice</string>
              <string>signalName</string>
              <string>fromEntrySignal</string>
            </Parameters>
          </Command>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set InitialEntry</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:43:24.2179834</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T10:43:24.2179834</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>bool</UserVariableType>
          <VariableName>InitialEntry</VariableName>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Print</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <MessageValue>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set 6: </StringValue>
                </NinjaScriptString>
                <NinjaScriptString>
                  <Action>
                    <Children />
                    <IsExpanded>false</IsExpanded>
                    <IsSelected>true</IsSelected>
                    <Name>Time series</Name>
                    <OffsetType>Arithmetic</OffsetType>
                    <AssignedCommand>
                      <Command>Times[{0}][{1}].TimeOfDay</Command>
                      <Parameters>
                        <string>Series1</string>
                        <string>BarsAgo</string>
                      </Parameters>
                    </AssignedCommand>
                    <BarsAgo>0</BarsAgo>
                    <CurrencyType>Currency</CurrencyType>
                    <Date>2024-09-29T12:16:36.5951082</Date>
                    <DayOfWeek>Sunday</DayOfWeek>
                    <EndBar>0</EndBar>
                    <ForceSeriesIndex>true</ForceSeriesIndex>
                    <LookBackPeriod>0</LookBackPeriod>
                    <MarketPosition>Long</MarketPosition>
                    <Period>0</Period>
                    <ReturnType>Time</ReturnType>
                    <StartBar>0</StartBar>
                    <State>Undefined</State>
                    <Time>0001-01-01T00:00:00</Time>
                  </Action>
                  <Index>1</Index>
                  <StringValue>Times[0][0].TimeOfDay</StringValue>
                </NinjaScriptString>
              </Strings>
            </MessageValue>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T12:17:59.8874949</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T12:17:59.8874949</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>Misc</ActionType>
          <Command>
            <Command>Print</Command>
            <Parameters>
              <string>MessageValue</string>
            </Parameters>
          </Command>
        </WizardAction>
      </Actions>
      <AnyOrAll>All</AnyOrAll>
      <Conditions>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Current market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.MarketPosition</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:17:12.5688017</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>MarketPosition.{0}</Command>
                  <Parameters>
                    <string>MarketPosition</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:17:12.5976349</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Short</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>Position.MarketPosition = MarketPosition.Short</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>IsFirstTickOfBar</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>IsFirstTickOfBar</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:38:55.0154843</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>True</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>true</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:38:55.0427548</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>InitialEntry</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>InitialEntry</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:42:40.3323622</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>True</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>true</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:42:40.3604866</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>true</IsGroup>
          <DisplayName>Condition group 1</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>TriggerTrailStop</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>TriggerTrailStop</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:48:05.4996371</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>False</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>false</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:48:05.5118311</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>TriggerTrailStop = false</DisplayName>
        </WizardConditionGroup>
      </Conditions>
      <SetName>Set 6</SetName>
      <SetNumber>6</SetNumber>
    </ConditionalAction>
    <ConditionalAction>
      <Actions>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set StoreTrailStop</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T12:13:49.948654</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T12:13:49.948654</VariableDateTime>
            <VariableBool>false</VariableBool>
            <VariableDouble>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">(Default input[0] + ((ATR(Close, Convert.ToInt32(ATRPeriod))[0] * TrailStopMultiplier) )) </BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Close</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:18:46.3104769</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <OffsetBuilder>
                  <ConditionOffset>
                    <IsSetEnabled>false</IsSetEnabled>
                    <OffsetValue>0</OffsetValue>
                    <OffsetOperator>Add</OffsetOperator>
                    <OffsetType>Arithmetic</OffsetType>
                  </ConditionOffset>
                  <Offset>
                    <DefaultValue>0</DefaultValue>
                    <IsInt>false</IsInt>
                    <BindingValue xsi:type="xsd:string">(ATR(ATRPeriod)[0] * TrailStopMultiplier) </BindingValue>
                    <DynamicValue>
                      <Children />
                      <IsExpanded>false</IsExpanded>
                      <IsSelected>true</IsSelected>
                      <Name>ATR</Name>
                      <OffsetType>Arithmetic</OffsetType>
                      <AssignedCommand>
                        <Command>ATR</Command>
                        <Parameters>
                          <string>AssociatedIndicator</string>
                          <string>BarsAgo</string>
                          <string>OffsetBuilder</string>
                        </Parameters>
                      </AssignedCommand>
                      <AssociatedIndicator>
                        <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                        <CustomProperties>
                          <item>
                            <key>
                              <string>Period</string>
                            </key>
                            <value>
                              <anyType xsi:type="NumberBuilder">
                                <DefaultValue>0</DefaultValue>
                                <IsInt>true</IsInt>
                                <BindingValue xsi:type="xsd:string">ATRPeriod</BindingValue>
                                <DynamicValue>
                                  <Children />
                                  <IsExpanded>false</IsExpanded>
                                  <IsSelected>true</IsSelected>
                                  <Name>ATRPeriod</Name>
                                  <OffsetType>Arithmetic</OffsetType>
                                  <AssignedCommand>
                                    <Command>ATRPeriod</Command>
                                    <Parameters />
                                  </AssignedCommand>
                                  <BarsAgo>0</BarsAgo>
                                  <CurrencyType>Currency</CurrencyType>
                                  <Date>2024-09-29T11:19:09.3078493</Date>
                                  <DayOfWeek>Sunday</DayOfWeek>
                                  <EndBar>0</EndBar>
                                  <ForceSeriesIndex>false</ForceSeriesIndex>
                                  <LookBackPeriod>0</LookBackPeriod>
                                  <MarketPosition>Long</MarketPosition>
                                  <Period>0</Period>
                                  <ReturnType>Number</ReturnType>
                                  <StartBar>0</StartBar>
                                  <State>Undefined</State>
                                  <Time>0001-01-01T00:00:00</Time>
                                </DynamicValue>
                                <IsLiteral>false</IsLiteral>
                                <LiveValue xsi:type="xsd:string">ATRPeriod</LiveValue>
                              </anyType>
                            </value>
                          </item>
                        </CustomProperties>
                        <IndicatorHolder>
                          <IndicatorName>ATR</IndicatorName>
                          <Plots>
                            <Plot>
                              <IsOpacityVisible>false</IsOpacityVisible>
                              <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008B8B&lt;/SolidColorBrush&gt;</BrushSerialize>
                              <DashStyleHelper>Solid</DashStyleHelper>
                              <Opacity>100</Opacity>
                              <Width>1</Width>
                              <AutoWidth>false</AutoWidth>
                              <Max>1.7976931348623157E+308</Max>
                              <Min>-1.7976931348623157E+308</Min>
                              <Name>ATR</Name>
                              <PlotStyle>Line</PlotStyle>
                            </Plot>
                          </Plots>
                        </IndicatorHolder>
                        <IsExplicitlyNamed>false</IsExplicitlyNamed>
                        <IsPriceTypeLocked>false</IsPriceTypeLocked>
                        <PlotOnChart>true</PlotOnChart>
                        <PriceType>Close</PriceType>
                        <SeriesType>Indicator</SeriesType>
                      </AssociatedIndicator>
                      <BarsAgo>0</BarsAgo>
                      <CurrencyType>Currency</CurrencyType>
                      <Date>2024-09-29T11:19:02.4143372</Date>
                      <DayOfWeek>Sunday</DayOfWeek>
                      <EndBar>0</EndBar>
                      <ForceSeriesIndex>false</ForceSeriesIndex>
                      <LookBackPeriod>0</LookBackPeriod>
                      <MarketPosition>Long</MarketPosition>
                      <OffsetBuilder>
                        <ConditionOffset>
                          <IsSetEnabled>false</IsSetEnabled>
                          <OffsetValue>0</OffsetValue>
                          <OffsetOperator>Multiply</OffsetOperator>
                          <OffsetType>Arithmetic</OffsetType>
                        </ConditionOffset>
                        <Offset>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">TrailStopMultiplier</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>TrailStopMultiplier</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>TrailStopMultiplier</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-09-29T11:19:14.9730054</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">TrailStopMultiplier</LiveValue>
                        </Offset>
                      </OffsetBuilder>
                      <Period>0</Period>
                      <ReturnType>Series</ReturnType>
                      <StartBar>0</StartBar>
                      <State>Undefined</State>
                      <Time>0001-01-01T00:00:00</Time>
                    </DynamicValue>
                    <IsLiteral>false</IsLiteral>
                    <LiveValue xsi:type="xsd:string">(ATR(Close, Convert.ToInt32(ATRPeriod))[0] * TrailStopMultiplier) </LiveValue>
                  </Offset>
                </OffsetBuilder>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">(Close[0] + ((ATR(Close, Convert.ToInt32(ATRPeriod))[0] * TrailStopMultiplier) )) </LiveValue>
            </VariableDouble>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>double</UserVariableType>
          <VariableName>StoreTrailStop</VariableName>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set TriggerTrailStop</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:19:49.1787588</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T11:19:49.1787588</VariableDateTime>
            <VariableBool>true</VariableBool>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>bool</UserVariableType>
          <VariableName>TriggerTrailStop</VariableName>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Print</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <MessageValue>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set 7: </StringValue>
                </NinjaScriptString>
                <NinjaScriptString>
                  <Action>
                    <Children />
                    <IsExpanded>false</IsExpanded>
                    <IsSelected>true</IsSelected>
                    <Name>Time series</Name>
                    <OffsetType>Arithmetic</OffsetType>
                    <AssignedCommand>
                      <Command>Times[{0}][{1}].TimeOfDay</Command>
                      <Parameters>
                        <string>Series1</string>
                        <string>BarsAgo</string>
                      </Parameters>
                    </AssignedCommand>
                    <BarsAgo>0</BarsAgo>
                    <CurrencyType>Currency</CurrencyType>
                    <Date>2024-09-29T12:16:36.5951082</Date>
                    <DayOfWeek>Sunday</DayOfWeek>
                    <EndBar>0</EndBar>
                    <ForceSeriesIndex>true</ForceSeriesIndex>
                    <LookBackPeriod>0</LookBackPeriod>
                    <MarketPosition>Long</MarketPosition>
                    <Period>0</Period>
                    <ReturnType>Time</ReturnType>
                    <StartBar>0</StartBar>
                    <State>Undefined</State>
                    <Time>0001-01-01T00:00:00</Time>
                  </Action>
                  <Index>1</Index>
                  <StringValue>Times[0][0].TimeOfDay</StringValue>
                </NinjaScriptString>
              </Strings>
            </MessageValue>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T12:18:29.0851927</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T12:18:29.0851927</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>Misc</ActionType>
          <Command>
            <Command>Print</Command>
            <Parameters>
              <string>MessageValue</string>
            </Parameters>
          </Command>
        </WizardAction>
      </Actions>
      <ActiveAction>
        <Children />
        <IsExpanded>false</IsExpanded>
        <IsSelected>true</IsSelected>
        <Name>Print</Name>
        <OffsetType>Arithmetic</OffsetType>
        <ActionProperties>
          <DashStyle>Solid</DashStyle>
          <DivideTimePrice>false</DivideTimePrice>
          <Id />
          <File />
          <IsAutoScale>false</IsAutoScale>
          <IsSimulatedStop>false</IsSimulatedStop>
          <IsStop>false</IsStop>
          <LogLevel>Information</LogLevel>
          <MessageValue>
            <SeparatorCharacter> </SeparatorCharacter>
            <Strings>
              <NinjaScriptString>
                <Index>0</Index>
                <StringValue>Set 7: </StringValue>
              </NinjaScriptString>
              <NinjaScriptString>
                <Action>
                  <Children />
                  <IsExpanded>false</IsExpanded>
                  <IsSelected>true</IsSelected>
                  <Name>Time series</Name>
                  <OffsetType>Arithmetic</OffsetType>
                  <AssignedCommand>
                    <Command>Times[{0}][{1}].TimeOfDay</Command>
                    <Parameters>
                      <string>Series1</string>
                      <string>BarsAgo</string>
                    </Parameters>
                  </AssignedCommand>
                  <BarsAgo>0</BarsAgo>
                  <CurrencyType>Currency</CurrencyType>
                  <Date>2024-09-29T12:16:36.5951082</Date>
                  <DayOfWeek>Sunday</DayOfWeek>
                  <EndBar>0</EndBar>
                  <ForceSeriesIndex>true</ForceSeriesIndex>
                  <LookBackPeriod>0</LookBackPeriod>
                  <MarketPosition>Long</MarketPosition>
                  <Period>0</Period>
                  <ReturnType>Time</ReturnType>
                  <StartBar>0</StartBar>
                  <State>Undefined</State>
                  <Time>0001-01-01T00:00:00</Time>
                </Action>
                <Index>1</Index>
                <StringValue>Times[0][0].TimeOfDay</StringValue>
              </NinjaScriptString>
            </Strings>
          </MessageValue>
          <Mode>Currency</Mode>
          <OffsetType>Currency</OffsetType>
          <Priority>Medium</Priority>
          <Quantity>
            <DefaultValue>0</DefaultValue>
            <IsInt>true</IsInt>
            <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
            <DynamicValue>
              <Children />
              <IsExpanded>false</IsExpanded>
              <IsSelected>false</IsSelected>
              <Name>Default order quantity</Name>
              <OffsetType>Arithmetic</OffsetType>
              <AssignedCommand>
                <Command>DefaultQuantity</Command>
                <Parameters />
              </AssignedCommand>
              <BarsAgo>0</BarsAgo>
              <CurrencyType>Currency</CurrencyType>
              <Date>2024-09-29T12:18:29.0851927</Date>
              <DayOfWeek>Sunday</DayOfWeek>
              <EndBar>0</EndBar>
              <ForceSeriesIndex>false</ForceSeriesIndex>
              <LookBackPeriod>0</LookBackPeriod>
              <MarketPosition>Long</MarketPosition>
              <Period>0</Period>
              <ReturnType>Number</ReturnType>
              <StartBar>0</StartBar>
              <State>Undefined</State>
              <Time>0001-01-01T00:00:00</Time>
            </DynamicValue>
            <IsLiteral>false</IsLiteral>
            <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
          </Quantity>
          <ServiceName />
          <ScreenshotPath />
          <SoundLocation />
          <TextPosition>BottomLeft</TextPosition>
          <VariableDateTime>2024-09-29T12:18:29.0851927</VariableDateTime>
          <VariableBool>false</VariableBool>
        </ActionProperties>
        <ActionType>Misc</ActionType>
        <Command>
          <Command>Print</Command>
          <Parameters>
            <string>MessageValue</string>
          </Parameters>
        </Command>
      </ActiveAction>
      <AnyOrAll>All</AnyOrAll>
      <Conditions>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Current market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.MarketPosition</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:17:12.5688017</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>MarketPosition.{0}</Command>
                  <Parameters>
                    <string>MarketPosition</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:17:12.5976349</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Short</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>Position.MarketPosition = MarketPosition.Short</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Close</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:11:38.5447508</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>LessEqual</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>StoreTrailTarget</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>StoreTrailTarget</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:11:38.5590272</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>Default input[0] &lt;= StoreTrailTarget</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>InitialEntry</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>InitialEntry</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:14:36.7088201</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>False</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>false</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:14:36.721998</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>InitialEntry = false</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>IsFirstTickOfBar</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>IsFirstTickOfBar</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:16:47.2012527</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>True</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>true</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:16:47.2139792</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>TrailStopUpdate</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>TrailStopUpdate</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:17:17.8891793</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>False</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>false</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:17:17.9027306</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>true</IsGroup>
          <DisplayName>Condition group 1</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Close</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>Series1</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:22:26.1137909</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <OffsetBuilder>
                  <ConditionOffset>
                    <IsSetEnabled>false</IsSetEnabled>
                    <OffsetValue>0</OffsetValue>
                    <OffsetOperator>Add</OffsetOperator>
                    <OffsetType>Arithmetic</OffsetType>
                  </ConditionOffset>
                  <Offset>
                    <DefaultValue>0</DefaultValue>
                    <IsInt>false</IsInt>
                    <BindingValue xsi:type="xsd:string">(ATR(ATRPeriod)[0] * TrailStopMultiplier) </BindingValue>
                    <DynamicValue>
                      <Children />
                      <IsExpanded>false</IsExpanded>
                      <IsSelected>true</IsSelected>
                      <Name>ATR</Name>
                      <OffsetType>Arithmetic</OffsetType>
                      <AssignedCommand>
                        <Command>ATR</Command>
                        <Parameters>
                          <string>AssociatedIndicator</string>
                          <string>BarsAgo</string>
                          <string>OffsetBuilder</string>
                        </Parameters>
                      </AssignedCommand>
                      <AssociatedIndicator>
                        <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                        <CustomProperties>
                          <item>
                            <key>
                              <string>Period</string>
                            </key>
                            <value>
                              <anyType xsi:type="NumberBuilder">
                                <DefaultValue>0</DefaultValue>
                                <IsInt>true</IsInt>
                                <BindingValue xsi:type="xsd:string">ATRPeriod</BindingValue>
                                <DynamicValue>
                                  <Children />
                                  <IsExpanded>false</IsExpanded>
                                  <IsSelected>true</IsSelected>
                                  <Name>ATRPeriod</Name>
                                  <OffsetType>Arithmetic</OffsetType>
                                  <AssignedCommand>
                                    <Command>ATRPeriod</Command>
                                    <Parameters />
                                  </AssignedCommand>
                                  <BarsAgo>0</BarsAgo>
                                  <CurrencyType>Currency</CurrencyType>
                                  <Date>2024-09-29T11:23:34.3954502</Date>
                                  <DayOfWeek>Sunday</DayOfWeek>
                                  <EndBar>0</EndBar>
                                  <ForceSeriesIndex>false</ForceSeriesIndex>
                                  <LookBackPeriod>0</LookBackPeriod>
                                  <MarketPosition>Long</MarketPosition>
                                  <Period>0</Period>
                                  <ReturnType>Number</ReturnType>
                                  <StartBar>0</StartBar>
                                  <State>Undefined</State>
                                  <Time>0001-01-01T00:00:00</Time>
                                </DynamicValue>
                                <IsLiteral>false</IsLiteral>
                                <LiveValue xsi:type="xsd:string">ATRPeriod</LiveValue>
                              </anyType>
                            </value>
                          </item>
                        </CustomProperties>
                        <IndicatorHolder>
                          <IndicatorName>ATR</IndicatorName>
                          <Plots>
                            <Plot>
                              <IsOpacityVisible>false</IsOpacityVisible>
                              <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF008B8B&lt;/SolidColorBrush&gt;</BrushSerialize>
                              <DashStyleHelper>Solid</DashStyleHelper>
                              <Opacity>100</Opacity>
                              <Width>1</Width>
                              <AutoWidth>false</AutoWidth>
                              <Max>1.7976931348623157E+308</Max>
                              <Min>-1.7976931348623157E+308</Min>
                              <Name>ATR</Name>
                              <PlotStyle>Line</PlotStyle>
                            </Plot>
                          </Plots>
                        </IndicatorHolder>
                        <IsExplicitlyNamed>false</IsExplicitlyNamed>
                        <IsPriceTypeLocked>false</IsPriceTypeLocked>
                        <PlotOnChart>true</PlotOnChart>
                        <PriceType>Close</PriceType>
                        <SeriesType>Indicator</SeriesType>
                      </AssociatedIndicator>
                      <BarsAgo>0</BarsAgo>
                      <CurrencyType>Currency</CurrencyType>
                      <Date>2024-09-29T11:23:27.5065309</Date>
                      <DayOfWeek>Sunday</DayOfWeek>
                      <EndBar>0</EndBar>
                      <ForceSeriesIndex>false</ForceSeriesIndex>
                      <LookBackPeriod>0</LookBackPeriod>
                      <MarketPosition>Long</MarketPosition>
                      <OffsetBuilder>
                        <ConditionOffset>
                          <IsSetEnabled>false</IsSetEnabled>
                          <OffsetValue>0</OffsetValue>
                          <OffsetOperator>Multiply</OffsetOperator>
                          <OffsetType>Arithmetic</OffsetType>
                        </ConditionOffset>
                        <Offset>
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">TrailStopMultiplier</BindingValue>
                          <DynamicValue>
                            <Children />
                            <IsExpanded>false</IsExpanded>
                            <IsSelected>true</IsSelected>
                            <Name>TrailStopMultiplier</Name>
                            <OffsetType>Arithmetic</OffsetType>
                            <AssignedCommand>
                              <Command>TrailStopMultiplier</Command>
                              <Parameters />
                            </AssignedCommand>
                            <BarsAgo>0</BarsAgo>
                            <CurrencyType>Currency</CurrencyType>
                            <Date>2024-09-29T11:23:41.2745038</Date>
                            <DayOfWeek>Sunday</DayOfWeek>
                            <EndBar>0</EndBar>
                            <ForceSeriesIndex>false</ForceSeriesIndex>
                            <LookBackPeriod>0</LookBackPeriod>
                            <MarketPosition>Long</MarketPosition>
                            <Period>0</Period>
                            <ReturnType>Number</ReturnType>
                            <StartBar>0</StartBar>
                            <State>Undefined</State>
                            <Time>0001-01-01T00:00:00</Time>
                          </DynamicValue>
                          <IsLiteral>false</IsLiteral>
                          <LiveValue xsi:type="xsd:string">TrailStopMultiplier</LiveValue>
                        </Offset>
                      </OffsetBuilder>
                      <Period>0</Period>
                      <ReturnType>Series</ReturnType>
                      <StartBar>0</StartBar>
                      <State>Undefined</State>
                      <Time>0001-01-01T00:00:00</Time>
                    </DynamicValue>
                    <IsLiteral>false</IsLiteral>
                    <LiveValue xsi:type="xsd:string">(ATR(Close, Convert.ToInt32(ATRPeriod))[0] * TrailStopMultiplier) </LiveValue>
                  </Offset>
                </OffsetBuilder>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Less</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>StoreTrailStop</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>StoreTrailStop</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:22:26.1406118</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>(Default input[0] + ((ATR(Close, Convert.ToInt32(ATRPeriod))[0] * TrailStopMultiplier) ))  &lt; StoreTrailStop</DisplayName>
        </WizardConditionGroup>
      </Conditions>
      <SetName>Set 7</SetName>
      <SetNumber>7</SetNumber>
    </ConditionalAction>
    <ConditionalAction>
      <Actions>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set TrailStopUpdate</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:55:57.4271709</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T11:55:57.4271709</VariableDateTime>
            <VariableBool>true</VariableBool>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>bool</UserVariableType>
          <VariableName>TrailStopUpdate</VariableName>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Exit short position by a limit order</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <FromEntrySignal>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>ShortEntry1</StringValue>
                </NinjaScriptString>
              </Strings>
            </FromEntrySignal>
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LimitPrice>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">StoreProfit1</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>StoreProfit1</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>StoreProfit1</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T12:10:14.194136</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">StoreProfit1</LiveValue>
            </LimitPrice>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">QTY1</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>QTY1</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>QTY1</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T12:10:19.9495209</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">QTY1</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>ShortProfit1</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <Tag>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set Exit short position by a limit order</StringValue>
                </NinjaScriptString>
              </Strings>
            </Tag>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T12:09:53.7712622</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>ExitLimit</ActionType>
          <Command>
            <Command>ExitShortLimit</Command>
            <Parameters>
              <string>quantity</string>
              <string>limitPrice</string>
              <string>signalName</string>
              <string>fromEntrySignal</string>
            </Parameters>
          </Command>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Exit short position by a limit order</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <FromEntrySignal>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>ShortEntry2</StringValue>
                </NinjaScriptString>
              </Strings>
            </FromEntrySignal>
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LimitPrice>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">StoreProfit2</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>StoreProfit2</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>StoreProfit2</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T12:10:50.7028417</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">StoreProfit2</LiveValue>
            </LimitPrice>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">QTY2</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>QTY2</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>QTY2</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T12:10:55.6986651</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">QTY2</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>ShortProfit2</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <Tag>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set Exit short position by a limit order</StringValue>
                </NinjaScriptString>
              </Strings>
            </Tag>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T12:10:38.8011913</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>ExitLimit</ActionType>
          <Command>
            <Command>ExitShortLimit</Command>
            <Parameters>
              <string>quantity</string>
              <string>limitPrice</string>
              <string>signalName</string>
              <string>fromEntrySignal</string>
            </Parameters>
          </Command>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Exit short position by a stop order</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <FromEntrySignal>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>ShortEntry1</StringValue>
                </NinjaScriptString>
              </Strings>
            </FromEntrySignal>
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">QTY1</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>QTY1</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>QTY1</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T12:11:23.2655973</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">QTY1</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>ShortStop1</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <StopPrice>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">StoreTrailStop</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>StoreTrailStop</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>StoreTrailStop</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T12:15:15.3915168</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">StoreTrailStop</LiveValue>
            </StopPrice>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T12:15:13.2507679</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>ExitStop</ActionType>
          <Command>
            <Command>ExitShortStopMarket</Command>
            <Parameters>
              <string>quantity</string>
              <string>stopPrice</string>
              <string>signalName</string>
              <string>fromEntrySignal</string>
            </Parameters>
          </Command>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Exit short position by a stop order</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <FromEntrySignal>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>ShortEntry2</StringValue>
                </NinjaScriptString>
              </Strings>
            </FromEntrySignal>
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">QTY2</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>QTY2</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>QTY2</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T12:11:57.0013734</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">QTY2</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SignalName>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>ShortStop2</StringValue>
                </NinjaScriptString>
              </Strings>
            </SignalName>
            <SoundLocation />
            <StopPrice>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">StoreTrailStop</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>StoreTrailStop</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>StoreTrailStop</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T12:15:22.9414548</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">StoreTrailStop</LiveValue>
            </StopPrice>
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T12:15:20.9130944</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>ExitStop</ActionType>
          <Command>
            <Command>ExitShortStopMarket</Command>
            <Parameters>
              <string>quantity</string>
              <string>stopPrice</string>
              <string>signalName</string>
              <string>fromEntrySignal</string>
            </Parameters>
          </Command>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Print</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <MessageValue>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set 8: </StringValue>
                </NinjaScriptString>
                <NinjaScriptString>
                  <Action>
                    <Children />
                    <IsExpanded>false</IsExpanded>
                    <IsSelected>true</IsSelected>
                    <Name>Time series</Name>
                    <OffsetType>Arithmetic</OffsetType>
                    <AssignedCommand>
                      <Command>Times[{0}][{1}].TimeOfDay</Command>
                      <Parameters>
                        <string>Series1</string>
                        <string>BarsAgo</string>
                      </Parameters>
                    </AssignedCommand>
                    <BarsAgo>0</BarsAgo>
                    <CurrencyType>Currency</CurrencyType>
                    <Date>2024-09-29T12:16:36.5951082</Date>
                    <DayOfWeek>Sunday</DayOfWeek>
                    <EndBar>0</EndBar>
                    <ForceSeriesIndex>true</ForceSeriesIndex>
                    <LookBackPeriod>0</LookBackPeriod>
                    <MarketPosition>Long</MarketPosition>
                    <Period>0</Period>
                    <ReturnType>Time</ReturnType>
                    <StartBar>0</StartBar>
                    <State>Undefined</State>
                    <Time>0001-01-01T00:00:00</Time>
                  </Action>
                  <Index>1</Index>
                  <StringValue>Times[0][0].TimeOfDay</StringValue>
                </NinjaScriptString>
              </Strings>
            </MessageValue>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T12:18:40.2999586</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T12:18:40.2999586</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>Misc</ActionType>
          <Command>
            <Command>Print</Command>
            <Parameters>
              <string>MessageValue</string>
            </Parameters>
          </Command>
        </WizardAction>
      </Actions>
      <ActiveAction>
        <Children />
        <IsExpanded>false</IsExpanded>
        <IsSelected>true</IsSelected>
        <Name>Print</Name>
        <OffsetType>Arithmetic</OffsetType>
        <ActionProperties>
          <DashStyle>Solid</DashStyle>
          <DivideTimePrice>false</DivideTimePrice>
          <Id />
          <File />
          <IsAutoScale>false</IsAutoScale>
          <IsSimulatedStop>false</IsSimulatedStop>
          <IsStop>false</IsStop>
          <LogLevel>Information</LogLevel>
          <MessageValue>
            <SeparatorCharacter> </SeparatorCharacter>
            <Strings>
              <NinjaScriptString>
                <Index>0</Index>
                <StringValue>Set 8: </StringValue>
              </NinjaScriptString>
              <NinjaScriptString>
                <Action>
                  <Children />
                  <IsExpanded>false</IsExpanded>
                  <IsSelected>true</IsSelected>
                  <Name>Time series</Name>
                  <OffsetType>Arithmetic</OffsetType>
                  <AssignedCommand>
                    <Command>Times[{0}][{1}].TimeOfDay</Command>
                    <Parameters>
                      <string>Series1</string>
                      <string>BarsAgo</string>
                    </Parameters>
                  </AssignedCommand>
                  <BarsAgo>0</BarsAgo>
                  <CurrencyType>Currency</CurrencyType>
                  <Date>2024-09-29T12:16:36.5951082</Date>
                  <DayOfWeek>Sunday</DayOfWeek>
                  <EndBar>0</EndBar>
                  <ForceSeriesIndex>true</ForceSeriesIndex>
                  <LookBackPeriod>0</LookBackPeriod>
                  <MarketPosition>Long</MarketPosition>
                  <Period>0</Period>
                  <ReturnType>Time</ReturnType>
                  <StartBar>0</StartBar>
                  <State>Undefined</State>
                  <Time>0001-01-01T00:00:00</Time>
                </Action>
                <Index>1</Index>
                <StringValue>Times[0][0].TimeOfDay</StringValue>
              </NinjaScriptString>
            </Strings>
          </MessageValue>
          <Mode>Currency</Mode>
          <OffsetType>Currency</OffsetType>
          <Priority>Medium</Priority>
          <Quantity>
            <DefaultValue>0</DefaultValue>
            <IsInt>true</IsInt>
            <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
            <DynamicValue>
              <Children />
              <IsExpanded>false</IsExpanded>
              <IsSelected>false</IsSelected>
              <Name>Default order quantity</Name>
              <OffsetType>Arithmetic</OffsetType>
              <AssignedCommand>
                <Command>DefaultQuantity</Command>
                <Parameters />
              </AssignedCommand>
              <BarsAgo>0</BarsAgo>
              <CurrencyType>Currency</CurrencyType>
              <Date>2024-09-29T12:18:40.2999586</Date>
              <DayOfWeek>Sunday</DayOfWeek>
              <EndBar>0</EndBar>
              <ForceSeriesIndex>false</ForceSeriesIndex>
              <LookBackPeriod>0</LookBackPeriod>
              <MarketPosition>Long</MarketPosition>
              <Period>0</Period>
              <ReturnType>Number</ReturnType>
              <StartBar>0</StartBar>
              <State>Undefined</State>
              <Time>0001-01-01T00:00:00</Time>
            </DynamicValue>
            <IsLiteral>false</IsLiteral>
            <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
          </Quantity>
          <ServiceName />
          <ScreenshotPath />
          <SoundLocation />
          <TextPosition>BottomLeft</TextPosition>
          <VariableDateTime>2024-09-29T12:18:40.2999586</VariableDateTime>
          <VariableBool>false</VariableBool>
        </ActionProperties>
        <ActionType>Misc</ActionType>
        <Command>
          <Command>Print</Command>
          <Parameters>
            <string>MessageValue</string>
          </Parameters>
        </Command>
      </ActiveAction>
      <AnyOrAll>All</AnyOrAll>
      <Conditions>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Current market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.MarketPosition</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:17:12.5688017</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>MarketPosition.{0}</Command>
                  <Parameters>
                    <string>MarketPosition</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:17:12.5976349</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Short</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>Position.MarketPosition = MarketPosition.Short</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>InitialEntry</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>InitialEntry</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:41:37.8072032</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>False</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>false</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:41:37.8331983</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>InitialEntry = false</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>TriggerTrailStop</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>TriggerTrailStop</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:50:41.0421594</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>True</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>true</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:50:41.0711653</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>TriggerTrailStop = true</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>IsFirstTickOfBar</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>IsFirstTickOfBar</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:51:40.9522529</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>True</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>true</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:51:40.9757658</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>TrailStopUpdate</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>TrailStopUpdate</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:51:55.8887945</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>False</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>false</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:51:55.902795</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>true</IsGroup>
          <DisplayName>Condition group 1</DisplayName>
        </WizardConditionGroup>
      </Conditions>
      <SetName>Set 8</SetName>
      <SetNumber>8</SetNumber>
    </ConditionalAction>
    <ConditionalAction>
      <Actions>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set InitialEntry</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:54:08.7610246</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T10:54:08.7610246</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>bool</UserVariableType>
          <VariableName>InitialEntry</VariableName>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set TriggerTrailStop</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:58:14.0307941</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T11:58:14.0307941</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>bool</UserVariableType>
          <VariableName>TriggerTrailStop</VariableName>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set TrailStopUpdate</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T11:58:25.5717792</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-09-29T11:58:25.5717792</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>bool</UserVariableType>
          <VariableName>TrailStopUpdate</VariableName>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Print</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <MessageValue>
              <SeparatorCharacter> </SeparatorCharacter>
              <Strings>
                <NinjaScriptString>
                  <Index>0</Index>
                  <StringValue>Set 9: </StringValue>
                </NinjaScriptString>
                <NinjaScriptString>
                  <Action>
                    <Children />
                    <IsExpanded>false</IsExpanded>
                    <IsSelected>true</IsSelected>
                    <Name>Time series</Name>
                    <OffsetType>Arithmetic</OffsetType>
                    <AssignedCommand>
                      <Command>Times[{0}][{1}].TimeOfDay</Command>
                      <Parameters>
                        <string>Series1</string>
                        <string>BarsAgo</string>
                      </Parameters>
                    </AssignedCommand>
                    <BarsAgo>0</BarsAgo>
                    <CurrencyType>Currency</CurrencyType>
                    <Date>2024-09-29T12:16:36.5951082</Date>
                    <DayOfWeek>Sunday</DayOfWeek>
                    <EndBar>0</EndBar>
                    <ForceSeriesIndex>true</ForceSeriesIndex>
                    <LookBackPeriod>0</LookBackPeriod>
                    <MarketPosition>Long</MarketPosition>
                    <Period>0</Period>
                    <ReturnType>Time</ReturnType>
                    <StartBar>0</StartBar>
                    <State>Undefined</State>
                    <Time>0001-01-01T00:00:00</Time>
                  </Action>
                  <Index>1</Index>
                  <StringValue>Times[0][0].TimeOfDay</StringValue>
                </NinjaScriptString>
              </Strings>
            </MessageValue>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-10-23T09:48:28.9821096</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2024-10-23T09:48:28.9821096</VariableDateTime>
            <VariableBool>false</VariableBool>
          </ActionProperties>
          <ActionType>Misc</ActionType>
          <Command>
            <Command>Print</Command>
            <Parameters>
              <string>MessageValue</string>
            </Parameters>
          </Command>
        </WizardAction>
      </Actions>
      <ActiveAction>
        <Children />
        <IsExpanded>false</IsExpanded>
        <IsSelected>true</IsSelected>
        <Name>Print</Name>
        <OffsetType>Arithmetic</OffsetType>
        <ActionProperties>
          <DashStyle>Solid</DashStyle>
          <DivideTimePrice>false</DivideTimePrice>
          <Id />
          <File />
          <IsAutoScale>false</IsAutoScale>
          <IsSimulatedStop>false</IsSimulatedStop>
          <IsStop>false</IsStop>
          <LogLevel>Information</LogLevel>
          <MessageValue>
            <SeparatorCharacter> </SeparatorCharacter>
            <Strings>
              <NinjaScriptString>
                <Index>0</Index>
                <StringValue>Set 9: </StringValue>
              </NinjaScriptString>
              <NinjaScriptString>
                <Action>
                  <Children />
                  <IsExpanded>false</IsExpanded>
                  <IsSelected>true</IsSelected>
                  <Name>Time series</Name>
                  <OffsetType>Arithmetic</OffsetType>
                  <AssignedCommand>
                    <Command>Times[{0}][{1}].TimeOfDay</Command>
                    <Parameters>
                      <string>Series1</string>
                      <string>BarsAgo</string>
                    </Parameters>
                  </AssignedCommand>
                  <BarsAgo>0</BarsAgo>
                  <CurrencyType>Currency</CurrencyType>
                  <Date>2024-09-29T12:16:36.5951082</Date>
                  <DayOfWeek>Sunday</DayOfWeek>
                  <EndBar>0</EndBar>
                  <ForceSeriesIndex>true</ForceSeriesIndex>
                  <LookBackPeriod>0</LookBackPeriod>
                  <MarketPosition>Long</MarketPosition>
                  <Period>0</Period>
                  <ReturnType>Time</ReturnType>
                  <StartBar>0</StartBar>
                  <State>Undefined</State>
                  <Time>0001-01-01T00:00:00</Time>
                </Action>
                <Index>1</Index>
                <StringValue>Times[0][0].TimeOfDay</StringValue>
              </NinjaScriptString>
            </Strings>
          </MessageValue>
          <Mode>Currency</Mode>
          <OffsetType>Currency</OffsetType>
          <Priority>Medium</Priority>
          <Quantity>
            <DefaultValue>0</DefaultValue>
            <IsInt>true</IsInt>
            <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
            <DynamicValue>
              <Children />
              <IsExpanded>false</IsExpanded>
              <IsSelected>false</IsSelected>
              <Name>Default order quantity</Name>
              <OffsetType>Arithmetic</OffsetType>
              <AssignedCommand>
                <Command>DefaultQuantity</Command>
                <Parameters />
              </AssignedCommand>
              <BarsAgo>0</BarsAgo>
              <CurrencyType>Currency</CurrencyType>
              <Date>2024-10-23T09:48:28.9821096</Date>
              <DayOfWeek>Sunday</DayOfWeek>
              <EndBar>0</EndBar>
              <ForceSeriesIndex>false</ForceSeriesIndex>
              <LookBackPeriod>0</LookBackPeriod>
              <MarketPosition>Long</MarketPosition>
              <Period>0</Period>
              <ReturnType>Number</ReturnType>
              <StartBar>0</StartBar>
              <State>Undefined</State>
              <Time>0001-01-01T00:00:00</Time>
            </DynamicValue>
            <IsLiteral>false</IsLiteral>
            <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
          </Quantity>
          <ServiceName />
          <ScreenshotPath />
          <SoundLocation />
          <TextPosition>BottomLeft</TextPosition>
          <VariableDateTime>2024-10-23T09:48:28.9821096</VariableDateTime>
          <VariableBool>false</VariableBool>
        </ActionProperties>
        <ActionType>Misc</ActionType>
        <Command>
          <Command>Print</Command>
          <Parameters>
            <string>MessageValue</string>
          </Parameters>
        </Command>
      </ActiveAction>
      <AnyOrAll>All</AnyOrAll>
      <Conditions>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Current market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>Position.MarketPosition</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:53:49.6043827</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Market position</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>MarketPosition.{0}</Command>
                  <Parameters>
                    <string>MarketPosition</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-09-29T10:53:49.6286289</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Flat</MarketPosition>
                <Period>0</Period>
                <ReturnType>MarketData</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>Position.MarketPosition = MarketPosition.Flat</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>InitialEntry</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>InitialEntry</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-10-23T09:46:34.5383063</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>True</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>true</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-10-23T09:46:34.564312</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>TriggerTrailStop</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>TriggerTrailStop</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-10-23T09:46:54.2556859</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>True</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>true</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-10-23T09:46:54.2821875</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>true</IsGroup>
          <DisplayName>Condition group 1</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>IsFirstTickOfBar</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>IsFirstTickOfBar</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-10-23T09:47:17.0029411</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>True</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>true</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2024-10-23T09:47:17.0380668</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Bool</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>IsFirstTickOfBar = true</DisplayName>
        </WizardConditionGroup>
      </Conditions>
      <SetName>Set 9</SetName>
      <SetNumber>9</SetNumber>
    </ConditionalAction>
    <ConditionalAction>
      <Actions>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set OrbHigh</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-05-17T23:34:30.4278308</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2025-05-17T23:34:30.4278308</VariableDateTime>
            <VariableBool>false</VariableBool>
            <VariableDouble>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">(ORB_TradeSaber(true, 0.2, DateTime.Parse("8:30 AM"), DateTime.Parse("8:45 AM"), @"Local", @"Recommended that PC and 
NinjaTrader clocks match", true, @"AboveUpper", @"BelowLower", Brushes.SteelBlue, Brushes.DarkOrange, false, @"AboveLower", @"BelowUpper", Brushes.LightGreen, Brushes.Pink, false, @"AboveMidline", @"BelowMidline", Brushes.Aqua, Brushes.Fuchsia, false, @"AboveUpperQuarter", @"BelowUpperQuarter", Brushes.Aqua, Brushes.Crimson, false, @"AboveLowerQuarter", @"BelowLowerQuarter", Brushes.Lime, Brushes.OrangeRed, false, @"AboveTarget1Upper", @"BelowTarget1Upper", Brushes.DodgerBlue, Brushes.Tomato, false, @"BelowTarget1Lower", @"AboveTarget1Lower", Brushes.Tomato, Brushes.DodgerBlue, false, @"AboveTarget2Upper", @"BelowTarget2Upper", Brushes.DeepSkyBlue, Brushes.DarkRed, false, @"AboveTarget2Lower", @"BelowTarget2Lower", Brushes.DeepSkyBlue, Brushes.DarkRed, false, false, @"SessionStartUp", @"SessionStartDown", Brushes.LightGreen, Brushes.Pink, false, 0.5, false, 0.25, false, 1, false, 2, false, @"TradeSaber - Built With Grok", @"Version 1.3 // April 2025", @"https://tradesaber.com/predator-guide/", @"https://Discord.gg/2YU9GDme8j", @"https://youtu.be/jUYT-Erzc_8").ORHigh[0] + (TickOffsetOrbHigh * TickSize)) </BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>ORB_TradeSaber</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>ORB_TradeSaber</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>HighlightRange</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>HighlightOpacity</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">0.2</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">0.2</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>StartTime</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:dateTime">2000-01-01T08:30:00</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>EndTime</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:dateTime">2000-01-01T08:45:00</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>TimeZoneSelection</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>Local</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>TZNote</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>Recommended that PC and 
NinjaTrader clocks match</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowOuterArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveUpperTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveUpper</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowLowerTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowLower</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveUpperColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>70</R>
                            <G>130</G>
                            <B>180</B>
                            <ScA>1</ScA>
                            <ScR>0.06124606</ScR>
                            <ScG>0.223227978</ScG>
                            <ScB>0.456411034</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowLowerColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>255</R>
                            <G>140</G>
                            <B>0</B>
                            <ScA>1</ScA>
                            <ScR>1</ScR>
                            <ScG>0.2622507</ScG>
                            <ScB>0</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowInnerArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveLowerTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveLower</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowUpperTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowUpper</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveLowerColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>144</R>
                            <G>238</G>
                            <B>144</B>
                            <ScA>1</ScA>
                            <ScR>0.2788943</ScR>
                            <ScG>0.8549926</ScG>
                            <ScB>0.2788943</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowUpperColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>255</R>
                            <G>192</G>
                            <B>203</B>
                            <ScA>1</ScA>
                            <ScR>1</ScR>
                            <ScG>0.527115166</ScG>
                            <ScB>0.5972018</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowMidlineArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveMidlineTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveMidline</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowMidlineTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowMidline</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveMidlineColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>0</R>
                            <G>255</G>
                            <B>255</B>
                            <ScA>1</ScA>
                            <ScR>0</ScR>
                            <ScG>1</ScG>
                            <ScB>1</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowMidlineColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>255</R>
                            <G>0</G>
                            <B>255</B>
                            <ScA>1</ScA>
                            <ScR>1</ScR>
                            <ScG>0</ScG>
                            <ScB>1</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowUpperQuarterArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveUpperQuarterTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveUpperQuarter</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowUpperQuarterTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowUpperQuarter</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveUpperQuarterColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>0</R>
                            <G>255</G>
                            <B>255</B>
                            <ScA>1</ScA>
                            <ScR>0</ScR>
                            <ScG>1</ScG>
                            <ScB>1</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowUpperQuarterColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>220</R>
                            <G>20</G>
                            <B>60</B>
                            <ScA>1</ScA>
                            <ScR>0.715693533</ScR>
                            <ScG>0.00699541066</ScG>
                            <ScB>0.045186203</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowLowerQuarterArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveLowerQuarterTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveLowerQuarter</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowLowerQuarterTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowLowerQuarter</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveLowerQuarterColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>0</R>
                            <G>255</G>
                            <B>0</B>
                            <ScA>1</ScA>
                            <ScR>0</ScR>
                            <ScG>1</ScG>
                            <ScB>0</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowLowerQuarterColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>255</R>
                            <G>69</G>
                            <B>0</B>
                            <ScA>1</ScA>
                            <ScR>1</ScR>
                            <ScG>0.0595112443</ScG>
                            <ScB>0</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowTarget1UpperArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveTarget1UpperTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveTarget1Upper</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowTarget1UpperTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowTarget1Upper</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveTarget1UpperColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>30</R>
                            <G>144</G>
                            <B>255</B>
                            <ScA>1</ScA>
                            <ScR>0.0129830325</ScR>
                            <ScG>0.2788943</ScG>
                            <ScB>1</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowTarget1UpperColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>255</R>
                            <G>99</G>
                            <B>71</B>
                            <ScA>1</ScA>
                            <ScR>1</ScR>
                            <ScG>0.124771819</ScG>
                            <ScB>0.06301002</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowTarget1LowerArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowTarget1LowerTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowTarget1Lower</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveTarget1LowerTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveTarget1Lower</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowTarget1LowerColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>255</R>
                            <G>99</G>
                            <B>71</B>
                            <ScA>1</ScA>
                            <ScR>1</ScR>
                            <ScG>0.124771819</ScG>
                            <ScB>0.06301002</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveTarget1LowerColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>30</R>
                            <G>144</G>
                            <B>255</B>
                            <ScA>1</ScA>
                            <ScR>0.0129830325</ScR>
                            <ScG>0.2788943</ScG>
                            <ScB>1</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowTarget2UpperArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveTarget2UpperTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveTarget2Upper</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowTarget2UpperTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowTarget2Upper</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveTarget2UpperColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>0</R>
                            <G>191</G>
                            <B>255</B>
                            <ScA>1</ScA>
                            <ScR>0</ScR>
                            <ScG>0.5209956</ScG>
                            <ScB>1</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowTarget2UpperColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>139</R>
                            <G>0</G>
                            <B>0</B>
                            <ScA>1</ScA>
                            <ScR>0.258182883</ScR>
                            <ScG>0</ScG>
                            <ScB>0</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowTarget2LowerArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveTarget2LowerTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveTarget2Lower</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowTarget2LowerTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowTarget2Lower</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveTarget2LowerColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>0</R>
                            <G>191</G>
                            <B>255</B>
                            <ScA>1</ScA>
                            <ScR>0</ScR>
                            <ScG>0.5209956</ScG>
                            <ScB>1</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowTarget2LowerColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>139</R>
                            <G>0</G>
                            <B>0</B>
                            <ScA>1</ScA>
                            <ScR>0.258182883</ScR>
                            <ScG>0</ScG>
                            <ScB>0</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowDayStartArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowRangeStartArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SessionStartUpTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>SessionStartUp</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SessionStartDownTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>SessionStartDown</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SessionStartUpColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>144</R>
                            <G>238</G>
                            <B>144</B>
                            <ScA>1</ScA>
                            <ScR>0.2788943</ScR>
                            <ScG>0.8549926</ScG>
                            <ScB>0.2788943</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SessionStartDownColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>255</R>
                            <G>192</G>
                            <B>203</B>
                            <ScA>1</ScA>
                            <ScR>1</ScR>
                            <ScG>0.527115166</ScG>
                            <ScB>0.5972018</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowMid</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>MiddleRangeMultiplier</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">0.5</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">0.5</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowQuarters</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>QuarterRangeMultiplier</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">0.25</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">0.25</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowTarget1</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Target1Multiplier</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">1</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">1</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowTarget2</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Target2Multiplier</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">2</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">2</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowSocials</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Author</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>TradeSaber - Built With Grok</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Version</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>Version 1.3 // April 2025</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>TradeSaber</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>https://tradesaber.com/predator-guide/</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Discord</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>https://Discord.gg/2YU9GDme8j</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>YouTube</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>https://youtu.be/jUYT-Erzc_8</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>ORB_TradeSaber</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF4682B4&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORHigh</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFF8C00&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORLow</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#00FFFFFF&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>Signal</Name>
                        <PlotStyle>Dot</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF808080&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORMiddle</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFADD8E6&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORTarget1</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF00FFFF&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORTarget2</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFFA07A&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORTargetMinus1</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFA8072&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORTargetMinus2</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF696969&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORLowerTargetQuarter</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF696969&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORUpperTargetQuarter</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>true</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                  <SelectedPlot>ORHigh</SelectedPlot>
                </AssociatedIndicator>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-05-17T23:32:28.3749667</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <OffsetBuilder>
                  <ConditionOffset>
                    <IsSetEnabled>false</IsSetEnabled>
                    <OffsetValue>0</OffsetValue>
                    <OffsetOperator>Add</OffsetOperator>
                    <OffsetType>Ticks</OffsetType>
                  </ConditionOffset>
                  <Offset>
                    <DefaultValue>0</DefaultValue>
                    <IsInt>false</IsInt>
                    <BindingValue xsi:type="xsd:string">TickOffsetOrbHigh</BindingValue>
                    <DynamicValue>
                      <Children />
                      <IsExpanded>false</IsExpanded>
                      <IsSelected>true</IsSelected>
                      <Name>TickOffsetOrbHigh</Name>
                      <OffsetType>Arithmetic</OffsetType>
                      <AssignedCommand>
                        <Command>TickOffsetOrbHigh</Command>
                        <Parameters />
                      </AssignedCommand>
                      <BarsAgo>0</BarsAgo>
                      <CurrencyType>Currency</CurrencyType>
                      <Date>2025-05-17T23:33:01.9457912</Date>
                      <DayOfWeek>Sunday</DayOfWeek>
                      <EndBar>0</EndBar>
                      <ForceSeriesIndex>false</ForceSeriesIndex>
                      <LookBackPeriod>0</LookBackPeriod>
                      <MarketPosition>Long</MarketPosition>
                      <Period>0</Period>
                      <ReturnType>Number</ReturnType>
                      <StartBar>0</StartBar>
                      <State>Undefined</State>
                      <Time>0001-01-01T00:00:00</Time>
                    </DynamicValue>
                    <IsLiteral>false</IsLiteral>
                    <LiveValue xsi:type="xsd:string">TickOffsetOrbHigh</LiveValue>
                  </Offset>
                </OffsetBuilder>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">(ORB_TradeSaber(Close, true, 0.2, DateTime.Parse("8:30 AM"), DateTime.Parse("8:45 AM"), @"Local", @"Recommended that PC and 
NinjaTrader clocks match", true, @"AboveUpper", @"BelowLower", Brushes.SteelBlue, Brushes.DarkOrange, false, @"AboveLower", @"BelowUpper", Brushes.LightGreen, Brushes.Pink, false, @"AboveMidline", @"BelowMidline", Brushes.Aqua, Brushes.Fuchsia, false, @"AboveUpperQuarter", @"BelowUpperQuarter", Brushes.Aqua, Brushes.Crimson, false, @"AboveLowerQuarter", @"BelowLowerQuarter", Brushes.Lime, Brushes.OrangeRed, false, @"AboveTarget1Upper", @"BelowTarget1Upper", Brushes.DodgerBlue, Brushes.Tomato, false, @"BelowTarget1Lower", @"AboveTarget1Lower", Brushes.Tomato, Brushes.DodgerBlue, false, @"AboveTarget2Upper", @"BelowTarget2Upper", Brushes.DeepSkyBlue, Brushes.DarkRed, false, @"AboveTarget2Lower", @"BelowTarget2Lower", Brushes.DeepSkyBlue, Brushes.DarkRed, false, false, @"SessionStartUp", @"SessionStartDown", Brushes.LightGreen, Brushes.Pink, false, 0.5, false, 0.25, false, 1, false, 2, false, @"TradeSaber - Built With Grok", @"Version 1.3 // April 2025", @"https://tradesaber.com/predator-guide/", @"https://Discord.gg/2YU9GDme8j", @"https://youtu.be/jUYT-Erzc_8").ORHigh[0] + (TickOffsetOrbHigh * TickSize)) </LiveValue>
            </VariableDouble>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>double</UserVariableType>
          <VariableName>OrbHigh</VariableName>
        </WizardAction>
        <WizardAction>
          <Children />
          <IsExpanded>false</IsExpanded>
          <IsSelected>true</IsSelected>
          <Name>Set OrbLow</Name>
          <OffsetType>Arithmetic</OffsetType>
          <ActionProperties>
            <DashStyle>Solid</DashStyle>
            <DivideTimePrice>false</DivideTimePrice>
            <Id />
            <File />
            <IsAutoScale>false</IsAutoScale>
            <IsSimulatedStop>false</IsSimulatedStop>
            <IsStop>false</IsStop>
            <LogLevel>Information</LogLevel>
            <Mode>Currency</Mode>
            <OffsetType>Currency</OffsetType>
            <Priority>Medium</Priority>
            <Quantity>
              <DefaultValue>0</DefaultValue>
              <IsInt>true</IsInt>
              <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>false</IsSelected>
                <Name>Default order quantity</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>DefaultQuantity</Command>
                  <Parameters />
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-05-17T23:33:26.9421893</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
            </Quantity>
            <ServiceName />
            <ScreenshotPath />
            <SoundLocation />
            <TextPosition>BottomLeft</TextPosition>
            <VariableDateTime>2025-05-17T23:33:26.9421893</VariableDateTime>
            <VariableBool>false</VariableBool>
            <VariableDouble>
              <DefaultValue>0</DefaultValue>
              <IsInt>false</IsInt>
              <BindingValue xsi:type="xsd:string">(ORB_TradeSaber(true, 0.2, DateTime.Parse("8:30 AM"), DateTime.Parse("8:45 AM"), @"Local", @"Recommended that PC and 
NinjaTrader clocks match", true, @"AboveUpper", @"BelowLower", Brushes.SteelBlue, Brushes.DarkOrange, false, @"AboveLower", @"BelowUpper", Brushes.LightGreen, Brushes.Pink, false, @"AboveMidline", @"BelowMidline", Brushes.Aqua, Brushes.Fuchsia, false, @"AboveUpperQuarter", @"BelowUpperQuarter", Brushes.Aqua, Brushes.Crimson, false, @"AboveLowerQuarter", @"BelowLowerQuarter", Brushes.Lime, Brushes.OrangeRed, false, @"AboveTarget1Upper", @"BelowTarget1Upper", Brushes.DodgerBlue, Brushes.Tomato, false, @"BelowTarget1Lower", @"AboveTarget1Lower", Brushes.Tomato, Brushes.DodgerBlue, false, @"AboveTarget2Upper", @"BelowTarget2Upper", Brushes.DeepSkyBlue, Brushes.DarkRed, false, @"AboveTarget2Lower", @"BelowTarget2Lower", Brushes.DeepSkyBlue, Brushes.DarkRed, false, false, @"SessionStartUp", @"SessionStartDown", Brushes.LightGreen, Brushes.Pink, false, 0.5, false, 0.25, false, 1, false, 2, false, @"TradeSaber - Built With Grok", @"Version 1.3 // April 2025", @"https://tradesaber.com/predator-guide/", @"https://Discord.gg/2YU9GDme8j", @"https://youtu.be/jUYT-Erzc_8").ORLow[0] + (TicjOffsetOrbLow * TickSize)) </BindingValue>
              <DynamicValue>
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>ORB_TradeSaber</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>ORB_TradeSaber</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>HighlightRange</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>HighlightOpacity</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">0.2</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">0.2</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>StartTime</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:dateTime">2000-01-01T08:30:00</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>EndTime</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:dateTime">2000-01-01T08:45:00</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>TimeZoneSelection</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>Local</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>TZNote</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>Recommended that PC and 
NinjaTrader clocks match</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowOuterArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveUpperTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveUpper</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowLowerTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowLower</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveUpperColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>70</R>
                            <G>130</G>
                            <B>180</B>
                            <ScA>1</ScA>
                            <ScR>0.06124606</ScR>
                            <ScG>0.223227978</ScG>
                            <ScB>0.456411034</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowLowerColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>255</R>
                            <G>140</G>
                            <B>0</B>
                            <ScA>1</ScA>
                            <ScR>1</ScR>
                            <ScG>0.2622507</ScG>
                            <ScB>0</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowInnerArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveLowerTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveLower</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowUpperTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowUpper</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveLowerColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>144</R>
                            <G>238</G>
                            <B>144</B>
                            <ScA>1</ScA>
                            <ScR>0.2788943</ScR>
                            <ScG>0.8549926</ScG>
                            <ScB>0.2788943</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowUpperColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>255</R>
                            <G>192</G>
                            <B>203</B>
                            <ScA>1</ScA>
                            <ScR>1</ScR>
                            <ScG>0.527115166</ScG>
                            <ScB>0.5972018</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowMidlineArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveMidlineTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveMidline</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowMidlineTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowMidline</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveMidlineColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>0</R>
                            <G>255</G>
                            <B>255</B>
                            <ScA>1</ScA>
                            <ScR>0</ScR>
                            <ScG>1</ScG>
                            <ScB>1</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowMidlineColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>255</R>
                            <G>0</G>
                            <B>255</B>
                            <ScA>1</ScA>
                            <ScR>1</ScR>
                            <ScG>0</ScG>
                            <ScB>1</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowUpperQuarterArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveUpperQuarterTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveUpperQuarter</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowUpperQuarterTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowUpperQuarter</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveUpperQuarterColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>0</R>
                            <G>255</G>
                            <B>255</B>
                            <ScA>1</ScA>
                            <ScR>0</ScR>
                            <ScG>1</ScG>
                            <ScB>1</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowUpperQuarterColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>220</R>
                            <G>20</G>
                            <B>60</B>
                            <ScA>1</ScA>
                            <ScR>0.715693533</ScR>
                            <ScG>0.00699541066</ScG>
                            <ScB>0.045186203</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowLowerQuarterArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveLowerQuarterTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveLowerQuarter</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowLowerQuarterTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowLowerQuarter</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveLowerQuarterColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>0</R>
                            <G>255</G>
                            <B>0</B>
                            <ScA>1</ScA>
                            <ScR>0</ScR>
                            <ScG>1</ScG>
                            <ScB>0</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowLowerQuarterColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>255</R>
                            <G>69</G>
                            <B>0</B>
                            <ScA>1</ScA>
                            <ScR>1</ScR>
                            <ScG>0.0595112443</ScG>
                            <ScB>0</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowTarget1UpperArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveTarget1UpperTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveTarget1Upper</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowTarget1UpperTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowTarget1Upper</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveTarget1UpperColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>30</R>
                            <G>144</G>
                            <B>255</B>
                            <ScA>1</ScA>
                            <ScR>0.0129830325</ScR>
                            <ScG>0.2788943</ScG>
                            <ScB>1</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowTarget1UpperColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>255</R>
                            <G>99</G>
                            <B>71</B>
                            <ScA>1</ScA>
                            <ScR>1</ScR>
                            <ScG>0.124771819</ScG>
                            <ScB>0.06301002</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowTarget1LowerArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowTarget1LowerTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowTarget1Lower</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveTarget1LowerTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveTarget1Lower</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowTarget1LowerColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>255</R>
                            <G>99</G>
                            <B>71</B>
                            <ScA>1</ScA>
                            <ScR>1</ScR>
                            <ScG>0.124771819</ScG>
                            <ScB>0.06301002</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveTarget1LowerColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>30</R>
                            <G>144</G>
                            <B>255</B>
                            <ScA>1</ScA>
                            <ScR>0.0129830325</ScR>
                            <ScG>0.2788943</ScG>
                            <ScB>1</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowTarget2UpperArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveTarget2UpperTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveTarget2Upper</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowTarget2UpperTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowTarget2Upper</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveTarget2UpperColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>0</R>
                            <G>191</G>
                            <B>255</B>
                            <ScA>1</ScA>
                            <ScR>0</ScR>
                            <ScG>0.5209956</ScG>
                            <ScB>1</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowTarget2UpperColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>139</R>
                            <G>0</G>
                            <B>0</B>
                            <ScA>1</ScA>
                            <ScR>0.258182883</ScR>
                            <ScG>0</ScG>
                            <ScB>0</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowTarget2LowerArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveTarget2LowerTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveTarget2Lower</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowTarget2LowerTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowTarget2Lower</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveTarget2LowerColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>0</R>
                            <G>191</G>
                            <B>255</B>
                            <ScA>1</ScA>
                            <ScR>0</ScR>
                            <ScG>0.5209956</ScG>
                            <ScB>1</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowTarget2LowerColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>139</R>
                            <G>0</G>
                            <B>0</B>
                            <ScA>1</ScA>
                            <ScR>0.258182883</ScR>
                            <ScG>0</ScG>
                            <ScB>0</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowDayStartArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowRangeStartArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SessionStartUpTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>SessionStartUp</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SessionStartDownTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>SessionStartDown</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SessionStartUpColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>144</R>
                            <G>238</G>
                            <B>144</B>
                            <ScA>1</ScA>
                            <ScR>0.2788943</ScR>
                            <ScG>0.8549926</ScG>
                            <ScB>0.2788943</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SessionStartDownColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>255</R>
                            <G>192</G>
                            <B>203</B>
                            <ScA>1</ScA>
                            <ScR>1</ScR>
                            <ScG>0.527115166</ScG>
                            <ScB>0.5972018</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowMid</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>MiddleRangeMultiplier</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">0.5</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">0.5</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowQuarters</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>QuarterRangeMultiplier</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">0.25</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">0.25</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowTarget1</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Target1Multiplier</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">1</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">1</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowTarget2</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Target2Multiplier</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">2</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">2</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowSocials</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Author</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>TradeSaber - Built With Grok</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Version</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>Version 1.3 // April 2025</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>TradeSaber</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>https://tradesaber.com/predator-guide/</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Discord</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>https://Discord.gg/2YU9GDme8j</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>YouTube</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>https://youtu.be/jUYT-Erzc_8</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>ORB_TradeSaber</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF4682B4&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORHigh</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFF8C00&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORLow</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#00FFFFFF&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>Signal</Name>
                        <PlotStyle>Dot</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF808080&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORMiddle</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFADD8E6&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORTarget1</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF00FFFF&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORTarget2</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFFA07A&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORTargetMinus1</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFA8072&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORTargetMinus2</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF696969&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORLowerTargetQuarter</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF696969&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORUpperTargetQuarter</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                  <SelectedPlot>ORLow</SelectedPlot>
                </AssociatedIndicator>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-05-17T23:33:33.0036353</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <OffsetBuilder>
                  <ConditionOffset>
                    <IsSetEnabled>false</IsSetEnabled>
                    <OffsetValue>0</OffsetValue>
                    <OffsetOperator>Add</OffsetOperator>
                    <OffsetType>Ticks</OffsetType>
                  </ConditionOffset>
                  <Offset>
                    <DefaultValue>0</DefaultValue>
                    <IsInt>false</IsInt>
                    <BindingValue xsi:type="xsd:string">TicjOffsetOrbLow</BindingValue>
                    <DynamicValue>
                      <Children />
                      <IsExpanded>false</IsExpanded>
                      <IsSelected>true</IsSelected>
                      <Name>TicjOffsetOrbLow</Name>
                      <OffsetType>Arithmetic</OffsetType>
                      <AssignedCommand>
                        <Command>TicjOffsetOrbLow</Command>
                        <Parameters />
                      </AssignedCommand>
                      <BarsAgo>0</BarsAgo>
                      <CurrencyType>Currency</CurrencyType>
                      <Date>2025-05-17T23:34:02.4027078</Date>
                      <DayOfWeek>Sunday</DayOfWeek>
                      <EndBar>0</EndBar>
                      <ForceSeriesIndex>false</ForceSeriesIndex>
                      <LookBackPeriod>0</LookBackPeriod>
                      <MarketPosition>Long</MarketPosition>
                      <Period>0</Period>
                      <ReturnType>Number</ReturnType>
                      <StartBar>0</StartBar>
                      <State>Undefined</State>
                      <Time>0001-01-01T00:00:00</Time>
                    </DynamicValue>
                    <IsLiteral>false</IsLiteral>
                    <LiveValue xsi:type="xsd:string">TicjOffsetOrbLow</LiveValue>
                  </Offset>
                </OffsetBuilder>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </DynamicValue>
              <IsLiteral>false</IsLiteral>
              <LiveValue xsi:type="xsd:string">(ORB_TradeSaber(Close, true, 0.2, DateTime.Parse("8:30 AM"), DateTime.Parse("8:45 AM"), @"Local", @"Recommended that PC and 
NinjaTrader clocks match", true, @"AboveUpper", @"BelowLower", Brushes.SteelBlue, Brushes.DarkOrange, false, @"AboveLower", @"BelowUpper", Brushes.LightGreen, Brushes.Pink, false, @"AboveMidline", @"BelowMidline", Brushes.Aqua, Brushes.Fuchsia, false, @"AboveUpperQuarter", @"BelowUpperQuarter", Brushes.Aqua, Brushes.Crimson, false, @"AboveLowerQuarter", @"BelowLowerQuarter", Brushes.Lime, Brushes.OrangeRed, false, @"AboveTarget1Upper", @"BelowTarget1Upper", Brushes.DodgerBlue, Brushes.Tomato, false, @"BelowTarget1Lower", @"AboveTarget1Lower", Brushes.Tomato, Brushes.DodgerBlue, false, @"AboveTarget2Upper", @"BelowTarget2Upper", Brushes.DeepSkyBlue, Brushes.DarkRed, false, @"AboveTarget2Lower", @"BelowTarget2Lower", Brushes.DeepSkyBlue, Brushes.DarkRed, false, false, @"SessionStartUp", @"SessionStartDown", Brushes.LightGreen, Brushes.Pink, false, 0.5, false, 0.25, false, 1, false, 2, false, @"TradeSaber - Built With Grok", @"Version 1.3 // April 2025", @"https://tradesaber.com/predator-guide/", @"https://Discord.gg/2YU9GDme8j", @"https://youtu.be/jUYT-Erzc_8").ORLow[0] + (TicjOffsetOrbLow * TickSize)) </LiveValue>
            </VariableDouble>
          </ActionProperties>
          <ActionType>SetValue</ActionType>
          <UserVariableType>double</UserVariableType>
          <VariableName>OrbLow</VariableName>
        </WizardAction>
      </Actions>
      <ActiveAction>
        <Children />
        <IsExpanded>false</IsExpanded>
        <IsSelected>true</IsSelected>
        <Name>Set OrbLow</Name>
        <OffsetType>Arithmetic</OffsetType>
        <ActionProperties>
          <DashStyle>Solid</DashStyle>
          <DivideTimePrice>false</DivideTimePrice>
          <Id />
          <File />
          <IsAutoScale>false</IsAutoScale>
          <IsSimulatedStop>false</IsSimulatedStop>
          <IsStop>false</IsStop>
          <LogLevel>Information</LogLevel>
          <Mode>Currency</Mode>
          <OffsetType>Currency</OffsetType>
          <Priority>Medium</Priority>
          <Quantity>
            <DefaultValue>0</DefaultValue>
            <IsInt>true</IsInt>
            <BindingValue xsi:type="xsd:string">DefaultQuantity</BindingValue>
            <DynamicValue>
              <Children />
              <IsExpanded>false</IsExpanded>
              <IsSelected>false</IsSelected>
              <Name>Default order quantity</Name>
              <OffsetType>Arithmetic</OffsetType>
              <AssignedCommand>
                <Command>DefaultQuantity</Command>
                <Parameters />
              </AssignedCommand>
              <BarsAgo>0</BarsAgo>
              <CurrencyType>Currency</CurrencyType>
              <Date>2025-05-17T23:33:26.9421893</Date>
              <DayOfWeek>Sunday</DayOfWeek>
              <EndBar>0</EndBar>
              <ForceSeriesIndex>false</ForceSeriesIndex>
              <LookBackPeriod>0</LookBackPeriod>
              <MarketPosition>Long</MarketPosition>
              <Period>0</Period>
              <ReturnType>Number</ReturnType>
              <StartBar>0</StartBar>
              <State>Undefined</State>
              <Time>0001-01-01T00:00:00</Time>
            </DynamicValue>
            <IsLiteral>false</IsLiteral>
            <LiveValue xsi:type="xsd:string">DefaultQuantity</LiveValue>
          </Quantity>
          <ServiceName />
          <ScreenshotPath />
          <SoundLocation />
          <TextPosition>BottomLeft</TextPosition>
          <VariableDateTime>2025-05-17T23:33:26.9421893</VariableDateTime>
          <VariableBool>false</VariableBool>
          <VariableDouble>
            <DefaultValue>0</DefaultValue>
            <IsInt>false</IsInt>
            <BindingValue xsi:type="xsd:string">(ORB_TradeSaber(true, 0.2, DateTime.Parse("8:30 AM"), DateTime.Parse("8:45 AM"), @"Local", @"Recommended that PC and 
NinjaTrader clocks match", true, @"AboveUpper", @"BelowLower", Brushes.SteelBlue, Brushes.DarkOrange, false, @"AboveLower", @"BelowUpper", Brushes.LightGreen, Brushes.Pink, false, @"AboveMidline", @"BelowMidline", Brushes.Aqua, Brushes.Fuchsia, false, @"AboveUpperQuarter", @"BelowUpperQuarter", Brushes.Aqua, Brushes.Crimson, false, @"AboveLowerQuarter", @"BelowLowerQuarter", Brushes.Lime, Brushes.OrangeRed, false, @"AboveTarget1Upper", @"BelowTarget1Upper", Brushes.DodgerBlue, Brushes.Tomato, false, @"BelowTarget1Lower", @"AboveTarget1Lower", Brushes.Tomato, Brushes.DodgerBlue, false, @"AboveTarget2Upper", @"BelowTarget2Upper", Brushes.DeepSkyBlue, Brushes.DarkRed, false, @"AboveTarget2Lower", @"BelowTarget2Lower", Brushes.DeepSkyBlue, Brushes.DarkRed, false, false, @"SessionStartUp", @"SessionStartDown", Brushes.LightGreen, Brushes.Pink, false, 0.5, false, 0.25, false, 1, false, 2, false, @"TradeSaber - Built With Grok", @"Version 1.3 // April 2025", @"https://tradesaber.com/predator-guide/", @"https://Discord.gg/2YU9GDme8j", @"https://youtu.be/jUYT-Erzc_8").ORLow[0] + (TicjOffsetOrbLow * TickSize)) </BindingValue>
            <DynamicValue>
              <Children />
              <IsExpanded>false</IsExpanded>
              <IsSelected>true</IsSelected>
              <Name>ORB_TradeSaber</Name>
              <OffsetType>Arithmetic</OffsetType>
              <AssignedCommand>
                <Command>ORB_TradeSaber</Command>
                <Parameters>
                  <string>AssociatedIndicator</string>
                  <string>BarsAgo</string>
                  <string>OffsetBuilder</string>
                </Parameters>
              </AssignedCommand>
              <AssociatedIndicator>
                <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                <CustomProperties>
                  <item>
                    <key>
                      <string>HighlightRange</string>
                    </key>
                    <value>
                      <anyType xsi:type="xsd:boolean">true</anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>HighlightOpacity</string>
                    </key>
                    <value>
                      <anyType xsi:type="NumberBuilder">
                        <DefaultValue>0</DefaultValue>
                        <IsInt>false</IsInt>
                        <BindingValue xsi:type="xsd:string">0.2</BindingValue>
                        <IsLiteral>true</IsLiteral>
                        <LiveValue xsi:type="xsd:string">0.2</LiveValue>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>StartTime</string>
                    </key>
                    <value>
                      <anyType xsi:type="xsd:dateTime">2000-01-01T08:30:00</anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>EndTime</string>
                    </key>
                    <value>
                      <anyType xsi:type="xsd:dateTime">2000-01-01T08:45:00</anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>TimeZoneSelection</string>
                    </key>
                    <value>
                      <anyType xsi:type="NSStringBuilder">
                        <SeparatorCharacter> </SeparatorCharacter>
                        <Strings>
                          <NinjaScriptString>
                            <Index>0</Index>
                            <StringValue>Local</StringValue>
                          </NinjaScriptString>
                        </Strings>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>TZNote</string>
                    </key>
                    <value>
                      <anyType xsi:type="NSStringBuilder">
                        <SeparatorCharacter> </SeparatorCharacter>
                        <Strings>
                          <NinjaScriptString>
                            <Index>0</Index>
                            <StringValue>Recommended that PC and 
NinjaTrader clocks match</StringValue>
                          </NinjaScriptString>
                        </Strings>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>ShowOuterArrows</string>
                    </key>
                    <value>
                      <anyType xsi:type="xsd:boolean">true</anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>AboveUpperTag</string>
                    </key>
                    <value>
                      <anyType xsi:type="NSStringBuilder">
                        <SeparatorCharacter> </SeparatorCharacter>
                        <Strings>
                          <NinjaScriptString>
                            <Index>0</Index>
                            <StringValue>AboveUpper</StringValue>
                          </NinjaScriptString>
                        </Strings>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>BelowLowerTag</string>
                    </key>
                    <value>
                      <anyType xsi:type="NSStringBuilder">
                        <SeparatorCharacter> </SeparatorCharacter>
                        <Strings>
                          <NinjaScriptString>
                            <Index>0</Index>
                            <StringValue>BelowLower</StringValue>
                          </NinjaScriptString>
                        </Strings>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>AboveUpperColor</string>
                    </key>
                    <value>
                      <anyType xsi:type="SolidColorBrush">
                        <Opacity>1</Opacity>
                        <Transform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </Transform>
                        <RelativeTransform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </RelativeTransform>
                        <Color>
                          <A>255</A>
                          <R>70</R>
                          <G>130</G>
                          <B>180</B>
                          <ScA>1</ScA>
                          <ScR>0.06124606</ScR>
                          <ScG>0.223227978</ScG>
                          <ScB>0.456411034</ScB>
                        </Color>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>BelowLowerColor</string>
                    </key>
                    <value>
                      <anyType xsi:type="SolidColorBrush">
                        <Opacity>1</Opacity>
                        <Transform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </Transform>
                        <RelativeTransform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </RelativeTransform>
                        <Color>
                          <A>255</A>
                          <R>255</R>
                          <G>140</G>
                          <B>0</B>
                          <ScA>1</ScA>
                          <ScR>1</ScR>
                          <ScG>0.2622507</ScG>
                          <ScB>0</ScB>
                        </Color>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>ShowInnerArrows</string>
                    </key>
                    <value>
                      <anyType xsi:type="xsd:boolean">false</anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>AboveLowerTag</string>
                    </key>
                    <value>
                      <anyType xsi:type="NSStringBuilder">
                        <SeparatorCharacter> </SeparatorCharacter>
                        <Strings>
                          <NinjaScriptString>
                            <Index>0</Index>
                            <StringValue>AboveLower</StringValue>
                          </NinjaScriptString>
                        </Strings>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>BelowUpperTag</string>
                    </key>
                    <value>
                      <anyType xsi:type="NSStringBuilder">
                        <SeparatorCharacter> </SeparatorCharacter>
                        <Strings>
                          <NinjaScriptString>
                            <Index>0</Index>
                            <StringValue>BelowUpper</StringValue>
                          </NinjaScriptString>
                        </Strings>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>AboveLowerColor</string>
                    </key>
                    <value>
                      <anyType xsi:type="SolidColorBrush">
                        <Opacity>1</Opacity>
                        <Transform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </Transform>
                        <RelativeTransform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </RelativeTransform>
                        <Color>
                          <A>255</A>
                          <R>144</R>
                          <G>238</G>
                          <B>144</B>
                          <ScA>1</ScA>
                          <ScR>0.2788943</ScR>
                          <ScG>0.8549926</ScG>
                          <ScB>0.2788943</ScB>
                        </Color>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>BelowUpperColor</string>
                    </key>
                    <value>
                      <anyType xsi:type="SolidColorBrush">
                        <Opacity>1</Opacity>
                        <Transform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </Transform>
                        <RelativeTransform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </RelativeTransform>
                        <Color>
                          <A>255</A>
                          <R>255</R>
                          <G>192</G>
                          <B>203</B>
                          <ScA>1</ScA>
                          <ScR>1</ScR>
                          <ScG>0.527115166</ScG>
                          <ScB>0.5972018</ScB>
                        </Color>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>ShowMidlineArrows</string>
                    </key>
                    <value>
                      <anyType xsi:type="xsd:boolean">false</anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>AboveMidlineTag</string>
                    </key>
                    <value>
                      <anyType xsi:type="NSStringBuilder">
                        <SeparatorCharacter> </SeparatorCharacter>
                        <Strings>
                          <NinjaScriptString>
                            <Index>0</Index>
                            <StringValue>AboveMidline</StringValue>
                          </NinjaScriptString>
                        </Strings>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>BelowMidlineTag</string>
                    </key>
                    <value>
                      <anyType xsi:type="NSStringBuilder">
                        <SeparatorCharacter> </SeparatorCharacter>
                        <Strings>
                          <NinjaScriptString>
                            <Index>0</Index>
                            <StringValue>BelowMidline</StringValue>
                          </NinjaScriptString>
                        </Strings>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>AboveMidlineColor</string>
                    </key>
                    <value>
                      <anyType xsi:type="SolidColorBrush">
                        <Opacity>1</Opacity>
                        <Transform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </Transform>
                        <RelativeTransform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </RelativeTransform>
                        <Color>
                          <A>255</A>
                          <R>0</R>
                          <G>255</G>
                          <B>255</B>
                          <ScA>1</ScA>
                          <ScR>0</ScR>
                          <ScG>1</ScG>
                          <ScB>1</ScB>
                        </Color>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>BelowMidlineColor</string>
                    </key>
                    <value>
                      <anyType xsi:type="SolidColorBrush">
                        <Opacity>1</Opacity>
                        <Transform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </Transform>
                        <RelativeTransform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </RelativeTransform>
                        <Color>
                          <A>255</A>
                          <R>255</R>
                          <G>0</G>
                          <B>255</B>
                          <ScA>1</ScA>
                          <ScR>1</ScR>
                          <ScG>0</ScG>
                          <ScB>1</ScB>
                        </Color>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>ShowUpperQuarterArrows</string>
                    </key>
                    <value>
                      <anyType xsi:type="xsd:boolean">false</anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>AboveUpperQuarterTag</string>
                    </key>
                    <value>
                      <anyType xsi:type="NSStringBuilder">
                        <SeparatorCharacter> </SeparatorCharacter>
                        <Strings>
                          <NinjaScriptString>
                            <Index>0</Index>
                            <StringValue>AboveUpperQuarter</StringValue>
                          </NinjaScriptString>
                        </Strings>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>BelowUpperQuarterTag</string>
                    </key>
                    <value>
                      <anyType xsi:type="NSStringBuilder">
                        <SeparatorCharacter> </SeparatorCharacter>
                        <Strings>
                          <NinjaScriptString>
                            <Index>0</Index>
                            <StringValue>BelowUpperQuarter</StringValue>
                          </NinjaScriptString>
                        </Strings>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>AboveUpperQuarterColor</string>
                    </key>
                    <value>
                      <anyType xsi:type="SolidColorBrush">
                        <Opacity>1</Opacity>
                        <Transform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </Transform>
                        <RelativeTransform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </RelativeTransform>
                        <Color>
                          <A>255</A>
                          <R>0</R>
                          <G>255</G>
                          <B>255</B>
                          <ScA>1</ScA>
                          <ScR>0</ScR>
                          <ScG>1</ScG>
                          <ScB>1</ScB>
                        </Color>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>BelowUpperQuarterColor</string>
                    </key>
                    <value>
                      <anyType xsi:type="SolidColorBrush">
                        <Opacity>1</Opacity>
                        <Transform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </Transform>
                        <RelativeTransform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </RelativeTransform>
                        <Color>
                          <A>255</A>
                          <R>220</R>
                          <G>20</G>
                          <B>60</B>
                          <ScA>1</ScA>
                          <ScR>0.715693533</ScR>
                          <ScG>0.00699541066</ScG>
                          <ScB>0.045186203</ScB>
                        </Color>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>ShowLowerQuarterArrows</string>
                    </key>
                    <value>
                      <anyType xsi:type="xsd:boolean">false</anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>AboveLowerQuarterTag</string>
                    </key>
                    <value>
                      <anyType xsi:type="NSStringBuilder">
                        <SeparatorCharacter> </SeparatorCharacter>
                        <Strings>
                          <NinjaScriptString>
                            <Index>0</Index>
                            <StringValue>AboveLowerQuarter</StringValue>
                          </NinjaScriptString>
                        </Strings>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>BelowLowerQuarterTag</string>
                    </key>
                    <value>
                      <anyType xsi:type="NSStringBuilder">
                        <SeparatorCharacter> </SeparatorCharacter>
                        <Strings>
                          <NinjaScriptString>
                            <Index>0</Index>
                            <StringValue>BelowLowerQuarter</StringValue>
                          </NinjaScriptString>
                        </Strings>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>AboveLowerQuarterColor</string>
                    </key>
                    <value>
                      <anyType xsi:type="SolidColorBrush">
                        <Opacity>1</Opacity>
                        <Transform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </Transform>
                        <RelativeTransform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </RelativeTransform>
                        <Color>
                          <A>255</A>
                          <R>0</R>
                          <G>255</G>
                          <B>0</B>
                          <ScA>1</ScA>
                          <ScR>0</ScR>
                          <ScG>1</ScG>
                          <ScB>0</ScB>
                        </Color>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>BelowLowerQuarterColor</string>
                    </key>
                    <value>
                      <anyType xsi:type="SolidColorBrush">
                        <Opacity>1</Opacity>
                        <Transform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </Transform>
                        <RelativeTransform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </RelativeTransform>
                        <Color>
                          <A>255</A>
                          <R>255</R>
                          <G>69</G>
                          <B>0</B>
                          <ScA>1</ScA>
                          <ScR>1</ScR>
                          <ScG>0.0595112443</ScG>
                          <ScB>0</ScB>
                        </Color>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>ShowTarget1UpperArrows</string>
                    </key>
                    <value>
                      <anyType xsi:type="xsd:boolean">false</anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>AboveTarget1UpperTag</string>
                    </key>
                    <value>
                      <anyType xsi:type="NSStringBuilder">
                        <SeparatorCharacter> </SeparatorCharacter>
                        <Strings>
                          <NinjaScriptString>
                            <Index>0</Index>
                            <StringValue>AboveTarget1Upper</StringValue>
                          </NinjaScriptString>
                        </Strings>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>BelowTarget1UpperTag</string>
                    </key>
                    <value>
                      <anyType xsi:type="NSStringBuilder">
                        <SeparatorCharacter> </SeparatorCharacter>
                        <Strings>
                          <NinjaScriptString>
                            <Index>0</Index>
                            <StringValue>BelowTarget1Upper</StringValue>
                          </NinjaScriptString>
                        </Strings>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>AboveTarget1UpperColor</string>
                    </key>
                    <value>
                      <anyType xsi:type="SolidColorBrush">
                        <Opacity>1</Opacity>
                        <Transform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </Transform>
                        <RelativeTransform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </RelativeTransform>
                        <Color>
                          <A>255</A>
                          <R>30</R>
                          <G>144</G>
                          <B>255</B>
                          <ScA>1</ScA>
                          <ScR>0.0129830325</ScR>
                          <ScG>0.2788943</ScG>
                          <ScB>1</ScB>
                        </Color>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>BelowTarget1UpperColor</string>
                    </key>
                    <value>
                      <anyType xsi:type="SolidColorBrush">
                        <Opacity>1</Opacity>
                        <Transform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </Transform>
                        <RelativeTransform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </RelativeTransform>
                        <Color>
                          <A>255</A>
                          <R>255</R>
                          <G>99</G>
                          <B>71</B>
                          <ScA>1</ScA>
                          <ScR>1</ScR>
                          <ScG>0.124771819</ScG>
                          <ScB>0.06301002</ScB>
                        </Color>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>ShowTarget1LowerArrows</string>
                    </key>
                    <value>
                      <anyType xsi:type="xsd:boolean">false</anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>BelowTarget1LowerTag</string>
                    </key>
                    <value>
                      <anyType xsi:type="NSStringBuilder">
                        <SeparatorCharacter> </SeparatorCharacter>
                        <Strings>
                          <NinjaScriptString>
                            <Index>0</Index>
                            <StringValue>BelowTarget1Lower</StringValue>
                          </NinjaScriptString>
                        </Strings>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>AboveTarget1LowerTag</string>
                    </key>
                    <value>
                      <anyType xsi:type="NSStringBuilder">
                        <SeparatorCharacter> </SeparatorCharacter>
                        <Strings>
                          <NinjaScriptString>
                            <Index>0</Index>
                            <StringValue>AboveTarget1Lower</StringValue>
                          </NinjaScriptString>
                        </Strings>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>BelowTarget1LowerColor</string>
                    </key>
                    <value>
                      <anyType xsi:type="SolidColorBrush">
                        <Opacity>1</Opacity>
                        <Transform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </Transform>
                        <RelativeTransform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </RelativeTransform>
                        <Color>
                          <A>255</A>
                          <R>255</R>
                          <G>99</G>
                          <B>71</B>
                          <ScA>1</ScA>
                          <ScR>1</ScR>
                          <ScG>0.124771819</ScG>
                          <ScB>0.06301002</ScB>
                        </Color>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>AboveTarget1LowerColor</string>
                    </key>
                    <value>
                      <anyType xsi:type="SolidColorBrush">
                        <Opacity>1</Opacity>
                        <Transform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </Transform>
                        <RelativeTransform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </RelativeTransform>
                        <Color>
                          <A>255</A>
                          <R>30</R>
                          <G>144</G>
                          <B>255</B>
                          <ScA>1</ScA>
                          <ScR>0.0129830325</ScR>
                          <ScG>0.2788943</ScG>
                          <ScB>1</ScB>
                        </Color>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>ShowTarget2UpperArrows</string>
                    </key>
                    <value>
                      <anyType xsi:type="xsd:boolean">false</anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>AboveTarget2UpperTag</string>
                    </key>
                    <value>
                      <anyType xsi:type="NSStringBuilder">
                        <SeparatorCharacter> </SeparatorCharacter>
                        <Strings>
                          <NinjaScriptString>
                            <Index>0</Index>
                            <StringValue>AboveTarget2Upper</StringValue>
                          </NinjaScriptString>
                        </Strings>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>BelowTarget2UpperTag</string>
                    </key>
                    <value>
                      <anyType xsi:type="NSStringBuilder">
                        <SeparatorCharacter> </SeparatorCharacter>
                        <Strings>
                          <NinjaScriptString>
                            <Index>0</Index>
                            <StringValue>BelowTarget2Upper</StringValue>
                          </NinjaScriptString>
                        </Strings>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>AboveTarget2UpperColor</string>
                    </key>
                    <value>
                      <anyType xsi:type="SolidColorBrush">
                        <Opacity>1</Opacity>
                        <Transform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </Transform>
                        <RelativeTransform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </RelativeTransform>
                        <Color>
                          <A>255</A>
                          <R>0</R>
                          <G>191</G>
                          <B>255</B>
                          <ScA>1</ScA>
                          <ScR>0</ScR>
                          <ScG>0.5209956</ScG>
                          <ScB>1</ScB>
                        </Color>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>BelowTarget2UpperColor</string>
                    </key>
                    <value>
                      <anyType xsi:type="SolidColorBrush">
                        <Opacity>1</Opacity>
                        <Transform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </Transform>
                        <RelativeTransform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </RelativeTransform>
                        <Color>
                          <A>255</A>
                          <R>139</R>
                          <G>0</G>
                          <B>0</B>
                          <ScA>1</ScA>
                          <ScR>0.258182883</ScR>
                          <ScG>0</ScG>
                          <ScB>0</ScB>
                        </Color>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>ShowTarget2LowerArrows</string>
                    </key>
                    <value>
                      <anyType xsi:type="xsd:boolean">false</anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>AboveTarget2LowerTag</string>
                    </key>
                    <value>
                      <anyType xsi:type="NSStringBuilder">
                        <SeparatorCharacter> </SeparatorCharacter>
                        <Strings>
                          <NinjaScriptString>
                            <Index>0</Index>
                            <StringValue>AboveTarget2Lower</StringValue>
                          </NinjaScriptString>
                        </Strings>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>BelowTarget2LowerTag</string>
                    </key>
                    <value>
                      <anyType xsi:type="NSStringBuilder">
                        <SeparatorCharacter> </SeparatorCharacter>
                        <Strings>
                          <NinjaScriptString>
                            <Index>0</Index>
                            <StringValue>BelowTarget2Lower</StringValue>
                          </NinjaScriptString>
                        </Strings>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>AboveTarget2LowerColor</string>
                    </key>
                    <value>
                      <anyType xsi:type="SolidColorBrush">
                        <Opacity>1</Opacity>
                        <Transform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </Transform>
                        <RelativeTransform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </RelativeTransform>
                        <Color>
                          <A>255</A>
                          <R>0</R>
                          <G>191</G>
                          <B>255</B>
                          <ScA>1</ScA>
                          <ScR>0</ScR>
                          <ScG>0.5209956</ScG>
                          <ScB>1</ScB>
                        </Color>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>BelowTarget2LowerColor</string>
                    </key>
                    <value>
                      <anyType xsi:type="SolidColorBrush">
                        <Opacity>1</Opacity>
                        <Transform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </Transform>
                        <RelativeTransform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </RelativeTransform>
                        <Color>
                          <A>255</A>
                          <R>139</R>
                          <G>0</G>
                          <B>0</B>
                          <ScA>1</ScA>
                          <ScR>0.258182883</ScR>
                          <ScG>0</ScG>
                          <ScB>0</ScB>
                        </Color>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>ShowDayStartArrows</string>
                    </key>
                    <value>
                      <anyType xsi:type="xsd:boolean">false</anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>ShowRangeStartArrows</string>
                    </key>
                    <value>
                      <anyType xsi:type="xsd:boolean">false</anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>SessionStartUpTag</string>
                    </key>
                    <value>
                      <anyType xsi:type="NSStringBuilder">
                        <SeparatorCharacter> </SeparatorCharacter>
                        <Strings>
                          <NinjaScriptString>
                            <Index>0</Index>
                            <StringValue>SessionStartUp</StringValue>
                          </NinjaScriptString>
                        </Strings>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>SessionStartDownTag</string>
                    </key>
                    <value>
                      <anyType xsi:type="NSStringBuilder">
                        <SeparatorCharacter> </SeparatorCharacter>
                        <Strings>
                          <NinjaScriptString>
                            <Index>0</Index>
                            <StringValue>SessionStartDown</StringValue>
                          </NinjaScriptString>
                        </Strings>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>SessionStartUpColor</string>
                    </key>
                    <value>
                      <anyType xsi:type="SolidColorBrush">
                        <Opacity>1</Opacity>
                        <Transform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </Transform>
                        <RelativeTransform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </RelativeTransform>
                        <Color>
                          <A>255</A>
                          <R>144</R>
                          <G>238</G>
                          <B>144</B>
                          <ScA>1</ScA>
                          <ScR>0.2788943</ScR>
                          <ScG>0.8549926</ScG>
                          <ScB>0.2788943</ScB>
                        </Color>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>SessionStartDownColor</string>
                    </key>
                    <value>
                      <anyType xsi:type="SolidColorBrush">
                        <Opacity>1</Opacity>
                        <Transform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </Transform>
                        <RelativeTransform xsi:type="MatrixTransform">
                          <Matrix>
                            <M11>1</M11>
                            <M12>0</M12>
                            <M21>0</M21>
                            <M22>1</M22>
                            <OffsetX>0</OffsetX>
                            <OffsetY>0</OffsetY>
                          </Matrix>
                        </RelativeTransform>
                        <Color>
                          <A>255</A>
                          <R>255</R>
                          <G>192</G>
                          <B>203</B>
                          <ScA>1</ScA>
                          <ScR>1</ScR>
                          <ScG>0.527115166</ScG>
                          <ScB>0.5972018</ScB>
                        </Color>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>ShowMid</string>
                    </key>
                    <value>
                      <anyType xsi:type="xsd:boolean">false</anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>MiddleRangeMultiplier</string>
                    </key>
                    <value>
                      <anyType xsi:type="NumberBuilder">
                        <DefaultValue>0</DefaultValue>
                        <IsInt>false</IsInt>
                        <BindingValue xsi:type="xsd:string">0.5</BindingValue>
                        <IsLiteral>true</IsLiteral>
                        <LiveValue xsi:type="xsd:string">0.5</LiveValue>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>ShowQuarters</string>
                    </key>
                    <value>
                      <anyType xsi:type="xsd:boolean">false</anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>QuarterRangeMultiplier</string>
                    </key>
                    <value>
                      <anyType xsi:type="NumberBuilder">
                        <DefaultValue>0</DefaultValue>
                        <IsInt>false</IsInt>
                        <BindingValue xsi:type="xsd:string">0.25</BindingValue>
                        <IsLiteral>true</IsLiteral>
                        <LiveValue xsi:type="xsd:string">0.25</LiveValue>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>ShowTarget1</string>
                    </key>
                    <value>
                      <anyType xsi:type="xsd:boolean">false</anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>Target1Multiplier</string>
                    </key>
                    <value>
                      <anyType xsi:type="NumberBuilder">
                        <DefaultValue>0</DefaultValue>
                        <IsInt>false</IsInt>
                        <BindingValue xsi:type="xsd:string">1</BindingValue>
                        <IsLiteral>true</IsLiteral>
                        <LiveValue xsi:type="xsd:string">1</LiveValue>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>ShowTarget2</string>
                    </key>
                    <value>
                      <anyType xsi:type="xsd:boolean">false</anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>Target2Multiplier</string>
                    </key>
                    <value>
                      <anyType xsi:type="NumberBuilder">
                        <DefaultValue>0</DefaultValue>
                        <IsInt>false</IsInt>
                        <BindingValue xsi:type="xsd:string">2</BindingValue>
                        <IsLiteral>true</IsLiteral>
                        <LiveValue xsi:type="xsd:string">2</LiveValue>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>ShowSocials</string>
                    </key>
                    <value>
                      <anyType xsi:type="xsd:boolean">false</anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>Author</string>
                    </key>
                    <value>
                      <anyType xsi:type="NSStringBuilder">
                        <SeparatorCharacter> </SeparatorCharacter>
                        <Strings>
                          <NinjaScriptString>
                            <Index>0</Index>
                            <StringValue>TradeSaber - Built With Grok</StringValue>
                          </NinjaScriptString>
                        </Strings>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>Version</string>
                    </key>
                    <value>
                      <anyType xsi:type="NSStringBuilder">
                        <SeparatorCharacter> </SeparatorCharacter>
                        <Strings>
                          <NinjaScriptString>
                            <Index>0</Index>
                            <StringValue>Version 1.3 // April 2025</StringValue>
                          </NinjaScriptString>
                        </Strings>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>TradeSaber</string>
                    </key>
                    <value>
                      <anyType xsi:type="NSStringBuilder">
                        <SeparatorCharacter> </SeparatorCharacter>
                        <Strings>
                          <NinjaScriptString>
                            <Index>0</Index>
                            <StringValue>https://tradesaber.com/predator-guide/</StringValue>
                          </NinjaScriptString>
                        </Strings>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>Discord</string>
                    </key>
                    <value>
                      <anyType xsi:type="NSStringBuilder">
                        <SeparatorCharacter> </SeparatorCharacter>
                        <Strings>
                          <NinjaScriptString>
                            <Index>0</Index>
                            <StringValue>https://Discord.gg/2YU9GDme8j</StringValue>
                          </NinjaScriptString>
                        </Strings>
                      </anyType>
                    </value>
                  </item>
                  <item>
                    <key>
                      <string>YouTube</string>
                    </key>
                    <value>
                      <anyType xsi:type="NSStringBuilder">
                        <SeparatorCharacter> </SeparatorCharacter>
                        <Strings>
                          <NinjaScriptString>
                            <Index>0</Index>
                            <StringValue>https://youtu.be/jUYT-Erzc_8</StringValue>
                          </NinjaScriptString>
                        </Strings>
                      </anyType>
                    </value>
                  </item>
                </CustomProperties>
                <IndicatorHolder>
                  <IndicatorName>ORB_TradeSaber</IndicatorName>
                  <Plots>
                    <Plot>
                      <IsOpacityVisible>false</IsOpacityVisible>
                      <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF4682B4&lt;/SolidColorBrush&gt;</BrushSerialize>
                      <DashStyleHelper>Solid</DashStyleHelper>
                      <Opacity>100</Opacity>
                      <Width>1</Width>
                      <AutoWidth>false</AutoWidth>
                      <Max>1.7976931348623157E+308</Max>
                      <Min>-1.7976931348623157E+308</Min>
                      <Name>ORHigh</Name>
                      <PlotStyle>Line</PlotStyle>
                    </Plot>
                    <Plot>
                      <IsOpacityVisible>false</IsOpacityVisible>
                      <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFF8C00&lt;/SolidColorBrush&gt;</BrushSerialize>
                      <DashStyleHelper>Solid</DashStyleHelper>
                      <Opacity>100</Opacity>
                      <Width>1</Width>
                      <AutoWidth>false</AutoWidth>
                      <Max>1.7976931348623157E+308</Max>
                      <Min>-1.7976931348623157E+308</Min>
                      <Name>ORLow</Name>
                      <PlotStyle>Line</PlotStyle>
                    </Plot>
                    <Plot>
                      <IsOpacityVisible>false</IsOpacityVisible>
                      <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#00FFFFFF&lt;/SolidColorBrush&gt;</BrushSerialize>
                      <DashStyleHelper>Solid</DashStyleHelper>
                      <Opacity>100</Opacity>
                      <Width>1</Width>
                      <AutoWidth>false</AutoWidth>
                      <Max>1.7976931348623157E+308</Max>
                      <Min>-1.7976931348623157E+308</Min>
                      <Name>Signal</Name>
                      <PlotStyle>Dot</PlotStyle>
                    </Plot>
                    <Plot>
                      <IsOpacityVisible>false</IsOpacityVisible>
                      <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF808080&lt;/SolidColorBrush&gt;</BrushSerialize>
                      <DashStyleHelper>Solid</DashStyleHelper>
                      <Opacity>100</Opacity>
                      <Width>1</Width>
                      <AutoWidth>false</AutoWidth>
                      <Max>1.7976931348623157E+308</Max>
                      <Min>-1.7976931348623157E+308</Min>
                      <Name>ORMiddle</Name>
                      <PlotStyle>Line</PlotStyle>
                    </Plot>
                    <Plot>
                      <IsOpacityVisible>false</IsOpacityVisible>
                      <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFADD8E6&lt;/SolidColorBrush&gt;</BrushSerialize>
                      <DashStyleHelper>Solid</DashStyleHelper>
                      <Opacity>100</Opacity>
                      <Width>1</Width>
                      <AutoWidth>false</AutoWidth>
                      <Max>1.7976931348623157E+308</Max>
                      <Min>-1.7976931348623157E+308</Min>
                      <Name>ORTarget1</Name>
                      <PlotStyle>Line</PlotStyle>
                    </Plot>
                    <Plot>
                      <IsOpacityVisible>false</IsOpacityVisible>
                      <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF00FFFF&lt;/SolidColorBrush&gt;</BrushSerialize>
                      <DashStyleHelper>Solid</DashStyleHelper>
                      <Opacity>100</Opacity>
                      <Width>1</Width>
                      <AutoWidth>false</AutoWidth>
                      <Max>1.7976931348623157E+308</Max>
                      <Min>-1.7976931348623157E+308</Min>
                      <Name>ORTarget2</Name>
                      <PlotStyle>Line</PlotStyle>
                    </Plot>
                    <Plot>
                      <IsOpacityVisible>false</IsOpacityVisible>
                      <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFFA07A&lt;/SolidColorBrush&gt;</BrushSerialize>
                      <DashStyleHelper>Solid</DashStyleHelper>
                      <Opacity>100</Opacity>
                      <Width>1</Width>
                      <AutoWidth>false</AutoWidth>
                      <Max>1.7976931348623157E+308</Max>
                      <Min>-1.7976931348623157E+308</Min>
                      <Name>ORTargetMinus1</Name>
                      <PlotStyle>Line</PlotStyle>
                    </Plot>
                    <Plot>
                      <IsOpacityVisible>false</IsOpacityVisible>
                      <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFA8072&lt;/SolidColorBrush&gt;</BrushSerialize>
                      <DashStyleHelper>Solid</DashStyleHelper>
                      <Opacity>100</Opacity>
                      <Width>1</Width>
                      <AutoWidth>false</AutoWidth>
                      <Max>1.7976931348623157E+308</Max>
                      <Min>-1.7976931348623157E+308</Min>
                      <Name>ORTargetMinus2</Name>
                      <PlotStyle>Line</PlotStyle>
                    </Plot>
                    <Plot>
                      <IsOpacityVisible>false</IsOpacityVisible>
                      <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF696969&lt;/SolidColorBrush&gt;</BrushSerialize>
                      <DashStyleHelper>Solid</DashStyleHelper>
                      <Opacity>100</Opacity>
                      <Width>1</Width>
                      <AutoWidth>false</AutoWidth>
                      <Max>1.7976931348623157E+308</Max>
                      <Min>-1.7976931348623157E+308</Min>
                      <Name>ORLowerTargetQuarter</Name>
                      <PlotStyle>Line</PlotStyle>
                    </Plot>
                    <Plot>
                      <IsOpacityVisible>false</IsOpacityVisible>
                      <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF696969&lt;/SolidColorBrush&gt;</BrushSerialize>
                      <DashStyleHelper>Solid</DashStyleHelper>
                      <Opacity>100</Opacity>
                      <Width>1</Width>
                      <AutoWidth>false</AutoWidth>
                      <Max>1.7976931348623157E+308</Max>
                      <Min>-1.7976931348623157E+308</Min>
                      <Name>ORUpperTargetQuarter</Name>
                      <PlotStyle>Line</PlotStyle>
                    </Plot>
                  </Plots>
                </IndicatorHolder>
                <IsExplicitlyNamed>false</IsExplicitlyNamed>
                <IsPriceTypeLocked>false</IsPriceTypeLocked>
                <PlotOnChart>false</PlotOnChart>
                <PriceType>Close</PriceType>
                <SeriesType>Indicator</SeriesType>
                <SelectedPlot>ORLow</SelectedPlot>
              </AssociatedIndicator>
              <BarsAgo>0</BarsAgo>
              <CurrencyType>Currency</CurrencyType>
              <Date>2025-05-17T23:33:33.0036353</Date>
              <DayOfWeek>Sunday</DayOfWeek>
              <EndBar>0</EndBar>
              <ForceSeriesIndex>false</ForceSeriesIndex>
              <LookBackPeriod>0</LookBackPeriod>
              <MarketPosition>Long</MarketPosition>
              <OffsetBuilder>
                <ConditionOffset>
                  <IsSetEnabled>false</IsSetEnabled>
                  <OffsetValue>0</OffsetValue>
                  <OffsetOperator>Add</OffsetOperator>
                  <OffsetType>Ticks</OffsetType>
                </ConditionOffset>
                <Offset>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>false</IsInt>
                  <BindingValue xsi:type="xsd:string">TicjOffsetOrbLow</BindingValue>
                  <DynamicValue>
                    <Children />
                    <IsExpanded>false</IsExpanded>
                    <IsSelected>true</IsSelected>
                    <Name>TicjOffsetOrbLow</Name>
                    <OffsetType>Arithmetic</OffsetType>
                    <AssignedCommand>
                      <Command>TicjOffsetOrbLow</Command>
                      <Parameters />
                    </AssignedCommand>
                    <BarsAgo>0</BarsAgo>
                    <CurrencyType>Currency</CurrencyType>
                    <Date>2025-05-17T23:34:02.4027078</Date>
                    <DayOfWeek>Sunday</DayOfWeek>
                    <EndBar>0</EndBar>
                    <ForceSeriesIndex>false</ForceSeriesIndex>
                    <LookBackPeriod>0</LookBackPeriod>
                    <MarketPosition>Long</MarketPosition>
                    <Period>0</Period>
                    <ReturnType>Number</ReturnType>
                    <StartBar>0</StartBar>
                    <State>Undefined</State>
                    <Time>0001-01-01T00:00:00</Time>
                  </DynamicValue>
                  <IsLiteral>false</IsLiteral>
                  <LiveValue xsi:type="xsd:string">TicjOffsetOrbLow</LiveValue>
                </Offset>
              </OffsetBuilder>
              <Period>0</Period>
              <ReturnType>Series</ReturnType>
              <StartBar>0</StartBar>
              <State>Undefined</State>
              <Time>0001-01-01T00:00:00</Time>
            </DynamicValue>
            <IsLiteral>false</IsLiteral>
            <LiveValue xsi:type="xsd:string">(ORB_TradeSaber(Close, true, 0.2, DateTime.Parse("8:30 AM"), DateTime.Parse("8:45 AM"), @"Local", @"Recommended that PC and 
NinjaTrader clocks match", true, @"AboveUpper", @"BelowLower", Brushes.SteelBlue, Brushes.DarkOrange, false, @"AboveLower", @"BelowUpper", Brushes.LightGreen, Brushes.Pink, false, @"AboveMidline", @"BelowMidline", Brushes.Aqua, Brushes.Fuchsia, false, @"AboveUpperQuarter", @"BelowUpperQuarter", Brushes.Aqua, Brushes.Crimson, false, @"AboveLowerQuarter", @"BelowLowerQuarter", Brushes.Lime, Brushes.OrangeRed, false, @"AboveTarget1Upper", @"BelowTarget1Upper", Brushes.DodgerBlue, Brushes.Tomato, false, @"BelowTarget1Lower", @"AboveTarget1Lower", Brushes.Tomato, Brushes.DodgerBlue, false, @"AboveTarget2Upper", @"BelowTarget2Upper", Brushes.DeepSkyBlue, Brushes.DarkRed, false, @"AboveTarget2Lower", @"BelowTarget2Lower", Brushes.DeepSkyBlue, Brushes.DarkRed, false, false, @"SessionStartUp", @"SessionStartDown", Brushes.LightGreen, Brushes.Pink, false, 0.5, false, 0.25, false, 1, false, 2, false, @"TradeSaber - Built With Grok", @"Version 1.3 // April 2025", @"https://tradesaber.com/predator-guide/", @"https://Discord.gg/2YU9GDme8j", @"https://youtu.be/jUYT-Erzc_8").ORLow[0] + (TicjOffsetOrbLow * TickSize)) </LiveValue>
          </VariableDouble>
        </ActionProperties>
        <ActionType>SetValue</ActionType>
        <UserVariableType>double</UserVariableType>
        <VariableName>OrbLow</VariableName>
      </ActiveAction>
      <AnyOrAll>Any</AnyOrAll>
      <Conditions>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>ORB_TradeSaber</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>ORB_TradeSaber</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>HighlightRange</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>HighlightOpacity</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">0.2</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">0.2</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>StartTime</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:dateTime">2000-01-01T08:30:00</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>EndTime</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:dateTime">2000-01-01T08:45:00</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>TimeZoneSelection</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>Local</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>TZNote</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>Recommended that PC and 
NinjaTrader clocks match</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowOuterArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveUpperTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveUpper</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowLowerTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowLower</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveUpperColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>70</R>
                            <G>130</G>
                            <B>180</B>
                            <ScA>1</ScA>
                            <ScR>0.06124606</ScR>
                            <ScG>0.223227978</ScG>
                            <ScB>0.456411034</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowLowerColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>255</R>
                            <G>140</G>
                            <B>0</B>
                            <ScA>1</ScA>
                            <ScR>1</ScR>
                            <ScG>0.2622507</ScG>
                            <ScB>0</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowInnerArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveLowerTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveLower</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowUpperTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowUpper</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveLowerColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>144</R>
                            <G>238</G>
                            <B>144</B>
                            <ScA>1</ScA>
                            <ScR>0.2788943</ScR>
                            <ScG>0.8549926</ScG>
                            <ScB>0.2788943</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowUpperColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>255</R>
                            <G>192</G>
                            <B>203</B>
                            <ScA>1</ScA>
                            <ScR>1</ScR>
                            <ScG>0.527115166</ScG>
                            <ScB>0.5972018</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowMidlineArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveMidlineTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveMidline</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowMidlineTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowMidline</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveMidlineColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>0</R>
                            <G>255</G>
                            <B>255</B>
                            <ScA>1</ScA>
                            <ScR>0</ScR>
                            <ScG>1</ScG>
                            <ScB>1</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowMidlineColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>255</R>
                            <G>0</G>
                            <B>255</B>
                            <ScA>1</ScA>
                            <ScR>1</ScR>
                            <ScG>0</ScG>
                            <ScB>1</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowUpperQuarterArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveUpperQuarterTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveUpperQuarter</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowUpperQuarterTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowUpperQuarter</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveUpperQuarterColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>0</R>
                            <G>255</G>
                            <B>255</B>
                            <ScA>1</ScA>
                            <ScR>0</ScR>
                            <ScG>1</ScG>
                            <ScB>1</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowUpperQuarterColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>220</R>
                            <G>20</G>
                            <B>60</B>
                            <ScA>1</ScA>
                            <ScR>0.715693533</ScR>
                            <ScG>0.00699541066</ScG>
                            <ScB>0.045186203</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowLowerQuarterArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveLowerQuarterTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveLowerQuarter</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowLowerQuarterTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowLowerQuarter</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveLowerQuarterColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>0</R>
                            <G>255</G>
                            <B>0</B>
                            <ScA>1</ScA>
                            <ScR>0</ScR>
                            <ScG>1</ScG>
                            <ScB>0</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowLowerQuarterColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>255</R>
                            <G>69</G>
                            <B>0</B>
                            <ScA>1</ScA>
                            <ScR>1</ScR>
                            <ScG>0.0595112443</ScG>
                            <ScB>0</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowTarget1UpperArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveTarget1UpperTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveTarget1Upper</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowTarget1UpperTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowTarget1Upper</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveTarget1UpperColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>30</R>
                            <G>144</G>
                            <B>255</B>
                            <ScA>1</ScA>
                            <ScR>0.0129830325</ScR>
                            <ScG>0.2788943</ScG>
                            <ScB>1</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowTarget1UpperColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>255</R>
                            <G>99</G>
                            <B>71</B>
                            <ScA>1</ScA>
                            <ScR>1</ScR>
                            <ScG>0.124771819</ScG>
                            <ScB>0.06301002</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowTarget1LowerArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowTarget1LowerTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowTarget1Lower</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveTarget1LowerTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveTarget1Lower</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowTarget1LowerColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>255</R>
                            <G>99</G>
                            <B>71</B>
                            <ScA>1</ScA>
                            <ScR>1</ScR>
                            <ScG>0.124771819</ScG>
                            <ScB>0.06301002</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveTarget1LowerColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>30</R>
                            <G>144</G>
                            <B>255</B>
                            <ScA>1</ScA>
                            <ScR>0.0129830325</ScR>
                            <ScG>0.2788943</ScG>
                            <ScB>1</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowTarget2UpperArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveTarget2UpperTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveTarget2Upper</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowTarget2UpperTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowTarget2Upper</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveTarget2UpperColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>0</R>
                            <G>191</G>
                            <B>255</B>
                            <ScA>1</ScA>
                            <ScR>0</ScR>
                            <ScG>0.5209956</ScG>
                            <ScB>1</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowTarget2UpperColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>139</R>
                            <G>0</G>
                            <B>0</B>
                            <ScA>1</ScA>
                            <ScR>0.258182883</ScR>
                            <ScG>0</ScG>
                            <ScB>0</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowTarget2LowerArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveTarget2LowerTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveTarget2Lower</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowTarget2LowerTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowTarget2Lower</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveTarget2LowerColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>0</R>
                            <G>191</G>
                            <B>255</B>
                            <ScA>1</ScA>
                            <ScR>0</ScR>
                            <ScG>0.5209956</ScG>
                            <ScB>1</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowTarget2LowerColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>139</R>
                            <G>0</G>
                            <B>0</B>
                            <ScA>1</ScA>
                            <ScR>0.258182883</ScR>
                            <ScG>0</ScG>
                            <ScB>0</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowDayStartArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowRangeStartArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SessionStartUpTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>SessionStartUp</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SessionStartDownTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>SessionStartDown</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SessionStartUpColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>144</R>
                            <G>238</G>
                            <B>144</B>
                            <ScA>1</ScA>
                            <ScR>0.2788943</ScR>
                            <ScG>0.8549926</ScG>
                            <ScB>0.2788943</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SessionStartDownColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>255</R>
                            <G>192</G>
                            <B>203</B>
                            <ScA>1</ScA>
                            <ScR>1</ScR>
                            <ScG>0.527115166</ScG>
                            <ScB>0.5972018</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowMid</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>MiddleRangeMultiplier</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">0.5</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">0.5</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowQuarters</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>QuarterRangeMultiplier</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">0.25</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">0.25</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowTarget1</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Target1Multiplier</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">1</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">1</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowTarget2</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Target2Multiplier</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">2</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">2</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowSocials</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Author</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>TradeSaber - Built With Grok</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Version</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>Version 1.3 // April 2025</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>TradeSaber</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>https://tradesaber.com/predator-guide/</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Discord</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>https://Discord.gg/2YU9GDme8j</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>YouTube</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>https://youtu.be/jUYT-Erzc_8</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>ORB_TradeSaber</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF4682B4&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORHigh</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFF8C00&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORLow</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#00FFFFFF&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>Signal</Name>
                        <PlotStyle>Dot</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF808080&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORMiddle</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFADD8E6&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORTarget1</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF00FFFF&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORTarget2</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFFA07A&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORTargetMinus1</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFA8072&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORTargetMinus2</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF696969&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORLowerTargetQuarter</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF696969&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORUpperTargetQuarter</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                  <SelectedPlot>Signal</SelectedPlot>
                </AssociatedIndicator>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-05-17T23:30:51.6526396</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Numeric value</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>NumericValue</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-05-17T23:30:51.6946413</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <NumericValue>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>false</IsInt>
                  <BindingValue xsi:type="xsd:string">1</BindingValue>
                  <IsLiteral>true</IsLiteral>
                  <LiveValue xsi:type="xsd:string">1</LiveValue>
                </NumericValue>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>ORB_TradeSaber(true, 0.2, DateTime.Parse("8:30 AM"), DateTime.Parse("8:45 AM"), @"Local", @"Recommended that PC and 
NinjaTrader clocks match", true, @"AboveUpper", @"BelowLower", Brushes.SteelBlue, Brushes.DarkOrange, false, @"AboveLower", @"BelowUpper", Brushes.LightGreen, Brushes.Pink, false, @"AboveMidline", @"BelowMidline", Brushes.Aqua, Brushes.Fuchsia, false, @"AboveUpperQuarter", @"BelowUpperQuarter", Brushes.Aqua, Brushes.Crimson, false, @"AboveLowerQuarter", @"BelowLowerQuarter", Brushes.Lime, Brushes.OrangeRed, false, @"AboveTarget1Upper", @"BelowTarget1Upper", Brushes.DodgerBlue, Brushes.Tomato, false, @"BelowTarget1Lower", @"AboveTarget1Lower", Brushes.Tomato, Brushes.DodgerBlue, false, @"AboveTarget2Upper", @"BelowTarget2Upper", Brushes.DeepSkyBlue, Brushes.DarkRed, false, @"AboveTarget2Lower", @"BelowTarget2Lower", Brushes.DeepSkyBlue, Brushes.DarkRed, false, false, @"SessionStartUp", @"SessionStartDown", Brushes.LightGreen, Brushes.Pink, false, 0.5, false, 0.25, false, 1, false, 2, false, @"TradeSaber - Built With Grok", @"Version 1.3 // April 2025", @"https://tradesaber.com/predator-guide/", @"https://Discord.gg/2YU9GDme8j", @"https://youtu.be/jUYT-Erzc_8").Signal[0] = 1</DisplayName>
        </WizardConditionGroup>
        <WizardConditionGroup>
          <AnyOrAll>Any</AnyOrAll>
          <Conditions>
            <WizardCondition>
              <LeftItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>ORB_TradeSaber</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>ORB_TradeSaber</Command>
                  <Parameters>
                    <string>AssociatedIndicator</string>
                    <string>BarsAgo</string>
                    <string>OffsetBuilder</string>
                  </Parameters>
                </AssignedCommand>
                <AssociatedIndicator>
                  <AcceptableSeries>Indicator DataSeries CustomSeries DefaultSeries</AcceptableSeries>
                  <CustomProperties>
                    <item>
                      <key>
                        <string>HighlightRange</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>HighlightOpacity</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">0.2</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">0.2</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>StartTime</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:dateTime">2000-01-01T08:30:00</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>EndTime</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:dateTime">2000-01-01T08:45:00</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>TimeZoneSelection</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>Local</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>TZNote</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>Recommended that PC and 
NinjaTrader clocks match</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowOuterArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">true</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveUpperTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveUpper</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowLowerTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowLower</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveUpperColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>70</R>
                            <G>130</G>
                            <B>180</B>
                            <ScA>1</ScA>
                            <ScR>0.06124606</ScR>
                            <ScG>0.223227978</ScG>
                            <ScB>0.456411034</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowLowerColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>255</R>
                            <G>140</G>
                            <B>0</B>
                            <ScA>1</ScA>
                            <ScR>1</ScR>
                            <ScG>0.2622507</ScG>
                            <ScB>0</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowInnerArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveLowerTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveLower</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowUpperTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowUpper</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveLowerColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>144</R>
                            <G>238</G>
                            <B>144</B>
                            <ScA>1</ScA>
                            <ScR>0.2788943</ScR>
                            <ScG>0.8549926</ScG>
                            <ScB>0.2788943</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowUpperColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>255</R>
                            <G>192</G>
                            <B>203</B>
                            <ScA>1</ScA>
                            <ScR>1</ScR>
                            <ScG>0.527115166</ScG>
                            <ScB>0.5972018</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowMidlineArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveMidlineTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveMidline</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowMidlineTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowMidline</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveMidlineColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>0</R>
                            <G>255</G>
                            <B>255</B>
                            <ScA>1</ScA>
                            <ScR>0</ScR>
                            <ScG>1</ScG>
                            <ScB>1</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowMidlineColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>255</R>
                            <G>0</G>
                            <B>255</B>
                            <ScA>1</ScA>
                            <ScR>1</ScR>
                            <ScG>0</ScG>
                            <ScB>1</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowUpperQuarterArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveUpperQuarterTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveUpperQuarter</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowUpperQuarterTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowUpperQuarter</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveUpperQuarterColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>0</R>
                            <G>255</G>
                            <B>255</B>
                            <ScA>1</ScA>
                            <ScR>0</ScR>
                            <ScG>1</ScG>
                            <ScB>1</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowUpperQuarterColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>220</R>
                            <G>20</G>
                            <B>60</B>
                            <ScA>1</ScA>
                            <ScR>0.715693533</ScR>
                            <ScG>0.00699541066</ScG>
                            <ScB>0.045186203</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowLowerQuarterArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveLowerQuarterTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveLowerQuarter</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowLowerQuarterTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowLowerQuarter</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveLowerQuarterColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>0</R>
                            <G>255</G>
                            <B>0</B>
                            <ScA>1</ScA>
                            <ScR>0</ScR>
                            <ScG>1</ScG>
                            <ScB>0</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowLowerQuarterColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>255</R>
                            <G>69</G>
                            <B>0</B>
                            <ScA>1</ScA>
                            <ScR>1</ScR>
                            <ScG>0.0595112443</ScG>
                            <ScB>0</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowTarget1UpperArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveTarget1UpperTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveTarget1Upper</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowTarget1UpperTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowTarget1Upper</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveTarget1UpperColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>30</R>
                            <G>144</G>
                            <B>255</B>
                            <ScA>1</ScA>
                            <ScR>0.0129830325</ScR>
                            <ScG>0.2788943</ScG>
                            <ScB>1</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowTarget1UpperColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>255</R>
                            <G>99</G>
                            <B>71</B>
                            <ScA>1</ScA>
                            <ScR>1</ScR>
                            <ScG>0.124771819</ScG>
                            <ScB>0.06301002</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowTarget1LowerArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowTarget1LowerTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowTarget1Lower</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveTarget1LowerTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveTarget1Lower</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowTarget1LowerColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>255</R>
                            <G>99</G>
                            <B>71</B>
                            <ScA>1</ScA>
                            <ScR>1</ScR>
                            <ScG>0.124771819</ScG>
                            <ScB>0.06301002</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveTarget1LowerColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>30</R>
                            <G>144</G>
                            <B>255</B>
                            <ScA>1</ScA>
                            <ScR>0.0129830325</ScR>
                            <ScG>0.2788943</ScG>
                            <ScB>1</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowTarget2UpperArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveTarget2UpperTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveTarget2Upper</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowTarget2UpperTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowTarget2Upper</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveTarget2UpperColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>0</R>
                            <G>191</G>
                            <B>255</B>
                            <ScA>1</ScA>
                            <ScR>0</ScR>
                            <ScG>0.5209956</ScG>
                            <ScB>1</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowTarget2UpperColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>139</R>
                            <G>0</G>
                            <B>0</B>
                            <ScA>1</ScA>
                            <ScR>0.258182883</ScR>
                            <ScG>0</ScG>
                            <ScB>0</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowTarget2LowerArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveTarget2LowerTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>AboveTarget2Lower</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowTarget2LowerTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>BelowTarget2Lower</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>AboveTarget2LowerColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>0</R>
                            <G>191</G>
                            <B>255</B>
                            <ScA>1</ScA>
                            <ScR>0</ScR>
                            <ScG>0.5209956</ScG>
                            <ScB>1</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>BelowTarget2LowerColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>139</R>
                            <G>0</G>
                            <B>0</B>
                            <ScA>1</ScA>
                            <ScR>0.258182883</ScR>
                            <ScG>0</ScG>
                            <ScB>0</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowDayStartArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowRangeStartArrows</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SessionStartUpTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>SessionStartUp</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SessionStartDownTag</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>SessionStartDown</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SessionStartUpColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>144</R>
                            <G>238</G>
                            <B>144</B>
                            <ScA>1</ScA>
                            <ScR>0.2788943</ScR>
                            <ScG>0.8549926</ScG>
                            <ScB>0.2788943</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>SessionStartDownColor</string>
                      </key>
                      <value>
                        <anyType xsi:type="SolidColorBrush">
                          <Opacity>1</Opacity>
                          <Transform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </Transform>
                          <RelativeTransform xsi:type="MatrixTransform">
                            <Matrix>
                              <M11>1</M11>
                              <M12>0</M12>
                              <M21>0</M21>
                              <M22>1</M22>
                              <OffsetX>0</OffsetX>
                              <OffsetY>0</OffsetY>
                            </Matrix>
                          </RelativeTransform>
                          <Color>
                            <A>255</A>
                            <R>255</R>
                            <G>192</G>
                            <B>203</B>
                            <ScA>1</ScA>
                            <ScR>1</ScR>
                            <ScG>0.527115166</ScG>
                            <ScB>0.5972018</ScB>
                          </Color>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowMid</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>MiddleRangeMultiplier</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">0.5</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">0.5</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowQuarters</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>QuarterRangeMultiplier</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">0.25</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">0.25</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowTarget1</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Target1Multiplier</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">1</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">1</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowTarget2</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Target2Multiplier</string>
                      </key>
                      <value>
                        <anyType xsi:type="NumberBuilder">
                          <DefaultValue>0</DefaultValue>
                          <IsInt>false</IsInt>
                          <BindingValue xsi:type="xsd:string">2</BindingValue>
                          <IsLiteral>true</IsLiteral>
                          <LiveValue xsi:type="xsd:string">2</LiveValue>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>ShowSocials</string>
                      </key>
                      <value>
                        <anyType xsi:type="xsd:boolean">false</anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Author</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>TradeSaber - Built With Grok</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Version</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>Version 1.3 // April 2025</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>TradeSaber</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>https://tradesaber.com/predator-guide/</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>Discord</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>https://Discord.gg/2YU9GDme8j</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                    <item>
                      <key>
                        <string>YouTube</string>
                      </key>
                      <value>
                        <anyType xsi:type="NSStringBuilder">
                          <SeparatorCharacter> </SeparatorCharacter>
                          <Strings>
                            <NinjaScriptString>
                              <Index>0</Index>
                              <StringValue>https://youtu.be/jUYT-Erzc_8</StringValue>
                            </NinjaScriptString>
                          </Strings>
                        </anyType>
                      </value>
                    </item>
                  </CustomProperties>
                  <IndicatorHolder>
                    <IndicatorName>ORB_TradeSaber</IndicatorName>
                    <Plots>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF4682B4&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORHigh</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFF8C00&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORLow</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#00FFFFFF&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>Signal</Name>
                        <PlotStyle>Dot</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF808080&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORMiddle</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFADD8E6&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORTarget1</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF00FFFF&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORTarget2</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFFA07A&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORTargetMinus1</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FFFA8072&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORTargetMinus2</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF696969&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORLowerTargetQuarter</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                      <Plot>
                        <IsOpacityVisible>false</IsOpacityVisible>
                        <BrushSerialize>&lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"&gt;#FF696969&lt;/SolidColorBrush&gt;</BrushSerialize>
                        <DashStyleHelper>Solid</DashStyleHelper>
                        <Opacity>100</Opacity>
                        <Width>1</Width>
                        <AutoWidth>false</AutoWidth>
                        <Max>1.7976931348623157E+308</Max>
                        <Min>-1.7976931348623157E+308</Min>
                        <Name>ORUpperTargetQuarter</Name>
                        <PlotStyle>Line</PlotStyle>
                      </Plot>
                    </Plots>
                  </IndicatorHolder>
                  <IsExplicitlyNamed>false</IsExplicitlyNamed>
                  <IsPriceTypeLocked>false</IsPriceTypeLocked>
                  <PlotOnChart>false</PlotOnChart>
                  <PriceType>Close</PriceType>
                  <SeriesType>Indicator</SeriesType>
                  <SelectedPlot>Signal</SelectedPlot>
                </AssociatedIndicator>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-05-17T23:30:51.6526396</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <Period>0</Period>
                <ReturnType>Series</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </LeftItem>
              <Lookback>1</Lookback>
              <Operator>Equals</Operator>
              <RightItem xsi:type="WizardConditionItem">
                <Children />
                <IsExpanded>false</IsExpanded>
                <IsSelected>true</IsSelected>
                <Name>Numeric value</Name>
                <OffsetType>Arithmetic</OffsetType>
                <AssignedCommand>
                  <Command>{0}</Command>
                  <Parameters>
                    <string>NumericValue</string>
                  </Parameters>
                </AssignedCommand>
                <BarsAgo>0</BarsAgo>
                <CurrencyType>Currency</CurrencyType>
                <Date>2025-05-17T23:31:49.7150294</Date>
                <DayOfWeek>Sunday</DayOfWeek>
                <EndBar>0</EndBar>
                <ForceSeriesIndex>false</ForceSeriesIndex>
                <LookBackPeriod>0</LookBackPeriod>
                <MarketPosition>Long</MarketPosition>
                <NumericValue>
                  <DefaultValue>0</DefaultValue>
                  <IsInt>false</IsInt>
                  <BindingValue xsi:type="xsd:string">-1</BindingValue>
                  <IsLiteral>true</IsLiteral>
                  <LiveValue xsi:type="xsd:string">-1</LiveValue>
                </NumericValue>
                <Period>0</Period>
                <ReturnType>Number</ReturnType>
                <StartBar>0</StartBar>
                <State>Undefined</State>
                <Time>0001-01-01T00:00:00</Time>
              </RightItem>
            </WizardCondition>
          </Conditions>
          <IsGroup>false</IsGroup>
          <DisplayName>ORB_TradeSaber(true, 0.2, DateTime.Parse("8:30 AM"), DateTime.Parse("8:45 AM"), @"Local", @"Recommended that PC and 
NinjaTrader clocks match", true, @"AboveUpper", @"BelowLower", Brushes.SteelBlue, Brushes.DarkOrange, false, @"AboveLower", @"BelowUpper", Brushes.LightGreen, Brushes.Pink, false, @"AboveMidline", @"BelowMidline", Brushes.Aqua, Brushes.Fuchsia, false, @"AboveUpperQuarter", @"BelowUpperQuarter", Brushes.Aqua, Brushes.Crimson, false, @"AboveLowerQuarter", @"BelowLowerQuarter", Brushes.Lime, Brushes.OrangeRed, false, @"AboveTarget1Upper", @"BelowTarget1Upper", Brushes.DodgerBlue, Brushes.Tomato, false, @"BelowTarget1Lower", @"AboveTarget1Lower", Brushes.Tomato, Brushes.DodgerBlue, false, @"AboveTarget2Upper", @"BelowTarget2Upper", Brushes.DeepSkyBlue, Brushes.DarkRed, false, @"AboveTarget2Lower", @"BelowTarget2Lower", Brushes.DeepSkyBlue, Brushes.DarkRed, false, false, @"SessionStartUp", @"SessionStartDown", Brushes.LightGreen, Brushes.Pink, false, 0.5, false, 0.25, false, 1, false, 2, false, @"TradeSaber - Built With Grok", @"Version 1.3 // April 2025", @"https://tradesaber.com/predator-guide/", @"https://Discord.gg/2YU9GDme8j", @"https://youtu.be/jUYT-Erzc_8").Signal[0] = -1</DisplayName>
        </WizardConditionGroup>
      </Conditions>
      <SetName>Set 10</SetName>
      <SetNumber>10</SetNumber>
    </ConditionalAction>
  </ConditionalActions>
  <CustomSeries />
  <DataSeries />
  <Description>Enter the description for your new custom Strategy here.</Description>
  <DisplayInDataBox>true</DisplayInDataBox>
  <DrawHorizontalGridLines>true</DrawHorizontalGridLines>
  <DrawOnPricePanel>true</DrawOnPricePanel>
  <DrawVerticalGridLines>true</DrawVerticalGridLines>
  <EntriesPerDirection>2</EntriesPerDirection>
  <EntryHandling>AllEntries</EntryHandling>
  <ExitOnSessionClose>true</ExitOnSessionClose>
  <ExitOnSessionCloseSeconds>30</ExitOnSessionCloseSeconds>
  <FillLimitOrdersOnTouch>false</FillLimitOrdersOnTouch>
  <InputParameters>
    <InputParameter>
      <Default>14</Default>
      <Description />
      <Name>ATRPeriod</Name>
      <Minimum>1</Minimum>
      <Type>int</Type>
    </InputParameter>
    <InputParameter>
      <Default>1</Default>
      <Description />
      <Name>QTY1</Name>
      <Minimum>1</Minimum>
      <Type>int</Type>
    </InputParameter>
    <InputParameter>
      <Default>2</Default>
      <Description />
      <Name>QTY2</Name>
      <Minimum>1</Minimum>
      <Type>int</Type>
    </InputParameter>
    <InputParameter>
      <Default>1</Default>
      <Description />
      <Name>ProfitMultiplier1</Name>
      <Minimum>0.1</Minimum>
      <Type>double</Type>
    </InputParameter>
    <InputParameter>
      <Default>5</Default>
      <Description />
      <Name>ProfitMultiplier2</Name>
      <Minimum>0.1</Minimum>
      <Type>double</Type>
    </InputParameter>
    <InputParameter>
      <Default>2</Default>
      <Description />
      <Name>StopMultiplier</Name>
      <Minimum>0.1</Minimum>
      <Type>double</Type>
    </InputParameter>
    <InputParameter>
      <Default>0.7</Default>
      <Description />
      <Name>TrailTargetStart</Name>
      <Minimum>0.1</Minimum>
      <Type>double</Type>
    </InputParameter>
    <InputParameter>
      <Default>2.5</Default>
      <Description />
      <Name>TrailStopMultiplier</Name>
      <Minimum>0.1</Minimum>
      <Type>double</Type>
    </InputParameter>
    <InputParameter>
      <Default>08:45</Default>
      <Description />
      <Name>StartTime</Name>
      <Minimum />
      <Type>time</Type>
    </InputParameter>
    <InputParameter>
      <Default>10:00</Default>
      <Description />
      <Name>StopTime</Name>
      <Minimum />
      <Type>time</Type>
    </InputParameter>
    <InputParameter>
      <Default>23</Default>
      <Description />
      <Name>ADXPeriod</Name>
      <Minimum>1</Minimum>
      <Type>int</Type>
    </InputParameter>
    <InputParameter>
      <Default>2</Default>
      <Description />
      <Name>TickOffsetOrbHigh</Name>
      <Minimum>1</Minimum>
      <Type>int</Type>
    </InputParameter>
    <InputParameter>
      <Default>-2</Default>
      <Description />
      <Name>TicjOffsetOrbLow</Name>
      <Minimum>-100</Minimum>
      <Type>int</Type>
    </InputParameter>
  </InputParameters>
  <IsTradingHoursBreakLineVisible>true</IsTradingHoursBreakLineVisible>
  <IsInstantiatedOnEachOptimizationIteration>true</IsInstantiatedOnEachOptimizationIteration>
  <MaximumBarsLookBack>TwoHundredFiftySix</MaximumBarsLookBack>
  <MinimumBarsRequired>20</MinimumBarsRequired>
  <OrderFillResolution>Standard</OrderFillResolution>
  <OrderFillResolutionValue>1</OrderFillResolutionValue>
  <OrderFillResolutionType>Minute</OrderFillResolutionType>
  <OverlayOnPrice>false</OverlayOnPrice>
  <PaintPriceMarkers>true</PaintPriceMarkers>
  <PlotParameters />
  <RealTimeErrorHandling>StopCancelClose</RealTimeErrorHandling>
  <ScaleJustification>Right</ScaleJustification>
  <ScriptType>Strategy</ScriptType>
  <Slippage>0</Slippage>
  <StartBehavior>WaitUntilFlat</StartBehavior>
  <StopsAndTargets />
  <StopTargetHandling>PerEntryExecution</StopTargetHandling>
  <TimeInForce>Gtc</TimeInForce>
  <TraceOrders>false</TraceOrders>
  <UseOnAddTradeEvent>false</UseOnAddTradeEvent>
  <UseOnAuthorizeAccountEvent>false</UseOnAuthorizeAccountEvent>
  <UseAccountItemUpdate>false</UseAccountItemUpdate>
  <UseOnCalculatePerformanceValuesEvent>true</UseOnCalculatePerformanceValuesEvent>
  <UseOnConnectionEvent>false</UseOnConnectionEvent>
  <UseOnDataPointEvent>true</UseOnDataPointEvent>
  <UseOnFundamentalDataEvent>false</UseOnFundamentalDataEvent>
  <UseOnExecutionEvent>false</UseOnExecutionEvent>
  <UseOnMouseDown>true</UseOnMouseDown>
  <UseOnMouseMove>true</UseOnMouseMove>
  <UseOnMouseUp>true</UseOnMouseUp>
  <UseOnMarketDataEvent>false</UseOnMarketDataEvent>
  <UseOnMarketDepthEvent>false</UseOnMarketDepthEvent>
  <UseOnMergePerformanceMetricEvent>false</UseOnMergePerformanceMetricEvent>
  <UseOnNextDataPointEvent>true</UseOnNextDataPointEvent>
  <UseOnNextInstrumentEvent>true</UseOnNextInstrumentEvent>
  <UseOnOptimizeEvent>true</UseOnOptimizeEvent>
  <UseOnOrderUpdateEvent>false</UseOnOrderUpdateEvent>
  <UseOnPositionUpdateEvent>false</UseOnPositionUpdateEvent>
  <UseOnRenderEvent>true</UseOnRenderEvent>
  <UseOnRestoreValuesEvent>false</UseOnRestoreValuesEvent>
  <UseOnShareEvent>true</UseOnShareEvent>
  <UseOnWindowCreatedEvent>false</UseOnWindowCreatedEvent>
  <UseOnWindowDestroyedEvent>false</UseOnWindowDestroyedEvent>
  <Variables>
    <InputParameter>
      <Default>1</Default>
      <Name>StoreProfit1</Name>
      <Type>double</Type>
    </InputParameter>
    <InputParameter>
      <Default>1</Default>
      <Name>StoreProfit2</Name>
      <Type>double</Type>
    </InputParameter>
    <InputParameter>
      <Default>1</Default>
      <Name>StoreStop</Name>
      <Type>double</Type>
    </InputParameter>
    <InputParameter>
      <Default>false</Default>
      <Name>InitialEntry</Name>
      <Type>bool</Type>
    </InputParameter>
    <InputParameter>
      <Default>1</Default>
      <Name>StoreTrailTarget</Name>
      <Type>double</Type>
    </InputParameter>
    <InputParameter>
      <Default>1</Default>
      <Name>StoreTrailStop</Name>
      <Type>double</Type>
    </InputParameter>
    <InputParameter>
      <Default>false</Default>
      <Name>TriggerTrailStop</Name>
      <Type>bool</Type>
    </InputParameter>
    <InputParameter>
      <Default>false</Default>
      <Name>TrailStopUpdate</Name>
      <Type>bool</Type>
    </InputParameter>
    <InputParameter>
      <Default>1</Default>
      <Name>OrbHigh</Name>
      <Type>double</Type>
    </InputParameter>
    <InputParameter>
      <Default>1</Default>
      <Name>OrbLow</Name>
      <Type>double</Type>
    </InputParameter>
  </Variables>
  <Name>ORBATROrderManagement2</Name>
</ScriptProperties>
@*/
#endregion
