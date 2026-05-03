using AgroMulti.Data.Models;
using AgroMulti.Tests.Infraestructura;
using AgroMulti.Ui.Services;
using Microsoft.EntityFrameworkCore;

namespace AgroMulti.Tests
{
    public class HistoricoEstadoEntregaServiceTest
    {
        [Fact]
        public async Task Buscar_CuandoExisteHistorico_RetornaEntidadConEstadoCargado()
        {
            // Arrange
            var dbName = TestDbContextFactory.NewDataBaseName();
            await using (var seedContext = TestDbContextFactory.CreateContext(dbName))
            {
                var productor = CrearProductor(1, "P001", "Juan", "Perez");
                var producto = CrearProducto(1, "Café");
                var estado = CrearEstadoEntrega(1, "Pendiente");
                var entrega = CrearEntrega(10, estado.EstadoEntregaId, productor.ProductorId, producto.ProductoId);

                seedContext.Productors.Add(productor);
                seedContext.Productos.Add(producto);
                seedContext.EstadoEntregas.Add(estado);
                seedContext.Entregas.Add(entrega);
                seedContext.HistoricosEstadoEntrega.Add(CrearHistorico(100, entrega.EntregaId, estado.EstadoEntregaId, DateTime.Now, "Registro inicial"));
                await seedContext.SaveChangesAsync();
            }

            await using var context = TestDbContextFactory.CreateContext(dbName);
            var service = new HistoricoEstadoEntregaService(context);

            // Act
            var result = await service.Buscar(100);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(100, result!.HistoricoEstadoEntregaId);
            Assert.Equal(10, result.EntregaId);
            Assert.Equal(1, result.EstadoEntregaId);
            Assert.NotNull(result.EstadoEntrega);
            Assert.Equal("Pendiente", result.EstadoEntrega.Nombre);
            Assert.Empty(context.ChangeTracker.Entries());
        }

