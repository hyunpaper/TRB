import { useState } from "react";
import ChartAnalysisModal from "./ChartAnalysisModal";
import { LineChart, Line, XAxis, YAxis, Tooltip, ResponsiveContainer } from "recharts";
import LivePriceWidget from "./LivePriceWidget";
import { useAuth } from "../../adapters.presentation/hooks/AuthContext"; // ✅ 추가

type Candle = {
  candle_date_time_kst: string;
  trade_price: number;
};

interface CoinChartProps {
  market: string;
  candles: Candle[];
}

export default function CoinChart({ market, candles }: CoinChartProps) {
  const [showModal, setShowModal] = useState(false);
  const { isLoggedIn } = useAuth(); // ✅ 로그인 여부 확인

  const simplified = candles.map((c) => ({
    time: new Date(c.candle_date_time_kst).toLocaleTimeString(),
    price: c.trade_price,
  }));

  const handleShowModal = () => {
    if (!isLoggedIn) {
      alert("로그인 후 이용해 주세요.");
      return;
    }
    setShowModal(true);
  };

  return (
    <div className="p-4 border rounded shadow bg-white relative">
      <div className="flex justify-between items-center mb-2">
        <h3 className="text-sm font-bold">{market}</h3>
        <LivePriceWidget market={market} />
        <button
          onClick={handleShowModal} // ✅ 이걸로 대체
          className="text-xs text-blue-500 hover:underline"
        >
          상세 분석
        </button>
      </div>

      <ResponsiveContainer width="100%" height={200}>
        <LineChart data={simplified}>
          <XAxis dataKey="time" fontSize={10} />
          <YAxis fontSize={10} domain={["dataMin", "dataMax"]} />
          <Tooltip />
          <Line type="monotone" dataKey="price" stroke="#3b82f6" strokeWidth={2} dot={false} />
        </LineChart>
      </ResponsiveContainer>

      {showModal && (
        <ChartAnalysisModal market={market} onClose={() => setShowModal(false)} />
      )}
    </div>
  );
}
