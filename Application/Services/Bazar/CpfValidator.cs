namespace CRMVEXA.Application.Services.Bazar;

public static class CpfValidator
{
    public static string? ApenasDigitos(string? cpf) =>
        string.IsNullOrWhiteSpace(cpf) ? null : new string(cpf.Where(char.IsDigit).ToArray());

    public static bool EhValido(string? cpf)
    {
        var digits = ApenasDigitos(cpf);
        if (digits == null || digits.Length != 11) return false;
        if (digits.Distinct().Count() == 1) return false;

        var nums = digits.Select(c => c - '0').ToArray();
        var sum = 0;
        for (var i = 0; i < 9; i++) sum += nums[i] * (10 - i);
        var d1 = sum % 11;
        d1 = d1 < 2 ? 0 : 11 - d1;
        if (nums[9] != d1) return false;

        sum = 0;
        for (var i = 0; i < 10; i++) sum += nums[i] * (11 - i);
        var d2 = sum % 11;
        d2 = d2 < 2 ? 0 : 11 - d2;
        return nums[10] == d2;
    }

    public static string Formatar(string digits) =>
        $"{digits[..3]}.{digits[3..6]}.{digits[6..9]}-{digits[9..]}";
}
