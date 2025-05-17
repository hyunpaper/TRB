export const getMinuteCandles = async (
  market: string,
  count: number = 30
): Promise<any[]> => {
  const response = await fetch(
    `http://localhost:5186/api/upbit/candles?market=${market}&count=${count}`
  );

  if (!response.ok) {
    throw new Error(`업비트 API 호출 실패: ${response.status}`);
  }

  const data = await response.json();
  return data.reverse(); // 시간순 정렬
};
