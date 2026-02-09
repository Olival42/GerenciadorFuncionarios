namespace GerenciadorFuncionarios.Exceptions;

using System;

public class EmailAlreadyExistsException : Exception
{
	public EmailAlreadyExistsException(string message) 
		: base(message) { }
}
