namespace MarketAssetPriceService.Enteties
{
    public class InstrumentResponse
    {
        public Paging Paging { get; set; }
        public List<InstrumentData> Data { get; set; }
    }
}
