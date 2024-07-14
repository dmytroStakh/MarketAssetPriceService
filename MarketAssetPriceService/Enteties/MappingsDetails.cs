namespace MarketAssetPriceService.Enteties
{
    public class MappingsDetails
    {
        public Guid Id { get; set; }
        public string Provider { get; set; }
        public string Symbol { get; set; }
        public string Exchange { get; set; }
        public int DefaultOrderSize { get; set; }
        public Guid InstrumentId { get; set; }
        public Instrument Instrument { get; set; }
    }
}
