using TowerDefenseWPF.EstructurasDeDatos;
using TowerDefenseWPF.Models;

namespace TowerDefenseWPF.GameLogic;

/// <summary>
/// Catálogo de las 10 oleadas del juego.
/// Cada oleada contiene una Lista&lt;GeneracionEnemigo&gt; propia que el
/// GestorOleadas volcará en su Cola al iniciarse.
/// </summary>
public static class BibliotecaOleadas
{
    public static Lista<Oleada> ConstruirTodasLasOleadas()
    {
        var lista = new Lista<Oleada>();
        int n = 1;

        lista.Agregar(HacerOleada(n++, Repetir(TipoEnemigo.Normal, 8, 0.9), 25));
        lista.Agregar(HacerOleada(n++, Repetir(TipoEnemigo.Normal, 12, 0.75), 30));
        lista.Agregar(HacerOleada(n++, Mezclar(
            (TipoEnemigo.Normal, 6, 0.8),
            (TipoEnemigo.Rapido, 5, 0.55)), 35));
        lista.Agregar(HacerOleada(n++, Mezclar(
            (TipoEnemigo.Normal, 14, 0.65),
            (TipoEnemigo.Rapido, 6, 0.5)), 40));
        lista.Agregar(HacerOleada(n++, Mezclar(
            (TipoEnemigo.Normal, 10, 0.6),
            (TipoEnemigo.Tanque, 3, 1.4)), 50));
        lista.Agregar(HacerOleada(n++, Mezclar(
            (TipoEnemigo.Rapido, 12, 0.45),
            (TipoEnemigo.Tanque, 4, 1.2)), 55));
        lista.Agregar(HacerOleada(n++, Mezclar(
            (TipoEnemigo.Normal, 18, 0.55),
            (TipoEnemigo.Tanque, 5, 1.1),
            (TipoEnemigo.Rapido, 6, 0.4)), 65));
        lista.Agregar(HacerOleada(n++, Mezclar(
            (TipoEnemigo.Rapido, 16, 0.4),
            (TipoEnemigo.Tanque, 8, 1.0)), 75));
        lista.Agregar(HacerOleada(n++, Mezclar(
            (TipoEnemigo.Normal, 20, 0.45),
            (TipoEnemigo.Rapido, 12, 0.35),
            (TipoEnemigo.Tanque, 6, 0.9)), 90));
        lista.Agregar(HacerOleada(n++, Mezclar(
            (TipoEnemigo.Tanque, 12, 0.75),
            (TipoEnemigo.Rapido, 18, 0.35),
            (TipoEnemigo.Normal, 15, 0.4)), 150));

        return lista;
    }

    private static Oleada HacerOleada(int numero, Lista<GeneracionEnemigo> generaciones, int bonusRecompensa) =>
        new() { Numero = numero, Generaciones = generaciones, BonusRecompensa = bonusRecompensa };

    private static Lista<GeneracionEnemigo> Repetir(TipoEnemigo tipo, int cantidad, double espera)
    {
        var l = new Lista<GeneracionEnemigo>();
        for (int i = 0; i < cantidad; i++)
            l.Agregar(new GeneracionEnemigo(tipo, i == 0 ? 0 : espera));
        return l;
    }

    private static Lista<GeneracionEnemigo> Mezclar(params (TipoEnemigo tipo, int cantidad, double espera)[] grupos)
    {
        var l = new Lista<GeneracionEnemigo>();
        bool primero = true;
        foreach (var (tipo, cantidad, espera) in grupos)
        {
            for (int i = 0; i < cantidad; i++)
            {
                l.Agregar(new GeneracionEnemigo(tipo, primero ? 0 : espera));
                primero = false;
            }
        }
        return l;
    }
}
