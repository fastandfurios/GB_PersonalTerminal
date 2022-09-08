namespace ManagerDirectory.Infrastructure.Models
{
    public class Help
    {
        public string Description { get; set; }

        public override string ToString()
        {
            return Description;
        }
    }
}
