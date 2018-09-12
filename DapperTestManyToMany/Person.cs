using System.Collections.Generic;

namespace DapperTestManyToMany
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Chor> Chöre { get; set; } = new List<Chor>();
    }
}
