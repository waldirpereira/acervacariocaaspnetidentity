using Acerva.Utils;
using FluentNHibernate.Mapping;

namespace Acerva.Modelo.Mapeamento
{
    public class ArtigoClassMap : ClassMap<Artigo>
    {
        public ArtigoClassMap()
        {
            Table("artigo");
            Cache.ReadWrite();

            Id(c => c.Codigo, "codigo_artigo").GeneratedBy.Increment();
            Map(c => c.Titulo, "titulo");
            Map(c => c.TextoHtml, "texto_html");
            Map(c => c.DataHora, "data_hora");
            Map(c => c.Ativo, "ativo").CustomType(typeof(SimNaoType));

            References(a => a.Categoria, "codigo_categoria_artigo");
            References(a => a.Usuario, "codigo_usuario");

            HasMany(s => s.Anexos)
                .KeyColumn("codigo_artigo")
                .Cascade.AllDeleteOrphan();
        }
    }
}
