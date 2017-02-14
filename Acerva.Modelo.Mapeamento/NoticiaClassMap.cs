﻿using Acerva.Utils;
using FluentNHibernate.Mapping;

namespace Acerva.Modelo.Mapeamento
{
    public class NoticiaClassMap : ClassMap<Noticia>
    {
        public NoticiaClassMap()
        {
            Table("noticia");
            Cache.ReadWrite();

            Id(c => c.Codigo, "codigo_noticia").GeneratedBy.Increment();
            Map(c => c.Titulo, "titulo");
            Map(c => c.TextoHtml, "texto_html");
            Map(c => c.Ordem, "ordem");
            Map(c => c.Ativo, "ativo").CustomType(typeof(SimNaoType));
        }
    }
}
