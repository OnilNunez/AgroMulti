using AgroMulti.Data.Models;
using AgroMulti.Tests.Infraestructura;
using AgroMulti.Ui.Services;
using Microsoft.EntityFrameworkCore;

namespace AgroMulti.Tests
{
    public class EstadoEntregaServiceTest
    {
        [Fact]
        public async Task Buscar_CuandoExisteEstado_RetornaEntidad()
        {
            // Arrange
            var dbName = TestDbContextFactory.NewDataBaseName();
            await using (var seedContext = TestDbContextFactory.CreateContext(dbName))
            {
                seedContext.EstadoEntregas.Add(CreateEstadoEntrega(id: 1, nombre: "Pendiente"));
                await seedContext.SaveChangesAsync();
            }

            await using var context = TestDbContextFactory.CreateContext(dbName);
            var service = new EstadoEntregaService(context);

            // Act
            var result = await service.Buscar(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result!.EstadoEntregaId);
            Assert.Equal("Pendiente", result.Nombre);
            Assert.Empty(context.ChangeTracker.Entries());
        }

        [Fact]
        public async Task Buscar_CuandoNoExisteEstado_RetornaNull()
        {
            // Arrange
            await using var context = TestDbContextFactory.CreateContext(
                TestDbContextFactory.NewDataBaseName());
            var service = new EstadoEntregaService(context);

            // Act
            var result = await service.Buscar(99);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetList_CuandoSeFiltraPorNombre_RetornaCoincidencias()
        {
            // Arrange
            var dbName = TestDbContextFactory.NewDataBaseName();
            await using (var seedContext = TestDbContextFactory.CreateContext(dbName))
            {
                seedContext.EstadoEntregas.AddRange(
                    CreateEstadoEntrega(id: 1, nombre: "Pendiente"),   
                    CreateEstadoEntrega(id: 2, nombre: "En proceso"),  
                    CreateEstadoEntrega(id: 3, nombre: "Finalizado")   
                );
                await seedContext.SaveChangesAsync();
            }

            await using var context = TestDbContextFactory.CreateContext(dbName);
            var service = new EstadoEntregaService(context);

            // Act
            
            var result = await service.GetList(e => e.Nombre.ToLower().Contains("en"));

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, e => e.EstadoEntregaId == 1);
            Assert.Contains(result, e => e.EstadoEntregaId == 2);
        }

        [Fact]
        public async Task Guardar_CuandoEstadoNoExiste_InsertaYRetornaTrue()
        {
            // Arrange
            await using var context = TestDbContextFactory.CreateContext(
                TestDbContextFactory.NewDataBaseName());
            var service = new EstadoEntregaService(context);
            var nuevoEstado = CreateEstadoEntrega(id: 10, nombre: "Rechazado");

            // Act
            var result = await service.Guardar(nuevoEstado);

            // Assert
            Assert.True(result);
            var saved = await context.EstadoEntregas.FirstOrDefaultAsync(e => e.EstadoEntregaId == 10);
            Assert.NotNull(saved);
            Assert.Equal("Rechazado", saved!.Nombre);
        }

        [Fact]
        public async Task Guardar_CuandoEstadoExiste_ModificaYRetornaTrue()
        {
            // Arrange
            var dbName = TestDbContextFactory.NewDataBaseName();
            await using (var seedContext = TestDbContextFactory.CreateContext(dbName))
            {
                seedContext.EstadoEntregas.Add(CreateEstadoEntrega(id: 20, nombre: "Viejo"));
                await seedContext.SaveChangesAsync();
            }

            await using var context = TestDbContextFactory.CreateContext(dbName);
            var service = new EstadoEntregaService(context);
            var actualizado = CreateEstadoEntrega(id: 20, nombre: "Actualizado");

            // Act
            var result = await service.Guardar(actualizado);

            // Assert
            Assert.True(result);
            var saved = await context.EstadoEntregas.FirstOrDefaultAsync(e => e.EstadoEntregaId == 20);
            Assert.NotNull(saved);
            Assert.Equal("Actualizado", saved!.Nombre);
        }

        [Fact]
        public async Task Existe_CuandoEstadoExiste_RetornaTrue()
        {
            // Arrange
            var dbName = TestDbContextFactory.NewDataBaseName();
            await using (var seedContext = TestDbContextFactory.CreateContext(dbName))
            {
                seedContext.EstadoEntregas.Add(CreateEstadoEntrega(id: 5, nombre: "Test"));
                await seedContext.SaveChangesAsync();
            }

            await using var context = TestDbContextFactory.CreateContext(dbName);
            var service = new EstadoEntregaService(context);

            // Act
            var result = await service.Existe(5);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Eliminar_CuandoExisteEstado_LoBorraYRetornaTrue()
        {
            // Arrange
            var dbName = TestDbContextFactory.NewDataBaseName();
            await using (var seedContext = TestDbContextFactory.CreateContext(dbName))
            {
                seedContext.EstadoEntregas.Add(CreateEstadoEntrega(id: 30, nombre: "Eliminable"));
                await seedContext.SaveChangesAsync();
            }

            await using var context = TestDbContextFactory.CreateContext(dbName);
            var service = new EstadoEntregaService(context);

            // Act
            var result = await service.Eliminar(30);

            // Assert
            Assert.True(result);
            var eliminado = await context.EstadoEntregas.FindAsync(30);
            Assert.Null(eliminado);
        }

        private static EstadoEntrega CreateEstadoEntrega(int id, string nombre)
        {
            return new EstadoEntrega
            {
                EstadoEntregaId = id,
                Nombre = nombre
            };
        }
    }
}