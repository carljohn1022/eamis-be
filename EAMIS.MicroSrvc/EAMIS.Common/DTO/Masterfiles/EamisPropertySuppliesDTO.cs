namespace EAMIS.Common.DTO.Masterfiles
{
    public class EamisPropertySuppliesDTO
    {
        public int Id { get; set; }
        public int Parent_Id { get; set; }
        public string Supply_Description { get; set; }
        public decimal Unit_Cost { get; set; }
        public bool Is_Stockable { get; set; }
        public string Brand { get; set; }
        public string Model_No { get; set; }
    }
}
