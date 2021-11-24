create database Panaderia;
go
use Panaderia;
go
--drop table cliente;

create table cliente (
id_cliente int IDENTITY (1,1) PRIMARY KEY, 
nombre_cliente varchar (64) NOT NULL, 
nombre_neg nvarchar(64), 
dom_cl nvarchar(64), 
mail_cliente varchar(64), 
telefono_cliente varchar(64), 
esta_cancelado bit NOT NULL);

go
--drop table proveedor

create table proveedor (
id_prov int IDENTITY (1,1) PRIMARY KEY, 
nombre_prov varchar (64) NOT NULL, 
razonsocial_prov nvarchar(64), 
mail_prov varchar(64), 
telefono_prov varchar(64), 
tel_rep varchar(64), 
dom_prov nvarchar(64), 
cuil_prov nvarchar(64),
esta_cancelado bit NOT NULL);

--drop table Autorizado
go

create table Autorizado (
Id_autorizado int IDENTITY (1,1) PRIMARY KEY,
Nombre_autorizado nvarchar (64) NOT NULL,
Apellido_autorizado nvarchar (64) NOT NULL,
Usuario_autorizado nvarchar (64) NOT NULL,
Clave_autorizado nvarchar(30) NOT NULL,
esta_cancelado bit NOT NULL);

--drop table categoría
go
create table categoria (
Id_categoria int IDENTITY (1,1) PRIMARY KEY, 
Nombre_categoria varchar(32) NOT NULL, 
Cod_cat varchar(100));


--drop table producto
go
create table producto (
Id_producto int IDENTITY (1,1) PRIMARY KEY, 
Cod_producto int NOT NULL, 
Nombre_producto varchar(255) NOT NULL, 
Stock_producto int,
Preciouc_producto decimal NOT NULL, 
Preciouv_producto decimal NOT NULL, 
Id_categoria int FOREIGN KEY REFERENCES categoria (Id_categoria));

--drop table fpago
go
create table fpago (
Id_fpago int IDENTITY (1,1) PRIMARY KEY, 
Nombre_fpago varchar (64) NOT NULL);

go
--drop table compra

create table compra (
Id_compra int IDENTITY (1,1) PRIMARY KEY, 
Id_proveedor int FOREIGN KEY REFERENCES proveedor (id_prov), 
Id_autorizado int FOREIGN KEY REFERENCES autorizado (Id_autorizado), 
Id_fpago int FOREIGN KEY REFERENCES fpago (Id_fpago), 
Montofinal decimal NOT NULL,
Fecha_compra nvarchar (20) NOT NULL, 
Estado_trans varchar(32) NOT NULL, 
N_Factura varchar (64));
go

--drop table producto_compra

create table producto_compra (
Id_producto_compra int IDENTITY (1,1) PRIMARY KEY, 
Id_compra int FOREIGN KEY REFERENCES compra (Id_compra), 
Id_producto int FOREIGN KEY REFERENCES producto (Id_producto), 
Cantidad decimal NOT NULL, 
Preciou_historico decimal NOT NULL); 


--drop table venta
go
create table venta (
Id_venta int IDENTITY (1,1) PRIMARY KEY, 
Id_cliente int FOREIGN KEY REFERENCES cliente (id_cliente), 
Id_autorizado int FOREIGN KEY REFERENCES autorizado (Id_autorizado), 
Id_fpago int FOREIGN KEY REFERENCES fpago (Id_fpago), 
Montofinal decimal NOT NULL,
Fecha_venta nvarchar (20) NOT NULL, 
Hora_venta nvarchar (20) NOT NULL, 
Estado_trans varchar(32) NOT NULL, 
N_Factura varchar (64));

go
--drop table producto_venta

create table producto_venta (
id_producto_venta int IDENTITY (1,1) PRIMARY KEY, 
id_venta int FOREIGN KEY REFERENCES venta (Id_venta), 
id_producto int FOREIGN KEY REFERENCES producto (Id_producto), 
cantidad int NOT NULL, 
preciou_historico decimal NOT NULL,
Monto decimal);
go
--drop table Deuda

create table Deuda(
ID_deuda int IDENTITY (1,1) PRIMARY KEY,
Id_cliente int FOREIGN KEY REFERENCES cliente (id_cliente),
Id_venta int FOREIGN KEY REFERENCES venta (Id_venta), 
Fecha nvarchar (20) NOT NULL, 
Importe decimal NOT NULL,
Descripcion nvarchar (200)
);
go

--drop table caja

create table Caja(
Id_Caja int identity primary key,
Id_autorizado int FOREIGN KEY REFERENCES Autorizado (id_autorizado),
Fecha nvarchar (20) not null,
ImporteInicial decimal (10,2) not null,
ImporteFinal decimal (10,2) not null,
Estado bit 
)

go
insert into fpago values ('Efectivo')
insert into fpago values ('Tarjeta de crédito')
insert into fpago values ('Débito')
go
insert into producto values (255, 'Cerealitas 250g', 6, 48.6, 75, 3);
insert into producto values (221, 'Alfajor Tatin Simple', 25, 12, 25, 2);
insert into producto values (300, 'Coca Cola 2L Retornable', 5, 105, 145, 2);
insert into producto values (322, 'Coca Cola 500ml Descartable', 12, 50, 85, 2);
insert into producto values (347, 'Pritty 2L Descartable', 7, 95, 130, 3);
insert into producto values (497, 'Flan Sancor x2', 4, 55, 80, 1);
insert into producto values (402, 'Postre Serenito', 6, 40, 65, 4);
insert into producto values (446, 'Yogurt Descremado Ser', 6, 75, 100, 4);
go
insert into proveedor values ('Coca Cola', 'Coca Cola', 'cocacola@coca.com.ar', 03514444444, 0351156111111,'Calle siemprevivas 123', '20-12345678-9', 0)
insert into proveedor values ('La lacteo', 555555, 'lalacteo@ll.com.ar', 4564612300, 11111111111,'Centro 456', '20-54645123-8', 0)
go
insert into cliente values ('Generico', '', '', '','', 0)
insert into cliente values ('Daiana', 'Daiana','Jose Villegas', 'example@hotmail.com', 1234567, 0)
go
insert into Autorizado values ('Daiana', 'Ramirez', 'ADMIN','ADMIN', 0)

insert into categoria values ('Panadería', 1)
insert into categoria values ('Galletas', 2)
insert into categoria values ('Gaseosas', 3)
insert into categoria values ('Lacteos', 4)
