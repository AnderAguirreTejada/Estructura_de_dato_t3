using TowerDefenseWPF.Models;

namespace TowerDefenseWPF.GameLogic;

public static class FabricaEnemigo
{
    public static Enemigo Crear(TipoEnemigo tipo)
    {
        return tipo switch
        {
            TipoEnemigo.Normal => new Enemigo
            {
                Tipo = tipo,
                VidaMaxima = 55,
                Vida = 55,
                VelocidadBase = 65,
                Recompensa = 8,
                Radio = 11
            },
            TipoEnemigo.Rapido => new Enemigo
            {
                Tipo = tipo,
                VidaMaxima = 32,
                Vida = 32,
                VelocidadBase = 115,
                Recompensa = 10,
                Radio = 9
            },
            TipoEnemigo.Tanque => new Enemigo
            {
                Tipo = tipo,
                VidaMaxima = 230,
                Vida = 230,
                VelocidadBase = 36,
                Recompensa = 25,
                Radio = 15
            },
            _ => throw new ArgumentOutOfRangeException(nameof(tipo))
        };
    }
}
