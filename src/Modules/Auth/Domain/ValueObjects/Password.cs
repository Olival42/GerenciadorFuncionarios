using System.Text.RegularExpressions;

namespace GerenciadorFuncionarios.Domain.ValueObjects;

public sealed class Password
{
    private static readonly Regex _regex = new(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        RegexOptions.Compiled);

    public string Value { get; }

    public Password(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Senha não pode ser vazia.");

        if (!_regex.IsMatch(value))
            throw new ArgumentException(
                "A senha deve conter no mínimo 8 caracteres, uma letra maiúscula, uma minúscula, um número e um caractere especial.");

        Value = value;
    }

    public static implicit operator string(Password password)
        => password.Value;
}