using FluentNHibernate.Mapping;

namespace Acerva.Modelo.Mapeamento
{
    public class AnexoArtigoClassMap : ClassMap<AnexoArtigo>
    {
        public AnexoArtigoClassMap()
        {
            Table("anexo_artigo");
            Cache.ReadWrite();

            Id(c => c.Codigo, "codigo_anexo_artigo").GeneratedBy.Increment();
            Map(c => c.Titulo, "titulo");
            Map(c => c.NomeArquivo, "nome_arquivo");

            References(a => a.Artigo, "codigo_artigo");
        }
    }
}
