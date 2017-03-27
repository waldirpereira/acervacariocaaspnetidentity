using FluentNHibernate.Mapping;

namespace Acerva.Modelo.Mapeamento
{
    public class AnexoNoticiaClassMap : ClassMap<AnexoNoticia>
    {
        public AnexoNoticiaClassMap()
        {
            Table("anexo_noticia");
            Cache.ReadWrite();

            Id(c => c.Codigo, "codigo_anexo_noticia").GeneratedBy.Increment();
            Map(c => c.Titulo, "titulo");
            Map(c => c.NomeArquivo, "nome_arquivo");

            References(a => a.Noticia, "codigo_noticia");
        }
    }
}
