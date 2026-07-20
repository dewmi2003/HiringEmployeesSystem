using System;
using System.Collections.Generic;

namespace Recruitment.Domain.Entities
{
    public class Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Navigation property for Users in this role
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
