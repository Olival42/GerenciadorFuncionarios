using System.Text.RegularExpressions;

namespace GerenciadorFuncionarios.Domain.ValueObjects;

public sealed class Phone
{
    public string Value { get; }

    public Phone(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Telefone não pode ser vazio.");

        value = Normalize(value);

        if (!IsValid(value))
            throw new ArgumentException("Telefone inválido.");

        Value = value;
    }

    public static implicit operator string(Phone phone)
        => phone.Value;

    private static string Normalize(string input)
        => Regex.Replace(input, @"\D", "");

    private static bool IsValid(string phone)
    {
        return phone.Length == 10 || phone.Length == 11;
    }
}