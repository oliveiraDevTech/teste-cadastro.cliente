using FluentValidation;

namespace Core.Application.Validators;

/// <summary>
/// Validador para ClienteUpdateDto usando FluentValidation
/// </summary>
public class ClienteUpdateDtoValidator : AbstractValidator<ClienteUpdateDto>
{
    /// <summary>
    /// Construtor que define as regras de validação
    /// </summary>
    public ClienteUpdateDtoValidator()
    {
        // Validação de ID
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("ID do cliente é obrigatório");

        // Validação de Nome
        RuleFor(x => x.Nome)
            .NotEmpty()
            .WithMessage("Nome é obrigatório")
            .MinimumLength(3)
            .WithMessage("Nome deve ter no mínimo 3 caracteres")
            .MaximumLength(150)
            .WithMessage("Nome não pode exceder 150 caracteres")
            .Matches(@"^[a-zA-ZÀ-ÿ\s]+$")
            .WithMessage("Nome deve conter apenas letras e espaços");

        // Validação de Email
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email é obrigatório")
            .EmailAddress()
            .WithMessage("Email deve ser um endereço de email válido")
            .MaximumLength(150)
            .WithMessage("Email não pode exceder 150 caracteres");

        // Validação de Telefone
        RuleFor(x => x.Telefone)
            .NotEmpty()
            .WithMessage("Telefone é obrigatório")
            .Matches(@"^\(?\d{2}\)?\s?\d{4,5}\-?\d{4}$|^\d{10,11}$")
            .WithMessage("Telefone deve ser um número válido (formato: (11) 99999-9999 ou 11999999999)")
            .MaximumLength(20)
            .WithMessage("Telefone não pode exceder 20 caracteres");

        // Validação de Endereço
        RuleFor(x => x.Endereco)
            .NotEmpty()
            .WithMessage("Endereço é obrigatório")
            .MinimumLength(5)
            .WithMessage("Endereço deve ter no mínimo 5 caracteres")
            .MaximumLength(255)
            .WithMessage("Endereço não pode exceder 255 caracteres");

        // Validação de Cidade
        RuleFor(x => x.Cidade)
            .NotEmpty()
            .WithMessage("Cidade é obrigatória")
            .MinimumLength(3)
            .WithMessage("Cidade deve ter no mínimo 3 caracteres")
            .MaximumLength(100)
            .WithMessage("Cidade não pode exceder 100 caracteres");

        // Validação de Estado
        RuleFor(x => x.Estado)
            .NotEmpty()
            .WithMessage("Estado é obrigatório")
            .Length(2)
            .WithMessage("Estado deve ser uma sigla com 2 caracteres (ex: SP, RJ)")
            .Matches(@"^[A-Z]{2}$")
            .WithMessage("Estado deve conter apenas 2 letras maiúsculas");

        // Validação de CEP
        RuleFor(x => x.Cep)
            .NotEmpty()
            .WithMessage("CEP é obrigatório")
            .Matches(@"^\d{5}\-\d{3}$|^\d{8}$")
            .WithMessage("CEP deve estar no formato XXXXX-XXX ou XXXXXXXX")
            .MaximumLength(9)
            .WithMessage("CEP não pode exceder 9 caracteres");
    }
}
