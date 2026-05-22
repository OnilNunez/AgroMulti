## AgroMulti - Sistema de Gestión Agrícola
<img width="940" height="501" alt="image" src="https://github.com/user-attachments/assets/386ed054-32fe-4b8f-b802-3920f3c043e9" />
AgroMulti es un sistema de escritorio desarrollado en Windows Forms (.NET) orientado a la gestión, control y trazabilidad de entregas agrícolas. El proyecto está pensado para organizar de forma estructurada la relación entre productores, productos, subproductos, estados de entrega y auditoría de procesos, con una base técnica preparada para crecer hacia módulos de inventario, análisis y reportes ejecutivos.

El sistema está inspirado en flujos reales de centros de acopio y procesos de fermentación/secado, con el objetivo de resolver el desorden administrativo, mejorar la trazabilidad y profesionalizar la gestión agrícola.

## Objetivo del sistema:

Diseñar una aplicación que permita:

- Gestionar productores, productos, subproductos y entregas
- Controlar el ciclo de recepción, proceso y finalización de entregas
- Registrar la ubicación física de cada entrega dentro del almacén
- Mantener trazabilidad completa de cambios de estado
- Generar reportes, exportaciones y análisis visuales
- Aplicar arquitectura por capas para facilitar mantenimiento y escalabilidad

<img width="940" height="499" alt="image" src="https://github.com/user-attachments/assets/9f65f6e1-46ed-4ca1-8c0a-486c409e1bcb" />
<img width="371" height="315" alt="image" src="https://github.com/user-attachments/assets/4702e598-b349-4c6f-8d71-4191859fc639" />


## Arquitectura del proyecto

```text
AgroMulti/
├── AgroMulti.Data (Class Library)
│   ├── Entities/
│   │   ├── Productor.cs
│   │   ├── Producto.cs
│   │   ├── SubProducto.cs
│   │   ├── EstadoEntrega.cs
│   │   ├── Entrega.cs
│   │   └── HistoricoEstadoEntrega.cs
│   ├── AgroMultiContext.cs
│   └── App.config
│
├── AgroMulti.Ui (Windows Forms Project)
│   ├── Forms/
│   │   ├── MainMenu.cs
│   │   ├── RegistroEntregaForm.cs
│   │   ├── ConsultaEntregasForm.cs
│   │   ├── DashboardAnalisisForm.cs
│   │   ├── ProductoresForm.cs
│   │   ├── ProductorDetalleForm.cs
│   │   ├── HistoricoEstadosForm.cs
│   │   └── AcercaDeForm.cs
│   ├── Services/
│   │   ├── ProductorService.cs
│   │   ├── ProductoService.cs
│   │   ├── SubProductoService.cs
│   │   ├── EstadoEntregaService.cs
│   │   ├── EntregaService.cs
│   │   └── HistoricoEstadoEntregaService.cs
│   └── Program.cs
│
└── AgroMulti.Tests (xUnit Project)
    ├── ProductorServiceTest.cs
    ├── ProductoServiceTest.cs
    ├── SubProductoServiceTest.cs
    ├── EstadoEntregaServiceTest.cs
    ├── EntregaServiceTest.cs
    ├── HistoricoEstadoEntregaServiceTest.cs
    └── TestDbContextFactory.cs
```

## Descripción de capas:

### AgroMulti.Data

Contiene la estructura del dominio y la persistencia en SQL Server. Aquí se definen las entidades principales del sistema, las relaciones entre ellas y la configuración del contexto de Entity Framework Core.

### AgroMulti.Ui

Es la capa de presentación en Windows Forms. Desde aquí el usuario interactúa con el sistema mediante formularios especializados para registrar entregas, consultar información, administrar productores, visualizar estadísticas y exportar reportes.

### AgroMulti.Tests

Contiene las pruebas unitarias del sistema. Su propósito es validar la lógica de negocio, evitar regresiones y asegurar que los procesos críticos funcionen correctamente.

## Componentes principales del sistema

### 1. Capa de Presentación (AgroMulti.Ui) - Formularios

La interfaz de usuario está compuesta por 8 formularios especializados:

1. **MainMenu.cs**: Panel central de la aplicación. Gestiona la navegación principal mediante un menú lateral y permite abrir los demás módulos sin cerrar la aplicación base.
2. **RegistroEntregaForm.cs**: Formulario principal para registrar entregas. Maneja la selección de productores por código, la vinculación de productos y subproductos, y la asignación de coordenadas de almacén como pasillos, anaqueles y piso.
3. **ConsultaEntregasForm.cs**: Centro de búsqueda avanzada. Permite filtrar entregas por fecha, productor y estado. Incluye exportación a Excel y PDF, además del acceso al cambio de estado.
4. **DashboardAnalisisForm.cs**: Módulo de análisis y Business Intelligence. Genera gráficos estadísticos y reportes ejecutivos.
5. **ProductoresForm.cs**: Gestión del catálogo de productores. Permite listar, buscar en tiempo real y eliminar productores con validaciones de integridad.
6. **ProductorDetalleForm.cs**: Formulario dual para crear y editar productores. Calcula automáticamente el siguiente código institucional.
7. **HistoricoEstadosForm.cs**: Visor de auditoría. Muestra el historial de movimientos de cada entrega con filtros por rango de tiempo.
8. **AcercaDeForm.cs**: Ventana informativa del sistema, con versión y créditos de desarrollo.

