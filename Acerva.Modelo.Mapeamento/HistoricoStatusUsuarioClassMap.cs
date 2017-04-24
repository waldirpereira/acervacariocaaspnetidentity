using FluentNHibernate.Mapping;
using Acerva.Utils;

namespace Acerva.Modelo.Mapeamento
{
    public class HistoricoStatusUsuarioClassMap : ClassMap<HistoricoStatusUsuario>
    {
        public HistoricoStatusUsuarioClassMap()
        {
            Table("historico_status_usuario");
            Cache.ReadWrite();
            
            Id(u => u.Codigo, "codigo_historico").GeneratedBy.Native();

            Map(u => u.DataHora, "data_hora");
            References(u => u.UsuarioLogado, "id_usuario");
            Map(u => u.StatusNovo, "status_novo").CustomType(typeof(EnumComCodigoBdMapper<StatusUsuario>));
            Map(u => u.IdUsuarioAlterado, "id_usuario_alterado");
        }
    }
}