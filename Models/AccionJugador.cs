using TowerDefenseWPF.EstructurasDeDatos;

namespace TowerDefenseWPF.Models;

public interface IContextoJuego
{
    void EliminarTorreSilenciosamente(Torre torre);
    void AñadirOro(int cantidad);
}

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
