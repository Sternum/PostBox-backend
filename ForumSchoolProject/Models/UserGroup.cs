namespace ForumSchoolProject.Models
{
        public class UserGroup
        {
            public int UserGroupID { get; set; }
            public string GDescription { get; set; }
            public bool AddPosts { get; set; }
            public bool EditPost { get; set; }
        }
    }
