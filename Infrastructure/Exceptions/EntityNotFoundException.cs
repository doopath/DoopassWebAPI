namespace Infrastructure.Exceptions;

public class EntityNotFoundException(string message) : InfrastructureBaseException(message);