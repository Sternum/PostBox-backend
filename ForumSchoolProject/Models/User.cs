using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
    [Required(ErrorMessage = "First name is required.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Last name is required.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Login is required.")]
    public string Login { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; }
}

// DTO for retrieving user information (GET requests)
public class GetUserDto
{
    public string Name { get; set; }
    public string LastName { get; set; }
    public string Login { get; set; }
    public int UserGroupId { get; set; }
}

public class UpdateUserDto
{
    public string Name { get; set; }
    public string LastName { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }

}