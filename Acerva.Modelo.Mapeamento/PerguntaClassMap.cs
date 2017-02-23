using Acerva.Utils;
using FluentNHibernate.Mapping;

namespace Acerva.Modelo.Mapeamento
{
    public class PerguntaClassMap : ClassMap<Pergunta>
    {
        public PerguntaClassMap()
        {
            Table("pergunta");
            Cache.ReadWrite();

            Id(c => c.Codigo, "codigo_pergunta").GeneratedBy.Increment();
            Map(c => c.Titulo, "titulo");
            Map(c => c.Texto, "texto");
            References(c => c.Votacao, "codigo_votacao");
            Map(c => c.Ativo, "ativo").CustomType(typeof(SimNaoType));

            HasMany(s => s.Opcoes)
                .KeyColumn("codigo_pergunta")
                .Cascade.AllDeleteOrphan();
        }
    }
}
