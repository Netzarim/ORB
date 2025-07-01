import React, { useState } from 'react';
import { TrendingUp, Shield, AlertTriangle, CheckCircle, BarChart3, Clock, Target, Zap } from 'lucide-react';
import ConfluenceAnalyzer from './components/ConfluenceAnalyzer';
import StrategyMetrics from './components/StrategyMetrics';
import ConfluenceRecommendations from './components/ConfluenceRecommendations';
import BacktestResults from './components/BacktestResults';

function App() {
  const [activeTab, setActiveTab] = useState('analyzer');

  const tabs = [
    { id: 'analyzer', label: 'Confluence Analyzer', icon: BarChart3 },
    { id: 'metrics', label: 'Strategy Metrics', icon: Target },
    { id: 'recommendations', label: 'Recommendations', icon: CheckCircle },
    { id: 'backtest', label: 'Backtest Results', icon: TrendingUp }
  ];

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-gray-100">
      {/* Header */}
      <header className="bg-white border-b border-gray-200 shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex items-center justify-between h-16">
            <div className="flex items-center space-x-3">
              <div className="bg-primary-600 p-2 rounded-lg">
                <Shield className="h-6 w-6 text-white" />
              </div>
              <div>
                <h1 className="text-xl font-bold text-gray-900">ORB Strategy Analyzer</h1>
                <p className="text-sm text-gray-500">False Breakout Reduction Tool</p>
              </div>
            </div>
            <div className="flex items-center space-x-2">
              <div className="flex items-center space-x-1 text-sm text-gray-600">
                <Zap className="h-4 w-4 text-warning-500" />
                <span>Live Analysis</span>
              </div>
            </div>
          </div>
        </div>
      </header>

      {/* Navigation */}
      <nav className="bg-white border-b border-gray-200">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex space-x-8">
            {tabs.map((tab) => {
              const Icon = tab.icon;
              return (
                <button
                  key={tab.id}
                  onClick={() => setActiveTab(tab.id)}
                  className={`flex items-center space-x-2 py-4 px-1 border-b-2 font-medium text-sm transition-colors duration-200 ${
                    activeTab === tab.id
                      ? 'border-primary-500 text-primary-600'
                      : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                  }`}
                >
                  <Icon className="h-4 w-4" />
                  <span>{tab.label}</span>
                </button>
              );
            })}
          </div>
        </div>
      </nav>

      {/* Main Content */}
      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {activeTab === 'analyzer' && <ConfluenceAnalyzer />}
        {activeTab === 'metrics' && <StrategyMetrics />}
        {activeTab === 'recommendations' && <ConfluenceRecommendations />}
        {activeTab === 'backtest' && <BacktestResults />}
      </main>
    </div>
  );
}

export default App;