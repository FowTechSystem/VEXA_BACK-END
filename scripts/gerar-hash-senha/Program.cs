var senha = args.Length > 0 ? args[0] : "Vexa@2026";
Console.WriteLine(BCrypt.Net.BCrypt.HashPassword(senha));
