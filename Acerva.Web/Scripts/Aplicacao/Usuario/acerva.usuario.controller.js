(function () {
    "use strict";

    angular.module("acerva.usuario")
        .controller("UsuarioController", ["Usuario", "ENUMS", UsuarioController]);

    function UsuarioController(Usuario, ENUMS) {
        var ctrl = this;

        ctrl.listaUsuarios = [];

        ctrl.dominio = {
            statusUsuario: ENUMS.statusUsuario
        };
        
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