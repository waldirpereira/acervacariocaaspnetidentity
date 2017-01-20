using FluentNHibernate.Mapping;

namespace Acerva.Modelo.Mapeamento
{
    public class EquipeClassMap : ClassMap<Equipe>
    {
        public EquipeClassMap()
        {
            Table("equipe");
            Cache.ReadWrite();

            Id(e => e.Codigo, "codigo_equipe").GeneratedBy.Assigned();
            Map(e => e.Nome, "nome");
            Map(e => e.Sigla, "sigla");
            Map(e => e.Escudo, "escudo");
        }
    }
}
