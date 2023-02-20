namespace EAMIS.Core.Domain.Entities
{
    public class EAMISROLEMODULELINK
    {
        public int ID { get; set; }
        public int MODULE_ID { get; set; }
        public int ROLE_ID { get; set; }

        public bool VIEW_RIGHT { get; set; }
        public bool INSERT_RIGHT { get; set; }
        public bool UPDATE_RIGHT { get; set; }
        public bool DEACTIVATE_RIGHT { get; set; }
        public bool PRINT_RIGHT { get; set; }
        public bool IS_ACTIVE { get; set; }
        public int USER_ID { get; set; }

    }
}
