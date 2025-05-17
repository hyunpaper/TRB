// src/components/LivePriceWidget.tsx
import { useEffect, useRef, useState } from "react";
import { priceSocketService } from "../../adapters.infrastructure/sockets/PriceSocketService";
import type { TickerData } from "../../adapters.infrastructure/sockets/PriceSocketService";
import {
  LineChart, Line, XAxis, YAxis, Tooltip, ResponsiveContainer,
} from "recharts";

interface LivePriceWidgetProps {
  market: string;
}

function getChangeColor(rate: number | null): string {
  if (rate === null || isNaN(rate)) return "text-gray-500";

  if (rate > 0.01) return "text-red-600";     // 1% 이상 상승
  if (rate > 0) return "text-red-400";        // 0~1% 상승

  if (rate < -0.01) return "text-blue-600";   // -1% 이상 하락
  if (rate < 0) return "text-blue-400";       // 0~-1% 하락

  return "text-gray-500";                     // 변화 없음
}

function getLineColor(rate: number | null): string {
  if (rate === null || isNaN(rate)) return "#9ca3af"; // gray-400

  if (rate > 0.01) return "#dc2626";   // red-600
  if (rate > 0) return "#f87171";      // red-400
  if (rate < -0.01) return "#2563eb";  // blue-600
  if (rate < 0) return "#60a5fa";      // blue-400

  return "#9ca3af"; // gray-400
}

export default function LivePriceWidget({ market }: LivePriceWidgetProps) {
    const [price, setPrice] = useState<number | null>(null);
    const [changeRate, setChangeRate] = useState<number | null>(null);
    const [changeColor, setChangeColor] = useState("text-gray-600");
    const [flash, setFlash] = useState<"up" | "down" | null>(null);
    const [history, setHistory] = useState<{ time: string; price: number }[]>([]);
    const [targetPrice, setTargetPrice] = useState<number | null>(null);
    const [targetActive, setTargetActive] = useState(false);
    const timerRef = useRef<number | null>(null);
    const alertedRef = useRef(false); // ✅ 중복 알림 방지
    const [lineColor, setLineColor] = useState("#0ea5e9"); // 기본값
    const [highPrice, setHighPrice] = useState<number | null>(null);
    const [lowPrice, setLowPrice] = useState<number | null>(null);

    
  useEffect(() => {
    const handlePrice = (data: TickerData) => {
        if (data.market !== market) return;

        if (price !== null && data.trade_price !== price) {
            setFlash(data.trade_price > price ? "up" : "down");
            if (timerRef.current) clearTimeout(timerRef.current);
            timerRef.current = window.setTimeout(() => setFlash(null), 500);
        }
                
        setHighPrice(data.high_price ?? null);
        setLowPrice(data.low_price ?? null);

        setPrice(data.trade_price);
        setChangeRate(data.signed_change_rate);

        // ✅ 콘솔 로그로 확인
        console.log(`[${market}] 변동률: ${(data.signed_change_rate * 100).toFixed(2)}%`);

        // ✅ 색상 결정
        setChangeColor(getChangeColor(data.signed_change_rate));
        setLineColor(getLineColor(data.signed_change_rate));

        const now = new Date();
        const time = now.toLocaleTimeString("ko-KR", { hour12: false });
        setHistory((prev) => {
            const updated = [...prev, { time, price: data.trade_price }];
            return updated.length > 20 ? updated.slice(-20) : updated;
        });

        if (targetActive && targetPrice && data.trade_price >= targetPrice) {
            alert(`${market} 목표가 도달: ${data.trade_price.toLocaleString()} KRW 🎯`);
            setTargetActive(false);
        }
        };


    priceSocketService.on("price", handlePrice);
    return () => {
      priceSocketService.off("price", handlePrice);
    };
  }, [market, price, targetActive, targetPrice]);

  return (
    <div className="text-sm">
      <div className="flex flex-wrap items-center gap-4 mb-1">
        <div className="flex items-center gap-2">
          <span className="font-semibold">실시간 시세:</span>
          {price !== null ? (
            <span
              className={`font-bold transition duration-200 ${changeColor} ${
                flash === "up"
                  ? "animate-pulse bg-red-100 px-1 rounded"
                  : flash === "down"
                  ? "animate-pulse bg-blue-100 px-1 rounded"
                  : ""
              }`}
            >
              {price.toLocaleString()} KRW ({(changeRate! * 100).toFixed(2)}%)
            </span>
          ) : (
            <span className="text-gray-400 ml-1">-</span>
          )}
        </div>
        
        {/* 고/저점 */}
        <div className="flex flex-col gap-1 text-xs text-gray-600 mt-1">
            {highPrice !== null && (
                <div>
                📈 고점: <span className="text-red-600 font-semibold">
                    {highPrice.toLocaleString()} KRW
                </span>
                </div>
            )}
            {lowPrice !== null && (
                <div>
                📉 저점: <span className="text-blue-600 font-semibold">
                    {lowPrice.toLocaleString()} KRW
                </span>
                </div>
            )}
        </div>

        {/* 목표가 설정 */}
        <div className="flex items-center gap-1 ml-4">
          <input
            type="text"
            inputMode="numeric"
            placeholder="목표가"
            className="border px-1 py-0.5 rounded text-xs w-24 text-right"
            value={targetPrice !== null ? targetPrice.toLocaleString() : ""}
            onChange={(e) => {
            const numeric = Number(e.target.value.replace(/,/g, ""));
            if (!isNaN(numeric)) setTargetPrice(numeric);
            alertedRef.current = false; // ✅ 다시 활성화 가능
            }}
          />
          <button
            onClick={() => setTargetActive(true)}
            className="text-xs text-blue-600 border border-blue-500 px-2 py-0.5 rounded hover:bg-blue-50"
          >
            적용
          </button>
        </div>
      </div>

      {history.length > 1 && (
        <div className="mt-2 h-16">
          <ResponsiveContainer width="100%" height="100%">
            <LineChart data={history}>
              <XAxis dataKey="time" hide />
              <YAxis domain={["dataMin", "dataMax"]} hide />
              <Tooltip />
                <Line
                    type="monotone"
                    dataKey="price"
                    stroke={lineColor} // ✅ 실시간 색상 적용!
                    strokeWidth={2}
                    dot={false}
                    isAnimationActive={false}
                />
            </LineChart>
          </ResponsiveContainer>
        </div>
      )}
    </div>
  );
}
