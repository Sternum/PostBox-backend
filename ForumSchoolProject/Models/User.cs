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
// DTO for creating new users
public class CreateUserDto
{
    public string Name { get; set; }
    public string LastName { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public int UserGroupId { get; set; }
}

// DTO for retrieving user information (GET requests)
public class UserDto
{
    public string Name { get; set; }
    public string LastName { get; set; }
    public string Login { get; set; }
    public int UserGroupId { get; set; }
}