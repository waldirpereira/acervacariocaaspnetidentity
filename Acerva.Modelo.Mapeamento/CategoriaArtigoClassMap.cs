using Acerva.Utils;
using FluentNHibernate.Mapping;

namespace Acerva.Modelo.Mapeamento
{
    public class CategoriaArtigoClassMap : ClassMap<CategoriaArtigo>
    {
        public CategoriaArtigoClassMap()
        {
            Table("categoria_artigo");
            Cache.ReadWrite();

            Id(c => c.Codigo, "codigo_categoria_artigo").GeneratedBy.Assigned();
            Map(c => c.Nome, "nome");
            Map(c => c.Ativo, "ativo").CustomType(typeof(SimNaoType));
        }
    }
}
