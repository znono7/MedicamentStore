namespace MedicamentStore
{
    public class StockEnter
    {
        public int Id { get; set; }
        public int IdStock {  get; set; }
        public int QuantiteAdded { get; set;}
        public int Quantite { get; set; }
        public int IdSupplie { get; set; }
        public string? Date { get; set; }
    }
}