        [Fact]
        public async Task Buscar_CuandoNoExisteHistorico_RetornaNull()
        {
            // Arrange
            await using var context = TestDbContextFactory.CreateContext(TestDbContextFactory.NewDataBaseName());
            var service = new HistoricoEstadoEntregaService(context);

            // Act
            var result = await service.Buscar(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetList_CuandoSeFiltraPorEntregaId_RetornaSoloHistorialDeEsaEntrega()
        {
            // Arrange
            var dbName = TestDbContextFactory.NewDataBaseName();
            await using (var seedContext = TestDbContextFactory.CreateContext(dbName))
            {
                var productor = CrearProductor(1, "P001", "Juan", "Perez");
                var producto = CrearProducto(1, "Café");
                var estado1 = CrearEstadoEntrega(1, "Pendiente");
                var estado2 = CrearEstadoEntrega(2, "En proceso");
                var entrega1 = CrearEntrega(20, estado1.EstadoEntregaId, productor.ProductorId, producto.ProductoId);
                var entrega2 = CrearEntrega(21, estado1.EstadoEntregaId, productor.ProductorId, producto.ProductoId);

                seedContext.Productors.Add(productor);
                seedContext.Productos.Add(producto);
                seedContext.EstadoEntregas.AddRange(estado1, estado2);
                seedContext.Entregas.AddRange(entrega1, entrega2);
                seedContext.HistoricosEstadoEntrega.AddRange(
                    CrearHistorico(200, entrega1.EntregaId, estado1.EstadoEntregaId, DateTime.Now.AddDays(-2), "Inicio"),
                    CrearHistorico(201, entrega1.EntregaId, estado2.EstadoEntregaId, DateTime.Now.AddDays(-1), "Cambio a proceso"),
                    CrearHistorico(202, entrega2.EntregaId, estado1.EstadoEntregaId, DateTime.Now, "Otra entrega")
                );
                await seedContext.SaveChangesAsync();
            }

            await using var context = TestDbContextFactory.CreateContext(dbName);
            var service = new HistoricoEstadoEntregaService(context);

            // Act
            var result = await service.GetList(h => h.EntregaId == 20);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, r => Assert.Equal(20, r.EntregaId));
            Assert.Contains(result, r => r.EstadoEntregaId == 1);
            Assert.Contains(result, r => r.EstadoEntregaId == 2);
            Assert.True(result[0].FechaCambio >= result[1].FechaCambio);
        }

        [Fact]
        public async Task Guardar_CuandoHistoricoNoExiste_InsertaYRetornaTrue()
        {
            // Arrange
            var dbName = TestDbContextFactory.NewDataBaseName();
            await using (var seedContext = TestDbContextFactory.CreateContext(dbName))
            {
                var productor = CrearProductor(1, "P001", "Juan", "Perez");
                var producto = CrearProducto(1, "Café");
                var estado = CrearEstadoEntrega(3, "Completado");
                var entrega = CrearEntrega(35, estado.EstadoEntregaId, productor.ProductorId, producto.ProductoId);
                seedContext.Productors.Add(productor);
                seedContext.Productos.Add(producto);
                seedContext.EstadoEntregas.Add(estado);
                seedContext.Entregas.Add(entrega);
                await seedContext.SaveChangesAsync();
            }

            await using var context = TestDbContextFactory.CreateContext(dbName);
            var service = new HistoricoEstadoEntregaService(context);
            var nuevo = CrearHistorico(0, entregaId: 35, estadoId: 3, fecha: default, obs: "Primer estado");

            // Act
            var result = await service.Guardar(nuevo);

            // Assert
            Assert.True(result);
            var saved = await context.HistoricosEstadoEntrega
                .Include(h => h.EstadoEntrega)
                .FirstOrDefaultAsync(h => h.EntregaId == 35);
            Assert.NotNull(saved);
            Assert.Equal(3, saved!.EstadoEntregaId);
            Assert.Equal("Primer estado", saved.Observaciones);
            Assert.NotEqual(default, saved.FechaCambio);
        }

        [Fact]
        public async Task Guardar_CuandoHistoricoYaExiste_ModificaYRetornaTrue()
        {
            // Arrange
            var dbName = TestDbContextFactory.NewDataBaseName();
            await using (var seedContext = TestDbContextFactory.CreateContext(dbName))
            {
                var productor = CrearProductor(1, "P001", "Juan", "Perez");
                var producto = CrearProducto(1, "Café");
                var estadoInicial = CrearEstadoEntrega(2, "En proceso");
                var entrega = CrearEntrega(40, estadoInicial.EstadoEntregaId, productor.ProductorId, producto.ProductoId);
                seedContext.Productors.Add(productor);
                seedContext.Productos.Add(producto);
                seedContext.EstadoEntregas.Add(estadoInicial);
                seedContext.Entregas.Add(entrega);
                seedContext.HistoricosEstadoEntrega.Add(CrearHistorico(45, entrega.EntregaId, estadoInicial.EstadoEntregaId, DateTime.Now, "Original"));
                await seedContext.SaveChangesAsync();
            }

            await using var context = TestDbContextFactory.CreateContext(dbName);
            var service = new HistoricoEstadoEntregaService(context);

            var estadoRechazado = await context.EstadoEntregas.FindAsync(4);
            if (estadoRechazado == null)
            {
                context.EstadoEntregas.Add(CrearEstadoEntrega(4, "Rechazado"));
                await context.SaveChangesAsync();
            }

            var modificado = CrearHistorico(45, entregaId: 40, estadoId: 4, fecha: DateTime.Now, obs: "Modificado");

            // Act
            var result = await service.Guardar(modificado);

            // Assert
            Assert.True(result);
            var saved = await context.HistoricosEstadoEntrega.FindAsync(45);
            Assert.NotNull(saved);
            Assert.Equal(4, saved!.EstadoEntregaId);
            Assert.Equal("Modificado", saved.Observaciones);
        }

        [Fact]
        public async Task Existe_CuandoHistoricoExiste_RetornaTrue()
        {
            // Arrange
            var dbName = TestDbContextFactory.NewDataBaseName();
            await using (var seedContext = TestDbContextFactory.CreateContext(dbName))
            {
                var productor = CrearProductor(1, "P001", "Juan", "Perez");
                var producto = CrearProducto(1, "Café");
                var estado = CrearEstadoEntrega(1, "Pendiente");
                var entrega = CrearEntrega(55, estado.EstadoEntregaId, productor.ProductorId, producto.ProductoId);
                seedContext.Productors.Add(productor);
                seedContext.Productos.Add(producto);
                seedContext.EstadoEntregas.Add(estado);
                seedContext.Entregas.Add(entrega);
                seedContext.HistoricosEstadoEntrega.Add(CrearHistorico(50, entrega.EntregaId, estado.EstadoEntregaId, DateTime.Now, "test"));
                await seedContext.SaveChangesAsync();
            }

            await using var context = TestDbContextFactory.CreateContext(dbName);
            var service = new HistoricoEstadoEntregaService(context);

            // Act
            var result = await service.Existe(50);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Existe_CuandoHistoricoNoExiste_RetornaFalse()
        {
            // Arrange
            await using var context = TestDbContextFactory.CreateContext(TestDbContextFactory.NewDataBaseName());
            var service = new HistoricoEstadoEntregaService(context);

            // Act
            var result = await service.Existe(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task Eliminar_CuandoExisteHistorico_LoBorraYRetornaTrue()
        {
            // Arrange
            var dbName = TestDbContextFactory.NewDataBaseName();
            await using (var seedContext = TestDbContextFactory.CreateContext(dbName))
            {
                var productor = CrearProductor(1, "P001", "Juan", "Perez");
                var producto = CrearProducto(1, "Café");
                var estado = CrearEstadoEntrega(1, "Pendiente");
                var entrega = CrearEntrega(60, estado.EstadoEntregaId, productor.ProductorId, producto.ProductoId);
                seedContext.Productors.Add(productor);
                seedContext.Productos.Add(producto);
                seedContext.EstadoEntregas.Add(estado);
                seedContext.Entregas.Add(entrega);
                seedContext.HistoricosEstadoEntrega.Add(CrearHistorico(70, entrega.EntregaId, estado.EstadoEntregaId, DateTime.Now, "A eliminar"));
                await seedContext.SaveChangesAsync();
            }

            await using var context = TestDbContextFactory.CreateContext(dbName);
            var service = new HistoricoEstadoEntregaService(context);

            // Act
            var result = await service.Eliminar(70);

            // Assert
            Assert.True(result);
            var deleted = await context.HistoricosEstadoEntrega.FindAsync(70);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task Eliminar_CuandoNoExisteHistorico_RetornaFalse()
        {
            // Arrange
            await using var context = TestDbContextFactory.CreateContext(TestDbContextFactory.NewDataBaseName());
            var service = new HistoricoEstadoEntregaService(context);

            // Act
            var result = await service.Eliminar(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ObtenerTodosAsync_DevuelveTodosOrdenadosAscendenteConEstadoCargado()
        {
            // Arrange
            var dbName = TestDbContextFactory.NewDataBaseName();
            await using (var seedContext = TestDbContextFactory.CreateContext(dbName))
            {
                var productor = CrearProductor(1, "P001", "Juan", "Perez");
                var producto = CrearProducto(1, "Café");
                var estado1 = CrearEstadoEntrega(1, "Pendiente");
                var estado2 = CrearEstadoEntrega(2, "En proceso");
                var entrega1 = CrearEntrega(10, estado1.EstadoEntregaId, productor.ProductorId, producto.ProductoId);
                var entrega2 = CrearEntrega(11, estado1.EstadoEntregaId, productor.ProductorId, producto.ProductoId);

                seedContext.Productors.Add(productor);
                seedContext.Productos.Add(producto);
                seedContext.EstadoEntregas.AddRange(estado1, estado2);
                seedContext.Entregas.AddRange(entrega1, entrega2);
                seedContext.HistoricosEstadoEntrega.AddRange(
                    CrearHistorico(300, 10, 2, DateTime.Now, "Reciente"),
                    CrearHistorico(301, 11, 1, DateTime.Now.AddDays(-5), "Antiguo"),
                    CrearHistorico(302, 10, 1, DateTime.Now.AddDays(-2), "Medio")
                );
                await seedContext.SaveChangesAsync();
            }

            // Act: usar el contexto de test
            await using var context = TestDbContextFactory.CreateContext(dbName);
            var service = new HistoricoEstadoEntregaService(context);
            var result = await service.ObtenerTodosAsync();

            // Assert
            Assert.Equal(3, result.Count);
            for (int i = 1; i < result.Count; i++)
            {
                Assert.True(result[i].FechaCambio >= result[i - 1].FechaCambio);
            }
            Assert.All(result, r => Assert.NotNull(r.EstadoEntrega));
            Assert.Contains(result, r => r.EstadoEntrega.Nombre == "Pendiente");
            Assert.Contains(result, r => r.EstadoEntrega.Nombre == "En proceso");
        }

        // ── Métodos auxiliares ──────────────────────────────────────
        private static Productor CrearProductor(int id, string codigo, string nombre, string apellido)
        {
            return new Productor
            {
                ProductorId = id,
                Codigo = codigo,
                Nombre = nombre,
                Apellido = apellido,
                Telefono = "00000000",
                Direccion = "Test"
            };
        }

        private static Producto CrearProducto(int id, string nombre)
        {
            return new Producto { ProductoId = id, Nombre = nombre };
        }

        private static EstadoEntrega CrearEstadoEntrega(int id, string nombre)
        {
            return new EstadoEntrega { EstadoEntregaId = id, Nombre = nombre };
        }

        private static Entrega CrearEntrega(int id, int estadoId, int productorId, int productoId)
        {
            return new Entrega
            {
                EntregaId = id,
                NumeroEntrega = $"ENT-{id}",
                FechaEntrega = DateOnly.FromDateTime(DateTime.Today),
                ProductorId = productorId,
                ProductoId = productoId,
                SubProductoId = null,
                EstadoEntregaId = estadoId,
                Placa = "ABC123",
                NombreConductor = "Conductor Test",
                Kilos = 100.5m,
                Cajas = 10,
                Sacos = 5,
                KilosSecos = 95.2m,
                Pasillo = "Pasillo Test",
                NumeroAnaquel = "Anaquel Test",
                Piso = "Piso Test",
                Observaciones = "Entrega de prueba"
            };
        }

        private static HistoricoEstadoEntrega CrearHistorico(int id, int entregaId, int estadoId, DateTime fecha, string obs)
        {
            return new HistoricoEstadoEntrega
            {
                HistoricoEstadoEntregaId = id,
                EntregaId = entregaId,
                EstadoEntregaId = estadoId,
                FechaCambio = fecha == default ? DateTime.Now : fecha,
                Observaciones = obs
            };
        }
    }
}