(function () {
    "use strict";

    angular.module("acerva.usuario")
        .controller("UsuarioController", ["Usuario", UsuarioController]);

    function UsuarioController(Usuario) {
        var ctrl = this;

        ctrl.listaUsuarios = [];
        
        init();

        function init() {
            atualizaListaUsuarios();
        }

        function atualizaListaUsuarios() {
            Usuario.buscaListaUsuarios().then(function (listaUsuarios) {
                ctrl.listaUsuarios = listaUsuarios;
            });
        }
    }
})();