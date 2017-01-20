using Acerva.Modelo.Mapeamento.Types;
using FluentNHibernate.Mapping;

namespace Acerva.Modelo.Mapeamento
{
    public class AcervaClassMap : ClassMap<Acerva>
    {
        public AcervaClassMap()
        {
            Table("acerva");
            Cache.ReadWrite();

            Id(e => e.Codigo, "codigo_acerva").GeneratedBy.Increment();
            Map(e => e.Nome, "nome");
            Map(e => e.Ativo, "ativo").CustomType(typeof(SimNaoType));
            References(e => e.UsuarioResponsavel, "codigo_usuario_responsavel");
            References(b => b.Regional, "codigo_regional");

            HasMany(b => b.Participacoes)
                .KeyColumn("codigo_acerva")
                .Inverse()
                .Cascade.AllDeleteOrphan();

            HasMany(b => b.Regras)
                .KeyColumn("codigo_acerva")
                .Inverse()
                .Cascade.AllDeleteOrphan();
        }
    }
}
