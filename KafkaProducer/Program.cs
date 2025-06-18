using Confluent.Kafka;
using System;
using System.Threading.Tasks;

namespace KafkaProducer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Iniciando Productor de Kafka...");

            // Configuración del productor
            var config = new ProducerConfig { BootstrapServers = "localhost:9092" }; // Dirección del broker de Kafka

            // Nombre del topic al que enviaremos los mensajes
            // string topicName = "mi-primer-topic";
            Console.Write("Ingrese el nombre del topic (o presione Enter para usar 'mi-primer-topic'): ");
            string topicName = Console.ReadLine() ?? "mi-primer-topic"; // Permite al usuario ingresar el nombre del topic

            // Usamos un bloque 'using' para asegurar que el productor se libere correctamente
            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                Console.WriteLine($" [!] Productor conectado a Kafka. Enviando mensajes al topic: '{topicName}'");
                Console.WriteLine(" Escribe un mensaje y presiona Enter para enviarlo. Escribe 'salir' para terminar.");

                string message;
                while ((message = Console.ReadLine()) != null && message.ToLower() != "salir")
                {
                    try
                    {
                        // Enviar el mensaje de forma asíncrona
                        // Key: Null (no estamos usando claves para particionar en este ejemplo simple)
                        // Value: El mensaje en sí (string)
                        var deliveryResult = await producer.ProduceAsync(topicName, new Message<Null, string> { Value = message });

                        // deliveryResult contiene información sobre la entrega del mensaje
                        Console.WriteLine($" [✓] Mensaje enviado a la partición: {deliveryResult.Partition.Value} @ offset: {deliveryResult.Offset.Value}");
                    }
                    catch (ProduceException<Null, string> e)
                    {
                        Console.WriteLine($" [X] Error al enviar mensaje: {e.Error.Reason}");
                    }
                }
            }

            Console.WriteLine("Productor de Kafka terminado.");
        }
    }
}