### 2. Capa de Datos (AgroMulti.Data) - Modelos

La capa de datos define la estructura del dominio y la persistencia:

1. **Productor.cs**: Representa al proveedor. Usa un código único tipo `PROD-00001`.
2. **Producto.cs**: Representa la categoría principal de materia prima, como cacao o café.
3. **SubProducto.cs**: Variedad específica asociada a un producto.
4. **EstadoEntrega.cs**: Catálogo de estados del proceso de entrega.
5. **Entrega.cs**: Entidad central del sistema. Almacena kilos, ubicación física, datos del conductor y relaciones con las demás entidades.
6. **HistoricoEstadoEntrega.cs**: Entidad de auditoría que registra la línea de tiempo de cambios de estado.

### 3. Capa de Lógica de Negocio (Servicios)

Implementan el patrón de servicios y la lógica de procesamiento:

1. **ProductorService.cs**: Gestiona la lógica de productores y validación de códigos.
2. **ProductoService.cs**: Maneja el catálogo de productos principales.
3. **SubProductoService.cs**: Filtra y administra las variedades dependientes de un producto.
4. **EstadoEntregaService.cs**: Provee los estados disponibles para la máquina de estados.
5. **EntregaService.cs**: Gestiona pesajes, relaciones complejas y procesamiento de entregas.
6. **HistoricoEstadoEntregaService.cs**: Especializado en inserción y consulta del historial de auditoría.

### 4. Capa de Calidad (AgroMulti.Tests) - Pruebas

Asegura el correcto funcionamiento del sistema mediante pruebas unitarias con xUnit:

1. **ProductorServiceTest.cs**: Verifica que los códigos no se dupliquen y que la búsqueda por nombre funcione.
2. **ProductoServiceTest.cs**: Valida el CRUD básico de productos.
3. **SubProductoServiceTest.cs**: Verifica la relación entre Producto y SubProducto.
4. **EstadoEntregaServiceTest.cs**: Asegura que los estados base existan correctamente.
5. **EntregaServiceTest.cs**: Valida que los kilos se graben correctamente y que las relaciones con productores sean íntegras.
6. **HistoricoEstadoEntregaServiceTest.cs**: Verifica que cada cambio de estado genere un registro de auditoría inmutable.

## Infraestructura y configuración

* **AgroMultiContext.cs**: Configuración de Entity Framework Core, relaciones y cadena de conexión.
* **Program.cs**: Punto de entrada de la aplicación y configuración de inyección de dependencias.
* **TestDbContextFactory.cs**: Fábrica de contextos en memoria para ejecutar pruebas sin afectar la base real.
* **App.config**: Almacena la cadena de conexión `AgroMultiConnection`.

## Principales funcionalidades del proyecto

### Gestión del ciclo de transformación

Permite registrar el peso de entrada y el peso final luego del proceso. Esto ayuda a calcular rendimientos y mermas de forma más precisa.

### Trazabilidad de estados y auditoría

Cada entrega pasa por una máquina de estados y cada cambio queda registrado en un historial que no se puede alterar fácilmente.

### Control físico de almacenamiento

El sistema asigna ubicación exacta dentro del almacén mediante pasillo, anaquel y piso, reduciendo errores y pérdidas de tiempo.

### Exportación y reportes

Genera exportaciones en Excel y PDF con formato profesional, además de dashboards visuales para análisis ejecutivo.

### Integridad de datos

Incluye validaciones para evitar duplicados, impedir eliminaciones inseguras y bloquear modificaciones indebidas en registros finalizados.

### Estandarización de productores

Cada productor recibe un código único e institucional, lo que evita duplicidad de identidad y mejora el control de la información.

## Qué viene a solucionar

AgroMulti viene a resolver el desorden administrativo y operativo de los centros de acopio agrícola. En lugar de depender de cuadernos, registros manuales o archivos dispersos, el sistema centraliza la información en una estructura organizada, trazable y confiable.

Con esto se logra:

* Mejor control de entregas
* Menos errores humanos
* Mayor trazabilidad de procesos
* Ubicación exacta de inventario físico
* Reportes más rápidos y profesionales
* Mejor toma de decisiones basada en datos

## Estado del proyecto

Actualmente AgroMulti se encuentra en una etapa avanzada de desarrollo funcional, con una arquitectura sólida por capas, formularios especializados, servicios de negocio, modelos bien definidos y pruebas unitarias para asegurar estabilidad y crecimiento futuro.

## Conclusión

AgroMulti no es solo un sistema de registro, sino una solución integral para la gestión, trazabilidad y control de procesos agrícolas. Su diseño técnico permite mantener orden, mejorar la eficiencia operativa y construir una base escalable para una plataforma completa del sector agroindustrial.

