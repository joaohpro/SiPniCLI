using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckerSipni.Models
{
    public class RespostasModel
    {
        public class CredentialToken
        {
            public string Credential { get; set; }
            public string Token { get; set; }

            public CredentialToken(string credential, string token)
            {
                Credential = credential;
                Token = token;
            }
        }

        // O token é gerado apos o login, credenciais devem ser codificadas formato (USER:PASS)
        // e codificadas em base64 e colocalas no header X-Authorization: Basic XXX==
        public class LoginToken
        {
            public string? accessToken { get; set; }
            public string? tokenType { get; set; }
            public string? refreshToken { get; set; }
            public string? scope { get; set; }
            public string? organization { get; set; }
            public string? jti { get; set; }
        }

        // DADOS RESPOSTA
        public class Endereco
        {
            public int tipoEndereco { get; set; }
            public string? logradouro { get; set; }
            public string? numero { get; set; }
            public string? bairro { get; set; }
            public string? municipio { get; set; }
            public string? siglaUf { get; set; }
            public string? pais { get; set; }
            public string? cep { get; set; }
            public string? complemento { get; set; }
        }

        public class Nacionalidade
        {
            public int nacionalidade { get; set; }
            public string? municipioNascimento { get; set; }
            public string? paisNascimento { get; set; }
        }

        public class Record
        {
            public string? cnsDefinitivo { get; set; }
            public List<string>? cnsProvisorio { get; set; }
            public string? nome { get; set; }
            public string? cpf { get; set; }
            public string? dataNascimento { get; set; }
            public string? sexo { get; set; }
            public string? nomeMae { get; set; }
            public string? nomePai { get; set; }
            public int grauQualidade { get; set; }
            public bool ativo { get; set; }
            public bool obito { get; set; }
            public bool partoGemelar { get; set; }
            public bool vip { get; set; }
            public string? racaCor { get; set; }
            public List<Telefone>? telefone { get; set; }
            public Nacionalidade? nacionalidade { get; set; }
            public Endereco? endereco { get; set; }
        }

        // Classe onde vai conter todos dados
        public class Root
        {
            public List<Record>? records { get; set; }
        }

        public class Telefone
        {
            public string? ddi { get; set; }
            public string? ddd { get; set; }
            public string? numero { get; set; }
            public int tipo { get; set; }
            public override string ToString()
            {
                return $"{ddd}-{numero}";
            }
        }
    }
}
