import { useEffect, useState } from "react";
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  Tooltip,
  ResponsiveContainer,
  BarChart,
  Bar,
} from "recharts";

const intervals = [
  { label: "1분봉", value: 1 },
  { label: "5분봉", value: 5 },
  { label: "30분봉", value: 30 },
  { label: "60분봉", value: 60 },
];

export default function ChartAnalysisModal({
  market,
  onClose,
}: {
  market: string;
  onClose: () => void;
}) {
  const [mode, setMode] = useState<"분석" | "시세">("분석");
  const [interval, setInterval] = useState(1);
  const [candles, setCandles] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  const fetchCandles = async () => {
    setLoading(true);
    try {
      const res = await fetch(
        `http://localhost:5186/api/upbit/candles?market=${market}&count=60&interval=${interval}`
      );
      if (!res.ok) throw new Error("API 호출 실패");
      const data = await res.json();
      setCandles(data.reverse());
    } catch (err) {
      console.error("데이터 가져오기 실패:", err);
    } finally {
      setLoading(false);
    }
  };

useEffect(() => {
  fetchCandles();
  if (mode === "시세") {
    const id = window.setInterval(() => {
      console.log("📦 자동 fetch 실행", new Date().toLocaleTimeString());
      fetchCandles();
    }, 60000);
    return () => clearInterval(id);
  }
}, [market, interval, mode]);

  const chartData = candles.map((c) => ({
    time: new Date(c.candle_date_time_kst).toLocaleTimeString(),
    price: c.trade_price,
    volume: c.candle_acc_trade_volume,
  }));

  const firstPrice = chartData[0]?.price;
  const lastPrice = chartData[chartData.length - 1]?.price;
  const diff =
    firstPrice && lastPrice ? (lastPrice - firstPrice).toFixed(2) : "-";

  return (
    <div className="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg w-[750px] max-h-[95vh] overflow-y-auto shadow-xl p-6">
        {/* 헤더 */}
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-lg font-semibold text-gray-800">
            {market} 상세 분석
          </h2>
          <button
            onClick={onClose}
            className="text-gray-500 hover:text-red-500 text-sm"
          >
            닫기 ✖
          </button>
        </div>

        {/* 모드 선택 */}
        <div className="mb-4 flex items-center gap-4">
          <label className="text-sm font-medium text-gray-600">모드 선택:</label>
          <select
            value={mode}
            onChange={(e) => setMode(e.target.value as "분석" | "시세")}
            className="border rounded px-2 py-1 text-sm"
          >
            <option value="분석">📊 분석</option>
            <option value="시세">⚡ 시세</option>
          </select>
        </div>

        {/* 분봉 선택 */}
        <div className="flex gap-2 mb-4">
          {intervals.map((item) => (
            <button
              key={item.value}
              onClick={() => setInterval(item.value)}
              className={`px-3 py-1 text-sm rounded border ${
                interval === item.value
                  ? "bg-blue-500 text-white border-blue-600"
                  : "text-gray-600 border-gray-300 hover:bg-gray-100"
              }`}
            >
              {item.label}
            </button>
          ))}
        </div>

        {loading ? (
          <div className="text-center text-gray-500 py-8">⏳ 데이터 로딩 중...</div>
        ) : (
          <>
            {/* 시세 차트 */}
            <ResponsiveContainer width="100%" height={200}>
              <LineChart data={chartData}>
                <XAxis dataKey="time" fontSize={10} />
                <YAxis fontSize={10} domain={["dataMin", "dataMax"]} />
                <Tooltip />
                <Line
                  type="monotone"
                  dataKey="price"
                  stroke="#3b82f6"
                  strokeWidth={2}
                  dot={false}
                />
              </LineChart>
            </ResponsiveContainer>

            {/* 거래량 차트 */}
            <div className="mt-6">
              <h4 className="text-sm font-medium text-gray-700 mb-1">📊 거래량</h4>
              <ResponsiveContainer width="100%" height={100}>
                <BarChart data={chartData}>
                  <XAxis dataKey="time" hide />
                  <YAxis fontSize={10} />
                  <Bar dataKey="volume" fill="#94a3b8" />
                </BarChart>
              </ResponsiveContainer>
            </div>

            {/* 요약 */}
            <div className="mt-6 border-t pt-4 text-sm text-gray-600">
              <p>
                🔍 최근 <strong>{interval}분봉</strong> 기준 가격 변화폭은{" "}
                <strong>{diff} KRW</strong> 입니다.
              </p>
            </div>
          </>
        )}
      </div>
    </div>
  );
}
