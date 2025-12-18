Informe técnico del proyecto Pedidos360
---------------------------------------

### Descripción general

Pedidos360 es una aplicación web ASP.NET Core MVC para la gestión de pedidos de clientes.

Incluye:

-   Gestión de clientes (Cliente)

-   Catálogo de productos y categorías (Producto, Categoria)

-   Gestión de pedidos y detalles de pedido (Pedido, PedidoDetalle)

-   Autenticación de usuarios con ASP.NET Core Identity (IdentityUser), integrada en el mismo ApplicationDbContext.

La aplicación utiliza Entity Framework Core con SQL Server como base de datos y sigue el patrón clásico MVC + capa de servicios (por ejemplo, PedidoService).

### Tecnologías principales

-   .NET: net8.0

-   Framework web: ASP.NET Core MVC + Razor Pages (para Identity)

-   ORM: Entity Framework Core (Microsoft.EntityFrameworkCore.SqlServer)

-   Autenticación / Autorización:

-   Microsoft.AspNetCore.Identity.EntityFrameworkCore

-   Microsoft.AspNetCore.Identity.UI

-   Base de datos: SQL Server (LocalDB o instancia SQL Server/SQL Express configurada en DefaultConnection)

### Arquitectura de la aplicación

-   Capa Web (UI):

-   Controladores en Controllers (ClienteController, ProductoController, PedidoController, HomeController).

-   Vistas Razor en Views (carpetas Cliente, Producto, Pedido, Home, Shared).

-   Páginas de Identity en Areas/Identity/Pages.

-   Capa de datos:

-   ApplicationDbContext hereda de IdentityDbContext<IdentityUser> y define los DbSet:

-   Clientes, Productos, Categorias, Pedidos, PedidoDetalles.

-   Migraciones en la carpeta Migrations.

-   Capa de servicios:

-   PedidoService registrado como Scoped en Program.cs:

-   builder.Services.AddScoped<PedidoService>();

-   Configuración y arranque (Program.cs):

