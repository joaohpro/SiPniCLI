/*
 * DESENVOLVIDO POR João H.
 * Versao do SI-PNI para sistemas sem interface grafica
 * Ética sempre (:
*/

using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Web;
using System.Linq;
using static CheckerSipni.Models.RespostasModel;
using System.Globalization;
using CsvHelper;

namespace CheckerSipni
{
    class Program
    {
        static string APITOKEN = "https://servicos-cloud.saude.gov.br/pni-bff/v1/autenticacao/tokenAcesso";
        static string APICONSULTACPF = "https://servicos-cloud.saude.gov.br/pni-bff/v1/cidadao/cpf";
        static HttpClient Client = new HttpClient();
        static string ASCIIART = @"
 ____  _   _ ___ ____ _ _ 
|  _ \| \ | |_ _/ ___| (_)
| |_) |  \| || | |   | | |
|  __/| |\  || | |___| | |
|_|   |_| \_|___\____|_|_|
By https://joaoh.netlify.app
Versao do SI-PNI para sistemas sem interface grafica
";

        static async Task Main()
        {
            string tokenValido = "";

            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(ASCIIART);
                Console.ResetColor();

                // Isso está ridículo, mas blz kkkk
                Console.WriteLine("======== MENU ========");
                Console.WriteLine("1. Consulta por Nome");
                Console.WriteLine("2. Consulta por CPF");
                Console.WriteLine("3. Consulta por Telefone");
                Console.WriteLine("4. Ver codigos de municipios");
                Console.WriteLine("0. Sair");
                Console.WriteLine("======================");
                Console.Write("Escolha uma opção: ");
                var option = int.Parse(Console.ReadLine());

                if (option != 0)
                {
                    Console.WriteLine("[*] Gerando token valido...");
                    tokenValido = await GerarTokenValido();
                }

                // memento júnior
                switch (option)
                {
                    case 1:
                        Console.WriteLine("Nome: ");
                        var nome = Console.ReadLine();
                        Console.WriteLine("Codigo Municipio: ");
                        var codMun = Console.ReadLine();

                        Console.WriteLine("[*] Buscando dados...");
                        if (!string.IsNullOrEmpty(nome.Trim()) && !string.IsNullOrEmpty(codMun.Trim()))
                        {

                            var resNome = await ConsultaNome(tokenValido, nome, codMun);
                            Resultado(resNome);
                            Console.WriteLine("Tecle enter para voltar...");
                            Console.ReadLine();
                        }
                        else
                        {
                            Console.WriteLine("Esta faltando coisa ai amigo...");
                            Console.ReadLine();
                        }
                        break;
                    case 2:
                        Console.WriteLine("CPF: ");
                        var cpf = Console.ReadLine();
                        Console.WriteLine("[*] Buscando dados...");
                        var resCpf = await ConsultaCPF(tokenValido, cpf);
                        Resultado(resCpf);
                        Console.WriteLine("Tecle enter para voltar...");
                        Console.ReadLine();
                        break;
                    case 3:
                        Console.WriteLine("Nao implementado.");
                        Console.ReadLine();
                        break;
                    case 4:
                        Console.WriteLine("Pesquise o código do seu municipio na tabela.");
                        Console.WriteLine("Tecle enter para voltar...");
                        Console.ReadLine();
                        break;
                    case 0:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Opçao invalida");
                        Console.ReadLine();
                        break;
                }
            }
        }

        // Exibe os dados após a consulta
        static void Resultado(Root dados)
        {
            if (dados != null && dados.records != null)
            {
                foreach (var dado in dados.records)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("=====================================");
                    Console.WriteLine($"NOME: {dado.nome ?? "N/A"}");
                    Console.WriteLine($"CPF: {dado.cpf ?? "N/A"}");
                    Console.WriteLine($"NASCIMENTO: {dado.dataNascimento ?? "N/A"}");
                    Console.WriteLine($"MAE: {dado.nomeMae ?? "N/A"}");
                    Console.WriteLine($"PAI: {dado.nomePai ?? "N/A"}");
                    Console.WriteLine($"ENDEREÇO: UF = {dado.endereco?.siglaUf ?? "N/A"}, BAIRRO = {dado.endereco?.bairro ?? "N/A"}, NUMERO = {dado.endereco?.numero ?? "N/A"}, CEP = {dado.endereco?.cep ?? "N/A"}, COMPLEMENTO = {dado.endereco?.complemento ?? "N/A"}");
                    Console.WriteLine("=====================================\n");
                }

                return;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[!] ATENCAO: Nenhum dado encontrado.\n");
            }
        }

        // Gerar Token para realizar a consulta sem erro de 401
        static async Task<string> GerarTokenValido()
        {
            var loginsFilePath = @".\logins.txt";
            var lines = File.ReadLines(loginsFilePath);
            string validToken = "";
            foreach (var credential in lines)
            {
                validToken = await CheckLogins(new List<string> { credential });
            }

            return validToken;
        }

        // Checar se os logins.txt estão válidos
        static async Task<string> CheckLogins(List<string> credentials)
        {
            try
            {
                foreach (var credential in credentials)
                {
                    Client.DefaultRequestHeaders.Clear();
                    var loginBytes = Encoding.ASCII.GetBytes(credential);
                    var encodedLogin = System.Convert.ToBase64String(loginBytes);

                    Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/129.0.6668.71");
                    Client.DefaultRequestHeaders.Add("X-Authorization", $"Basic {encodedLogin}");

                    var response = await Client.PostAsync(APITOKEN, null);
                    var json = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        // pegar accessToken
                        LoginToken logintoken = JsonSerializer.Deserialize<LoginToken>(json);
                        return logintoken.accessToken;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[-] ERRO: {ex.Message}");
            }

            return string.Empty;
        }

        // Faz a requisição na API com o token Bearer e retorna a classe com os dados deserializados
        static async Task<Root> ConsultaNome(string bearerToken, string nome, string codMun)
        {
            try
            {
                Client.DefaultRequestHeaders.Clear();
                Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/129.0.6668.71");
                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

                nome = HttpUtility.UrlEncode(nome);
                var response = await Client.GetAsync($"https://servicos-cloud.saude.gov.br/pni-bff/v1/cidadao?nome={nome}&municipioNascimento={codMun}");
                var json = await response.Content.ReadAsStringAsync();
                Root root = JsonSerializer.Deserialize<Root>(json);

                return root;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[-] ERRO: {ex.Message}");
                return null;
            }
        }

        static async Task<Root> ConsultaCPF(string bearerToken, string cpf)
        {
            try
            {
                Client.DefaultRequestHeaders.Clear();
                Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/129.0.6668.71");
                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

                var response = await Client.GetAsync($"{APICONSULTACPF}/{cpf}");
                var json = await response.Content.ReadAsStringAsync();
                Root root = JsonSerializer.Deserialize<Root>(json);

                return root;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[-] ERRO: {ex.Message}");
                return null;
            }
        }
    }
}