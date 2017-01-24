(function () {
    "use strict";

    angular.module("acerva.usuario")
        .controller("UsuarioController", ["Usuario", "ENUMS", UsuarioController]);

    function UsuarioController(Usuario, ENUMS) {
        var ctrl = this;

        ctrl.listaUsuarios = [];
        ctrl.filtroStatus = [];

        ctrl.dominio = {
            statusUsuario: ENUMS.statusUsuario
        };

        ctrl.mudaFiltroStatus = mudaFiltroStatus;
        ctrl.estaFiltradoPorStatus = estaFiltradoPorStatus;
        ctrl.pegaUsuariosFiltrados = pegaUsuariosFiltrados;
        
        init();

        function init() {
            atualizaListaUsuarios();
        }

        function atualizaListaUsuarios() {
            Usuario.buscaListaUsuarios().then(function (listaUsuarios) {
                ctrl.listaUsuarios = listaUsuarios;
            });
        }

        function mudaFiltroStatus(status) {
            var indexOf = ctrl.filtroStatus.map(function(st) { return st.codigoBd }).indexOf(status.codigoBd);
            if (indexOf >= 0) {
                ctrl.filtroStatus.splice(indexOf, 1);
                return;
            }
            ctrl.filtroStatus.push(status);
        }

        function estaFiltradoPorStatus(status) {
            return ctrl.filtroStatus.map(function(st) { return st.codigoBd }).indexOf(status.codigoBd) >= 0;
        }

        function pegaUsuariosFiltrados() {
            var todosUsuarios = ctrl.listaUsuarios;
            if (ctrl.filtroStatus.length === 0)
                return todosUsuarios;

            return todosUsuarios.filter(function(usuario) { return estaFiltradoPorStatus(usuario.status); });
        }
    }
})();