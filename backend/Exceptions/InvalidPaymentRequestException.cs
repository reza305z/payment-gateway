namespace PaymentApi.Exceptions;

public class InvalidPaymentRequestException(string message) : Exception(message)
{
}