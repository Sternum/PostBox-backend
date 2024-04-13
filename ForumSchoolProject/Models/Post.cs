using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ForumSchoolProject.Models;

public partial class Post
{
    public int Pid { get; set; }

    public DateTime PostDate { get; set; }

    public DateTime? EditDate { get; set; }

    public string? PostDescription { get; set; }

    public int Uid { get; set; }

    public virtual User UidNavigation { get; set; } = null!;
}
// DTO for creating new posts and Retrieve post information
public class CreatePostDto
{
    public DateTime PostDate { get; set; } = DateTime.UtcNow; // Default to current time if not provided

    public DateTime? EditDate { get; set; } // Optional

    [Required(ErrorMessage = "Post description is required.")]
    public string PostDescription { get; set; }

    [Required(ErrorMessage = "User ID is required.")]
    public int Uid { get; set; }
}

public class GetPostDto
{
    public int Pid { get; set; }

    public DateTime PostDate { get; set; }

    public DateTime? EditDate { get; set; }

    public string? PostDescription { get; set; }

    public int Uid { get; set; }
}

