using Newtonsoft.Json;

namespace TRB.Server.Presentation.Models
{
    public class TickerData
    {
        [JsonProperty("market")]
        public string Market { get; set; } = "";

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty("trade_price")]
        public decimal TradePrice { get; set; }

        [JsonProperty("signed_change_rate")]
        public decimal SignedChangeRate { get; set; }

        [JsonProperty("high_price")]
        public decimal HighPrice { get; set; }

        [JsonProperty("low_price")]
        public decimal LowPrice { get; set; }

        [JsonProperty("acc_bid_volume")]
        public decimal AccBidVolume { get; set; }

        [JsonProperty("acc_ask_volume")]
        public decimal AccAskVolume { get; set; }

        [JsonProperty("acc_trade_volume_24h")]
        public decimal AccTradeVolume24h { get; set; }

        [JsonProperty("trade_volume")]
        public decimal TradeVolume { get; set; }
    }
}
