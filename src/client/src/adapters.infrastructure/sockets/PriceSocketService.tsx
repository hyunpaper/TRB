// src/services/PriceSocketService.ts
export interface TickerData {
  type: string;
  code: string;
  trade_price: number;
  signed_change_rate: number;
  market: string; 
  high_price: number; 
  low_price: number; 

}

type PriceHandler = (data: TickerData) => void;

class PriceSocketService {
  private socket: WebSocket | null = null;
  private backendSocket: WebSocket | null = null;
  private handlers: PriceHandler[] = [];

  isReady(): boolean {
    return this.socket !== null && this.socket.readyState === WebSocket.OPEN;
  }

  connect(markets: string[]) {
  if (this.socket || !markets.length) return;

  // 업비트 WebSocket 연결
  this.socket = new WebSocket("wss://api.upbit.com/websocket/v1");

  // 백엔드 WebSocket 연결
  const backendSocket = new WebSocket("ws://localhost:5186/ws/price");

  this.socket.onopen = () => {
    const subscribeMsg = [
      { ticket: "price-socket" },
      { type: "ticker", codes: markets },
    ];
    this.socket?.send(JSON.stringify(subscribeMsg));
  };

  this.socket.onmessage = (event) => {
    const blob = event.data;
    const reader = new FileReader();
    reader.onload = () => {
      const text = reader.result as string;
      const json = JSON.parse(text);
      const data: TickerData = {
        ...json,
        market: json.code, // code → market 으로 통일
      };

      // 프론트 핸들러에 전달
      this.handlers.forEach((h) => h(data));

      // 백엔드 WebSocket으로 전달
      if (backendSocket.readyState === WebSocket.OPEN) {
        backendSocket.send(JSON.stringify(data));
      } else {
        console.warn("Error : 백엔드 WebSocket이 아직 열려있지 않습니다.");
      }
    };
    reader.readAsText(blob);
  };

  this.socket.onerror = (err) => {
    console.error("Error : 업비트 WebSocket 오류:", err);
  };

  backendSocket.onerror = (err) => {
    console.error("Error : 백엔드 WebSocket 오류:", err);
  };
}


  on(event: "price", handler: PriceHandler) {
    if (event === "price") this.handlers.push(handler);
  }

  off(event: "price", handler: PriceHandler) {
    if (event === "price") {
      this.handlers = this.handlers.filter((h) => h !== handler);
    }
  }
}

export const priceSocketService = new PriceSocketService();
