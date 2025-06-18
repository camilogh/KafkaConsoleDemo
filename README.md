---
# Aplicaciones de Consola con Apache Kafka en C#

Este repositorio contiene un ejemplo de aplicaciones de consola en C# para demostrar la comunicación básica con Apache Kafka. Incluye un **Productor** que envía mensajes a un topic de Kafka y un **Consumidor** que lee esos mensajes.
---

## 🚀 Requisitos Previos

Antes de empezar, asegúrate de tener lo siguiente instalado en tu sistema:

- **Docker y Docker Compose:** Necesarios para levantar el servidor Kafka y ZooKeeper.
  - [Instalar Docker Desktop](https://www.docker.com/products/docker-desktop/)
- **.NET SDK (versión 6.0 o superior):** Para compilar y ejecutar las aplicaciones C#.
  - [Descargar .NET SDK](https://dotnet.microsoft.com/download)

---

## 📦 Estructura del Proyecto

```
KafkaConsoleApp/
├── KafkaConsumer/           # Proyecto del Consumidor de Kafka
│   ├── KafkaConsumer.csproj
│   └── Program.cs
├── KafkaProducer/           # Proyecto del Productor de Kafka
│   ├── KafkaProducer.csproj
│   └── Program.cs
└── docker-compose.yml       # Archivo para levantar Kafka y ZooKeeper con Docker Compose
```

---

## ⚙️ Configuración e Inicio

Sigue estos pasos para poner en marcha el entorno y las aplicaciones.

### 1. Levantar el Servidor Kafka con Docker Compose

Primero, necesitamos iniciar los servicios de Kafka y ZooKeeper usando Docker Compose.

1.  Abre tu terminal y navega hasta la raíz del proyecto `KafkaConsoleApp` (donde se encuentra el archivo `docker-compose.yml`).
2.  Ejecuta el siguiente comando para levantar los servicios en segundo plano:

    ```bash
    docker-compose up -d
    ```

    Esto descargará las imágenes de Docker y levantará los contenedores de ZooKeeper y Kafka. Este proceso puede tardar unos minutos la primera vez.

3.  Puedes verificar que los contenedores están corriendo con:

    ```bash
    docker ps
    ```

    Deberías ver `zookeeper` y `broker` en la lista de contenedores activos.

### 2. Compilar los Proyectos C#

Ahora, compila las aplicaciones de productor y consumidor.

1.  Aún en la raíz del proyecto `KafkaConsoleApp` en tu terminal, ejecuta:

    ```bash
    dotnet restore
    dotnet build
    ```

    Esto restaurará las dependencias de NuGet y compilará ambos proyectos.

---

## 🏃 Ejecución y Pruebas

Con Kafka corriendo y los proyectos compilados, podemos ejecutar las aplicaciones y probar diferentes escenarios.

### **Escenario 1: Productor y un Consumidor (Mismo Grupo)**

Este escenario básico muestra cómo un productor envía mensajes y un consumidor los recibe.

1.  **Abre una primera terminal:**
    Navega a `KafkaConsoleApp/KafkaConsumer`.
    Asegúrate de que en `KafkaConsumer/Program.cs`, el `GroupId` esté configurado como:

    ```csharp
    GroupId = "mi-grupo-consumidor",
    ```

    Ejecuta el consumidor:

    ```bash
    dotnet run
    ```

    Verás un mensaje indicando que está escuchando.

2.  **Abre una segunda terminal:**
    Navega a `KafkaConsoleApp/KafkaProducer`.
    Ejecuta el productor:
    ```bash
    dotnet run
    ```
3.  **Envía mensajes:**
    Desde la terminal del productor, escribe mensajes y presiona `Enter`.
    Observa cómo los mensajes aparecen en la terminal del consumidor.

### **Escenario 2: Persistencia de Mensajes (Kafka Retiene Mensajes)**

Demuestra que Kafka retiene los mensajes incluso si no hay consumidores activos.

1.  **Detén el Consumidor del Escenario 1** (presiona `Ctrl+C` en su terminal).
2.  **Desde la terminal del Productor**, envía algunos mensajes adicionales.
3.  **Reinicia el Consumidor** (en la primera terminal, `dotnet run`).
4.  **Observa:** El consumidor recibirá los mensajes que se enviaron mientras estaba inactivo. Esto es clave en Kafka.

### **Escenario 3: Múltiples Consumidores en el Mismo Grupo (Balanceo de Carga)**

Muestra cómo Kafka distribuye los mensajes entre consumidores que pertenecen al **mismo grupo de consumidores**. Cada mensaje será procesado por _solo uno_ de los consumidores del grupo.

1.  **Abre una primera terminal (Consumidor 1 - Grupo A):**
    Navega a `KafkaConsoleApp/KafkaConsumer`.
    Asegúrate de que en `KafkaConsumer/Program.cs`, el `GroupId` esté configurado como:

    ```csharp
    GroupId = "mi-grupo-compartido",
    ```

    Ejecuta:

    ```bash
    dotnet run
    ```

2.  **Abre una segunda terminal (Consumidor 2 - Grupo A):**
    Navega a `KafkaConsoleApp/KafkaConsumer`.
    Asegúrate de que el `GroupId` siga siendo el **mismo**:

    ```csharp
    GroupId = "mi-grupo-compartido",
    ```

    Ejecuta:

    ```bash
    dotnet run
    ```

    Ahora tienes dos consumidores en el mismo grupo.

3.  **Abre una tercera terminal (Productor):**
    Navega a `KafkaConsoleApp/KafkaProducer`.
    Ejecuta:

    ```bash
    dotnet run
    ```

4.  **Envía mensajes desde el productor.**
5.  **Observa:** Los mensajes se alternarán entre el Consumidor 1 y el Consumidor 2. Cada mensaje solo será procesado por uno de ellos.

### **Escenario 4: Múltiples Grupos de Consumidores (Cada Grupo Recibe Todos los Mensajes)**

Este escenario crucial demuestra que cada **grupo de consumidores independiente** recibe una copia completa de todos los mensajes.

1.  **Abre una primera terminal (Consumidor - Grupo A):**
    Navega a `KafkaConsoleApp/KafkaConsumer`.
    Edita `KafkaConsumer/Program.cs` y cambia el `GroupId` a:

    ```csharp
    GroupId = "grupo-A",
    ```

    Guarda y ejecuta:

    ```bash
    dotnet run
    ```

2.  **Abre una segunda terminal (Consumidor - Grupo B):**
    Navega a `KafkaConsoleApp/KafkaConsumer`.
    Edita `KafkaConsumer/Program.cs` y cambia el `GroupId` a:

    ```csharp
    GroupId = "grupo-B",
    ```

    **¡Importante!** Guarda y ejecuta:

    ```bash
    dotnet run
    ```

    Ahora tienes dos consumidores, cada uno en un grupo distinto.

3.  **Abre una tercera terminal (Productor):**
    Navega a `KafkaConsoleApp/KafkaProducer`.
    Ejecuta:

    ```bash
    dotnet run
    ```

4.  **Envía mensajes desde el productor.**
5.  **Observa:** Tanto la terminal del Consumidor del "grupo-A" como la del Consumidor del "grupo-B" deberían recibir _todos los mensajes_ enviados por el productor.

---

## 🧹 Limpieza

Cuando hayas terminado tus pruebas, puedes detener y eliminar los contenedores de Docker para liberar recursos:

1.  Abre tu terminal y navega hasta la raíz del proyecto `KafkaConsoleApp`.
2.  Ejecuta:

    ```bash
    docker-compose down
    ```

    Esto detendrá y eliminará los contenedores de Kafka y ZooKeeper.

---
