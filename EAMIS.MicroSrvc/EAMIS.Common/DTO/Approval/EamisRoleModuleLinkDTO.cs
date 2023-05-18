using EAMIS.Common.DTO.Masterfiles;

namespace EAMIS.Common.DTO.Rolemanager
{
    public class EamisRoleModuleLinkDTO
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public int RoleId { get; set; }
        public bool ViewRight { get; set; }
        public bool InsertRight { get; set; }
        public bool UpdateRight { get; set; }
        public bool DeactivateRight { get; set; }
        public bool PrintRight { get; set; }
        public bool IsActive { get; set; }
        public int UserId { get; set; }
        public bool Own_Record { get; set; }
        public EamisModulesDTO ModulesNameList { get; set; }
        public EamisRolesDTO Roles { get; set; }
    }
}
