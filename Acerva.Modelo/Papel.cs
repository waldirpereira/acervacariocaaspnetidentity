using Microsoft.AspNet.Identity;

namespace Acerva.Modelo
{
    public class Papel : IRole
    {
        public Papel() { }
        public Papel(string name) : this()
        {
            Name = name;
        }

        public Papel(string name, string id)
        {
            Name = name;
            Id = id;
        }

        public virtual string Id { get; set; }
        public virtual string Name { get; set; }
    }
}
