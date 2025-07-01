import React, { useState } from 'react';
import { TrendingUp, TrendingDown, Minus, AlertTriangle, CheckCircle, Clock, Volume2, BarChart3 } from 'lucide-react';

interface ConfluenceFactor {
  id: string;
  name: string;
  description: string;
  weight: number;
  status: 'bullish' | 'bearish' | 'neutral';
  strength: number;
  category: 'technical' | 'volume' | 'market' | 'time';
}

const ConfluenceAnalyzer: React.FC = () => {
  const [confluenceFactors, setConfluenceFactors] = useState<ConfluenceFactor[]>([
    {
      id: 'volume_confirmation',
      name: 'Volume Confirmation',
      description: 'Breakout volume vs. average volume (2x minimum)',
      weight: 25,
      status: 'bullish',
      strength: 85,
      category: 'volume'
    },
    {
      id: 'atr_filter',
      name: 'ATR Volatility Filter',
      description: 'Current ATR vs. 20-day average (prevents low volatility false breaks)',
      weight: 20,
      status: 'bullish',
      strength: 78,
      category: 'technical'
    },
    {
      id: 'market_structure',
      name: 'Market Structure',
      description: 'Higher highs/lower lows pattern confirmation',
      weight: 20,
      status: 'neutral',
      strength: 65,
      category: 'technical'
    },
    {
      id: 'time_filter',
      name: 'Time-Based Filter',
      description: 'Avoid breakouts in first/last 30 minutes',
      weight: 15,
      status: 'bullish',
      strength: 90,
      category: 'time'
    },
    {
      id: 'momentum_divergence',
      name: 'Momentum Divergence',
      description: 'RSI/MACD divergence check',
      weight: 10,
      status: 'bearish',
      strength: 45,
      category: 'technical'
    },
    {
      id: 'support_resistance',
      name: 'S/R Confluence',
      description: 'Previous support/resistance levels alignment',
      weight: 10,
      status: 'bullish',
      strength: 72,
      category: 'technical'
    }
  ]);

  const calculateOverallScore = () => {
    return confluenceFactors.reduce((total, factor) => {
      const multiplier = factor.status === 'bullish' ? 1 : factor.status === 'bearish' ? -0.5 : 0.3;
      return total + (factor.weight * factor.strength * multiplier) / 100;
    }, 0);
  };

  const overallScore = calculateOverallScore();
  const maxPossibleScore = confluenceFactors.reduce((total, factor) => total + factor.weight, 0);
  const scorePercentage = Math.max(0, (overallScore / maxPossibleScore) * 100);

  const getScoreColor = (score: number) => {
    if (score >= 70) return 'text-success-600';
    if (score >= 50) return 'text-warning-600';
    return 'text-danger-600';
  };

  const getStatusIcon = (status: string) => {
    switch (status) {
      case 'bullish': return <TrendingUp className="h-4 w-4 text-success-500" />;
      case 'bearish': return <TrendingDown className="h-4 w-4 text-danger-500" />;
      default: return <Minus className="h-4 w-4 text-gray-400" />;
    }
  };

  const getCategoryIcon = (category: string) => {
    switch (category) {
      case 'volume': return <Volume2 className="h-4 w-4" />;
      case 'technical': return <BarChart3 className="h-4 w-4" />;
      case 'time': return <Clock className="h-4 w-4" />;
      default: return <TrendingUp className="h-4 w-4" />;
    }
  };

  return (
    <div className="space-y-6">
      {/* Overall Score Card */}
      <div className="card">
        <div className="flex items-center justify-between mb-6">
          <h2 className="text-2xl font-bold text-gray-900">Confluence Analysis</h2>
          <div className="text-right">
            <div className={`text-3xl font-bold ${getScoreColor(scorePercentage)}`}>
              {scorePercentage.toFixed(1)}%
            </div>
            <div className="text-sm text-gray-500">Breakout Confidence</div>
          </div>
        </div>

        {/* Score Visualization */}
        <div className="mb-6">
          <div className="flex items-center justify-between mb-2">
            <span className="text-sm font-medium text-gray-700">Overall Confluence Score</span>
            <span className="text-sm text-gray-500">{overallScore.toFixed(1)} / {maxPossibleScore}</span>
          </div>
          <div className="w-full bg-gray-200 rounded-full h-3">
            <div 
              className={`h-3 rounded-full transition-all duration-500 ${
                scorePercentage >= 70 ? 'bg-success-500' : 
                scorePercentage >= 50 ? 'bg-warning-500' : 'bg-danger-500'
              }`}
              style={{ width: `${Math.min(100, scorePercentage)}%` }}
            />
          </div>
        </div>

        {/* Recommendation */}
        <div className={`p-4 rounded-lg border-l-4 ${
          scorePercentage >= 70 ? 'bg-success-50 border-success-400' :
          scorePercentage >= 50 ? 'bg-warning-50 border-warning-400' : 'bg-danger-50 border-danger-400'
        }`}>
          <div className="flex items-center space-x-2">
            {scorePercentage >= 70 ? (
              <CheckCircle className="h-5 w-5 text-success-600" />
            ) : (
              <AlertTriangle className="h-5 w-5 text-warning-600" />
            )}
            <span className="font-medium">
              {scorePercentage >= 70 ? 'High Confidence Breakout' :
               scorePercentage >= 50 ? 'Moderate Confidence - Use Caution' : 'Low Confidence - Avoid Trade'}
            </span>
          </div>
        </div>
      </div>

      {/* Confluence Factors */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {confluenceFactors.map((factor) => (
          <div key={factor.id} className="card hover:shadow-md transition-shadow duration-200">
            <div className="flex items-start justify-between mb-4">
              <div className="flex items-center space-x-3">
                {getCategoryIcon(factor.category)}
                <div>
                  <h3 className="font-semibold text-gray-900">{factor.name}</h3>
                  <p className="text-sm text-gray-600">{factor.description}</p>
                </div>
              </div>
              {getStatusIcon(factor.status)}
            </div>

            <div className="space-y-3">
              <div className="flex items-center justify-between">
                <span className="text-sm text-gray-600">Weight</span>
                <span className="font-medium">{factor.weight}%</span>
              </div>
              
              <div className="flex items-center justify-between">
                <span className="text-sm text-gray-600">Strength</span>
                <span className="font-medium">{factor.strength}%</span>
              </div>

              <div className="w-full bg-gray-200 rounded-full h-2">
                <div 
                  className={`h-2 rounded-full ${
                    factor.status === 'bullish' ? 'bg-success-500' :
                    factor.status === 'bearish' ? 'bg-danger-500' : 'bg-gray-400'
                  }`}
                  style={{ width: `${factor.strength}%` }}
                />
              </div>

              <div className="text-xs text-gray-500 capitalize">
                Category: {factor.category}
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* Additional Filters Section */}
      <div className="card">
        <h3 className="text-lg font-semibold text-gray-900 mb-4">Additional False Breakout Filters</h3>
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          <div className="p-4 bg-gray-50 rounded-lg">
            <h4 className="font-medium text-gray-900 mb-2">News Filter</h4>
            <p className="text-sm text-gray-600">Avoid breakouts during high-impact news events</p>
          </div>
          <div className="p-4 bg-gray-50 rounded-lg">
            <h4 className="font-medium text-gray-900 mb-2">Gap Analysis</h4>
            <p className="text-sm text-gray-600">Consider overnight gaps in breakout validation</p>
          </div>
          <div className="p-4 bg-gray-50 rounded-lg">
            <h4 className="font-medium text-gray-900 mb-2">Multi-Timeframe</h4>
            <p className="text-sm text-gray-600">Confirm breakout on higher timeframes</p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ConfluenceAnalyzer;