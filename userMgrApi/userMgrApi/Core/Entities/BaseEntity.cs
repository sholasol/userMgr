using System;
namespace userMgrApi.Core.Entities
{
	public class BaseEntity<TID>
	{
        public TID Id { get; set; } //TID stands for generic Id

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public bool isActive { get; set; } = true;

        public bool isDeleted { get; set; } = false;
    }
}

