namespace Domain.Exceptions
{
    /// <summary>
    /// Excepción base para errores de dominio
    /// </summary>
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
        public DomainException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Excepción para errores de validación de negocio
    /// </summary>
    public class ValidationException : DomainException
    {
        public ValidationException(string message) : base(message) { }
    }

    /// <summary>
    /// Excepción cuando un recurso no existe
    /// </summary>
    public class NotFoundException : DomainException
    {
        public NotFoundException(string message) : base(message) { }
    }

    /// <summary>
    /// Excepción cuando hay conflicto de datos (p.ej. orden duplicada)
    /// </summary>
    public class ConflictException : DomainException
    {
        public ConflictException(string message) : base(message) { }
    }
}
