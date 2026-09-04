namespace PaymentApi.Exceptions;

public class PaymentTransactionNotFoundException(string message) : Exception(message)
{
}