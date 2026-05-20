using TowerDefenseWPF.Models;

namespace TowerDefenseWPF.GameLogic;

/// <summary>
/// Catálogo de las 10 oleadas del juego.
/// Cada oleada es una Lista de GeneracionEnemigo que el GestorOleadas
/// volcará en su Queue al iniciarse.
/// </summary>
public static class BibliotecaOleadas
{
    public static List<Oleada> ConstruirTodasLasOleadas()
    {
        var lista = new List<Oleada>();
        int n = 1;

        lista.Add(HacerOleada(n++, Repetir(TipoEnemigo.Normal, 8, 0.9), 25));
        lista.Add(HacerOleada(n++, Repetir(TipoEnemigo.Normal, 12, 0.75), 30));
        lista.Add(HacerOleada(n++, Mezclar(
            (TipoEnemigo.Normal, 6, 0.8),
            (TipoEnemigo.Rapido, 5, 0.55)), 35));
        lista.Add(HacerOleada(n++, Mezclar(
            (TipoEnemigo.Normal, 14, 0.65),
            (TipoEnemigo.Rapido, 6, 0.5)), 40));
        lista.Add(HacerOleada(n++, Mezclar(
            (TipoEnemigo.Normal, 10, 0.6),
            (TipoEnemigo.Tanque, 3, 1.4)), 50));
        lista.Add(HacerOleada(n++, Mezclar(
            (TipoEnemigo.Rapido, 12, 0.45),
            (TipoEnemigo.Tanque, 4, 1.2)), 55));
        lista.Add(HacerOleada(n++, Mezclar(
            (TipoEnemigo.Normal, 18, 0.55),
            (TipoEnemigo.Tanque, 5, 1.1),
            (TipoEnemigo.Rapido, 6, 0.4)), 65));
        lista.Add(HacerOleada(n++, Mezclar(
            (TipoEnemigo.Rapido, 16, 0.4),
            (TipoEnemigo.Tanque, 8, 1.0)), 75));
        lista.Add(HacerOleada(n++, Mezclar(
            (TipoEnemigo.Normal, 20, 0.45),
            (TipoEnemigo.Rapido, 12, 0.35),
            (TipoEnemigo.Tanque, 6, 0.9)), 90));
        lista.Add(HacerOleada(n++, Mezclar(
            (TipoEnemigo.Tanque, 12, 0.75),
            (TipoEnemigo.Rapido, 18, 0.35),
            (TipoEnemigo.Normal, 15, 0.4)), 150));

        return lista;
    }

    private static Oleada HacerOleada(int numero, List<GeneracionEnemigo> generaciones, int bonusRecompensa) =>
        new() { Numero = numero, Generaciones = generaciones, BonusRecompensa = bonusRecompensa };

    private static List<GeneracionEnemigo> Repetir(TipoEnemigo tipo, int cantidad, double espera)
    {
        var l = new List<GeneracionEnemigo>(cantidad);
        for (int i = 0; i < cantidad; i++)
            l.Add(new GeneracionEnemigo(tipo, i == 0 ? 0 : espera));
        return l;
    }

    private static List<GeneracionEnemigo> Mezclar(params (TipoEnemigo tipo, int cantidad, double espera)[] grupos)
    {
        var l = new List<GeneracionEnemigo>();
        bool primero = true;
        foreach (var (tipo, cantidad, espera) in grupos)
        {
            for (int i = 0; i < cantidad; i++)
            {
                l.Add(new GeneracionEnemigo(tipo, primero ? 0 : espera));
                primero = false;
            }
        }
        return l;
    }
}
