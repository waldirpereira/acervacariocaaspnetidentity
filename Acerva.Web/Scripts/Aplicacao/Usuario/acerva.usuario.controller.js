(function () {
    "use strict";

    angular.module("acerva.usuario")
        .controller("UsuarioController", ["Usuario", "ENUMS", "localStorageService", UsuarioController]);

    function UsuarioController(Usuario, ENUMS, localStorageService) {
        var ctrl = this;

        ctrl.listaUsuarios = [];
        
        ctrl.dominio = {
            statusUsuario: ENUMS.statusUsuario
        };

        var filtroStatusKey = "usuario.filtroStatus";
        ctrl.filtroStatus = [];

        atualizaFiltrosDaLocalStorage();
        
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
            atualizaFiltrosDaLocalStorage();
            var indexOf = ctrl.filtroStatus.map(function(st) { return st.codigoBd }).indexOf(status.codigoBd);
            if (indexOf >= 0) {
                ctrl.filtroStatus.splice(indexOf, 1);
                atualizaFiltrosNaLocalStorage();
                return;
            }
            ctrl.filtroStatus.push(status);
            atualizaFiltrosNaLocalStorage();
        }

        function estaFiltradoPorStatus(status) {
            atualizaFiltrosDaLocalStorage();
            return ctrl.filtroStatus.map(function(st) { return st.codigoBd }).indexOf(status.codigoBd) >= 0;
        }

        function pegaUsuariosFiltrados() {
            var todosUsuarios = ctrl.listaUsuarios;
            if (ctrl.filtroStatus.length === 0)
                return todosUsuarios;

            return todosUsuarios.filter(function(usuario) { return estaFiltradoPorStatus(usuario.status); });
        }

        function atualizaFiltrosDaLocalStorage() {
            if (localStorageService.isSupported) {
                ctrl.filtroStatus = localStorageService.get(filtroStatusKey) || [ctrl.dominio.statusUsuario.ativo];
            }
        }

        function atualizaFiltrosNaLocalStorage() {
            if (localStorageService.isSupported) {
                localStorageService.set(filtroStatusKey, ctrl.filtroStatus);
            }
        }
    }
})();