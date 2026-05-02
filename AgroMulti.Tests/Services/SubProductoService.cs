using AgroMulti.Data.Models;
using AgroMulti.Tests.Infraestructura;
using AgroMulti.Ui.Services;
using Microsoft.EntityFrameworkCore;

namespace AgroMulti.Tests
{
    public class SubProductoServiceTest
    {
        [Fact]
        public async Task Buscar_CuandoExisteSubProducto_RetornaEntidad()
        {
            // Arrange
            var dbName = TestDbContextFactory.NewDataBaseName();
            await using (var seedContext = TestDbContextFactory.CreateContext(dbName))
            {
                seedContext.SubProductos.Add(CreateSubProducto(id: 1, nombre: "Sub Café", productoId: 1));
                await seedContext.SaveChangesAsync();
            }

            await using var context = TestDbContextFactory.CreateContext(dbName);
            var service = new SubProductoService(context);

            // Act
            var result = await service.Buscar(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result!.SubProductoId);
            Assert.Equal("Sub Café", result.Nombre);
            Assert.Equal(1, result.ProductoId);
            Assert.Empty(context.ChangeTracker.Entries());
        }

        [Fact]
        public async Task Buscar_CuandoNoExisteSubProducto_RetornaNull()
        {
            // Arrange
            await using var context = TestDbContextFactory.CreateContext(
                TestDbContextFactory.NewDataBaseName());
            var service = new SubProductoService(context);

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
                seedContext.SubProductos.AddRange(
                    CreateSubProducto(id: 1, nombre: "Sub Café", productoId: 1),
                    CreateSubProducto(id: 2, nombre: "Sub Cacao", productoId: 1),
                    CreateSubProducto(id: 3, nombre: "Sub Té", productoId: 2)
                );
                await seedContext.SaveChangesAsync();
            }

            await using var context = TestDbContextFactory.CreateContext(dbName);
            var service = new SubProductoService(context);

            // Act
            var result = await service.GetList(s => s.Nombre.ToLower().Contains("sub"));

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Contains(result, s => s.SubProductoId == 1);
            Assert.Contains(result, s => s.SubProductoId == 2);
            Assert.Contains(result, s => s.SubProductoId == 3);
        }

        [Fact]
        public async Task Guardar_CuandoSubProductoNoExiste_InsertaYRetornaTrue()
        {
            // Arrange
            await using var context = TestDbContextFactory.CreateContext(
                TestDbContextFactory.NewDataBaseName());
            var service = new SubProductoService(context);
            var nuevo = CreateSubProducto(id: 10, nombre: "Sub Naranja", productoId: 5);

            // Act
            var result = await service.Guardar(nuevo);

            // Assert
            Assert.True(result);
            var saved = await context.SubProductos.FirstOrDefaultAsync(s => s.SubProductoId == 10);
            Assert.NotNull(saved);
            Assert.Equal("Sub Naranja", saved!.Nombre);
            Assert.Equal(5, saved.ProductoId);
        }

        [Fact]
        public async Task Guardar_CuandoSubProductoExiste_ModificaYRetornaTrue()
        {
            // Arrange
            var dbName = TestDbContextFactory.NewDataBaseName();
            await using (var seedContext = TestDbContextFactory.CreateContext(dbName))
            {
                seedContext.SubProductos.Add(CreateSubProducto(id: 20, nombre: "Sub Manzana", productoId: 3));
                await seedContext.SaveChangesAsync();
            }

            await using var context = TestDbContextFactory.CreateContext(dbName);
            var service = new SubProductoService(context);
            var actualizado = CreateSubProducto(id: 20, nombre: "Sub Manzana Verde", productoId: 4);

            // Act
            var result = await service.Guardar(actualizado);

            // Assert
            Assert.True(result);
            var saved = await context.SubProductos.FirstOrDefaultAsync(s => s.SubProductoId == 20);
            Assert.NotNull(saved);
            Assert.Equal("Sub Manzana Verde", saved!.Nombre);
            Assert.Equal(4, saved.ProductoId);
        }

        [Fact]
        public async Task Existe_CuandoSubProductoExiste_RetornaTrue()
        {
            // Arrange
            var dbName = TestDbContextFactory.NewDataBaseName();
            await using (var seedContext = TestDbContextFactory.CreateContext(dbName))
            {
                seedContext.SubProductos.Add(CreateSubProducto(id: 5, nombre: "Sub Uva", productoId: 1));
                await seedContext.SaveChangesAsync();
            }

            await using var context = TestDbContextFactory.CreateContext(dbName);
            var service = new SubProductoService(context);

            // Act
            var result = await service.Existe(5);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Eliminar_CuandoExisteSubProducto_LoBorraYRetornaTrue()
        {
            // Arrange
            var dbName = TestDbContextFactory.NewDataBaseName();
            await using (var seedContext = TestDbContextFactory.CreateContext(dbName))
            {
                seedContext.SubProductos.Add(CreateSubProducto(id: 30, nombre: "Sub Pera", productoId: 2));
                await seedContext.SaveChangesAsync();
            }

            await using var context = TestDbContextFactory.CreateContext(dbName);
            var service = new SubProductoService(context);

            // Act
            var result = await service.Eliminar(30);

            // Assert
            Assert.True(result);
            var eliminado = await context.SubProductos.FindAsync(30);
            Assert.Null(eliminado);
        }

        private static SubProducto CreateSubProducto(int id, string nombre, int productoId)
        {
            return new SubProducto
            {
                SubProductoId = id,
                Nombre = nombre,
                ProductoId = productoId
            };
        }
    }
}