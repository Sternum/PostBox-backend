using System;
using System.Collections.Generic;

namespace ForumSchoolProject.Models;

public partial class UserGroup
{
    public int UserGroupId { get; set; }

    public string Gdescription { get; set; } = null!;

    public string AddPosts { get; set; } = null!;

    public string EditPost { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
