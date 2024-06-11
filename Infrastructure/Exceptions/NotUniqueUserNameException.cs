namespace Infrastructure.Exceptions;

public class NotUniqueUserNameException(string message) : InfrastructureBaseException(message);