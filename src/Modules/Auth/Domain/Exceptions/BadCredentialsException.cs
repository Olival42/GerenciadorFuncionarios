namespace GerenciadorFuncionarios.Modules.Auth.Domain.Exceptions;

using System;

public class BadCredentialsException : Exception
{
	public BadCredentialsException(string message) 
		: base(message) { }
}
