using Confluent.Kafka;
using System;
using System.Threading;

namespace KafkaConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Iniciando Consumidor de Kafka...");

            // Configuración del consumidor
            int idguest = DateTime.Now.Second;
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092", // Dirección del broker de Kafka
                GroupId = $"mi-grupo-consumidor-{idguest}",    // Un ID de grupo para este consumidor
                AutoOffsetReset = AutoOffsetReset.Earliest // Empieza a leer desde el principio del topic si no hay offset guardado
            };

            // Nombre del topic del que leeremos los mensajes
            Console.Write("Ingrese el nombre del topic (o presione Enter para usar 'mi-primer-topic'): ");
            string topicName = Console.ReadLine() ?? "mi-primer-topic";

            // Usamos un bloque 'using' para asegurar que el consumidor se libere correctamente
            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                // Suscribirse al topic
                consumer.Subscribe(topicName);

                Console.WriteLine($" [!] Consumidor conectado a Kafka. Escuchando en el topic: '{topicName}'");
                Console.WriteLine(" Presiona Ctrl+C para salir.");

                // Usamos un CancellationTokenSource para manejar la cancelación (Ctrl+C)
                var cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) =>
                {
                    e.Cancel = true; // Previene que el proceso se termine inmediatamente
                    cts.Cancel();    // Señaliza la cancelación
                };

                try
                {
                    while (true)
                    {
                        try
                        {
                            // Esperar por un mensaje (o tiempo de espera si no hay mensajes)
                            var consumeResult = consumer.Consume(cts.Token);

                            // Cuando se recibe un mensaje
                            Console.WriteLine($" [x] Mensaje recibido de la partición: {consumeResult.Partition.Value}, offset: {consumeResult.Offset.Value} - '{consumeResult.Message.Value}'");
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($" [X] Error al consumir: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Esto se dispara cuando se presiona Ctrl+C y cts.Cancel() es llamado
                    Console.WriteLine(" [!] Consumo cancelado.");
                }
                finally
                {
                    // Asegúrate de que el consumidor deje de leer y se desuscriba
                    consumer.Close();
                }
            }

            Console.WriteLine("Consumidor de Kafka terminado.");
        }
    }
}