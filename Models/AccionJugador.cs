using TowerDefenseWPF.EstructurasDeDatos;

namespace TowerDefenseWPF.Models;

/// <summary>
/// Operaciones del juego que las acciones del jugador necesitan invocar
/// para revertirse. Lo expone <c>MainWindow</c> para que los Models
/// no dependan directamente de la ventana.
/// </summary>
public interface IContextoJuego
{
    void EliminarTorreSilenciosamente(Torre torre);
    void AñadirOro(int cantidad);
}

/// <summary>
/// Acción del jugador que se apila en la Pila de historial para
/// poder deshacerse con LIFO.
/// </summary>
public abstract class AccionJugador
{
    public abstract string Descripcion { get; }
    public abstract void Deshacer(IContextoJuego ctx);
}

public class AccionColocarTorre : AccionJugador
{
    public Torre Torre { get; }
    public int OroGastado { get; }

    public AccionColocarTorre(Torre torre, int oroGastado)
    {
        Torre = torre;
        OroGastado = oroGastado;
    }

    public override string Descripcion => $"Colocar {Torre.Tipo}";

    public override void Deshacer(IContextoJuego ctx)
    {
        ctx.EliminarTorreSilenciosamente(Torre);
        ctx.AñadirOro(OroGastado);
    }
}

public class AccionMejorarTorre : AccionJugador
{
    public Torre Torre { get; }
    public NodoArbolBinario<NodoMejora>? NodoAnterior { get; }
    public NodoArbolBinario<NodoMejora>? NodoNuevo { get; }
    public int OroGastado { get; }

    public AccionMejorarTorre(Torre torre, NodoArbolBinario<NodoMejora>? nodoAnterior, NodoArbolBinario<NodoMejora>? nodoNuevo, int oroGastado)
    {
        Torre = torre;
        NodoAnterior = nodoAnterior;
        NodoNuevo = nodoNuevo;
        OroGastado = oroGastado;
    }

    public override string Descripcion => $"Mejorar {Torre.Tipo} → {NodoNuevo?.Dato?.Nombre ?? "Base"}";

    public override void Deshacer(IContextoJuego ctx)
    {
        NodoNuevo?.Dato?.Revertir(Torre);
        Torre.NodoActual = NodoAnterior;
        ctx.AñadirOro(OroGastado);
    }
}
