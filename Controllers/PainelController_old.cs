using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MonitorRedis.Models;

namespace MonitorRedis.Controllers
{
    public class PainelController : Controller
    {
        public Memoria memoria;
        public int CNPJ_A { get; set; }
        public int CNPJ_B { get; set; }
        public int CNPJ_C { get; set; }
        public int MensagensTot { get; set; }

        // GET: Painel
        public ActionResult Index()
        {
            CNPJ_A = CNPJ_B = CNPJ_C = MensagensTot = 0;
            memoria = new Memoria();
            string recebeJson;
            string respostaJson;
            string memoriaJson;
           

            BuscaJson(out respostaJson, out recebeJson, out memoriaJson);

            RemoveCaracteresIndesejados(ref recebeJson,ref respostaJson);

            memoriaJson = RemoveCaracteresIndesejadosMemoria(memoriaJson);

            PopulaVariavelMemoria(memoriaJson);

            var splitRecebe = recebeJson.Split(',');
            var splitresposta = respostaJson.Split(',');

            var filaRecebe = new List<Mensagens>();
            var filaresposta = new List<Mensagens>();

            PopulaVariavelRecebe(splitRecebe,ref filaRecebe);
            PopulaVariavelresposta(splitresposta, ref filaresposta);

            ViewBag.memoria = memoria;
            ViewBag.Recebe = filaRecebe;
            ViewBag.resposta = filaresposta;
            ViewBag.MensagensTot = MensagensTot;
            ViewBag.cnpjA = CNPJ_A;
            ViewBag.cnpjB = CNPJ_B;
            ViewBag.cnpjC = CNPJ_C;
            return View("Index");
        }

        private void PopulaVariavelMemoria(string memoriaJson)
        {
            var memory = memoriaJson.Split(',');

            memoria.MemoriaAtual = int.Parse(memory[0].Split(':')[1]);
            memoria.MemoriaAtualString = memory[1].Split(':')[1];
            memoria.MemoriaPico = int.Parse(memory[2].Split(':')[1]);
            memoria.MemoriaPicoString = memory[3].Split(':')[1];
        }

        private static string RemoveCaracteresIndesejadosMemoria(string memoriaJson)
        {
            memoriaJson = memoriaJson.Replace("{", "");
            memoriaJson = memoriaJson.Replace("}", "");
            memoriaJson = memoriaJson.Replace('"', ' ');
            memoriaJson = memoriaJson.Replace(" ", "");
            return memoriaJson;
        }

        private void PopulaVariavelRecebe(string[] split,ref List<Mensagens> fila)
        {
            foreach (var t in split)
            {
                var msg = new Mensagens();
                var temp = t.Split(':');
                msg.Fila = temp[0];
                msg.Msg = temp[2];
                if (int.Parse(msg.Msg) > 2000)
                {
                   CNPJ_A++;
                }else if (int.Parse(msg.Msg) > 1000)
                {
                    CNPJ_B++;
                }
                else
                {
                    CNPJ_C++;
                }
                MensagensTot += int.Parse(msg.Msg);
                msg.CNPJ = temp[1];
                fila.Add(msg);
            }
        }

        private void PopulaVariavelresposta(string[] split,ref List<Mensagens> fila)
        {
            foreach (var t in split)
            {
                var msg = new Mensagens();
                var temp = t.Split(':');
                msg.Fila = temp[0];
                msg.Msg = temp[2];
                if (int.Parse(msg.Msg) > 2000)
                {
                    CNPJ_A++;
                }
                else if (int.Parse(msg.Msg) > 1000)
                {
                    CNPJ_B++;
                }
                else
                {
                    CNPJ_C++;
                }
                MensagensTot += int.Parse(msg.Msg);
                msg.CNPJ = temp[1];
                fila.Add(msg);
            }
        }

        private void RemoveCaracteresIndesejados(ref string recebe,ref string resposta)
        {
            recebe = recebe.Replace("{", "");
            recebe = recebe.Replace("}", "");
            recebe = recebe.Replace('"', ' ');
            resposta = resposta.Replace("{", "");
            resposta = resposta.Replace("}", "");
            resposta = resposta.Replace('"', ' ');
           
        }

        private void BuscaJson(out string resposta, out string recebe, out string memoria)
        { 
            using (var wc = new WebClient())
            {
                resposta = wc.DownloadString(@"http://redis.oobj-dfe.com.br/respostasPorCnpj");
                recebe = wc.DownloadString(@"http://redis.oobj-dfe.com.br/recebePorCnpj");
                memoria = wc.DownloadString(@"http://redis.oobj-dfe.com.br/memoriaDisponivel");
            }
            
        }
    }
}