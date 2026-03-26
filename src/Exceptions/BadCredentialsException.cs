namespace GerenciadorFuncionarios.Exceptions;

using System;

public class BadCredentialsException : Exception
{
	public BadCredentialsException(string message) 
		: base(message) { }
}
