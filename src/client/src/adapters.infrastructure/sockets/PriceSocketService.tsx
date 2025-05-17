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
  private handlers: PriceHandler[] = [];

  isReady(): boolean {
    return this.socket !== null && this.socket.readyState === WebSocket.OPEN;
  }

  connect(markets: string[]) {
    if (this.socket || !markets.length) return;

    this.socket = new WebSocket("wss://api.upbit.com/websocket/v1");

    this.socket.onopen = () => {
      const subscribeMsg = [
        { ticket: "price-socket" },
        { type: "ticker", codes: markets }
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
          market: json.code, // socket에서 오는 건 `code`, 우리 앱에서는 `market`을 사용
        };
        this.handlers.forEach((h) => h(data));
      };
      reader.readAsText(blob);
    };

    this.socket.onerror = (err) => {
      console.error("소켓 오류:", err);
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
