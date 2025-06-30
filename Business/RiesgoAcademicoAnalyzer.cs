using CeiliApi.Models.DTOs;
using CeiliApi.Models.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace CeiliApi.Business
{
    public class RiesgoAcademicoAnalyzer
    {
        public static (string Prompt, string NivelGlobal) ConstruirPrompt(Evaluacion evaluacion)
        {
            var aspectos = JsonConvert.DeserializeObject<List<AspectoEvaluadoDto>>(evaluacion.AspectosJson ?? "[]") ?? new();
            var resumen = aspectos
                .GroupBy(a => a.Categoria)
                .Select(g => $"{g.Key}: {ObtenRiesgoCategoria(g)}")
                .ToList();

            string nivelGlobal = ObtenerRiesgoGlobal(aspectos);

            string instrucciones;
            if (nivelGlobal == "Bajo")
            {
                instrucciones = @"
- El alumno no presenta riesgos significativos.
- Escribe un mensaje breve y motivador dirigido al docente (máximo dos párrafos), reconociendo el buen trabajo realizado, y si acaso, una sugerencia de mejora general.";
            }
            else
            {
                instrucciones = @"
- El alumno presenta áreas de mejora.
- Da sugerencias claras en párrafos breves o una lista corta (máximo 5 puntos), según lo creas más útil.
- No repitas introducción, ve al grano y sé concreto, profesional y empático.";
            }

            string prompt = $@"
Eres un orientador escolar experto. Analiza el siguiente resumen de riesgos:
{string.Join("\n", resumen)}
Observaciones del docente: '{evaluacion.ObservacionDocente}'

INSTRUCCIONES:
{instrucciones}
";
            return (prompt, nivelGlobal);
        }

        private static string ObtenRiesgoCategoria(IGrouping<string, AspectoEvaluadoDto> grupo)
        {
            var cuenta = grupo.GroupBy(a => a.Riesgo)
                .ToDictionary(g => g.Key, g => g.Count());
            if (cuenta.TryGetValue("Alto", out int alto) && alto > 0) return "Alto";
            if (cuenta.TryGetValue("Medio", out int medio) && medio > 0) return "Medio";
            return "Bajo";
        }

        private static string ObtenerRiesgoGlobal(List<AspectoEvaluadoDto> aspectos)
        {
            int bajo = 0, medio = 0, alto = 0;
            foreach (var g in aspectos.GroupBy(a => a.Categoria))
            {
                var cuenta = g.GroupBy(a => a.Riesgo).ToDictionary(x => x.Key, x => x.Count());
                if (cuenta.TryGetValue("Alto", out int nAlto)) alto += nAlto;
                if (cuenta.TryGetValue("Medio", out int nMedio)) medio += nMedio;
                if (cuenta.TryGetValue("Bajo", out int nBajo)) bajo += nBajo;
            }
            if (alto >= medio && alto >= bajo && alto > 0) return "Alto";
            if (medio >= bajo && medio > 0) return "Medio";
            return "Bajo";
        }
    }
}
