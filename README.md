# AgroMulti - Sistema de Gestión Agrícola

Este es un sistema de escritorio desarrollado en Windows Forms (.NET) orientado a la gestión de actividades agrícolas. Permite organizar productos del campo como cultivos e insumos, con una estructura preparada para escalar hacia inventario, ventas y control de recursos.

El proyecto está inspirado en modelos de agrotiendas reales, con el objetivo de construir una base sólida para una solución completa del sector agrícola.

## Objetivo del sistema

Diseñar una aplicación que permita:

- Gestionar productos agrícolas (cultivos, insumos, etc.)
- Organizar información del campo de forma estructurada
- Servir como base para un sistema completo de inventario y ventas
- Aplicar arquitectura por capas para mejor mantenimiento y escalabilidad

## Arquitectura del proyecto

AgroMulti/

├── AgroMulti.Data (Class Library)  
│   ├── Entities/  
│   │   ├── Producto.cs  
│   │   ├── Cultivo.cs  
│   │   ├── Insumo.cs  
│   │   └── Categoria.cs  
│   └── AgroMultiContext.cs  
│  
├── AgroMulti.Ui (Windows Forms Project)  
│   ├── Services/  
│   │   ├── IProductoService.cs  
│   │   └── ProductoService.cs  
│   ├── Forms/  
│   │   └── MainForm.cs  
│   └── Program.cs  
│  
└── AgroMulti.Tests (xUnit Project)  
    └── ProductoServiceTests.cs  

## Descripción de capas

AgroMulti.Data  
Contiene las entidades del sistema y la lógica de acceso a datos. Aquí se definen los modelos principales del dominio agrícola.

AgroMulti.Ui  
Es la capa de presentación en Windows Forms. Aquí el usuario interactúa con el sistema mediante formularios y servicios.

AgroMulti.Tests  
Contiene las pruebas unitarias para validar la lógica del sistema y asegurar su correcto funcionamiento.

## Enfoque del proyecto

- Arquitectura por capas bien definida  
- Separación de responsabilidades  
- Base escalable para un sistema agrícola completo  
- Preparado para pruebas unitarias  
- Enfoque en mantenibilidad y crecimiento del sistema  

## Estado del proyecto

Actualmente se encuentra en fase inicial de desarrollo, enfocado en la estructura base, organización de capas y definición del dominio agrícola..
