/*--------------------------------------------------------
	DATABASE
--------------------------------------------------------*/

CREATE DATABASE ProjetoDB;
GO

USE ProjetoDB
GO

/*--------------------------------------------------------
	TABELAS
--------------------------------------------------------*/

CREATE TABLE Funcionarios (
 id int NOT NULL PRIMARY KEY,
 nome varchar(50) not NULL,
 idade int not NULL,
 cargo varchar(50) not NULL,
 foto varbinary(max) null
)

CREATE TABLE Usuarios (
 id int NOT NULL PRIMARY KEY,
 id_pessoa int not NULL FOREIGN KEY REFERENCES Funcionarios(id),
 tipo int not null,
 username varchar(50) not NULL,
 senha varchar(50) not NULL
)

CREATE TABLE Dispositivos (
 Id int NOT NULL PRIMARY KEY,
 apelido varchar(50) not NULL,
 device_id varchar(50) not NULL,
 entity_name varchar(50) not NULL
)
GO


-- Modificação no atributo para tornar o login possível
ALTER TABLE Usuarios
ADD CONSTRAINT UQ_Usuarios_Username UNIQUE (username)
GO

 -- Inserção de um usuário admin
INSERT INTO Funcionarios (id, nome, idade, cargo, foto)
VALUES (1, 'Administrador', 30, 'Administrador do Sistema', NULL)
GO

INSERT INTO Usuarios (id, id_pessoa, tipo, username, senha)
VALUES (1, 1, 1, 'admin', 'admin123')
GO

/*--------------------------------------------------------
	PROCEDURES
----------------------------------------------------------
	Gen�ricas
--------------------------------------------------------*/

create procedure spDelete
(
 @id int ,
 @tabela varchar(max)
)
as
begin
 declare @sql varchar(max);
 set @sql = ' delete ' + @tabela +
 ' where id = ' + cast(@id as varchar(max))
 exec(@sql)
end
GO

create procedure spConsulta
(
 @id int ,
 @tabela varchar(max)
)
as
begin
 declare @sql varchar(max);
 set @sql = 'select * from ' + @tabela +
 ' where id = ' + cast(@id as varchar(max))
 exec(@sql)
end
GO

create procedure spListagem
(
 @tabela varchar(max),
 @ordem varchar(max))
as
begin
 exec('select * from ' + @tabela +
 ' order by ' + @ordem)
end
GO

create procedure spProximoId
(@tabela varchar(max))
as
begin
 exec('select isnull(max(id) +1, 1) as MAIOR from '
 +@tabela)
end
GO

/*--------------------------------------------------------
	Funcion�rio
--------------------------------------------------------*/
create procedure spInsert_Funcionarios
(
 @id int,
 @nome varchar(max),
 @idade int,
 @cargo varchar(max),
 @foto varbinary(max)
)
as
begin
 insert into Funcionarios
 (id, nome, idade, cargo, foto)
 values
 (@id, @nome, @idade, @cargo, @foto)
end
GO

create procedure spUpdate_Funcionarios
(
 @id int,
 @nome varchar(max),
 @idade int,
 @cargo varchar(max),
 @foto varbinary(max)
)
as
begin
 update Funcionarios set
 nome = @nome,
 idade = @idade,
 cargo = @cargo,
 foto = @foto
 where id = @id
end
GO

/*--------------------------------------------------------
	Usu�rio
--------------------------------------------------------*/
create procedure spInsert_Usuarios
(
 @id int,
 @id_pessoa int,
 @tipo int,
 @username varchar(max),
 @senha varchar(max)
)
as
begin
 insert into Usuarios
 (id, id_pessoa, tipo, username, senha)
 values
 (@id, @id_pessoa, @tipo, @username, @senha)
end
GO

create procedure spUpdate_Usuarios
(
 @id int,
 @id_pessoa int,
 @tipo int,
 @username varchar(max),
 @senha varchar(max)
)
as
begin
 update Usuarios set
 tipo = @tipo, 
 username = @username, 
 senha = @senha
 where id = @id
end
GO

create procedure spListagem_Usuario
as
begin
select Usuarios.id,
	   Usuarios.id_pessoa,
	   Funcionarios.nome,
	   Usuarios.tipo,
	   Usuarios.username,
	   Usuarios.senha
from Usuarios
left join Funcionarios
on Usuarios.id_pessoa = Funcionarios.id
order by Usuarios.id
end
GO

create procedure spConsultaUsername
(
 @username varchar(50)
)
as
begin
 select * from Usuarios where username = @username
end
GO

/*--------------------------------------------------------
	Dispositivo
--------------------------------------------------------*/
create procedure spInsert_Dispositivos
(
 @id int,
 @apelido varchar(max),
 @device_id varchar(max),
 @entity_name varchar(max)
)
as
begin
 insert into Dispositivos
 (id, apelido, device_id, entity_name)
 values
 (@id, @apelido, @device_id, @entity_name)
end
GO

create procedure spUpdate_Dispositivos
(
 @id int,
 @apelido varchar(max),
 @device_id varchar(max),
 @entity_name varchar(max)
)
as
begin
 update Dispositivos set
 apelido = @apelido, 
 device_id = @device_id, 
 entity_name = @entity_name
 where id = @id
end
GO