namespace ForumSchoolProject.Models
{
    public class Post
    {
        public int PID { get; set; }
        public DateTime PostDate { get; set; }
        public DateTime EditDate { get; set; }
        public string PostDescription { get; set; }
        public int UID { get; set; }
    }
}