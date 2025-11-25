namespace CRMVEXA.Domain.ValueObjects
{
    public class Endereco
    {
        public string Rua { get; private set; }
        public string Numero { get; private set; }
        public string Bairro { get; private set; }
        public string Cidade { get; private set; }
        public string CEP { get; private set; }

        private Endereco() { }

        public Endereco(string rua, string numero, string bairro, string cidade, string cep)
        {
            Rua = rua;
            Numero = numero;
            Bairro = bairro;
            Cidade = cidade;
            CEP = cep;
        }
    }
}
