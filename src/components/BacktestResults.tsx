import React, { useState } from 'react';
import { TrendingUp, TrendingDown, BarChart3, Calendar, DollarSign, Percent, Target, AlertTriangle } from 'lucide-react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, AreaChart, Area, BarChart, Bar } from 'recharts';

const BacktestResults: React.FC = () => {
  const [selectedPeriod, setSelectedPeriod] = useState('6months');

  const equityCurveData = [
    { date: '2024-01', equity: 100000, drawdown: 0 },
    { date: '2024-02', equity: 102500, drawdown: -1.2 },
    { date: '2024-03', equity: 105700, drawdown: -0.8 },
    { date: '2024-04', equity: 103200, drawdown: -2.4 },
    { date: '2024-05', equity: 109800, drawdown: -0.5 },
    { date: '2024-06', equity: 113600, drawdown: -1.1 },
    { date: '2024-07', equity: 118100, drawdown: -0.3 },
    { date: '2024-08', equity: 115400, drawdown: -2.8 },
    { date: '2024-09', equity: 121200, drawdown: -0.7 },
    { date: '2024-10', equity: 124800, drawdown: -1.5 },
    { date: '2024-11', equity: 128300, drawdown: -0.9 },
    { date: '2024-12', equity: 132100, drawdown: -0.4 }
  ];

  const monthlyReturnsData = [
    { month: 'Jan', withConfluence: 2.5, withoutConfluence: 1.8, falseBreakouts: 12 },
    { month: 'Feb', withConfluence: 3.2, withoutConfluence: 2.1, falseBreakouts: 8 },
    { month: 'Mar', withConfluence: 1.8, withoutConfluence: 0.9, falseBreakouts: 15 },
    { month: 'Apr', withConfluence: 4.1, withoutConfluence: 2.8, falseBreakouts: 6 },
    { month: 'May', withConfluence: 2.9, withoutConfluence: 1.5, falseBreakouts: 9 },
    { month: 'Jun', withConfluence: 3.6, withoutConfluence: 2.3, falseBreakouts: 7 }
  ];

  const scenarios = [
    {
      name: 'Current Strategy',
      winRate: 65.2,
      avgWin: 145,
      avgLoss: -89,
      maxDrawdown: 8.2,
      sharpe: 1.45,
      totalReturn: 18.1,
      falseBreakoutRate: 32.5
    },
    {
      name: 'With Volume Filter',
      winRate: 71.8,
      avgWin: 152,
      avgLoss: -85,
      maxDrawdown: 6.8,
      sharpe: 1.72,
      totalReturn: 24.3,
      falseBreakoutRate: 21.2
    },
    {
      name: 'With All Confluences',
      winRate: 78.4,
      avgWin: 158,
      avgLoss: -78,
      maxDrawdown: 5.1,
      sharpe: 2.18,
      totalReturn: 32.1,
      falseBreakoutRate: 14.8
    }
  ];

  const tradeAnalysis = [
    { timeframe: '9:30-10:00', trades: 45, winRate: 58, avgReturn: 0.8 },
    { timeframe: '10:00-11:00', trades: 38, winRate: 72, avgReturn: 1.4 },
    { timeframe: '11:00-12:00', trades: 32, winRate: 78, avgReturn: 1.6 },
    { timeframe: '12:00-13:00', trades: 28, winRate: 65, avgReturn: 1.1 },
    { timeframe: '13:00-14:00', trades: 35, winRate: 74, avgReturn: 1.3 },
    { timeframe: '14:00-15:00', trades: 42, winRate: 68, avgReturn: 1.2 },
    { timeframe: '15:00-16:00', trades: 38, winRate: 62, avgReturn: 0.9 }
  ];

  return (
    <div className="space-y-6">
      {/* Header with Period Selection */}
      <div className="card">
        <div className="flex items-center justify-between mb-4">
          <div className="flex items-center space-x-3">
            <BarChart3 className="h-6 w-6 text-primary-600" />
            <h2 className="text-2xl font-bold text-gray-900">Backtest Results</h2>
          </div>
          <select 
            value={selectedPeriod}
            onChange={(e) => setSelectedPeriod(e.target.value)}
            className="border border-gray-300 rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
          >
            <option value="3months">Last 3 Months</option>
            <option value="6months">Last 6 Months</option>
            <option value="1year">Last 12 Months</option>
          </select>
        </div>
        <p className="text-gray-600">
          Comprehensive backtest analysis comparing strategy performance with and without confluence factors.
        </p>
      </div>

      {/* Scenario Comparison */}
      <div className="card">
        <h3 className="text-lg font-semibold text-gray-900 mb-6">Strategy Scenario Comparison</h3>
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead>
              <tr className="border-b border-gray-200">
                <th className="text-left py-3 px-4 font-medium text-gray-900">Scenario</th>
                <th className="text-right py-3 px-4 font-medium text-gray-900">Win Rate</th>
                <th className="text-right py-3 px-4 font-medium text-gray-900">Avg Win</th>
                <th className="text-right py-3 px-4 font-medium text-gray-900">Avg Loss</th>
                <th className="text-right py-3 px-4 font-medium text-gray-900">Max DD</th>
                <th className="text-right py-3 px-4 font-medium text-gray-900">Sharpe</th>
                <th className="text-right py-3 px-4 font-medium text-gray-900">Total Return</th>
                <th className="text-right py-3 px-4 font-medium text-gray-900">False Breakouts</th>
              </tr>
            </thead>
            <tbody>
              {scenarios.map((scenario, index) => (
                <tr key={index} className={`border-b border-gray-100 ${index === 2 ? 'bg-success-50' : ''}`}>
                  <td className="py-4 px-4">
                    <div className="flex items-center space-x-2">
                      <span className="font-medium text-gray-900">{scenario.name}</span>
                      {index === 2 && <Target className="h-4 w-4 text-success-600" />}
                    </div>
                  </td>
                  <td className="text-right py-4 px-4 font-medium">{scenario.winRate}%</td>
                  <td className="text-right py-4 px-4 text-success-600">${scenario.avgWin}</td>
                  <td className="text-right py-4 px-4 text-danger-600">${scenario.avgLoss}</td>
                  <td className="text-right py-4 px-4">{scenario.maxDrawdown}%</td>
                  <td className="text-right py-4 px-4">{scenario.sharpe}</td>
                  <td className="text-right py-4 px-4 font-medium text-success-600">{scenario.totalReturn}%</td>
                  <td className="text-right py-4 px-4">
                    <span className={`font-medium ${scenario.falseBreakoutRate < 20 ? 'text-success-600' : 'text-warning-600'}`}>
                      {scenario.falseBreakoutRate}%
                    </span>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {/* Equity Curve */}
      <div className="card">
        <h3 className="text-lg font-semibold text-gray-900 mb-6">Equity Curve & Drawdown</h3>
        <div className="h-80">
          <ResponsiveContainer width="100%" height="100%">
            <LineChart data={equityCurveData}>
              <CartesianGrid strokeDasharray="3 3" stroke="#f0f0f0" />
              <XAxis dataKey="date" stroke="#6b7280" />
              <YAxis yAxisId="equity" orientation="left" stroke="#6b7280" />
              <YAxis yAxisId="drawdown" orientation="right" stroke="#ef4444" />
              <Tooltip 
                contentStyle={{ 
                  backgroundColor: 'white', 
                  border: '1px solid #e5e7eb',
                  borderRadius: '8px',
                  boxShadow: '0 4px 6px -1px rgba(0, 0, 0, 0.1)'
                }}
                formatter={(value, name) => [
                  name === 'equity' ? `$${value.toLocaleString()}` : `${value}%`,
                  name === 'equity' ? 'Equity' : 'Drawdown'
                ]}
              />
              <Line 
                yAxisId="equity"
                type="monotone" 
                dataKey="equity" 
                stroke="#0ea5e9" 
                strokeWidth={3}
                dot={false}
              />
              <Line 
                yAxisId="drawdown"
                type="monotone" 
                dataKey="drawdown" 
                stroke="#ef4444" 
                strokeWidth={2}
                dot={false}
                strokeDasharray="5 5"
              />
            </LineChart>
          </ResponsiveContainer>
        </div>
      </div>

      {/* Monthly Performance Comparison */}
      <div className="card">
        <h3 className="text-lg font-semibold text-gray-900 mb-6">Monthly Returns: With vs Without Confluence</h3>
        <div className="h-80">
          <ResponsiveContainer width="100%" height="100%">
            <BarChart data={monthlyReturnsData}>
              <CartesianGrid strokeDasharray="3 3" stroke="#f0f0f0" />
              <XAxis dataKey="month" stroke="#6b7280" />
              <YAxis stroke="#6b7280" />
              <Tooltip 
                contentStyle={{ 
                  backgroundColor: 'white', 
                  border: '1px solid #e5e7eb',
                  borderRadius: '8px',
                  boxShadow: '0 4px 6px -1px rgba(0, 0, 0, 0.1)'
                }}
                formatter={(value, name) => [
                  `${value}%`,
                  name === 'withConfluence' ? 'With Confluence' : 'Without Confluence'
                ]}
              />
              <Bar dataKey="withoutConfluence" fill="#e5e7eb" name="withoutConfluence" />
              <Bar dataKey="withConfluence" fill="#22c55e" name="withConfluence" />
            </BarChart>
          </ResponsiveContainer>
        </div>
      </div>

      {/* Time-Based Analysis */}
      <div className="card">
        <h3 className="text-lg font-semibold text-gray-900 mb-6">Performance by Time of Day</h3>
        <div className="h-80">
          <ResponsiveContainer width="100%" height="100%">
            <AreaChart data={tradeAnalysis}>
              <CartesianGrid strokeDasharray="3 3" stroke="#f0f0f0" />
              <XAxis dataKey="timeframe" stroke="#6b7280" />
              <YAxis stroke="#6b7280" />
              <Tooltip 
                contentStyle={{ 
                  backgroundColor: 'white', 
                  border: '1px solid #e5e7eb',
                  borderRadius: '8px',
                  boxShadow: '0 4px 6px -1px rgba(0, 0, 0, 0.1)'
                }}
                formatter={(value, name) => [
                  name === 'winRate' ? `${value}%` : name === 'trades' ? `${value} trades` : `${value}%`,
                  name === 'winRate' ? 'Win Rate' : name === 'trades' ? 'Total Trades' : 'Avg Return'
                ]}
              />
              <Area 
                type="monotone" 
                dataKey="winRate" 
                stroke="#0ea5e9" 
                fill="#0ea5e9" 
                fillOpacity={0.3}
                name="winRate"
              />
            </AreaChart>
          </ResponsiveContainer>
        </div>
      </div>

      {/* Key Insights */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div className="card bg-gradient-to-r from-success-50 to-success-100 border-success-200">
          <div className="flex items-center space-x-3 mb-4">
            <TrendingUp className="h-6 w-6 text-success-600" />
            <h3 className="text-lg font-semibold text-success-900">Key Improvements</h3>
          </div>
          <div className="space-y-3">
            <div className="flex justify-between items-center">
              <span className="text-success-800">Win Rate Improvement</span>
              <span className="font-bold text-success-900">+13.2%</span>
            </div>
            <div className="flex justify-between items-center">
              <span className="text-success-800">False Breakout Reduction</span>
              <span className="font-bold text-success-900">-54.5%</span>
            </div>
            <div className="flex justify-between items-center">
              <span className="text-success-800">Sharpe Ratio Increase</span>
              <span className="font-bold text-success-900">+50.3%</span>
            </div>
            <div className="flex justify-between items-center">
              <span className="text-success-800">Max Drawdown Reduction</span>
              <span className="font-bold text-success-900">-37.8%</span>
            </div>
          </div>
        </div>

        <div className="card">
          <div className="flex items-center space-x-3 mb-4">
            <AlertTriangle className="h-6 w-6 text-warning-600" />
            <h3 className="text-lg font-semibold text-gray-900">Risk Considerations</h3>
          </div>
          <div className="space-y-3 text-sm text-gray-600">
            <p>• Confluence filters may reduce trade frequency by ~25%</p>
            <p>• Market regime changes could affect filter effectiveness</p>
            <p>• Regular recalibration of thresholds recommended</p>
            <p>• Consider position sizing adjustments during high volatility</p>
            <p>• Monitor correlation with broader market conditions</p>
          </div>
        </div>
      </div>

      {/* Implementation Impact */}
      <div className="card bg-gradient-to-r from-primary-50 to-primary-100 border-primary-200">
        <h3 className="text-lg font-semibold text-primary-900 mb-4">Expected Implementation Impact</h3>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div className="text-center">
            <div className="text-3xl font-bold text-primary-600 mb-2">+76%</div>
            <div className="text-sm text-primary-800">Return Improvement</div>
          </div>
          <div className="text-center">
            <div className="text-3xl font-bold text-success-600 mb-2">-55%</div>
            <div className="text-sm text-primary-800">False Breakouts</div>
          </div>
          <div className="text-center">
            <div className="text-3xl font-bold text-warning-600 mb-2">-25%</div>
            <div className="text-sm text-primary-800">Trade Frequency</div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default BacktestResults;