-   Configuración del DbContext con SQL Server:

        builder.Services.AddDbContext<ApplicationDbContext>(options =>

            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

-   Configuración de Identity:

        builder.Services.AddDefaultIdentity<IdentityUser>(options =>

                options.SignIn.RequireConfirmedAccount = false)

            .AddEntityFrameworkStores<ApplicationDbContext>();

-   Registro de MVC + Razor Pages, middleware de autenticación/autorización y rutas:

-   Ruta por defecto: {controller=Home}/{action=Index}/{id?}

-   app.MapRazorPages(); para las páginas de Identity.

-   Inicialización de datos (semilla):

-   await DbInitializer.SeedAsync(app.Services);

* * * * *

Requisitos para ejecutar el proyecto
------------------------------------

### Requisitos de software

-   SDK de .NET 8.0 instalado.

-   SQL Server:

-   Puede ser SQL Server Express, LocalDB o una instancia completa, según lo configurado en la cadena de conexión.

-   Editor/IDE recomendado:

-   Visual Studio 2022 (con soporte .NET 8) o

-   Visual Studio Code + extensión C# + CLI de .NET.

### Configuración de la base de datos

1.  Cadena de conexión:

-   Editar appsettings.json (o appsettings.Development.json) en la sección ConnectionStrings:DefaultConnection para apuntar a tu instancia de SQL Server.

-   Ejemplo típico (puede variar según tu entorno):

         "ConnectionStrings": {

           "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=Pedidos360;Trusted_Connection=True;MultipleActiveResultSets=true"

         }

1.  Aplicar migraciones / crear base de datos (si aún no existe):

-   Desde la raíz del proyecto:

         dotnet ef database update

-   Esto creará la base de datos según las migraciones en Migrations.

### Pasos para ejecutar

1.  Abrir una terminal en la carpeta del proyecto (Pedidos360_Proyecto).

1.  Restaurar paquetes y compilar (opcional, se hace implícito al correr):

       dotnet build

1.  Ejecutar la aplicación:

       dotnet run

1.  Abrir el navegador en la URL que la consola indique (normalmente https://localhost:xxxx).

1.  Registrar un usuario (si no hay usuarios) usando las páginas de Identity y empezar a usar la gestión de clientes, productos y pedidos.

* * * * *

Modelo de datos y relaciones en la base de datos
------------------------------------------------

A continuación se describen las entidades principales y sus relaciones, según los modelos en Models y el ApplicationDbContext.

### Entidades

-   Cliente

-   Id (PK)

-   Nombre, Cedula, Correo, Telefono, Direccion

-   Navegación:

-   Pedidos: colección de pedidos asociados a ese cliente.

-   Categoria

-   Id (PK)

-   Nombre

-   Navegación:

-   Productos: colección de productos de esta categoría.

-   Producto

-   Id (PK)

-   Nombre

-   CategoriaId (FK → Categoria.Id)

-   Precio, ImpuestoPorc, Stock, ImagenUrl, Activo

-   Navegación:

-   Categoria: referencia a la categoría.

-   Pedido

-   Id (PK)

-   ClienteId (FK → Cliente.Id)

-   UsuarioId (FK → AspNetUsers.Id de IdentityUser)

-   Fecha, Subtotal, Impuestos, Total, Estado

-   Navegación:

-   Cliente: cliente que hizo el pedido.

-   Usuario: usuario de Identity que registró el pedido.

-   Detalles: colección de PedidoDetalle asociados.

-   PedidoDetalle

-   Id (PK)

-   PedidoId (FK → Pedido.Id)

-   ProductoId (FK → Producto.Id)

-   Cantidad, PrecioUnit, Descuento, ImpuestoPorc, TotalLinea

-   Navegación:

-   Pedido: pedido al que pertenece la línea.

-   Producto: producto de la línea.

-   IdentityUser (tabla AspNetUsers)

-   Gestionado por ASP.NET Core Identity.

-   El modelo Pedido tiene un campo UsuarioId que referencia este usuario.

### Relaciones principales

-   Cliente -- Pedido

-   Tipo: 1 a N

-   Un Cliente puede tener muchos Pedidos.

-   Un Pedido pertenece a un único Cliente.

-   Implementación:

-   Pedido.ClienteId (FK) y Pedido.Cliente

-   Cliente.Pedidos (colección).

-   Pedido -- PedidoDetalle

-   Tipo: 1 a N

-   Un Pedido tiene muchas líneas de detalle (PedidoDetalle).

-   Cada PedidoDetalle pertenece a un único Pedido.

-   Implementación:

-   PedidoDetalle.PedidoId (FK) y PedidoDetalle.Pedido

-   Pedido.Detalles (colección).

-   Producto -- PedidoDetalle

-   Tipo: 1 a N

-   Un Producto puede aparecer en muchos PedidoDetalle.

-   Cada PedidoDetalle referencia un único Producto.

-   Implementación:

-   PedidoDetalle.ProductoId (FK) y PedidoDetalle.Producto.

-   Categoria -- Producto

-   Tipo: 1 a N

-   Una Categoria puede tener muchos Productos.

-   Cada Producto pertenece a una única Categoria.

-   Implementación:

-   Producto.CategoriaId (FK) y Producto.Categoria

-   Categoria.Productos (colección).

-   IdentityUser -- Pedido

-   Tipo: 1 a N

-   Un usuario del sistema (IdentityUser) puede registrar muchos Pedidos.

-   Cada Pedido está asociado a un único usuario (campo UsuarioId).

-   Implementación:

-   Pedido.UsuarioId (FK) y Pedido.Usuario.

### Resumen visual (texto)

-   Cliente (1) ─── (N) Pedido

-   Pedido (1) ─── (N) PedidoDetalle

-   Producto (1) ─── (N) PedidoDetalle

-   Categoria (1) ─── (N) Producto

-   IdentityUser (1) ─── (N) Pedido

Si quieres, en un siguiente mensaje puedo generarte un diagrama ER en formato texto (por ejemplo, estilo mermaid) o un dibujo más detallado para documentación.

Hazlo en formato markdown

Informe técnico del proyecto Pedidos360
---------------------------------------

### 1\. Descripción general

Pedidos360 es una aplicación web desarrollada con ASP.NET Core MVC para la gestión de pedidos de clientes.

Incluye:

-   Gestión de clientes (Cliente)

-   Gestión de productos y categorías (Producto, Categoria)

-   Gestión de pedidos y sus detalles (Pedido, PedidoDetalle)

-   Autenticación y gestión de usuarios con ASP.NET Core Identity (IdentityUser), integrada en el mismo contexto de datos.

### 2\. Tecnologías y arquitectura

-   Plataforma: .NET 8 (TargetFramework: net8.0)

-   Framework web: ASP.NET Core MVC + Razor Pages (para Identity)

-   ORM: Entity Framework Core con SQL Server

-   Paquetes:

-   Microsoft.EntityFrameworkCore.SqlServer

-   Microsoft.EntityFrameworkCore.Tools

-   Autenticación / Identidad:

-   Microsoft.AspNetCore.Identity.EntityFrameworkCore

-   Microsoft.AspNetCore.Identity.UI

Capas principales:

-   Presentación (UI)

-   Controladores: ClienteController, ProductoController, PedidoController, HomeController

-   Vistas Razor en la carpeta Views (subcarpetas Cliente, Producto, Pedido, Home, Shared)

-   Páginas de Identity en Areas/Identity/Pages.

-   Acceso a datos

-   ApplicationDbContext (hereda de IdentityDbContext<IdentityUser>) con los DbSet:

-   Clientes, Productos, Categorias, Pedidos, PedidoDetalles

-   Migraciones en la carpeta Migrations.

-   Servicios de dominio

-   PedidoService, registrado como Scoped en Program.cs.

-   Configuración de arranque (Program.cs)

-   Registro del DbContext con SQL Server:

        builder.Services.AddDbContext<ApplicationDbContext>(options =>

            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

-   Configuración de Identity:

        builder.Services.AddDefaultIdentity<IdentityUser>(options =>

                options.SignIn.RequireConfirmedAccount = false)

            .AddEntityFrameworkStores<ApplicationDbContext>();

-   Registro de servicios y pipeline:

-   builder.Services.AddControllersWithViews();

-   builder.Services.AddRazorPages();

-   Rutas MVC: {controller=Home}/{action=Index}/{id?}

-   app.MapRazorPages();

-   Inicialización de datos:

-   await DbInitializer.SeedAsync(app.Services);

* * * * *

3\. Requisitos para ejecutar el proyecto
----------------------------------------

### 3.1. Requisitos de software

-   SDK de .NET 8.0 instalado.

-   SQL Server (cualquiera de estas opciones):

-   SQL Server Express

-   LocalDB

-   Instancia completa de SQL Server

-   IDE recomendado:

-   Visual Studio 2022 con soporte .NET 8

-   o Visual Studio Code con extensión C# y CLI de .NET

### 3.2. Configuración de la base de datos

1.  Cadena de conexión (DefaultConnection)

Editar appsettings.json o appsettings.Development.json en la sección ConnectionStrings:

   "ConnectionStrings": {

     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=Pedidos360;Trusted_Connection=True;MultipleActiveResultSets=true"

   }

Ajusta el Server, Database y credenciales según tu entorno.

1.  Crear/aplicar migraciones y base de datos

Desde la carpeta raíz del proyecto:

   dotnet ef database update

Esto creará la base de datos y tablas según las migraciones existentes.

### 3.3. Pasos para ejecutar

1.  Abrir una terminal en la carpeta Pedidos360_Proyecto.

1.  (Opcional) Compilar el proyecto:

       dotnet build

1.  Ejecutar la aplicación:

       dotnet run

1.  Abrir el navegador en la URL indicada en consola (normalmente https://localhost:xxxx).

1.  Usar las páginas de Identity (login/register) para crear un usuario y luego acceder a la gestión de clientes, productos y pedidos.

* * * * *

4. Modelo de datos y relaciones
-------------------------------

### 4.1. Entidades principales

-   Cliente

-   Campos:

-   Id (PK)

-   Nombre (obligatorio, máx. 150)

-   Cedula (obligatorio, máx. 20)

-   Correo (obligatorio, email)

-   Telefono (teléfono)

-   Direccion (máx. 250)

-   Navegaciones:

-   Pedidos: ICollection<Pedido>?

-   Categoria

-   Campos:

-   Id (PK)

-   Nombre (obligatorio, máx. 100)

-   Navegaciones:

-   Productos: ICollection<Producto>

-   Producto

-   Campos:

-   Id (PK)

-   Nombre (obligatorio, máx. 150)

-   CategoriaId (FK → Categoria.Id)

-   Precio (decimal, ≥ 0.01)

-   ImpuestoPorc (0--100)

-   Stock (≥ 0)

-   ImagenUrl (URL opcional)

-   Activo (bool, por defecto true)

-   Navegaciones:

-   Categoria?: referencia a la categoría

-   Pedido

-   Campos:

-   Id (PK)

-   ClienteId (FK → Cliente.Id)

-   UsuarioId (FK → AspNetUsers.Id de IdentityUser)

-   Fecha (por defecto DateTime.UtcNow)

-   Subtotal (decimal)

-   Impuestos (decimal)

-   Total (decimal)

-   Estado (string, máx. 20, por defecto "Confirmado")

-   Navegaciones:

-   Cliente: referencia a Cliente

-   Usuario: IdentityUser

-   Detalles: ICollection<PedidoDetalle>

-   PedidoDetalle

-   Campos:

-   Id (PK)

-   PedidoId (FK → Pedido.Id)

-   ProductoId (FK → Producto.Id)

-   Cantidad (int, ≥ 1)

-   PrecioUnit (decimal)

-   Descuento (decimal)

-   ImpuestoPorc (decimal)

-   TotalLinea (decimal)

-   Navegaciones:

-   Pedido: referencia a Pedido

-   Producto: referencia a Producto

-   IdentityUser (tablas AspNetUsers, etc.)

-   Gestionado por ASP.NET Core Identity.

-   Relacionado con Pedido a través de UsuarioId.

* * * * *

### 4.2. Relaciones entre tablas

-   Cliente -- Pedido

-   Tipo: Uno a muchos (1--N)

-   Descripción:

-   Un Cliente puede tener muchos Pedidos.

-   Cada Pedido pertenece a un único Cliente.

-   Implementación:

-   FK: Pedido.ClienteId

-   Navegaciones: Pedido.Cliente y Cliente.Pedidos

-   Pedido -- PedidoDetalle

-   Tipo: Uno a muchos (1--N)

-   Descripción:

-   Un Pedido tiene muchas líneas de detalle (PedidoDetalle).

-   Cada PedidoDetalle pertenece a un único Pedido.

-   Implementación:

-   FK: PedidoDetalle.PedidoId

-   Navegaciones: PedidoDetalle.Pedido y Pedido.Detalles

-   Producto -- PedidoDetalle

-   Tipo: Uno a muchos (1--N)

-   Descripción:

-   Un Producto puede estar en muchos PedidoDetalle.

-   Cada PedidoDetalle referencia un único Producto.

-   Implementación:

-   FK: PedidoDetalle.ProductoId

-   Navegación: PedidoDetalle.Producto

-   Categoria -- Producto

-   Tipo: Uno a muchos (1--N)

-   Descripción:

-   Una Categoria puede tener muchos Productos.

-   Cada Producto pertenece a una única Categoria.

-   Implementación:

-   FK: Producto.CategoriaId

-   Navegaciones: Producto.Categoria y Categoria.Productos

-   IdentityUser -- Pedido

-   Tipo: Uno a muchos (1--N)

-   Descripción:

-   Un usuario del sistema (IdentityUser) puede registrar muchos Pedidos.

-   Cada Pedido está asociado a un único usuario mediante UsuarioId.

-   Implementación:

-   FK: Pedido.UsuarioId

-   Navegación: Pedido.Usuario

* * * * *

### 4.3. Esquema relacional (texto)

-   Cliente (1) ─── (N) Pedido

-   Pedido (1) ─── (N) PedidoDetalle

-   Producto (1) ─── (N) PedidoDetalle

-   Categoria (1) ─── (N) Producto

-   IdentityUser (1) ─── (N) Pedido