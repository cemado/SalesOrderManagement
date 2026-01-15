using System;
using System.Collections.Generic;

namespace WcfService
{
    /// <summary>
    /// Interfaz que define el contrato del servicio WCF  
    /// Este es el ejemplo de arquitectura legacy que será modernizada
    /// 
    /// NOTA: En .NET 8.0 usamos una versión simplificada sin atributos WCF
    /// que mantiene la estructura conceptual de la arquitectura legacy
    /// </summary>
    public interface IOrdenService
    {
        int RegistrarOrden(OrdenWcfDto orden);
        OrdenWcfDto ObtenerOrden(int id);
        List<OrdenWcfDto> ListarOrdenes();
        bool ActualizarOrden(OrdenWcfDto orden);
        bool EliminarOrden(int id);
    }

    public class OrdenWcfDto
    {
        public int Id { get; set; }
        public string Cliente { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public List<DetalleWcfDto> Detalles { get; set; } = new();
        public string Estado { get; set; } = "Pendiente";
    }

    public class DetalleWcfDto
    {
        public int Id { get; set; }
        public int OrdenId { get; set; }
        public string Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }

    public class OrdenFault : Exception
    {
        public string Mensaje { get; set; }
        public int CodigoError { get; set; }

        public OrdenFault(string mensaje, int codigoError = 500) 
            : base(mensaje)
        {
            Mensaje = mensaje;
            CodigoError = codigoError;
        }
    }
}
