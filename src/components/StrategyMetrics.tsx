import React from 'react';
import { TrendingUp, TrendingDown, DollarSign, Percent, Target, Shield, Clock, BarChart3 } from 'lucide-react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, BarChart, Bar } from 'recharts';

const StrategyMetrics: React.FC = () => {
  const performanceData = [
    { month: 'Jan', pnl: 2500, trades: 45, winRate: 68 },
    { month: 'Feb', pnl: 3200, trades: 52, winRate: 72 },
    { month: 'Mar', pnl: 1800, trades: 38, winRate: 65 },
    { month: 'Apr', pnl: 4100, trades: 61, winRate: 75 },
    { month: 'May', pnl: 2900, trades: 47, winRate: 70 },
    { month: 'Jun', pnl: 3600, trades: 55, winRate: 73 }
  ];

  const falseBreakoutData = [
    { timeframe: '9:30-10:00', falseBreakouts: 35, totalBreakouts: 120 },
    { timeframe: '10:00-11:00', falseBreakouts: 18, totalBreakouts: 95 },
    { timeframe: '11:00-12:00', falseBreakouts: 12, totalBreakouts: 78 },
    { timeframe: '12:00-13:00', falseBreakouts: 22, totalBreakouts: 65 },
    { timeframe: '13:00-14:00', falseBreakouts: 15, totalBreakouts: 88 },
    { timeframe: '14:00-15:00', falseBreakouts: 28, totalBreakouts: 102 },
    { timeframe: '15:00-16:00', falseBreakouts: 42, totalBreakouts: 135 }
  ];

  const metrics = [
    {
      title: 'Total P&L',
      value: '$18,100',
      change: '+12.5%',
      trend: 'up',
      icon: DollarSign,
      color: 'success'
    },
    {
      title: 'Win Rate',
      value: '70.5%',
      change: '+2.1%',
      trend: 'up',
      icon: Target,
      color: 'primary'
    },
    {
      title: 'False Breakout Rate',
      value: '24.8%',
      change: '-5.2%',
      trend: 'down',
      icon: Shield,
      color: 'warning'
    },
    {
      title: 'Avg Trade Duration',
      value: '2.3h',
      change: '-0.4h',
      trend: 'down',
      icon: Clock,
      color: 'primary'
    },
    {
      title: 'Max Drawdown',
      value: '8.2%',
      change: '-1.1%',
      trend: 'down',
      icon: TrendingDown,
      color: 'danger'
    },
    {
      title: 'Sharpe Ratio',
      value: '1.85',
      change: '+0.15',
      trend: 'up',
      icon: BarChart3,
      color: 'success'
    }
  ];

  const getColorClasses = (color: string) => {
    const colors = {
      success: 'bg-success-500 text-success-600',
      primary: 'bg-primary-500 text-primary-600',
      warning: 'bg-warning-500 text-warning-600',
      danger: 'bg-danger-500 text-danger-600'
    };
    return colors[color as keyof typeof colors] || colors.primary;
  };

  return (
    <div className="space-y-6">
      {/* Key Metrics Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {metrics.map((metric, index) => {
          const Icon = metric.icon;
          const colorClasses = getColorClasses(metric.color);
          
          return (
            <div key={index} className="metric-card">
              <div className="flex items-center justify-between mb-4">
                <div className={`p-2 rounded-lg ${colorClasses.split(' ')[0]} bg-opacity-10`}>
                  <Icon className={`h-6 w-6 ${colorClasses.split(' ')[1]}`} />
                </div>
                <div className={`flex items-center space-x-1 text-sm ${
                  metric.trend === 'up' ? 'text-success-600' : 'text-danger-600'
                }`}>
                  {metric.trend === 'up' ? (
                    <TrendingUp className="h-4 w-4" />
                  ) : (
                    <TrendingDown className="h-4 w-4" />
                  )}
                  <span>{metric.change}</span>
                </div>
              </div>
              <div>
                <div className="text-2xl font-bold text-gray-900 mb-1">{metric.value}</div>
                <div className="text-sm text-gray-600">{metric.title}</div>
              </div>
            </div>
          );
        })}
      </div>

      {/* Performance Chart */}
      <div className="card">
        <h3 className="text-lg font-semibold text-gray-900 mb-6">Monthly Performance</h3>
        <div className="h-80">
          <ResponsiveContainer width="100%" height="100%">
            <LineChart data={performanceData}>
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
              />
              <Line 
                type="monotone" 
                dataKey="pnl" 
                stroke="#0ea5e9" 
                strokeWidth={3}
                dot={{ fill: '#0ea5e9', strokeWidth: 2, r: 6 }}
                activeDot={{ r: 8, stroke: '#0ea5e9', strokeWidth: 2 }}
              />
            </LineChart>
          </ResponsiveContainer>
        </div>
      </div>

      {/* False Breakout Analysis */}
      <div className="card">
        <h3 className="text-lg font-semibold text-gray-900 mb-6">False Breakout Analysis by Time</h3>
        <div className="h-80">
          <ResponsiveContainer width="100%" height="100%">
            <BarChart data={falseBreakoutData}>
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
                  `${value} ${name === 'falseBreakouts' ? 'False' : 'Total'} Breakouts`,
                  name === 'falseBreakouts' ? 'False Breakouts' : 'Total Breakouts'
                ]}
              />
              <Bar dataKey="totalBreakouts" fill="#e5e7eb" name="totalBreakouts" />
              <Bar dataKey="falseBreakouts" fill="#ef4444" name="falseBreakouts" />
            </BarChart>
          </ResponsiveContainer>
        </div>
        <div className="mt-4 text-sm text-gray-600">
          <p>Red bars show false breakouts, gray bars show total breakouts for each time period.</p>
        </div>
      </div>

      {/* Strategy Statistics */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div className="card">
          <h3 className="text-lg font-semibold text-gray-900 mb-4">Trade Distribution</h3>
          <div className="space-y-4">
            <div className="flex justify-between items-center">
              <span className="text-gray-600">Winning Trades</span>
              <span className="font-medium text-success-600">203 (70.5%)</span>
            </div>
            <div className="flex justify-between items-center">
              <span className="text-gray-600">Losing Trades</span>
              <span className="font-medium text-danger-600">85 (29.5%)</span>
            </div>
            <div className="flex justify-between items-center">
              <span className="text-gray-600">Average Win</span>
              <span className="font-medium text-success-600">$145</span>
            </div>
            <div className="flex justify-between items-center">
              <span className="text-gray-600">Average Loss</span>
              <span className="font-medium text-danger-600">-$78</span>
            </div>
            <div className="flex justify-between items-center">
              <span className="text-gray-600">Profit Factor</span>
              <span className="font-medium">2.18</span>
            </div>
          </div>
        </div>

        <div className="card">
          <h3 className="text-lg font-semibold text-gray-900 mb-4">Risk Metrics</h3>
          <div className="space-y-4">
            <div className="flex justify-between items-center">
              <span className="text-gray-600">Max Consecutive Losses</span>
              <span className="font-medium">5</span>
            </div>
            <div className="flex justify-between items-center">
              <span className="text-gray-600">Max Consecutive Wins</span>
              <span className="font-medium">12</span>
            </div>
            <div className="flex justify-between items-center">
              <span className="text-gray-600">Largest Win</span>
              <span className="font-medium text-success-600">$485</span>
            </div>
            <div className="flex justify-between items-center">
              <span className="text-gray-600">Largest Loss</span>
              <span className="font-medium text-danger-600">-$195</span>
            </div>
            <div className="flex justify-between items-center">
              <span className="text-gray-600">Recovery Factor</span>
              <span className="font-medium">2.21</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default StrategyMetrics;