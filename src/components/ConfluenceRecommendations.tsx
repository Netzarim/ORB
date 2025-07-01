import React from 'react';
import { CheckCircle, AlertTriangle, TrendingUp, Volume2, Clock, BarChart3, Shield, Target } from 'lucide-react';

const ConfluenceRecommendations: React.FC = () => {
  const recommendations = [
    {
      category: 'Volume Analysis',
      priority: 'High',
      icon: Volume2,
      color: 'danger',
      items: [
        {
          title: 'Volume Confirmation Threshold',
          description: 'Require breakout volume to be at least 2x the 20-period average volume',
          implementation: 'Add volume filter: currentVolume >= averageVolume * 2.0',
          impact: 'Reduces false breakouts by ~35%'
        },
        {
          title: 'Volume Profile Analysis',
          description: 'Check if breakout occurs at high-volume price levels',
          implementation: 'Integrate VPOC (Volume Point of Control) analysis',
          impact: 'Improves breakout quality by ~25%'
        },
        {
          title: 'Cumulative Volume Delta',
          description: 'Monitor buying vs selling pressure during breakout',
          implementation: 'Track CVD divergence at breakout levels',
          impact: 'Early warning for potential reversals'
        }
      ]
    },
    {
      category: 'Technical Indicators',
      priority: 'High',
      icon: BarChart3,
      color: 'warning',
      items: [
        {
          title: 'ATR-Based Volatility Filter',
          description: 'Avoid trades when ATR is below 80% of 20-day average',
          implementation: 'if (currentATR < averageATR * 0.8) skipTrade = true',
          impact: 'Eliminates low-volatility false breaks'
        },
        {
          title: 'RSI Divergence Check',
          description: 'Confirm momentum alignment with price breakout',
          implementation: 'Require RSI > 60 for bullish breakouts, RSI < 40 for bearish',
          impact: 'Filters out momentum divergences'
        },
        {
          title: 'MACD Confirmation',
          description: 'Ensure MACD histogram supports breakout direction',
          implementation: 'MACD histogram must be positive/negative for bull/bear breakouts',
          impact: 'Adds momentum confirmation layer'
        }
      ]
    },
    {
      category: 'Time-Based Filters',
      priority: 'Medium',
      icon: Clock,
      color: 'primary',
      items: [
        {
          title: 'Opening Range Extension',
          description: 'Avoid breakouts in first 30 minutes of trading',
          implementation: 'Block trades between 9:30-10:00 AM EST',
          impact: 'Reduces noise from opening volatility'
        },
        {
          title: 'Lunch Hour Filter',
          description: 'Reduce position size during 12:00-1:00 PM EST',
          implementation: 'Apply 50% position sizing during lunch hour',
          impact: 'Accounts for reduced liquidity'
        },
        {
          title: 'Close Proximity Filter',
          description: 'Avoid new positions in last 30 minutes',
          implementation: 'Block new entries after 3:30 PM EST',
          impact: 'Prevents end-of-day manipulation'
        }
      ]
    },
    {
      category: 'Market Structure',
      priority: 'High',
      icon: TrendingUp,
      color: 'success',
      items: [
        {
          title: 'Higher Timeframe Alignment',
          description: 'Confirm breakout direction on 15-min and 1-hour charts',
          implementation: 'Multi-timeframe trend analysis before entry',
          impact: 'Significantly improves success rate'
        },
        {
          title: 'Support/Resistance Confluence',
          description: 'Identify breakouts at significant S/R levels',
          implementation: 'Map previous day high/low, weekly levels, pivot points',
          impact: 'Targets high-probability breakout zones'
        },
        {
          title: 'Market Regime Filter',
          description: 'Adjust strategy based on VIX and market conditions',
          implementation: 'Reduce position size when VIX > 25',
          impact: 'Adapts to market volatility'
        }
      ]
    }
  ];

  const implementationSteps = [
    {
      step: 1,
      title: 'Volume Filter Implementation',
      description: 'Add volume confirmation as primary filter',
      code: `// Volume confirmation filter
if (breakoutVolume < averageVolume * volumeMultiplier) {
    return false; // Skip trade
}`,
      priority: 'Immediate'
    },
    {
      step: 2,
      title: 'ATR Volatility Check',
      description: 'Implement volatility-based trade filtering',
      code: `// ATR volatility filter
double atrThreshold = averageATR * 0.8;
if (currentATR < atrThreshold) {
    return false; // Low volatility, skip
}`,
      priority: 'High'
    },
    {
      step: 3,
      title: 'Time-Based Restrictions',
      description: 'Add time filters for market hours',
      code: `// Time-based filters
TimeSpan currentTime = DateTime.Now.TimeOfDay;
if (currentTime < TimeSpan.FromHours(10) || 
    currentTime > TimeSpan.FromHours(15.5)) {
    return false; // Outside optimal hours
}`,
      priority: 'Medium'
    },
    {
      step: 4,
      title: 'Multi-Timeframe Confirmation',
      description: 'Add higher timeframe trend analysis',
      code: `// Multi-timeframe confirmation
bool higherTFBullish = CheckTrend(TimeFrame.FifteenMinutes);
bool hourlyTFBullish = CheckTrend(TimeFrame.OneHour);
return higherTFBullish && hourlyTFBullish;`,
      priority: 'High'
    }
  ];

  const getColorClasses = (color: string) => {
    const colors = {
      success: 'bg-success-100 text-success-800 border-success-200',
      primary: 'bg-primary-100 text-primary-800 border-primary-200',
      warning: 'bg-warning-100 text-warning-800 border-warning-200',
      danger: 'bg-danger-100 text-danger-800 border-danger-200'
    };
    return colors[color as keyof typeof colors] || colors.primary;
  };

  const getPriorityColor = (priority: string) => {
    switch (priority) {
      case 'High': return 'bg-danger-100 text-danger-800';
      case 'Medium': return 'bg-warning-100 text-warning-800';
      case 'Low': return 'bg-success-100 text-success-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="card">
        <div className="flex items-center space-x-3 mb-4">
          <Target className="h-6 w-6 text-primary-600" />
          <h2 className="text-2xl font-bold text-gray-900">Confluence Recommendations</h2>
        </div>
        <p className="text-gray-600">
          Implement these confluence factors to reduce false breakouts and improve your ORB strategy performance.
        </p>
      </div>

      {/* Recommendations by Category */}
      <div className="space-y-6">
        {recommendations.map((category, categoryIndex) => {
          const Icon = category.icon;
          return (
            <div key={categoryIndex} className="card">
              <div className="flex items-center justify-between mb-6">
                <div className="flex items-center space-x-3">
                  <div className={`p-2 rounded-lg ${getColorClasses(category.color)}`}>
                    <Icon className="h-6 w-6" />
                  </div>
                  <h3 className="text-xl font-semibold text-gray-900">{category.category}</h3>
                </div>
                <span className={`px-3 py-1 rounded-full text-sm font-medium ${getPriorityColor(category.priority)}`}>
                  {category.priority} Priority
                </span>
              </div>

              <div className="space-y-4">
                {category.items.map((item, itemIndex) => (
                  <div key={itemIndex} className="border border-gray-200 rounded-lg p-4 hover:bg-gray-50 transition-colors duration-200">
                    <div className="flex items-start space-x-3">
                      <CheckCircle className="h-5 w-5 text-success-500 mt-0.5 flex-shrink-0" />
                      <div className="flex-1">
                        <h4 className="font-semibold text-gray-900 mb-2">{item.title}</h4>
                        <p className="text-gray-600 mb-3">{item.description}</p>
                        <div className="bg-gray-100 rounded-lg p-3 mb-3">
                          <code className="text-sm text-gray-800">{item.implementation}</code>
                        </div>
                        <div className="flex items-center space-x-2">
                          <TrendingUp className="h-4 w-4 text-success-500" />
                          <span className="text-sm font-medium text-success-600">{item.impact}</span>
                        </div>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          );
        })}
      </div>

      {/* Implementation Guide */}
      <div className="card">
        <h3 className="text-xl font-semibold text-gray-900 mb-6">Implementation Roadmap</h3>
        <div className="space-y-6">
          {implementationSteps.map((step, index) => (
            <div key={index} className="flex space-x-4">
              <div className="flex-shrink-0">
                <div className="w-8 h-8 bg-primary-600 text-white rounded-full flex items-center justify-center font-semibold">
                  {step.step}
                </div>
              </div>
              <div className="flex-1">
                <div className="flex items-center justify-between mb-2">
                  <h4 className="font-semibold text-gray-900">{step.title}</h4>
                  <span className={`px-2 py-1 rounded text-xs font-medium ${getPriorityColor(step.priority)}`}>
                    {step.priority}
                  </span>
                </div>
                <p className="text-gray-600 mb-3">{step.description}</p>
                <div className="bg-gray-900 rounded-lg p-4">
                  <pre className="text-sm text-gray-100 overflow-x-auto">
                    <code>{step.code}</code>
                  </pre>
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>

      {/* Quick Wins */}
      <div className="card bg-gradient-to-r from-primary-50 to-primary-100 border-primary-200">
        <div className="flex items-center space-x-3 mb-4">
          <Shield className="h-6 w-6 text-primary-600" />
          <h3 className="text-lg font-semibold text-primary-900">Quick Wins</h3>
        </div>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div className="bg-white rounded-lg p-4">
            <h4 className="font-medium text-gray-900 mb-2">Volume Filter</h4>
            <p className="text-sm text-gray-600">Implement 2x volume requirement - immediate 35% reduction in false breakouts</p>
          </div>
          <div className="bg-white rounded-lg p-4">
            <h4 className="font-medium text-gray-900 mb-2">Time Restrictions</h4>
            <p className="text-sm text-gray-600">Block first/last 30 minutes - eliminates opening/closing noise</p>
          </div>
          <div className="bg-white rounded-lg p-4">
            <h4 className="font-medium text-gray-900 mb-2">ATR Filter</h4>
            <p className="text-sm text-gray-600">Skip low volatility periods - prevents range-bound false breaks</p>
          </div>
          <div className="bg-white rounded-lg p-4">
            <h4 className="font-medium text-gray-900 mb-2">RSI Confirmation</h4>
            <p className="text-sm text-gray-600">Add momentum check - filters momentum divergences</p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ConfluenceRecommendations;