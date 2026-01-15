using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WcfService;

namespace WindowsServiceSim
{
    /// <summary>
    /// SIMULACIÓN DE WINDOWS SERVICE
    /// 
    /// En un sistema legacy, los Windows Services eran aplicaciones Windows que:
    /// 1. Se ejecutaban en segundo plano sin interfaz gráfica
    /// 2. Se iniciaban automáticamente al encender el sistema
    /// 3. Se instalaban como servicios del SO usando InstallUtil.exe
    /// 4. Ejecutaban tareas de procesamiento periódico
    /// 
    /// PROBLEMAS DE WINDOWS SERVICES LEGACY:
    /// - Difíciles de debuggear
    /// - Requieren privilegio de administrador
    /// - No tienen salida visible (logging complejo)
    /// - Acoplamiento al SO Windows
    /// 
    /// MODERNIZACIÓN A ARQUITECTURA ACTUAL:
    /// - BackgroundService de .NET Core/5+
    /// - Hosted Services en ASP.NET Core
    /// - Azure Functions o Azure Service Bus
    /// - Kubernetes Jobs o CronJobs
    /// - AWS Lambda con EventBridge
    /// 
    /// Este programa simula el comportamiento: ejecutar código cada 30 segundos
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Instancia global del servicio WCF
        /// En un sistema real, esto vendría de DI o de una conexión remota a WCF
        /// </summary>
        private static OrdenService _ordenService;

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            PrintBanner();
            
            try
            {
                // Inicializar servicio
                _ordenService = new OrdenService();

                // Registrar órdenes de prueba
                await RegistrarOrdenesIniciales();

                // Iniciar procesamiento periódico
                await IniciarProcesamiento();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[ERROR FATAL] {ex.Message}");
                Console.ResetColor();
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Imprime el banner del servicio
        /// </summary>
        static void PrintBanner()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                                                                  ║");
            Console.WriteLine("║   SIMULACIÓN DE WINDOWS SERVICE - PROCESADOR DE ÓRDENES         ║");
            Console.WriteLine("║   (Arquitectura Legacy: Tareas en Segundo Plano)                 ║");
            Console.WriteLine("║                                                                  ║");
            Console.WriteLine("║   Procesando cada 30 segundos...                                ║");
            Console.WriteLine("║   Presione Ctrl+C para detener                                   ║");
            Console.WriteLine("║                                                                  ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
        }

        /// <summary>
        /// Registra órdenes de prueba en el servicio WCF
        /// </summary>
        static async Task RegistrarOrdenesIniciales()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("➤ Registrando órdenes iniciales de prueba...\n");
            Console.ResetColor();

            try
            {
                // Orden 1
                var orden1 = new OrdenWcfDto
                {
                    Cliente = "Acme Corporation",
                    Fecha = DateTime.Now,
                    Detalles = new List<DetalleWcfDto>
                    {
                        new DetalleWcfDto 
                        { 
                            Producto = "Widget Standard", 
                            Cantidad = 5, 
                            PrecioUnitario = 100m 
                        },
                        new DetalleWcfDto 
                        { 
                            Producto = "Widget Premium", 
                            Cantidad = 3, 
                            PrecioUnitario = 150m 
                        }
                    }
                };

                var id1 = _ordenService.RegistrarOrden(orden1);

                // Orden 2
                var orden2 = new OrdenWcfDto
                {
                    Cliente = "Global Industries",
                    Fecha = DateTime.Now,
                    Detalles = new List<DetalleWcfDto>
                    {
                        new DetalleWcfDto 
                        { 
                            Producto = "Component A", 
                            Cantidad = 10, 
                            PrecioUnitario = 50m 
                        },
                        new DetalleWcfDto 
                        { 
                            Producto = "Component B", 
                            Cantidad = 7, 
                            PrecioUnitario = 75m 
                        },
                        new DetalleWcfDto 
                        { 
                            Producto = "Service Fee", 
                            Cantidad = 1, 
                            PrecioUnitario = 200m 
                        }
                    }
                };

                var id2 = _ordenService.RegistrarOrden(orden2);

                // Orden 3
                var orden3 = new OrdenWcfDto
                {
                    Cliente = "Tech Solutions Inc",
                    Fecha = DateTime.Now,
                    Detalles = new List<DetalleWcfDto>
                    {
                        new DetalleWcfDto 
                        { 
                            Producto = "License Bundle", 
                            Cantidad = 1, 
                            PrecioUnitario = 5000m 
                        }
                    }
                };

                var id3 = _ordenService.RegistrarOrden(orden3);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"✓ {id1}, {id2}, {id3} órdenes iniciales registradas exitosamente\n");
                Console.ResetColor();

                await Task.Delay(500); // Pequeña pausa para claridad
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ Error al registrar órdenes iniciales: {ex.Message}\n");
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Inicia el procesamiento periódico cada 30 segundos
        /// Este es el comportamiento típico de un Windows Service
        /// </summary>
        static async Task IniciarProcesamiento()
        {
            int ciclo = 1;
            
            // Usar PeriodicTimer (recomendado en .NET 6+)
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(30));

            // Mostrar información del sistema
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"[Hora de inicio] {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"[Thread] {System.Threading.Thread.CurrentThread.ManagedThreadId}");
            Console.ResetColor();
            Console.WriteLine();

            // Bucle de procesamiento
            while (await timer.WaitForNextTickAsync())
            {
                await ProcesarOrdenesAsync(ciclo);
                ciclo++;
            }
        }

        /// <summary>
        /// Procesa las órdenes pendientes
        /// Este es el trabajo que hace el Windows Service cada 30 segundos
        /// 
        /// EN UN SISTEMA REAL:
        /// - Consultaría BD para órdenes en estado "Pendiente"
        /// - Ejecutaría lógica de negocio (validaciones, cálculos)
        /// - Actualizaría estado a "Procesado"
        /// - Registraría en auditoría
        /// - Notificaría a otros sistemas (vía WCF, API, Eventos)
        /// </summary>
        static async Task ProcesarOrdenesAsync(int ciclo)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ▶ CICLO #{ciclo}: Iniciando procesamiento de órdenes...");
                Console.ResetColor();

                // Obtener todas las órdenes del servicio WCF
                var ordenes = _ordenService.ListarOrdenes();

                if (!ordenes.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("  ✓ No hay órdenes pendientes");
                    Console.ResetColor();
                    Console.WriteLine();
                    return;
                }

                var ordenesPendientes = ordenes.Where(o => o.Estado == "Pendiente").ToList();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  ✓ {ordenesPendientes.Count} orden(es) pendiente(s) encontrada(s)");
                Console.ResetColor();

                // Procesar cada orden
                int procesadas = 0;
                foreach (var orden in ordenesPendientes)
                {
                    Console.WriteLine($"    ┌─ Procesando orden #{orden.Id}");
                    Console.WriteLine($"    │  Cliente: {orden.Cliente}");
                    Console.WriteLine($"    │  Fecha: {orden.Fecha:yyyy-MM-dd HH:mm:ss}");
                    Console.WriteLine($"    │  Total: ${orden.Total:N2}");
                    Console.WriteLine($"    │  Detalles: {orden.Detalles.Count}");

                    // Simular procesamiento
                    await Task.Delay(500);

                    // Cambiar estado a Procesado
                    orden.Estado = "Procesado";
                    _ordenService.ActualizarOrden(orden);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"    └─ ✓ Orden #{orden.Id} procesada correctamente");
                    Console.ResetColor();

                    procesadas++;
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n✓ {procesadas} orden(es) procesada(s) en este ciclo\n");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"✗ [ERROR] Ciclo #{ciclo}: {ex.Message}\n");
                Console.ResetColor();
            }
        }
    }
}
