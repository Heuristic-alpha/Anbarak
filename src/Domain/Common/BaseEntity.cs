using System.ComponentModel.DataAnnotations.Schema;

namespace Anbarak.Domain.Common;

public abstract class BaseEntity
{
    public long Id { get; set; }
}