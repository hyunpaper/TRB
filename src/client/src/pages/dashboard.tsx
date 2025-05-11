import { useEffect, useState } from "react";
import { getMinuteCandles } from "../api/upbit";
import CoinChart from "../components/CoinChart";
import LivePriceWidget from "../components/LivePriceWidget";
import AuthPage from "../pages/AuthPage";
import { priceSocketService } from "../services/PriceSocketService";
import { useAuth } from "../context/AuthContext";

const markets = [
  "KRW-BTC", "KRW-ETH", "KRW-XRP", "KRW-DOGE", "KRW-SOL",
  "KRW-ADA", "KRW-DOT", "KRW-AVAX", "KRW-LINK", "KRW-TRX"
];

export default function DashboardPage() {
  const [chartData, setChartData] = useState<Record<string, any[]>>({});
  const { isLoggedIn, email, role, logout } = useAuth();

  useEffect(() => {
    const fetchAll = async () => {
      const results: Record<string, any[]> = {};
      for (const market of markets) {
        try {
          const data = await getMinuteCandles(market);
          results[market] = data;
          await new Promise((res) => setTimeout(res, 200));
        } catch (err) {
          console.error(`${market} 불러오기 실패`, err);
        }
      }
      setChartData(results);
    };

    fetchAll();
  }, []);

  useEffect(() => {
    if (!priceSocketService.isReady()) {
      priceSocketService.connect(markets);
    }
  }, []);

  return (
    <div className="flex h-screen bg-gradient-to-br from-gray-50 via-gray-100 to-gray-200">
      <div className="flex-1 overflow-y-auto px-8 py-6">
        <h1 className="text-2xl font-bold text-gray-800 mb-4 tracking-tight">📊 코인 차트</h1>
        <div className="grid grid-cols-1 gap-5">
          {Object.entries(chartData).map(([market, candles]) => (
            <div
              key={market}
              className="bg-white p-5 rounded-xl shadow-md border border-gray-200 hover:shadow-lg transition"
            >
              <CoinChart market={market} candles={candles} />
            </div>
          ))}
        </div>
      </div>

      <div className="w-[360px] bg-white shadow-2xl border-l px-6 py-8 flex flex-col justify-between">
        {isLoggedIn ? (
          <div className="text-center space-y-4">
            <h2 className="text-2xl font-semibold">👋 환영합니다!</h2>
            <p className="text-gray-600 text-sm">
                {email}
                {role && <> · <span className="font-semibold">{role}</span></>}
            </p>
            <button
              onClick={logout}
              className="mt-4 w-full bg-red-500 hover:bg-red-600 text-white py-2 rounded"
            >
              로그아웃
            </button>
          </div>
        ) : (
          <div>
            <h2 className="text-2xl font-semibold text-center mb-2">🔐 로그인</h2>
            <p className="text-center text-gray-500 text-sm mb-6">TRB 서비스를 사용하려면 로그인하세요.</p>
            <div className="border-t border-gray-200 mb-4" />
            <AuthPage mode="panel" />
          </div>
        )}

        <footer className="text-center text-gray-400 text-xs mt-8">
          &copy; {new Date().getFullYear()} TRB. All rights reserved.
        </footer>
      </div>
    </div>
  );
}
