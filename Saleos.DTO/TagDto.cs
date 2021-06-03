namespace Saleos.DTO
{
    public class TagDto
    {
        public int Id { get; set; }
        public string Tags { get; set; }

        public object Clone()
        {
            return new TagDto()
            {
                Id = Id,
                Tags = Tags
            };
        } 
    }
}