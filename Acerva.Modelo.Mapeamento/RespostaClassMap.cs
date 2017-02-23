using FluentNHibernate.Mapping;

namespace Acerva.Modelo.Mapeamento
{
    public class RespostaClassMap : ClassMap<Resposta>
    {
        public RespostaClassMap()
        {
            Table("resposta");
            Cache.ReadWrite();

            Id(c => c.Codigo, "codigo_resposta").GeneratedBy.Increment();
            References(c => c.Opcao, "codigo_opcao");
            References(c => c.Usuario, "id_usuario");
            Map(c => c.DataHora, "data_hora");
        }
    }
}
