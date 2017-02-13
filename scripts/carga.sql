/*INSERTS*/


/*CARGA DE HISTORICO DE STATUS*/
INSERT INTO `historico_status_usuario`(`id_usuario_alterado`, `data_hora`, `status_novo`, `nome_usuario_logado`) 
select id, now(), status, 'waldirpereira@gmail.com' from users;