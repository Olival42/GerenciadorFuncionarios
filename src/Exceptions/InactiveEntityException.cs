namespace GerenciadorFuncionarios.Exceptions;

using System;

public class InactiveEntityException : Exception
{
	public InactiveEntityException() 
		: base("A operação não pode ser realizada: a entidade está inativa.") { }

	public InactiveEntityException(string message) 
		: base(message) { }
}
