namespace ForumSchoolProject.Models
{
    public class User
    {
        public int UID { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int UserGroupID { get; set; }
    }
}