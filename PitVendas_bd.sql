-- =====================================================================
-- Pit Vendas - Sistema Web de Controle e Emissao de Vendas
-- Script de criacao do banco de dados
--
-- Gerado a partir das migrations do Entity Framework Core.
-- =====================================================================

DROP DATABASE IF EXISTS pitvendas_bd;
CREATE DATABASE pitvendas_bd CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
USE pitvendas_bd;

CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `migration_id` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `product_version` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `pk___ef_migrations_history` PRIMARY KEY (`migration_id`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;
ALTER DATABASE CHARACTER SET utf8mb4;

CREATE TABLE `AspNetRoles` (
    `id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `name` varchar(256) CHARACTER SET utf8mb4 NULL,
    `normalized_name` varchar(256) CHARACTER SET utf8mb4 NULL,
    `concurrency_stamp` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `pk_asp_net_roles` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `AspNetUsers` (
    `id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `nome` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    `foto` varchar(300) CHARACTER SET utf8mb4 NULL,
    `ativo` tinyint(1) NOT NULL,
    `criado_em` datetime(6) NOT NULL,
    `user_name` varchar(256) CHARACTER SET utf8mb4 NULL,
    `normalized_user_name` varchar(256) CHARACTER SET utf8mb4 NULL,
    `email` varchar(256) CHARACTER SET utf8mb4 NULL,
    `normalized_email` varchar(256) CHARACTER SET utf8mb4 NULL,
    `email_confirmed` tinyint(1) NOT NULL,
    `password_hash` longtext CHARACTER SET utf8mb4 NULL,
    `security_stamp` longtext CHARACTER SET utf8mb4 NULL,
    `concurrency_stamp` longtext CHARACTER SET utf8mb4 NULL,
    `phone_number` longtext CHARACTER SET utf8mb4 NULL,
    `phone_number_confirmed` tinyint(1) NOT NULL,
    `two_factor_enabled` tinyint(1) NOT NULL,
    `lockout_end` datetime(6) NULL,
    `lockout_enabled` tinyint(1) NOT NULL,
    `access_failed_count` int NOT NULL,
    CONSTRAINT `pk_asp_net_users` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `cores` (
    `id` int NOT NULL AUTO_INCREMENT,
    `nome` varchar(60) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `pk_cores` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `estados` (
    `id` int NOT NULL AUTO_INCREMENT,
    `nome` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `sigla` varchar(2) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `pk_estados` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `tamanhos` (
    `id` int NOT NULL AUTO_INCREMENT,
    `codigo` varchar(10) CHARACTER SET utf8mb4 NOT NULL,
    `ordem` int NOT NULL,
    CONSTRAINT `pk_tamanhos` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `AspNetRoleClaims` (
    `id` int NOT NULL AUTO_INCREMENT,
    `role_id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `claim_type` longtext CHARACTER SET utf8mb4 NULL,
    `claim_value` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `pk_asp_net_role_claims` PRIMARY KEY (`id`),
    CONSTRAINT `fk_asp_net_role_claims_asp_net_roles_role_id` FOREIGN KEY (`role_id`) REFERENCES `AspNetRoles` (`id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `AspNetUserClaims` (
    `id` int NOT NULL AUTO_INCREMENT,
    `user_id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `claim_type` longtext CHARACTER SET utf8mb4 NULL,
    `claim_value` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `pk_asp_net_user_claims` PRIMARY KEY (`id`),
    CONSTRAINT `fk_asp_net_user_claims_asp_net_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `AspNetUsers` (`id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `AspNetUserLogins` (
    `login_provider` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `provider_key` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `provider_display_name` longtext CHARACTER SET utf8mb4 NULL,
    `user_id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `pk_asp_net_user_logins` PRIMARY KEY (`login_provider`, `provider_key`),
    CONSTRAINT `fk_asp_net_user_logins_asp_net_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `AspNetUsers` (`id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `AspNetUserRoles` (
    `user_id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `role_id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `pk_asp_net_user_roles` PRIMARY KEY (`user_id`, `role_id`),
    CONSTRAINT `fk_asp_net_user_roles_asp_net_roles_role_id` FOREIGN KEY (`role_id`) REFERENCES `AspNetRoles` (`id`) ON DELETE CASCADE,
    CONSTRAINT `fk_asp_net_user_roles_asp_net_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `AspNetUsers` (`id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `AspNetUserTokens` (
    `user_id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `login_provider` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `name` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `value` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `pk_asp_net_user_tokens` PRIMARY KEY (`user_id`, `login_provider`, `name`),
    CONSTRAINT `fk_asp_net_user_tokens_asp_net_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `AspNetUsers` (`id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `cidades` (
    `id` int NOT NULL AUTO_INCREMENT,
    `nome` varchar(120) CHARACTER SET utf8mb4 NOT NULL,
    `estado_id` int NOT NULL,
    CONSTRAINT `pk_cidades` PRIMARY KEY (`id`),
    CONSTRAINT `fk_cidades_estados_estado_id` FOREIGN KEY (`estado_id`) REFERENCES `estados` (`id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `enderecos` (
    `id` int NOT NULL AUTO_INCREMENT,
    `logradouro` varchar(200) CHARACTER SET utf8mb4 NULL,
    `numero` varchar(20) CHARACTER SET utf8mb4 NULL,
    `complemento` varchar(120) CHARACTER SET utf8mb4 NULL,
    `bairro` varchar(120) CHARACTER SET utf8mb4 NULL,
    `cep` varchar(9) CHARACTER SET utf8mb4 NULL,
    `cidade_id` int NULL,
    CONSTRAINT `pk_enderecos` PRIMARY KEY (`id`),
    CONSTRAINT `fk_enderecos_cidades_cidade_id` FOREIGN KEY (`cidade_id`) REFERENCES `cidades` (`id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `clientes` (
    `id` int NOT NULL AUTO_INCREMENT,
    `nome` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    `razao_social` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    `cnpj` varchar(14) CHARACTER SET utf8mb4 NOT NULL,
    `inscricao_estadual` varchar(20) CHARACTER SET utf8mb4 NULL,
    `email` varchar(200) CHARACTER SET utf8mb4 NULL,
    `telefone` varchar(20) CHARACTER SET utf8mb4 NULL,
    `foto` varchar(300) CHARACTER SET utf8mb4 NULL,
    `observacao` varchar(500) CHARACTER SET utf8mb4 NULL,
    `ativo` tinyint(1) NOT NULL,
    `endereco_id` int NULL,
    `vendedor_id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `pk_clientes` PRIMARY KEY (`id`),
    CONSTRAINT `fk_clientes_enderecos_endereco_id` FOREIGN KEY (`endereco_id`) REFERENCES `enderecos` (`id`) ON DELETE SET NULL,
    CONSTRAINT `fk_clientes_users_vendedor_id` FOREIGN KEY (`vendedor_id`) REFERENCES `AspNetUsers` (`id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `fabricas` (
    `id` int NOT NULL AUTO_INCREMENT,
    `nome` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    `razao_social` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    `cnpj` varchar(14) CHARACTER SET utf8mb4 NOT NULL,
    `email` varchar(200) CHARACTER SET utf8mb4 NULL,
    `telefone` varchar(20) CHARACTER SET utf8mb4 NULL,
    `comissao` decimal(5,2) NOT NULL,
    `ativo` tinyint(1) NOT NULL,
    `endereco_id` int NULL,
    `vendedor_id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `pk_fabricas` PRIMARY KEY (`id`),
    CONSTRAINT `fk_fabricas_enderecos_endereco_id` FOREIGN KEY (`endereco_id`) REFERENCES `enderecos` (`id`) ON DELETE SET NULL,
    CONSTRAINT `fk_fabricas_users_vendedor_id` FOREIGN KEY (`vendedor_id`) REFERENCES `AspNetUsers` (`id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `pedidos` (
    `id` int NOT NULL AUTO_INCREMENT,
    `numero` int NOT NULL,
    `data` datetime(6) NOT NULL,
    `status` int NOT NULL,
    `valor_bruto` decimal(12,2) NOT NULL,
    `desconto` decimal(12,2) NOT NULL,
    `valor_total` decimal(12,2) NOT NULL,
    `percentual_comissao` decimal(5,2) NOT NULL,
    `valor_comissao` decimal(12,2) NOT NULL,
    `total_pecas` int NOT NULL,
    `prazo` varchar(100) CHARACTER SET utf8mb4 NULL,
    `frete` varchar(100) CHARACTER SET utf8mb4 NULL,
    `condicao_pagamento` varchar(100) CHARACTER SET utf8mb4 NULL,
    `parcelas` int NOT NULL,
    `observacao` varchar(500) CHARACTER SET utf8mb4 NULL,
    `enviado_em` datetime(6) NULL,
    `cliente_id` int NOT NULL,
    `fabrica_id` int NOT NULL,
    `vendedor_id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `pk_pedidos` PRIMARY KEY (`id`),
    CONSTRAINT `fk_pedidos_clientes_cliente_id` FOREIGN KEY (`cliente_id`) REFERENCES `clientes` (`id`) ON DELETE RESTRICT,
    CONSTRAINT `fk_pedidos_fabricas_fabrica_id` FOREIGN KEY (`fabrica_id`) REFERENCES `fabricas` (`id`) ON DELETE RESTRICT,
    CONSTRAINT `fk_pedidos_users_vendedor_id` FOREIGN KEY (`vendedor_id`) REFERENCES `AspNetUsers` (`id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `produtos` (
    `id` int NOT NULL AUTO_INCREMENT,
    `codigo` varchar(60) CHARACTER SET utf8mb4 NOT NULL,
    `descricao` varchar(300) CHARACTER SET utf8mb4 NOT NULL,
    `observacao` varchar(500) CHARACTER SET utf8mb4 NULL,
    `foto` varchar(300) CHARACTER SET utf8mb4 NULL,
    `ativo` tinyint(1) NOT NULL,
    `fabrica_id` int NOT NULL,
    CONSTRAINT `pk_produtos` PRIMARY KEY (`id`),
    CONSTRAINT `fk_produtos_fabricas_fabrica_id` FOREIGN KEY (`fabrica_id`) REFERENCES `fabricas` (`id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `pedido_itens` (
    `id` int NOT NULL AUTO_INCREMENT,
    `descricao_produto` varchar(300) CHARACTER SET utf8mb4 NOT NULL,
    `codigo_produto` varchar(60) CHARACTER SET utf8mb4 NOT NULL,
    `nome_cor` varchar(60) CHARACTER SET utf8mb4 NOT NULL,
    `valor_unitario` decimal(10,2) NOT NULL,
    `quantidade` int NOT NULL,
    `subtotal` decimal(12,2) NOT NULL,
    `observacao` varchar(300) CHARACTER SET utf8mb4 NULL,
    `pedido_id` int NOT NULL,
    `produto_id` int NOT NULL,
    `cor_id` int NOT NULL,
    CONSTRAINT `pk_pedido_itens` PRIMARY KEY (`id`),
    CONSTRAINT `fk_pedido_itens_cores_cor_id` FOREIGN KEY (`cor_id`) REFERENCES `cores` (`id`) ON DELETE RESTRICT,
    CONSTRAINT `fk_pedido_itens_pedidos_pedido_id` FOREIGN KEY (`pedido_id`) REFERENCES `pedidos` (`id`) ON DELETE CASCADE,
    CONSTRAINT `fk_pedido_itens_produtos_produto_id` FOREIGN KEY (`produto_id`) REFERENCES `produtos` (`id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `precos` (
    `id` int NOT NULL AUTO_INCREMENT,
    `codigo` varchar(30) CHARACTER SET utf8mb4 NOT NULL,
    `valor` decimal(10,2) NOT NULL,
    `observacao` varchar(300) CHARACTER SET utf8mb4 NULL,
    `data_alteracao` date NOT NULL,
    `ativo` tinyint(1) NOT NULL,
    `produto_id` int NOT NULL,
    CONSTRAINT `pk_precos` PRIMARY KEY (`id`),
    CONSTRAINT `fk_precos_produtos_produto_id` FOREIGN KEY (`produto_id`) REFERENCES `produtos` (`id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `produto_cores` (
    `id` int NOT NULL AUTO_INCREMENT,
    `produto_id` int NOT NULL,
    `cor_id` int NOT NULL,
    CONSTRAINT `pk_produto_cores` PRIMARY KEY (`id`),
    CONSTRAINT `fk_produto_cores_cores_cor_id` FOREIGN KEY (`cor_id`) REFERENCES `cores` (`id`) ON DELETE RESTRICT,
    CONSTRAINT `fk_produto_cores_produtos_produto_id` FOREIGN KEY (`produto_id`) REFERENCES `produtos` (`id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `produto_tamanhos` (
    `id` int NOT NULL AUTO_INCREMENT,
    `produto_id` int NOT NULL,
    `tamanho_id` int NOT NULL,
    CONSTRAINT `pk_produto_tamanhos` PRIMARY KEY (`id`),
    CONSTRAINT `fk_produto_tamanhos_produtos_produto_id` FOREIGN KEY (`produto_id`) REFERENCES `produtos` (`id`) ON DELETE CASCADE,
    CONSTRAINT `fk_produto_tamanhos_tamanhos_tamanho_id` FOREIGN KEY (`tamanho_id`) REFERENCES `tamanhos` (`id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

CREATE TABLE `pedido_item_grades` (
    `id` int NOT NULL AUTO_INCREMENT,
    `quantidade` int NOT NULL,
    `codigo_tamanho` varchar(10) CHARACTER SET utf8mb4 NOT NULL,
    `pedido_item_id` int NOT NULL,
    `tamanho_id` int NOT NULL,
    CONSTRAINT `pk_pedido_item_grades` PRIMARY KEY (`id`),
    CONSTRAINT `fk_pedido_item_grades_pedido_itens_pedido_item_id` FOREIGN KEY (`pedido_item_id`) REFERENCES `pedido_itens` (`id`) ON DELETE CASCADE,
    CONSTRAINT `fk_pedido_item_grades_tamanhos_tamanho_id` FOREIGN KEY (`tamanho_id`) REFERENCES `tamanhos` (`id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;

INSERT INTO `cores` (`id`, `nome`)
VALUES (1, 'Verde'),
(2, 'Azul'),
(3, 'Preto'),
(4, 'Cinza'),
(5, 'Branco'),
(6, 'Vermelho'),
(7, 'Rosa'),
(8, 'Sortido');

INSERT INTO `estados` (`id`, `nome`, `sigla`)
VALUES (1, 'Acre', 'AC'),
(2, 'Alagoas', 'AL'),
(3, 'Amazonas', 'AM'),
(4, 'Amapá', 'AP'),
(5, 'Bahia', 'BA'),
(6, 'Ceará', 'CE'),
(7, 'Distrito Federal', 'DF'),
(8, 'Espírito Santo', 'ES'),
(9, 'Goiás', 'GO'),
(10, 'Maranhão', 'MA'),
(11, 'Minas Gerais', 'MG'),
(12, 'Mato Grosso do Sul', 'MS'),
(13, 'Mato Grosso', 'MT'),
(14, 'Pará', 'PA'),
(15, 'Paraíba', 'PB'),
(16, 'Pernambuco', 'PE'),
(17, 'Piauí', 'PI'),
(18, 'Paraná', 'PR'),
(19, 'Rio de Janeiro', 'RJ'),
(20, 'Rio Grande do Norte', 'RN'),
(21, 'Rondônia', 'RO'),
(22, 'Roraima', 'RR'),
(23, 'Rio Grande do Sul', 'RS'),
(24, 'Santa Catarina', 'SC'),
(25, 'Sergipe', 'SE'),
(26, 'São Paulo', 'SP'),
(27, 'Tocantins', 'TO');

INSERT INTO `tamanhos` (`id`, `codigo`, `ordem`)
VALUES (1, 'PP', 1),
(2, 'P', 2),
(3, 'M', 3),
(4, 'G', 4),
(5, 'GG', 5),
(6, 'XG', 6),
(7, 'XGG', 7),
(8, '2', 8),
(9, '4', 9),
(10, '6', 10),
(11, '8', 11),
(12, '10', 12),
(13, '12', 13),
(14, '14', 14),
(15, '16', 15);

INSERT INTO `cidades` (`id`, `estado_id`, `nome`)
VALUES (1, 21, 'Alta Floresta d''Oeste'),
(2, 21, 'Alto Alegre dos Parecis'),
(3, 21, 'Alto Paraíso'),
(4, 21, 'Alvorada d''Oeste'),
(5, 21, 'Ariquemes'),
(6, 21, 'Buritis'),
(7, 21, 'Cabixi'),
(8, 21, 'Cacaulândia'),
(9, 21, 'Cacoal'),
(10, 21, 'Campo Novo de Rondônia'),
(11, 21, 'Candeias do Jamari'),
(12, 21, 'Castanheiras'),
(13, 21, 'Cerejeiras'),
(14, 21, 'Chupinguaia'),
(15, 21, 'Colorado do Oeste'),
(16, 21, 'Corumbiara'),
(17, 21, 'Costa Marques'),
(18, 21, 'Cujubim'),
(19, 21, 'Espigão d''Oeste'),
(20, 21, 'Governador Jorge Teixeira'),
(21, 21, 'Guajará-Mirim'),
(22, 21, 'Itapuã do Oeste'),
(23, 21, 'Jaru'),
(24, 21, 'Ji-Paraná'),
(25, 21, 'Machadinho d''Oeste'),
(26, 21, 'Ministro Andreazza'),
(27, 21, 'Mirante da Serra'),
(28, 21, 'Monte Negro'),
(29, 21, 'Nova Brasilândia d''Oeste'),
(30, 21, 'Nova Mamoré'),
(31, 21, 'Nova União'),
(32, 21, 'Novo Horizonte do Oeste'),
(33, 21, 'Ouro Preto do Oeste'),
(34, 21, 'Parecis'),
(35, 21, 'Pimenta Bueno'),
(36, 21, 'Pimenteiras do Oeste'),
(37, 21, 'Porto Velho'),
(38, 21, 'Presidente Médici'),
(39, 21, 'Primavera de Rondônia'),
(40, 21, 'Rio Crespo'),
(41, 21, 'Rolim de Moura'),
(42, 21, 'Santa Luzia d''Oeste');
INSERT INTO `cidades` (`id`, `estado_id`, `nome`)
VALUES (43, 21, 'São Felipe d''Oeste'),
(44, 21, 'São Francisco do Guaporé'),
(45, 21, 'São Miguel do Guaporé'),
(46, 21, 'Seringueiras'),
(47, 21, 'Teixeirópolis'),
(48, 21, 'Theobroma'),
(49, 21, 'Urupá'),
(50, 21, 'Vale do Anari'),
(51, 21, 'Vale do Paraíso'),
(52, 21, 'Vilhena');

CREATE INDEX `ix_asp_net_role_claims_role_id` ON `AspNetRoleClaims` (`role_id`);

CREATE UNIQUE INDEX `RoleNameIndex` ON `AspNetRoles` (`normalized_name`);

CREATE INDEX `ix_asp_net_user_claims_user_id` ON `AspNetUserClaims` (`user_id`);

CREATE INDEX `ix_asp_net_user_logins_user_id` ON `AspNetUserLogins` (`user_id`);

CREATE INDEX `ix_asp_net_user_roles_role_id` ON `AspNetUserRoles` (`role_id`);

CREATE INDEX `EmailIndex` ON `AspNetUsers` (`normalized_email`);

CREATE UNIQUE INDEX `UserNameIndex` ON `AspNetUsers` (`normalized_user_name`);

CREATE INDEX `ix_cidades_estado_id_nome` ON `cidades` (`estado_id`, `nome`);

CREATE INDEX `ix_clientes_endereco_id` ON `clientes` (`endereco_id`);

CREATE UNIQUE INDEX `ix_clientes_vendedor_id_cnpj` ON `clientes` (`vendedor_id`, `cnpj`);

CREATE UNIQUE INDEX `ix_cores_nome` ON `cores` (`nome`);

CREATE INDEX `ix_enderecos_cidade_id` ON `enderecos` (`cidade_id`);

CREATE UNIQUE INDEX `ix_estados_sigla` ON `estados` (`sigla`);

CREATE INDEX `ix_fabricas_endereco_id` ON `fabricas` (`endereco_id`);

CREATE UNIQUE INDEX `ix_fabricas_vendedor_id_cnpj` ON `fabricas` (`vendedor_id`, `cnpj`);

CREATE UNIQUE INDEX `ix_pedido_item_grades_pedido_item_id_tamanho_id` ON `pedido_item_grades` (`pedido_item_id`, `tamanho_id`);

CREATE INDEX `ix_pedido_item_grades_tamanho_id` ON `pedido_item_grades` (`tamanho_id`);

CREATE INDEX `ix_pedido_itens_cor_id` ON `pedido_itens` (`cor_id`);

CREATE UNIQUE INDEX `ix_pedido_itens_pedido_id_produto_id_cor_id` ON `pedido_itens` (`pedido_id`, `produto_id`, `cor_id`);

CREATE INDEX `ix_pedido_itens_produto_id` ON `pedido_itens` (`produto_id`);

CREATE INDEX `ix_pedidos_cliente_id` ON `pedidos` (`cliente_id`);

CREATE INDEX `ix_pedidos_data` ON `pedidos` (`data`);

CREATE INDEX `ix_pedidos_fabrica_id` ON `pedidos` (`fabrica_id`);

CREATE UNIQUE INDEX `ix_pedidos_vendedor_id_numero` ON `pedidos` (`vendedor_id`, `numero`);

CREATE UNIQUE INDEX `ix_precos_produto_id_codigo` ON `precos` (`produto_id`, `codigo`);

CREATE INDEX `ix_produto_cores_cor_id` ON `produto_cores` (`cor_id`);

CREATE UNIQUE INDEX `ix_produto_cores_produto_id_cor_id` ON `produto_cores` (`produto_id`, `cor_id`);

CREATE UNIQUE INDEX `ix_produto_tamanhos_produto_id_tamanho_id` ON `produto_tamanhos` (`produto_id`, `tamanho_id`);

CREATE INDEX `ix_produto_tamanhos_tamanho_id` ON `produto_tamanhos` (`tamanho_id`);

CREATE UNIQUE INDEX `ix_produtos_fabrica_id_codigo` ON `produtos` (`fabrica_id`, `codigo`);

CREATE UNIQUE INDEX `ix_tamanhos_codigo` ON `tamanhos` (`codigo`);

INSERT INTO `__EFMigrationsHistory` (`migration_id`, `product_version`)
VALUES ('20260720192919_InitialCreate', '9.0.9');

ALTER TABLE `pedidos` ADD `data_faturamento` date NULL;

ALTER TABLE `pedidos` ADD `percentual_desconto` decimal(5,2) NOT NULL DEFAULT 0.0;

ALTER TABLE `pedidos` ADD `tabela_preco` varchar(30) CHARACTER SET utf8mb4 NULL;

UPDATE `tamanhos` SET `ordem` = 7
WHERE `id` = 6;
SELECT ROW_COUNT();


UPDATE `tamanhos` SET `codigo` = 'XXG', `ordem` = 8
WHERE `id` = 7;
SELECT ROW_COUNT();


UPDATE `tamanhos` SET `ordem` = 10
WHERE `id` = 8;
SELECT ROW_COUNT();


UPDATE `tamanhos` SET `ordem` = 11
WHERE `id` = 9;
SELECT ROW_COUNT();


UPDATE `tamanhos` SET `ordem` = 12
WHERE `id` = 10;
SELECT ROW_COUNT();


UPDATE `tamanhos` SET `ordem` = 13
WHERE `id` = 11;
SELECT ROW_COUNT();


UPDATE `tamanhos` SET `ordem` = 14
WHERE `id` = 12;
SELECT ROW_COUNT();


UPDATE `tamanhos` SET `ordem` = 15
WHERE `id` = 13;
SELECT ROW_COUNT();


UPDATE `tamanhos` SET `ordem` = 16
WHERE `id` = 14;
SELECT ROW_COUNT();


UPDATE `tamanhos` SET `ordem` = 17
WHERE `id` = 15;
SELECT ROW_COUNT();


INSERT INTO `tamanhos` (`id`, `codigo`, `ordem`)
VALUES (16, 'EG', 6),
(17, '3XG', 9);

INSERT INTO `__EFMigrationsHistory` (`migration_id`, `product_version`)
VALUES ('20260721135947_AlinhamentoComPrototipo', '9.0.9');

COMMIT;

