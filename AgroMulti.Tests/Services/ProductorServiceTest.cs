using AgroMulti.Data.Models;
using AgroMulti.Tests.Infraestructura;
using AgroMulti.Ui.Services;
using Microsoft.EntityFrameworkCore;

namespace AgroMulti.Tests
{
    public class ProductorServiceTest
    {
        [Fact]
        public async Task Buscar_CuandoExisteProductor_RetornaEntidad()
        {
            // Arrange
            var dbName = TestDbContextFactory.NewDataBaseName();
            await using (var seedContext = TestDbContextFactory.CreateContext(dbName))
            {
                seedContext.Productors.Add(CreateProductor(id: 1, codigo: "P001", nombre: "Juan", apellido: "Pérez"));
                await seedContext.SaveChangesAsync();
            }

            await using var context = TestDbContextFactory.CreateContext(dbName);
            var service = new ProductorService(context);

            // Act
            var result = await service.Buscar(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result!.ProductorId);
            Assert.Equal("P001", result.Codigo);
            Assert.Equal("Juan", result.Nombre);
            Assert.Equal("Pérez", result.Apellido);
            Assert.Empty(context.ChangeTracker.Entries());
        }

        [Fact]
        public async Task Buscar_CuandoNoExisteProductor_RetornaNull()
        {
            // Arrange
            await using var context = TestDbContextFactory.CreateContext(
                TestDbContextFactory.NewDataBaseName());
            var service = new ProductorService(context);

            // Act
            var result = await service.Buscar(99);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetList_CuandoSeFiltraPorApellido_RetornaCoincidencias()
        {
            // Arrange
            var dbName = TestDbContextFactory.NewDataBaseName();
            await using (var seedContext = TestDbContextFactory.CreateContext(dbName))
            {
                seedContext.Productors.AddRange(
                    CreateProductor(id: 1, codigo: "P001", nombre: "Juan", apellido: "López"),
                    CreateProductor(id: 2, codigo: "P002", nombre: "María", apellido: "López"),
                    CreateProductor(id: 3, codigo: "P003", nombre: "Carlos", apellido: "García")
                );
                await seedContext.SaveChangesAsync();
            }

            await using var context = TestDbContextFactory.CreateContext(dbName);
            var service = new ProductorService(context);

            // Act
            var result = await service.GetList(p => p.Apellido.ToLower().Contains("lópez"));

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, p => p.ProductorId == 1);
            Assert.Contains(result, p => p.ProductorId == 2);
        }

        [Fact]
        public async Task Guardar_CuandoProductorNoExiste_InsertaYRetornaTrue()
        {
            // Arrange
            await using var context = TestDbContextFactory.CreateContext(
                TestDbContextFactory.NewDataBaseName());
            var service = new ProductorService(context);
            var nuevoProductor = CreateProductor(id: 10, codigo: "P010", nombre: "Ana", apellido: "Martínez");

            // Act
            var result = await service.Guardar(nuevoProductor);

            // Assert
            Assert.True(result);
            var saved = await context.Productors.FirstOrDefaultAsync(p => p.ProductorId == 10);
            Assert.NotNull(saved);
            Assert.Equal("Ana", saved!.Nombre);
            Assert.Equal("Martínez", saved.Apellido);
        }

        [Fact]
        public async Task Guardar_CuandoProductorExiste_ModificaYRetornaTrue()
        {
            // Arrange
            var dbName = TestDbContextFactory.NewDataBaseName();
            await using (var seedContext = TestDbContextFactory.CreateContext(dbName))
            {
                seedContext.Productors.Add(CreateProductor(id: 20, codigo: "P020", nombre: "Luis", apellido: "Ramírez"));
                await seedContext.SaveChangesAsync();
            }

            await using var context = TestDbContextFactory.CreateContext(dbName);
            var service = new ProductorService(context);
            var actualizado = CreateProductor(id: 20, codigo: "P020", nombre: "Luis", apellido: "Ramírez Modificado");

            // Act
            var result = await service.Guardar(actualizado);

            // Assert
            Assert.True(result);
            var saved = await context.Productors.FirstOrDefaultAsync(p => p.ProductorId == 20);
            Assert.NotNull(saved);
            Assert.Equal("Ramírez Modificado", saved!.Apellido);
        }

        [Fact]
        public async Task Existe_CuandoProductorExiste_RetornaTrue()
        {
            // Arrange
            var dbName = TestDbContextFactory.NewDataBaseName();
            await using (var seedContext = TestDbContextFactory.CreateContext(dbName))
            {
                seedContext.Productors.Add(CreateProductor(id: 5, codigo: "P005", nombre: "Elena", apellido: "Torres"));
                await seedContext.SaveChangesAsync();
            }

            await using var context = TestDbContextFactory.CreateContext(dbName);
            var service = new ProductorService(context);

            // Act
            var result = await service.Existe(5);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Eliminar_CuandoExisteProductor_LoBorraYRetornaTrue()
        {
            // Arrange
            var dbName = TestDbContextFactory.NewDataBaseName();
            await using (var seedContext = TestDbContextFactory.CreateContext(dbName))
            {
                seedContext.Productors.Add(CreateProductor(id: 30, codigo: "P030", nombre: "Pedro", apellido: "Sánchez"));
                await seedContext.SaveChangesAsync();
            }

            await using var context = TestDbContextFactory.CreateContext(dbName);
            var service = new ProductorService(context);

            // Act
            var result = await service.Eliminar(30);

            // Assert
            Assert.True(result);
            var eliminado = await context.Productors.FindAsync(30);
            Assert.Null(eliminado);
        }

        private static Productor CreateProductor(int id, string codigo, string nombre, string apellido)
        {
            return new Productor
            {
                ProductorId = id,
                Codigo = codigo,
                Nombre = nombre,
                Apellido = apellido
            };
        }
    }
}