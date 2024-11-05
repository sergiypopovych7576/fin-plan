using System.ComponentModel.DataAnnotations;

namespace FP.Domain.Base
{
    public class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
    }
}
