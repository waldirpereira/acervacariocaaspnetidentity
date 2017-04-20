/*INSERTS*/


/*CARGA DE HISTORICO DE STATUS*/
INSERT INTO `historico_status_usuario`(`id_usuario_alterado`, `data_hora`, `status_novo`, `id_usuario`) 
select id, now(), status, '6be52512-d272-4cc2-98a9-f5bf93534082' from users u where not exists (select 1 from historico_status_usuario h where u.id = h.id_usuario_alterado);