using Acerva.Utils;
using FluentNHibernate.Mapping;

namespace Acerva.Modelo.Mapeamento
{
    public class OpcaoClassMap : ClassMap<Opcao>
    {
        public OpcaoClassMap()
        {
            Table("opcao");
            Cache.ReadWrite();

            Id(c => c.Codigo, "codigo_opcao").GeneratedBy.Increment();
            Map(c => c.Texto, "texto_html");
            References(c => c.Pergunta, "codigo_pergunta");
            Map(c => c.Ativo, "ativo").CustomType(typeof(SimNaoType));

            HasManyToMany(u => u.Respostas)
                .Table("resposta")
                .ParentKeyColumn("codigo_opcao")
                .ChildKeyColumn("codigo_resposta");
        }
    }
}
