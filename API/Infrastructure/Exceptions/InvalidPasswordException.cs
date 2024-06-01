namespace Doopass.API.Infrastructure.Exceptions;

public class InvalidPasswordException(string message) : InfrastructureBaseException(message);