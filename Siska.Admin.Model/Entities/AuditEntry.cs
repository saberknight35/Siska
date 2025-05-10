using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siska.Admin.Model.Entities
{
    [Table("AuditEntry")]
    public class AuditEntry
    {
        [Key]
        public Guid Id { get; set; }

        public string Metadata { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string ActionTable { get; set; } = string.Empty;

        public string ActionTaken { get; set; } = string.Empty;

        public string ActionBy { get; set; } = string.Empty;

        public bool Succeeded { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;
    }
}
