using FluentValidation;

namespace Core.Application.Validators;

/// <summary>
/// Validador para ClienteCreateDto usando FluentValidation
/// </summary>
public class ClienteCreateDtoValidator : AbstractValidator<ClienteCreateDto>
{
    /// <summary>
    /// Construtor que define as regras de validação
    /// </summary>
    public ClienteCreateDtoValidator()
    {
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

        // Validação de CPF
        RuleFor(x => x.Cpf)
            .NotEmpty()
            .WithMessage("CPF é obrigatório")
            .Must(ValidarCpf)
            .WithMessage("CPF deve ser um número válido")
            .MaximumLength(14)
            .WithMessage("CPF não pode exceder 14 caracteres");

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

    /// <summary>
    /// Valida CPF
    /// </summary>
    private static bool ValidarCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        // Remove caracteres não numéricos
        var cpfLimpo = System.Text.RegularExpressions.Regex.Replace(cpf, @"\D", "");

        // Deve ter 11 dígitos
        if (cpfLimpo.Length != 11)
            return false;

        // Não pode ser uma sequência de números iguais
        if (cpfLimpo == new string(cpfLimpo[0], 11))
            return false;

        // Validação do primeiro dígito verificador
        int sum = 0;
        for (int i = 0; i < 9; i++)
            sum += int.Parse(cpfLimpo[i].ToString()) * (10 - i);

        int firstDigit = 11 - (sum % 11);
        if (firstDigit > 9)
            firstDigit = 0;

        if (int.Parse(cpfLimpo[9].ToString()) != firstDigit)
            return false;

        // Validação do segundo dígito verificador
        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += int.Parse(cpfLimpo[i].ToString()) * (11 - i);

        int secondDigit = 11 - (sum % 11);
        if (secondDigit > 9)
            secondDigit = 0;

        if (int.Parse(cpfLimpo[10].ToString()) != secondDigit)
            return false;

        return true;
    }
}
