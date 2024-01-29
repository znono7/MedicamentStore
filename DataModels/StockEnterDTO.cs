namespace MedicamentStore
{
    public class StockEnterDTO
    {
        public int Id { get; set; }
        public int IdStock { get; set; }
        public int QuantiteAdded { get; set; }
        public int Quantite { get; set; }
        public int IdSupplie { get; set; }
        public string? Date { get; set; }
        public string? Nom_Commercial { get; set; }
        public string? Forme { get; set; }
        public string? Dosage { get; set; }
        public string? Conditionnement { get; set; }
        public string? Unite { get; set; }
        public double Prix { get; set; }
        public double PrixTotal { get; set; }
        public string? Status { get; set; }
        public string? PrimaryBackground { get; set; }


    }
}
