namespace EAMIS.Core.Domain.Entities
{
    public class EAMISMODULES
    {
        public int ID { get; set; }
        public string MODULE_NAME { get; set; }
        public bool IS_ACTIVE { get; set; }
        public string VIEW_RIGHT { get; set; }
        public string INSERT_RIGHT { get; set; }
        public string UPDATE_RIGHT { get; set; }
        public string DEACTIVATE_RIGHT { get; set; }
        public string PRINT_RIGHT { get; set; }
        public string ACTIVE { get; set; }
    }
}
