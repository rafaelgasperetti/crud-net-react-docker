using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace api.Helper
{
    public class Helper
    {
        public static readonly Guid CHAVE_API = new Guid("7d3c01ae-d986-4af3-9ebe-f930fbc15bbc");

        public enum CodigoInterno
        {
            Sucesso = 0,
            ValidarSessao_SessaoInvalida = 1,
            ModeloInexistente = 2,
            Catch_ObterDados = 3,
            TabelaInexistente = 4,
            Catch_EnviarDados = 5,
            RequisicaoNula = 6,
            ChaveAPIInvalida = 7,
            CredenciaisInvalidas = 8,
            Catch_ValidarLogin = 9,
            Catch_EfetuarLogin = 10,
            OperacaoInexistente = 11,
            ErroValidacao = 12,
            ErroWebClient = 13,
            ErroWebClientNoResponse = 14,
            ErroWebCall = 15
        }

        public static string Replicar(string str, int qtd)
        {
            if (string.IsNullOrEmpty(str) || qtd < 1)
            {
                return string.Empty;
            }

            string ret = string.Empty;

            for (int counter = 0; counter < qtd; counter++)
            {
                ret += str;
            }

            return ret;
        }

        public static string ReadStream(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static string ObterVersao()
        {
            return typeof(Helper).Assembly.GetName().Version.ToString();
        }

        public static string Criptografar(string s)
        {
            if (s != null)
            {
                Seguranca encoder = new Seguranca();
                return JsonConvert.SerializeObject(encoder.Encrypt(s));
            }
            else
            {
                return null;
            }
        }

        public static string Decriptografar(string s)
        {
            if (s != null)
            {
                Seguranca encoder = new Seguranca();
                return Seguranca.ToString(encoder.Decrypt(JsonConvert.DeserializeObject<byte[]>(s)), encoder.GetEncodingType());
            }
            else
            {
                return null;
            }
        }

        public static bool CPFValido(string cpf)
        {
            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf;
            string digito;
            int soma;
            int resto;
            cpf = cpf.Trim();
            cpf = cpf.Replace(".", "").Replace("-", "");
            if (cpf.Length != 11)
                return false;
            tempCpf = cpf.Substring(0, 9);
            soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto.ToString();
            return cpf.EndsWith(digito);
        }

        public static bool CNPJValido(string cnpj)
        {
            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma;
            int resto;
            string digito;
            string tempCnpj;
            cnpj = cnpj.Trim();
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
            if (cnpj.Length != 14)
                return false;
            tempCnpj = cnpj.Substring(0, 12);
            soma = 0;
            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = resto.ToString();
            tempCnpj = tempCnpj + digito;
            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto.ToString();
            return cnpj.EndsWith(digito);
        }
    }
}
