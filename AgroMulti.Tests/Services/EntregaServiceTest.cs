using AgroMulti.Data.Models;
using AgroMulti.Tests.Infraestructura;
using AgroMulti.Ui.Services;
using Microsoft.EntityFrameworkCore;

namespace AgroMulti.Tests
{
    public class EntregaServiceTest
    {
        [Fact]
        public async Task Buscar_CuandoExisteEntrega_RetornaEntidad()
        {
            // Arrange
            var dbName = TestDbContextFactory.NewDataBaseName();
            await using (var seedContext = TestDbContextFactory.CreateContext(dbName))
            {
                seedContext.Entregas.Add(CreateEntrega(id: 1, numeroEntrega: "E001", kilos: 50));
                await seedContext.SaveChangesAsync();
            }

            await using var context = TestDbContextFactory.CreateContext(dbName);
            var service = new EntregaService(context);

            // Act
            var result = await service.Buscar(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result!.EntregaId);
            Assert.Equal("E001", result.NumeroEntrega);   // ← corregido
            Assert.Equal(50, result.Kilos);
            Assert.Empty(context.ChangeTracker.Entries());
        }

        [Fact]
        public async Task Buscar_CuandoNoExisteEntrega_RetornaNull()
        {
            // Arrange
            await using var context = TestDbContextFactory.CreateContext(
                TestDbContextFactory.NewDataBaseName());
            var service = new EntregaService(context);

            // Act
            var result = await service.Buscar(99);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetList_CuandoSeFiltraPorKilos_RetornaCoincidencias()
        {
            // Arrange
            var dbName = TestDbContextFactory.NewDataBaseName();
            await using (var seedContext = TestDbContextFactory.CreateContext(dbName))
            {
                seedContext.Entregas.AddRange(
                    CreateEntrega(id: 1, numeroEntrega: "E001", kilos: 20),
                    CreateEntrega(id: 2, numeroEntrega: "E002", kilos: 80),
                    CreateEntrega(id: 3, numeroEntrega: "E003", kilos: 120)
                );
                await seedContext.SaveChangesAsync();
            }

            await using var context = TestDbContextFactory.CreateContext(dbName);
            var service = new EntregaService(context);

            // Act
            var result = await service.GetList(e => e.Kilos >= 80);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, e => e.EntregaId == 2);
            Assert.Contains(result, e => e.EntregaId == 3);
        }

        [Fact]
        public async Task Guardar_CuandoEntregaNoExiste_InsertaYRetornaTrue()
        {
            // Arrange
            await using var context = TestDbContextFactory.CreateContext(
                TestDbContextFactory.NewDataBaseName());
            var service = new EntregaService(context);
            var nuevaEntrega = CreateEntrega(id: 10, numeroEntrega: "E010", kilos: 30);

            // Act
            var result = await service.Guardar(nuevaEntrega);

            // Assert
            Assert.True(result);
            var saved = await context.Entregas.FirstOrDefaultAsync(e => e.EntregaId == 10);
            Assert.NotNull(saved);
            Assert.Equal(30, saved!.Kilos);
        }

        [Fact]
        public async Task Guardar_CuandoEntregaExiste_ModificaYRetornaTrue()
        {
            // Arrange
            var dbName = TestDbContextFactory.NewDataBaseName();
            await using (var seedContext = TestDbContextFactory.CreateContext(dbName))
            {
                seedContext.Entregas.Add(CreateEntrega(id: 20, numeroEntrega: "E020", kilos: 50));
                await seedContext.SaveChangesAsync();
            }

            await using var context = TestDbContextFactory.CreateContext(dbName);
            var service = new EntregaService(context);
            var actualizada = CreateEntrega(id: 20, numeroEntrega: "E020", kilos: 75);

            // Act
            var result = await service.Guardar(actualizada);

            // Assert
            Assert.True(result);
            var saved = await context.Entregas.FirstOrDefaultAsync(e => e.EntregaId == 20);
            Assert.NotNull(saved);
            Assert.Equal(75, saved!.Kilos);
        }

        [Fact]
        public async Task Existe_CuandoEntregaExiste_RetornaTrue()
        {
            // Arrange
            var dbName = TestDbContextFactory.NewDataBaseName();
            await using (var seedContext = TestDbContextFactory.CreateContext(dbName))
            {
                seedContext.Entregas.Add(CreateEntrega(id: 5, numeroEntrega: "E005", kilos: 10));
                await seedContext.SaveChangesAsync();
            }

            await using var context = TestDbContextFactory.CreateContext(dbName);
            var service = new EntregaService(context);

            // Act
            var result = await service.Existe(5);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Eliminar_CuandoExisteEntrega_LoBorraYRetornaTrue()
        {
            // Arrange
            var dbName = TestDbContextFactory.NewDataBaseName();
            await using (var seedContext = TestDbContextFactory.CreateContext(dbName))
            {
                seedContext.Entregas.Add(CreateEntrega(id: 30, numeroEntrega: "E030", kilos: 60));
                await seedContext.SaveChangesAsync();
            }

            await using var context = TestDbContextFactory.CreateContext(dbName);
            var service = new EntregaService(context);

            // Act
            var result = await service.Eliminar(30);

            // Assert
            Assert.True(result);
            var eliminada = await context.Entregas.FindAsync(30);
            Assert.Null(eliminada);
        }

        
        private static Entrega CreateEntrega(int id, string numeroEntrega, decimal kilos)
        {
            return new Entrega
            {
                EntregaId = id,
                NumeroEntrega = numeroEntrega,                   
                FechaEntrega = DateOnly.FromDateTime(DateTime.Today), 
                ProductorId = 1,
                ProductoId = 1,
                SubProductoId = 1,
                Kilos = kilos,
                EstadoEntregaId = 1,
                Observaciones = "Test"
            };
        }
    }
}