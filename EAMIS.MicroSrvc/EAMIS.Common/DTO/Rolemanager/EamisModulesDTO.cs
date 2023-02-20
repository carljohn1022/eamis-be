namespace EAMIS.Common.DTO.Rolemanager
{
    public class EamisModulesDTO
    {
        public int Id { get; set; }
        public string ModuleName { get; set; }
        public bool IsActive { get; set; }
        public string ViewRight { get; set; }
        public string InsertRight { get; set; }
        public string UpdateRight { get; set; }
        public string DeactivateRight { get; set; }
        public string PrintRight { get; set; }
        public string Active { get; set; }
    }
}
