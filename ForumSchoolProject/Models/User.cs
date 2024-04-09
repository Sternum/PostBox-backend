using System;
using System.Collections.Generic;

namespace ForumSchoolProject.Models;

public partial class User
{
    public int Uid { get; set; }

    public string Name { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int UserGroupId { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual UserGroup UserGroup { get; set; } = null!;
}
