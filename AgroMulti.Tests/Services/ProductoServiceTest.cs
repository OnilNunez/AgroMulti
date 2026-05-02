using AgroMulti.Data.Models;
using AgroMulti.Tests.Infraestructura;
using AgroMulti.Ui.Services;
using Microsoft.EntityFrameworkCore;

namespace AgroMulti.Tests
{
    public class ProductoServiceTest
    {
        [Fact]
        public async Task Buscar_CuandoExisteProducto_RetornaEntidad()
        {
            // Arrange
            var dbName = TestDbContextFactory.NewDataBaseName();
            await using (var seedContext = TestDbContextFactory.CreateContext(dbName))
            {
                seedContext.Productos.Add(CreateProducto(id: 1, nombre: "Café"));
                await seedContext.SaveChangesAsync();
            }

            await using var context = TestDbContextFactory.CreateContext(dbName);
            var service = new ProductoService(context);

            // Act
            var result = await service.Buscar(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result!.ProductoId);
            Assert.Equal("Café", result.Nombre);
            Assert.Empty(context.ChangeTracker.Entries());
        }

        [Fact]
        public async Task Buscar_CuandoNoExisteProducto_RetornaNull()
        {
            // Arrange
            await using var context = TestDbContextFactory.CreateContext(
                TestDbContextFactory.NewDataBaseName());
            var service = new ProductoService(context);

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
                seedContext.Productos.AddRange(
                    CreateProducto(id: 1, nombre: "Café"),
                    CreateProducto(id: 2, nombre: "Cacao"),
                    CreateProducto(id: 3, nombre: "Té")
                );
                await seedContext.SaveChangesAsync();
            }

            await using var context = TestDbContextFactory.CreateContext(dbName);
            var service = new ProductoService(context);

            // Act
            var result = await service.GetList(p => p.Nombre.ToLower().Contains("ca"));

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, p => p.ProductoId == 1);
            Assert.Contains(result, p => p.ProductoId == 2);
        }

        [Fact]
        public async Task Guardar_CuandoProductoNoExiste_InsertaYRetornaTrue()
        {
            // Arrange
            await using var context = TestDbContextFactory.CreateContext(
                TestDbContextFactory.NewDataBaseName());
            var service = new ProductoService(context);
            var nuevoProducto = CreateProducto(id: 10, nombre: "Naranja");

            // Act
            var result = await service.Guardar(nuevoProducto);

            // Assert
            Assert.True(result);
            var saved = await context.Productos.FirstOrDefaultAsync(p => p.ProductoId == 10);
            Assert.NotNull(saved);
            Assert.Equal("Naranja", saved!.Nombre);
        }

        [Fact]
        public async Task Guardar_CuandoProductoExiste_ModificaYRetornaTrue()
        {
            // Arrange
            var dbName = TestDbContextFactory.NewDataBaseName();
            await using (var seedContext = TestDbContextFactory.CreateContext(dbName))
            {
                seedContext.Productos.Add(CreateProducto(id: 20, nombre: "Manzana"));
                await seedContext.SaveChangesAsync();
            }

            await using var context = TestDbContextFactory.CreateContext(dbName);
            var service = new ProductoService(context);
            var actualizado = CreateProducto(id: 20, nombre: "Manzana Roja");

            // Act
            var result = await service.Guardar(actualizado);

            // Assert
            Assert.True(result);
            var saved = await context.Productos.FirstOrDefaultAsync(p => p.ProductoId == 20);
            Assert.NotNull(saved);
            Assert.Equal("Manzana Roja", saved!.Nombre);
        }

        [Fact]
        public async Task Existe_CuandoProductoExiste_RetornaTrue()
        {
            // Arrange
            var dbName = TestDbContextFactory.NewDataBaseName();
            await using (var seedContext = TestDbContextFactory.CreateContext(dbName))
            {
                seedContext.Productos.Add(CreateProducto(id: 5, nombre: "Uva"));
                await seedContext.SaveChangesAsync();
            }

            await using var context = TestDbContextFactory.CreateContext(dbName);
            var service = new ProductoService(context);

            // Act
            var result = await service.Existe(5);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Eliminar_CuandoExisteProducto_LoBorraYRetornaTrue()
        {
            // Arrange
            var dbName = TestDbContextFactory.NewDataBaseName();
            await using (var seedContext = TestDbContextFactory.CreateContext(dbName))
            {
                seedContext.Productos.Add(CreateProducto(id: 30, nombre: "Pera"));
                await seedContext.SaveChangesAsync();
            }

            await using var context = TestDbContextFactory.CreateContext(dbName);
            var service = new ProductoService(context);

            // Act
            var result = await service.Eliminar(30);

            // Assert
            Assert.True(result);
            var eliminado = await context.Productos.FindAsync(30);
            Assert.Null(eliminado);
        }

        private static Producto CreateProducto(int id, string nombre)
        {
            return new Producto
            {
                ProductoId = id,
                Nombre = nombre
            };
        }
    }
}